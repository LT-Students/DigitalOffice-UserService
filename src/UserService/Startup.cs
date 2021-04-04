using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Business.Helpers;
using LT.DigitalOffice.UserService.Configuration;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
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
        private readonly ILogger<Startup> _logger;

        public IConfiguration Configuration { get; }

        #region private methods

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<UserServiceDbContext>();

            context.Database.Migrate();
        }

        private void InjectObjects(IServiceCollection services)
        {
            services.InjectObjects(
                InjectObjectType.Command,
                InjectType.Transient,
                "LT.DigitalOffice.UserService.Business",
                _logger);
            services.InjectObjects(
                InjectObjectType.Repository,
                InjectType.Transient,
                "LT.DigitalOffice.UserService.Data",
                _logger);
            services.InjectObjects(
                InjectObjectType.Mapper,
                InjectType.Transient,
                "LT.DigitalOffice.UserService.Mappers",
                _logger);
            services.InjectObjects(
                InjectObjectType.Validator,
                InjectType.Transient,
                "LT.DigitalOffice.UserService.Validation",
                _logger);
        }

        #region configure masstransit

        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqOptions.RabbitMqSectionName)
                .Get<RabbitMqConfig>();

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.Host, "/", host =>
                    {
                        host.Username($"{rabbitMqConfig.Username}_{rabbitMqConfig.Password}");
                        host.Password(rabbitMqConfig.Password);
                    });

                    ConfigureEndpoints(context, cfg, rabbitMqConfig);
                });

                ConfigureConsumers(busConfigurator);

                ConfigureRequestClients(busConfigurator, rabbitMqConfig);

                busConfigurator.ConfigureKernelMassTransit(rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
        }

        private void ConfigureConsumers(IServiceCollectionBusConfigurator x)
        {
            x.AddConsumer<UserLoginConsumer>();
            x.AddConsumer<GetUserDataConsumer>();
            x.AddConsumer<AccessValidatorConsumer>();
        }

        private void ConfigureRequestClients(IServiceCollectionBusConfigurator x, RabbitMqConfig rabbitMqConfig)
        {
            x.AddRequestClient<ISendEmailRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.SendEmailEndpoint}"));
            x.AddRequestClient<IGetPositionRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetPositionEndpoint}"));
            x.AddRequestClient<ICheckTokenRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.ValidateTokenEndpoint}"));
            x.AddRequestClient<IAddImageRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.AddImageEndpoint}"));
            x.AddRequestClient<IGetFileRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetFileEndpoint}"));
            x.AddRequestClient<IGetDepartmentRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetDepartmentEndpoint}"));
            x.AddRequestClient<IChangeUserDepartmentRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.ChangeUserDepartmentEndpoint}"));
            x.AddRequestClient<IChangeUserPositionRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.ChangeUserPositionEndpoint}"));
            x.AddRequestClient<IGetUserProjectsRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetProjectsEndpoint}"));
            x.AddRequestClient<IGetProjectRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetProjectEndpoint}"));
            x.AddRequestClient<IGetTokenRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetTokenEndpoint}"));
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

            cfg.ReceiveEndpoint(rabbitMqConfig.GetUserDataEndpoint, ep =>
            {
                ep.ConfigureConsumer<GetUserDataConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.GetUserCredentialsEndpoint, ep =>
            {
                ep.ConfigureConsumer<UserLoginConsumer>(context);
            });
        }

        #endregion

        #endregion

        #region public methods

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("LT.DigitalOffice.UserService.Startup", LogLevel.Trace)
                    .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKernelExtensions();

            services.AddHealthChecks();

            string connStr = Environment.GetEnvironmentVariable("ConnectionString");
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = Configuration.GetConnectionString("SQLConnectionString");
            }

            _logger.LogTrace(connStr.EncodeSqlConnectionString());

            services.AddDbContext<UserServiceDbContext>(options =>
            {
                options.UseSqlServer(connStr);
            });

            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
            services.Configure<CacheConfig>(Configuration.GetSection(CacheConfig.MemoryCache));

            services.AddMemoryCache();

            services.AddTransient<IDataProvider, UserServiceDbContext>();

            ConfigureMassTransit(services);

            InjectObjects(services);

            services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            UpdateDatabase(app);

            app.UseHealthChecks("/api/healthcheck");

            app.AddExceptionsHandler(loggerFactory);

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

        #endregion
    }
}
