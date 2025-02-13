using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    
    public static ImmutableArray<ServerSetting> DefaultSettings;

    public static async Task SeedThemes(DataContext context)
    {
        await context.Database.EnsureCreatedAsync();

        var defaultTheme = await context.Themes
            .Where(t => t.Name == Theme.DefaultTheme)
            .FirstOrDefaultAsync();

        if (defaultTheme != null)
        {
            return;
        }
        
        context.Themes.Add(new Theme()
        {
            Name = Theme.DefaultTheme,
            FileName = "",
            ThemeProvider = Provider.System
        });
        await context.SaveChangesAsync();
    }
    
    public static async Task SeedSettings(DataContext context)
    {
        await context.Database.EnsureCreatedAsync();

        DefaultSettings = ImmutableArray.Create(new List<ServerSetting>
        {
            new() {Key = ServerSettingKey.OpenIdAuthority, Value = ""},
            new() {Key = ServerSettingKey.LoggingLevel, Value = "Information"}
        }.ToArray());

        foreach (var serverSetting in DefaultSettings)
        {
            var existing = await context.ServerSettings.FirstOrDefaultAsync(x => x.Key == serverSetting.Key);
            if (existing == null)
            {
                await context.ServerSettings.AddAsync(serverSetting);
            }
        }

        await context.SaveChangesAsync();
    }
    
}