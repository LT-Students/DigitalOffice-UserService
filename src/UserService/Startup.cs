using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Configuration;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Dto;
using MassTransit;
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

            services.AddControllers();

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

        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqOptions.RabbitMqSectionName)
                .Get<RabbitMqConfig>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserLoginConsumer>();
                x.AddConsumer<GetUserDataConsumer>();
                x.AddConsumer<AccessValidatorConsumer>();

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

                x.AddRequestClient<IAddImageRequest>(
                    new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.AddImageEndpoint}"));

                x.ConfigureKernelMassTransit(rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
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

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<UserServiceDbContext>();

            context.Database.Migrate();
        }
    }
}
