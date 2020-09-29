using System;
using FluentValidation;
using GreenPipes;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Mappers;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using MassTransit;
using LT.DigitalOffice.Kernel.Broker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LT.DigitalOffice.Kernel;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Business;
using LT.DigitalOffice.UserService.Validation;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.CompanyService.Data.Provider;

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
            services.Configure<RabbitMQOptions>(Configuration);

            services.AddHealthChecks();

            services.AddControllers();

            services.AddKernelExtensions(Configuration);

            services.AddDbContext<UserServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString"));
            });

            services.AddControllers();

            ConfigureCommands(services);
            ConfigureRepositories(services);
            ConfigureValidators(services);
            ConfigureMappers(services);
            ConfigRabbitMq(services);

            services.AddMassTransitHostedService();
        }

        private void ConfigRabbitMq(IServiceCollection services)
        {
            const string serviceSection = "RabbitMQ";
            string serviceName = Configuration.GetSection(serviceSection)["Username"];
            string servicePassword = Configuration.GetSection(serviceSection)["Password"];

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserLoginConsumer>();
                x.AddConsumer<GetUserInfoConsumer>();
                x.AddConsumer<AccessValidatorConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", host =>
                    {
                        host.Username($"{serviceName}_{servicePassword}");
                        host.Password(servicePassword);
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
                x.AddRequestClient<IGetUserPositionRequest>(new Uri("rabbitmq://localhost/CompanyService"));
                x.AddRequestClient<IGetUserPositionRequest>(
                    new Uri("rabbitmq://localhost/CompanyService"));
                x.AddRequestClient<IGetFileRequest>(
                    new Uri("rabbitmq://localhost/FileService"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionHandler(tempApp => tempApp.Run(CustomExceptionHandler.HandleCustomException));

            UpdateDatabase(app);

            app.UseHttpsRedirection();
            app.UseRouting();

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
        }

        private void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IMapper<UserCreateRequest, DbUser>, UserMapper>();
            services.AddTransient<IMapper<DbUser, User>, UserMapper>();
            services.AddTransient<IMapper<EditUserRequest, DbUser>, UserMapper>();
            services.AddTransient<IMapper<DbUser, string, object>, UserMapper>();
            services.AddTransient<IMapper<EditUserRequest, string, DbUserCredentials>, UserCredentialsMapper>();
            services.AddTransient<IMapper<UserCreateRequest, Guid, DbUserCredentials>, UserCredentialsMapper>();
        }

        private void ConfigureCommands(IServiceCollection services)
        {
            services.AddTransient<IUserCreateCommand, UserCreateCommand>();
            services.AddTransient<IEditUserCommand, EditUserCommand>();
            services.AddTransient<IGetUserByEmailCommand, GetUserByEmailCommand>();
            services.AddTransient<IGetUserByIdCommand, GetUserByIdCommand>();
        }

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<EditUserRequest>, EditUserRequestValidator>();
            services.AddTransient<IValidator<UserCreateRequest>, UserCreateRequestValidator>();
            services.AddTransient<IValidator<string>, GetUserByEmailValidator>();
        }
    }
}