using API.Data;
using API.Entities.Enums;
using API.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel
            .Information()
            .CreateBootstrapLogger();

        try
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            try
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<DataContext>();
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var alreadyCreated = await context.Database.CanConnectAsync();

                if (alreadyCreated && pendingMigrations.Any())
                {
                    logger.LogInformation("Backing up database as migrations are needed...");
                    // TODO: Backup database
                }

                await context.Database.MigrateAsync();

                // Database seeding
                await Seed.SeedSettings(context);

            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogCritical(ex, "An exception occurred while migrating the database. Restoring from backup");
                // TODO: Restore from backup
            }

            var logLevel = await unitOfWork.SettingsRepository.GetSettingAsync(ServerSettingKey.LoggingLevel);
            LogLevelOptions.SwitchLogLevel(logLevel.Value);
            
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An exception occurred while running the API");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((_, services, configuration) =>
            {
                LogLevelOptions.CreateConfig(configuration);
            }, true)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Sources.Clear();

                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"config/appsettings.{env.EnvironmentName}.json",
                        optional: true, reloadOnChange: false);
            })
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseKestrel(opts =>
                {
                    opts.ListenAnyIP(5050, options =>
                    {
                        options.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                    });
                });
                builder.UseStartup<Startup>();
            });
}