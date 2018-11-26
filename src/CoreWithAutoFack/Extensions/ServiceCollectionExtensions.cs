using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace WebServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerilogServices(this IServiceCollection services)
        {
            var loggerConf = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.File("logs/Main_Log.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                .WriteTo.File("logs/Error_Log.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Day);
            // .WriteTo.Seq("http://localhost:5341");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development)
            {
                loggerConf.WriteTo.File("logs/Debug_Log.txt", rollingInterval: RollingInterval.Day);
            }
            Log.Logger = loggerConf.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services.AddSingleton(Log.Logger);
        }
    }
}