using System.IO.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.Entities.Enums;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        Task.Run(services.AddAuthenticationAsync).Wait();
    }

    private static async Task AddAuthenticationAsync(this IServiceCollection services)
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
        var clientId = await settingsRepository.GetSettingAsync(ServerSettingKey.OpenIdClientId);
        var clientSecret = await settingsRepository.GetSettingAsync(ServerSettingKey.OpenIdClientSecret);
        
        services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(opts =>
            {
                opts.Authority = authority.Value;
                opts.ClientId = clientId.Value;
                opts.ClientSecret = clientSecret.Value;
                opts.SaveTokens = true;
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.RequireHttpsMetadata = false;
                opts.ResponseType = OpenIdConnectResponseType.Code;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true
                };

                opts.Scope.Add("openid");
                opts.Scope.Add("profile");
                opts.Scope.Add("email");
                opts.TokenValidationParameters.RoleClaimType = "roles";
                opts.ClaimActions.MapUniqueJsonKey(ClaimTypes.Name, "name");
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