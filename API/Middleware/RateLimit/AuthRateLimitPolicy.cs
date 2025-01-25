using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Middleware.RateLimit;

public class AuthRateLimitPolicy: IRateLimiterPolicy<string>
{
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        return RateLimitPartition.GetSlidingWindowLimiter(httpContext.Request.Headers.Host.ToString(),
            partition => new SlidingWindowRateLimiterOptions()
            {
                AutoReplenishment = true,
                PermitLimit = 1,
                Window = TimeSpan.FromMinutes(1),
            });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (ctx, _) =>
    {
        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            ctx.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return new ValueTask();
    };
}