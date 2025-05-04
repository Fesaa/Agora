using System;
using System.IO.Compression;
using System.Linq;
using API.Extensions;
using API.Logging;
using API.Middleware;
using API.Middleware.RateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace API;

public class Startup(IConfiguration configuration, IWebHostEnvironment env)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices(configuration, env);

        services.AddCors();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddResponseCompression(opts =>
        {
            opts.Providers.Add<BrotliCompressionProvider>();
            opts.Providers.Add<GzipCompressionProvider>();
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            opts.EnableForHttps = true;
        });

        services.Configure<BrotliCompressionProviderOptions>(opts =>
        {
            opts.Level = CompressionLevel.Fastest;
        });

        services.AddResponseCaching();

        services.AddRateLimiter(opts =>
        {
            opts.AddPolicy("Authentication", 
                httpCtx => new AuthRateLimitPolicy().GetPartition(httpCtx));
        });
    }

    public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        app.UseMiddleware<ExceptionMiddleware>();
        
        app.UseResponseCompression();
        app.UseForwardedHeaders();
        app.UseRateLimiter();

        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseCors(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200")
                .WithExposedHeaders("Content-Disposition", "Pagination")
            );

            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseCors(policy => policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition", "Pagination")
            );
        }

        app.UseResponseCaching();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.UseDefaultFiles();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
            opts.IncludeQueryInRequestPath = true;
        });

        app.UseEndpoints(builder =>
        {
            builder.MapControllers();
            builder.MapFallbackToController("Index", "Fallback");
        });
        
        
        logger.LogInformation("Starting agora");
    }
}