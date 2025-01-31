using System.Collections.Generic;
using System.IO.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Constants;
using API.Data;
using API.Entities.Enums;
using API.Helpers;
using API.Helpers.RoleClaimTransformers;
using API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        services.AddScoped<IMemoryCache, MemoryCache>();
        services.AddScoped<IFileSystem, FileSystem>();
        services.AddScoped<IDirectoryService, DirectoryService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILocalizationService, LocalizationService>();
        
        services.AddSqlite();

        Task.Run(async () => await services.AddAuthenticationAsync(environment)).Wait();
    }

    private static async Task AddAuthenticationAsync(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<DataContext>();
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        var logger = provider.GetRequiredService<ILogger<IServiceCollection>>();
        var settingsRepository = unitOfWork.SettingsRepository;

        var canConnect = await context.Database.CanConnectAsync();

        // First start up, only local authentication
        // TODO: Check how we can best set this up.
        if (!canConnect || ! await unitOfWork.SettingsRepository.CompleteOpenIdConnectSettingsAsync())
        {
            logger.LogCritical("First start, or OpenId Connect not set up completely. [This is not an error]");
            services.AddAuthentication();
            return;
        }

        var authority = await settingsRepository.GetSettingAsync(ServerSettingKey.OpenIdAuthority);

        // TODO: Switch on provider type, just using KeyCloak for now as we use that locally
        services.AddTransient<IClaimsTransformation, KeyCloakRoleClaimsTransformation>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority.Value;
                options.Audience = "agora-api";

                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyConstants.AdminRole,
                policy => policy.RequireClaim(ClaimTypes.Role,
                    PolicyConstants.AdminRole, PolicyConstants.AdminRole.ToLower(), PolicyConstants.AdminRole.ToUpper()));
        });
    }

    private static void AddSqlite(this IServiceCollection services)
    {
        services.AddDbContextPool<DataContext>(opts =>
        {
            opts.UseSqlite("Data Source=config/agora.db", builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            opts.EnableDetailedErrors();
            opts.EnableSensitiveDataLogging();
        });
    }
}