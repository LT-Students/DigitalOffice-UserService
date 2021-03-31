using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LT.DigitalOffice.UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogger();

            CreateHostBuilder(args).Build().Run();
        }

        private static void ConfigureLogger()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsetting.Production.json", optional: false)
                .Build();
            var logstashUrl = config.GetSection("LogstashUrl").Value;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Http(logstashUrl)
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if RELEASE
            .UseSerilog()
#endif
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}