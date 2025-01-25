using API.Data;
using API.Logging;
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


            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogCritical(ex, "An exception occurred while migrating the database. Restoring from backup");
                // TODO: Restore from backup
            }
            
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
            })
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
                builder.UseStartup<Startup>();
            });
}