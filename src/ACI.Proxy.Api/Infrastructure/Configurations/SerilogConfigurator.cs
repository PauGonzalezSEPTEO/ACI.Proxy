using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace ACI.Proxy.Api.Infrastructure.Configurations
{
    public static class SerilogConfigurator
    {
        public static Logger CreateLogger()
        {
            var configuration = LoadAppConfiguration();
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfigurationRoot LoadAppConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();
        }
    }
}
