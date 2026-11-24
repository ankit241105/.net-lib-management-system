using Serilog;

namespace lib_management_system.Extensions
{
    // Serilog config
    public static class LoggingSetup
    {
        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, config) =>
                config
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

            return builder;
        }
    }
}
