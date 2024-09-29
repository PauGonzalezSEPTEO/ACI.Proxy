using ACI.HAM.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
#if DEBUG
using System.Diagnostics;
#endif


public class Program
{
    public static async Task Main(string[] args)
    {
#if DEBUG
        while (!Debugger.IsAttached) { Thread.Sleep(100); }
#endif
        try
        {
            Log.Logger.Information("Starting up");
            using var webHost = CreateWebHostBuilder(args).Build();
            await webHost.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Application start-up failed");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSerilog((hostingContext, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
            );
}
