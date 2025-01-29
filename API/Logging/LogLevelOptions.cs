using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace API.Logging;

public class LogLevelOptions
{
    public const string LogFile = "config/logs/agora.log";
    public const bool LogRollingEnabled = true;
    
    private static readonly LoggingLevelSwitch LogLevelSwitch = new ();
    private static readonly LoggingLevelSwitch MicrosoftLogLevelSwitch = new (LogEventLevel.Error);
    private static readonly LoggingLevelSwitch MicrosoftHostingLifetimeLogLevelSwitch = new (LogEventLevel.Error);
    private static readonly LoggingLevelSwitch AspNetCoreLogLevelSwitch = new (LogEventLevel.Error);

    public static LoggerConfiguration CreateConfig(LoggerConfiguration configuration)
    {
        const string outputTemplate = "[Agora] [{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {ThreadId}@{ThreadName}] [{Level}] {SourceContext} {Message:lj}{NewLine}{Exception}";
        return configuration
            .MinimumLevel.ControlledBy(LogLevelSwitch)
            .MinimumLevel.Override("Microsoft", MicrosoftLogLevelSwitch)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", MicrosoftHostingLifetimeLogLevelSwitch)
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Internal.WebHost", AspNetCoreLogLevelSwitch)

            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .WriteTo.Console(new MessageTemplateTextFormatter(outputTemplate))
            .WriteTo.File(LogFile,
                shared: true,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: LogRollingEnabled,
                outputTemplate: outputTemplate
            )
            .Filter.ByIncludingOnly(ShouldIncludeLogStatement);
    }
    
    public static void SwitchLogLevel(string level)
    {
        switch (level)
        {
            case "Debug":
                LogLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                MicrosoftLogLevelSwitch.MinimumLevel = LogEventLevel.Warning; // This is DB output information, Inf shows the SQL
                MicrosoftHostingLifetimeLogLevelSwitch.MinimumLevel = LogEventLevel.Information;
                AspNetCoreLogLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                break;
            case "Information":
                LogLevelSwitch.MinimumLevel = LogEventLevel.Information;
                MicrosoftLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                MicrosoftHostingLifetimeLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                AspNetCoreLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                break;
            case "Trace":
                LogLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                MicrosoftLogLevelSwitch.MinimumLevel = LogEventLevel.Information;
                MicrosoftHostingLifetimeLogLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                AspNetCoreLogLevelSwitch.MinimumLevel = LogEventLevel.Information;
                break;
            case "Warning":
                LogLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                MicrosoftLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                MicrosoftHostingLifetimeLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                AspNetCoreLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                break;
            case "Critical":
                LogLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
                MicrosoftLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                MicrosoftHostingLifetimeLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                AspNetCoreLogLevelSwitch.MinimumLevel = LogEventLevel.Error;
                break;
        }
    }

    private static bool ShouldIncludeLogStatement(LogEvent e)
    {
        var isRequestLoggingMiddleware = e.Properties.ContainsKey("SourceContext") &&
                                         e.Properties["SourceContext"].ToString().Replace("\"", string.Empty) ==
                                         "Serilog.AspNetCore.RequestLoggingMiddleware";

        if (isRequestLoggingMiddleware && LogLevelSwitch.MinimumLevel > LogEventLevel.Information)
        {
            return false;
        }

        return true;
    }
}