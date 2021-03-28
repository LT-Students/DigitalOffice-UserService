using FluentValidation;
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
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;

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

            string connStr = Environment.GetEnvironmentVariable("ConnectionString");
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = Configuration.GetConnectionString("SQLConnectionString");
            }

            Console.WriteLine(connStr);

            services.AddDbContext<UserServiceDbContext>(options =>
            {
                options.UseSqlServer(connStr);
            });

            services.AddControllers();

            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
            services.Configure<CacheOptions>(Configuration.GetSection(CacheOptions.MemoryCache));

            services.AddMemoryCache();

            services.AddKernelExtensions();

            ConfigureCommands(services);
            ConfigureRepositories(services);
            ConfigureValidators(services);
            ConfigureMappers(services);
            ConfigureMassTransit(services);

            services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        }

        private void ConfigureEndpoints(
            IBusRegistrationContext context,
            IRabbitMqBusFactoryConfigurator cfg,
            RabbitMqConfig rabbitMqConfig)
        {
            cfg.ReceiveEndpoint(rabbitMqConfig.CheckUserIsAdminEndpoint, ep =>
            {
                // TODO Rename
                ep.ConfigureConsumer<AccessValidatorConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.GetUserInfoEndpoint, ep =>
            {
                ep.ConfigureConsumer<GetUserInfoConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.GetUserCredentialsEndpoint, ep =>
            {
                ep.ConfigureConsumer<UserLoginConsumer>(context);
            });
        }

        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqOptions.RabbitMqSectionName)
                .Get<RabbitMqConfig>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserLoginConsumer>();
                x.AddConsumer<GetUserInfoConsumer>();
                x.AddConsumer<AccessValidatorConsumer>();
                x.AddConsumer<GetUserInfoConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.Host, "/", host =>
                    {
                        host.Username($"{rabbitMqConfig.Username}_{rabbitMqConfig.Password}");
                        host.Password(rabbitMqConfig.Password);
                    });

                    ConfigureEndpoints(context, cfg, rabbitMqConfig);
                });

                x.AddRequestClient<IUserDescriptionRequest>(
                  new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.UserDescriptionUrl}"));

                x.AddRequestClient<IGetUserPositionRequest>(
                  new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.CompanyServiceUrl}"));

                x.AddRequestClient<ICheckTokenRequest>(
                  new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.ValidateTokenEndpoint}"));

                x.ConfigureKernelMassTransit(rabbitMqConfig);
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
            services.AddTransient<IGeneratePasswordCommand, GeneratePasswordCommand>();
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