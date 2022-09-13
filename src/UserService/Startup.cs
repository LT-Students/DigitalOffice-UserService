using DigitalOffice.Kernel.RedisSupport.Extensions;
using FluentValidation;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker.Consumer;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.EFSupport.Extensions;
using LT.DigitalOffice.Kernel.EFSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LT.DigitalOffice.UserService
{
  public class Startup : BaseApiInfo
  {
    public const string CorsPolicyName = "LtDoCorsPolicy";
    private string redisConnStr;

    private readonly BaseServiceInfoConfig _serviceInfoConfig;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public IConfiguration Configuration { get; }

    #region private methods

    private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
      var builder = new ServiceCollection()
        .AddLogging()
        .AddMvc()
        .AddNewtonsoftJson()
        .Services.BuildServiceProvider();

      return builder
        .GetRequiredService<IOptions<MvcOptions>>()
        .Value
        .InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>()
        .First();
    }

    #region configure masstransit

    private void ConfigureMassTransit(IServiceCollection services)
    {
      (string username, string password) = RabbitMqCredentialsHelper
        .Get(_rabbitMqConfig, _serviceInfoConfig);

      services.AddMassTransit(busConfigurator =>
      {
        busConfigurator.UsingRabbitMq((context, cfg) =>
          {
            cfg.Host(_rabbitMqConfig.Host, "/", host =>
              {
                host.Username(username);
                host.Password(password);
              });

            ConfigureEndpoints(context, cfg, _rabbitMqConfig);
          });

        ConfigureConsumers(busConfigurator);

        busConfigurator.AddRequestClients(_rabbitMqConfig);
      });

      services.AddMassTransitHostedService();
    }

    private void ConfigureConsumers(IServiceCollectionBusConfigurator x)
    {
      x.AddConsumer<UserLoginConsumer>();
      x.AddConsumer<GetUsersDataConsumer>();
      x.AddConsumer<AccessValidatorConsumer>();
      x.AddConsumer<SearchUsersConsumer>();
      x.AddConsumer<CreateAdminConsumer>();
      x.AddConsumer<FindParseEntitiesConsumer>();
      x.AddConsumer<CheckUsersExistenceConsumer>();
      x.AddConsumer<FilterUsersDataConsumer>();
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

      cfg.ReceiveEndpoint(rabbitMqConfig.GetUsersDataEndpoint, ep =>
      {
        ep.ConfigureConsumer<GetUsersDataConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.GetUserCredentialsEndpoint, ep =>
      {
        ep.ConfigureConsumer<UserLoginConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.SearchUsersEndpoint, ep =>
      {
        ep.ConfigureConsumer<SearchUsersConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.CreateAdminEndpoint, ep =>
      {
        ep.ConfigureConsumer<CreateAdminConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.FindParseEntitiesEndpoint, ep =>
      {
        ep.ConfigureConsumer<FindParseEntitiesConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.CheckUsersExistenceEndpoint, ep =>
      {
        ep.ConfigureConsumer<CheckUsersExistenceConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.FilterUsersDataEndpoint, ep =>
      {
        ep.ConfigureConsumer<FilterUsersDataConsumer>(context);
      });
    }

    #endregion

    #endregion

    #region public methods

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      _serviceInfoConfig = Configuration
        .GetSection(BaseServiceInfoConfig.SectionName)
        .Get<BaseServiceInfoConfig>();

      _rabbitMqConfig = Configuration
        .GetSection(BaseRabbitMqConfig.SectionName)
        .Get<RabbitMqConfig>();

      Version = "1.4.5.5";
      Description = "UserService is an API that intended to work with users.";
      StartTime = DateTime.UtcNow;
      ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy(
          CorsPolicyName,
          builder =>
          {
            builder
              //.WithOrigins(
              //    "https://*.ltdo.xyz",
              //    "http://*.ltdo.xyz",
              //    "http://ltdo.xyz",
              //    "http://ltdo.xyz:9802",
              //    "http://localhost:4200",
              //    "http://localhost:4500")
              .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
          });
      });

      services.AddHttpContextAccessor();

      string dbConnStr = ConnectionStringHandler.Get(Configuration);

      services.AddDbContext<UserServiceDbContext>(options =>
      {
        options.UseSqlServer(dbConnStr);
      });

      services.AddHealthChecks()
        .AddRabbitMqCheck()
        .AddSqlServer(dbConnStr);

      if (int.TryParse(Environment.GetEnvironmentVariable("MemoryCacheLiveInMinutes"), out int memoryCacheLifetime))
      {
        services.Configure<MemoryCacheConfig>(options =>
        {
          options.CacheLiveInMinutes = memoryCacheLifetime;
        });
      }
      else
      {
        services.Configure<MemoryCacheConfig>(Configuration.GetSection(MemoryCacheConfig.SectionName));
      }

      if (int.TryParse(Environment.GetEnvironmentVariable("RedisCacheLiveInMinutes"), out int redisCacheLifeTime))
      {
        services.Configure<RedisConfig>(options =>
        {
          options.CacheLiveInMinutes = redisCacheLifeTime;
        });
      }
      else
      {
        services.Configure<RedisConfig>(Configuration.GetSection(RedisConfig.SectionName));
      }

      services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
      services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));
      services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
      services.Configure<ForwardedHeadersOptions>(options =>
      {
        options.ForwardedHeaders =
          ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
      });

      services.AddMemoryCache();
      services.AddBusinessObjects();

      ConfigureMassTransit(services);

      //TODO this will be used when all validation takes place on the pipeline
      //string path = Path.Combine(
      //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
      //    "LT.DigitalOffice.UserService.Validation.dll");
      services.AddScoped<IValidator<JsonPatchDocument<EditUserRequest>>, EditUserRequestValidator>();

      redisConnStr = services.AddRedisSingleton(Configuration);

      services
        .AddControllers(options =>
        {
          options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
        }) // TODO check enum serialization from request without .AddJsonOptions()
           //this will be used when all validation takes place on the pipeline
           //.AddFluentValidation(x => x.RegisterValidatorsFromAssembly(Assembly.LoadFrom(path)))
           //.AddFluentValidation()
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
      app.UpdateDatabase<UserServiceDbContext>();

      FlushRedisDbHelper.FlushDatabase(redisConnStr, Cache.Users);

      app.UseForwardedHeaders();

      app.UseExceptionsHandler(loggerFactory);

      app.UseApiInformation();

      app.UseRouting();

      app.UseMiddleware<TokenMiddleware>();

      app.UseCors(CorsPolicyName);

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors(CorsPolicyName);
        endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
        {
          ResultStatusCodes = new Dictionary<HealthStatus, int>
          {
            { HealthStatus.Unhealthy, 200 },
            { HealthStatus.Healthy, 200 },
            { HealthStatus.Degraded, 200 },
          },
          Predicate = check => check.Name != "masstransit-bus",
          ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
      });
    }

    #endregion
  }
}
