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