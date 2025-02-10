using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using API.Entities;
using API.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    
    public static ImmutableArray<ServerSetting> DefaultSettings;
    
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