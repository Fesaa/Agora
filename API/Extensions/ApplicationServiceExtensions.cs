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
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IMeetingService, MeetingService>();
        services.AddScoped<INotifyService, NotifyService>();
        
        services.AddSqlite();

        Task.Run(async () =>
        {
            await services.AddAuthenticationAsync(environment);
            await services.AddCalenderSyncAsync();
        }).Wait();
    }

    private static async Task AddCalenderSyncAsync(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<DataContext>();
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        var logger = provider.GetRequiredService<ILogger<IServiceCollection>>();
        var settingsRepository = unitOfWork.SettingsRepository;

        var canConnect = await context.Database.CanConnectAsync();

        if (!canConnect)
        {
            logger.LogCritical("First start? Cannot load settings, falling back to DefaultCalenderSyncService");
            services.AddScoped<ICalenderSyncService, DefaultCalenderSyncService>();
            return;
        }
        
        var setting = await settingsRepository.GetSettingsDtoAsync();
        switch (setting.CalenderSyncProvider)
        {
            case CalenderSyncProvider.None:
                logger.LogDebug("Using DefaultCalenderSyncService");
                services.AddScoped<ICalenderSyncService, DefaultCalenderSyncService>();
                break;
            default:
                logger.LogCritical("No valid CalenderSyncProvider found, falling back to DefaultCalenderSyncService");
                services.AddScoped<ICalenderSyncService, DefaultCalenderSyncService>();
                break;
        }
    }

    private static async Task AddAuthenticationAsync(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<DataContext>();
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        var logger = provider.GetRequiredService<ILogger<IServiceCollection>>();
        var settingsRepository = unitOfWork.SettingsRepository;

        var canConnect = await context.Database.CanConnectAsync();
        
        // Need the policies to be always present, or requests fail 
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyConstants.AdminRole,
                policy => policy.RequireClaim(ClaimTypes.Role,
                    PolicyConstants.AdminRole, PolicyConstants.AdminRole.ToLower(), PolicyConstants.AdminRole.ToUpper()));
        });

        // First start up, only local authentication
        // TODO: Check how we can best set this up.
        if (!canConnect || ! await unitOfWork.SettingsRepository.CompleteOpenIdConnectSettingsAsync())
        {
            logger.LogCritical("First start, or OpenId Connect not set up completely. [This is not an error]");
            services.AddAuthentication();
            return;
        }

        var dto = await settingsRepository.GetSettingsDtoAsync();
        var authority = dto.OpenIdAuthority;
        var openIdProvider = dto.OpenIdProvider;

        switch (openIdProvider)
        {
            case OpenIdProvider.KeyCloak:
                logger.LogDebug("Using the KeyCloak claims transformer");
                services.AddTransient<IClaimsTransformation, KeyCloakRoleClaimsTransformation>();
                break;
            case OpenIdProvider.AzureAd:
                logger.LogDebug("Using the Azure claims transformer");
                services.AddTransient<IClaimsTransformation, AzureRoleClaimsTransformation>();
                break;
            default:
                logger.LogCritical("The configured provider is not known, Authorization may not work as expected. Check your configuration!");
                break;
        }
        

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
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