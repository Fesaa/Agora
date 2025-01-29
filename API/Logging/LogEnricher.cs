using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace API.Logging;

public class LogEnricher
{
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            diagnosticContext.Set("ClientIP", remoteIp.ToString());
        }

        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();
        if (!string.IsNullOrEmpty(userAgent))
        {
            diagnosticContext.Set("User-Agent", userAgent);
        }

        diagnosticContext.Set("Path", httpContext.Request.Path);
    }
}