using API.Data;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        
        services.AddSqlite();
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