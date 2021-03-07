using FluentValidation;
using GreenPipes;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Business;
using LT.DigitalOffice.UserService.Business.Cache.Options;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Configuration;
using LT.DigitalOffice.UserService.Data;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LT.DigitalOffice.UserService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddHttpContextAccessor();

            services.AddDbContext<UserServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString"));
            });

            services.AddControllers();

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
            services.Configure<CacheOptions>(Configuration.GetSection(CacheOptions.MemoryCache));

            services.AddMemoryCache();

            services.AddKernelExtensions();

            ConfigureCommands(services);
            ConfigureRepositories(services);
            ConfigureValidators(services);
            ConfigureMappers(services);
            ConfigureMassTransit(services);
        }

        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitmqOptions = Configuration.GetSection(BaseRabbitMqOptions.RabbitMqSectionName).Get<RabbitMqConfig>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserLoginConsumer>();
                x.AddConsumer<GetUserInfoConsumer>();
                x.AddConsumer<AccessValidatorConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitmqOptions.Host, "/", host =>
                    {
                        host.Username($"{rabbitmqOptions.Username}_{rabbitmqOptions.Password}");
                        host.Password(rabbitmqOptions.Password);
                    });

                    cfg.ReceiveEndpoint("UserService", ep =>
                    {
                        ep.ConfigureConsumer<GetUserInfoConsumer>(context);
                        ep.ConfigureConsumer<AccessValidatorConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("UserService_AuthenticationService", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));

                        ep.ConfigureConsumer<UserLoginConsumer>(context);
                    });
                });

                x.AddRequestClient<IUserDescriptionRequest>(new Uri(rabbitmqOptions.UserDescriptionUrl));
                x.AddRequestClient<IGetUserPositionRequest>(new Uri(rabbitmqOptions.CompanyServiceUrl));
                x.AddRequestClient<IGetFileRequest>(new Uri(rabbitmqOptions.FileServiceUrl));
                x.AddRequestClient<ICheckTokenRequest>(new Uri(rabbitmqOptions.AuthenticationServiceValidationUrl));

                x.ConfigureKernelMassTransit(rabbitmqOptions);
            });

            services.AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionHandler(tempApp => tempApp.Run(CustomExceptionHandler.HandleCustomException));

            UpdateDatabase(app);

#if RELEASE
            app.UseHttpsRedirection();
#endif

            app.UseRouting();

            app.UseMiddleware<TokenMiddleware>();

            string corsUrl = Configuration.GetSection("Settings")["CorsUrl"];

            app.UseCors(builder =>
                builder
                    .WithOrigins(corsUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<UserServiceDbContext>();

            context.Database.Migrate();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient<IDataProvider, UserServiceDbContext>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserCredentialsRepository, UserCredentialsRepository>();
        }

        private void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IUserRequestMapper, UserRequestMapper>();
            services.AddTransient<IUserResponseMapper, UserResponseMapper>();
            services.AddTransient<IUserCredentialsRequestMapper, UserCredentialsRequestMapper>();
        }

        private void ConfigureCommands(IServiceCollection services)
        {
            services.AddTransient<ICreateUserCommand, CreateUserCommand>();
            services.AddTransient<IChangePasswordCommand, ChangePasswordCommand>();
            services.AddTransient<IEditUserCommand, EditUserCommand>();
            services.AddTransient<IGetUserByEmailCommand, GetUserByEmailCommand>();
            services.AddTransient<IGetUserByIdCommand, GetUserByIdCommand>();
            services.AddTransient<IGetUsersByIdsCommand, GetUsersByIdsCommand>();
            services.AddTransient<IForgotPasswordCommand, ForgotUserPasswordCommand>();
            services.AddTransient<IGetAllUsersCommand, GetAllUsersCommand>();
        }

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<UserRequest>, UserValidator>();
            services.AddTransient<IValidator<string>, UserEmailValidator>();
        }
    }
}