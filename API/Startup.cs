using System.IO.Compression;
using API.Extensions;
using API.Logging;
using API.Middleware;
using API.Middleware.RateLimit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace API;

public class Startup(IConfiguration configuration, IWebHostEnvironment env)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices(configuration, env);

        services.AddCors();
        services.AddIdentityServices(configuration);

        #region Authentication
        services.AddAuthentication(opts =>
        {
            opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(opts =>
        {
            opts.Authority = "http://localhost:8080/realms/dev-realm";
            opts.ClientId = "agora";
            opts.ClientSecret = "82OFTHpTNVuw4t80jkv42PTHXmt1gxpR";
            opts.SaveTokens = true;
            opts.GetClaimsFromUserInfoEndpoint = true;
            opts.RequireHttpsMetadata = false;
            opts.ResponseType = "code";
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true
            };

            opts.Scope.Add("openid");
            opts.Scope.Add("profile");
            opts.Scope.Add("email");
        });
        #endregion
        
        
        services.AddAuthorization();
        services.AddControllers();

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
        app.UseDefaultFiles();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
            opts.IncludeQueryInRequestPath = true;
        });

        app.UseEndpoints(builder =>
        {
            builder.MapControllers();
        });
        
        
        logger.LogInformation("Starting agora");
    }
}