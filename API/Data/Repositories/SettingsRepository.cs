using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public interface ISettingsRepository
{
    Task<bool> CompleteOpenIdConnectSettingsAsync();
    void Update(ServerSetting setting);
    
    Task<ServerSettingDto> GetSettingsDtoAsync();
    
    Task<ServerSetting> GetSettingAsync(ServerSettingKey key);
    
    Task<IEnumerable<ServerSetting>> GetSettingsAsync();
}

public class SettingsRepository(DataContext context, IMapper mapper): ISettingsRepository
{
    
    public async Task<bool> CompleteOpenIdConnectSettingsAsync()
    {
        return await context.ServerSettings
            .CountAsync(s => s.Key == ServerSettingKey.OpenIdAuthority && !string.IsNullOrEmpty(s.Value)) == 1;
    }

    public void Update(ServerSetting setting)
    {
        context.Entry(setting).State = EntityState.Modified;
    }

    public async Task<ServerSettingDto> GetSettingsDtoAsync()
    {
        var settings = await context.ServerSettings
            .Select(x => x)
            .AsNoTracking()
            .ToListAsync();
        return mapper.Map<ServerSettingDto>(settings);
    }

    public Task<ServerSetting> GetSettingAsync(ServerSettingKey key)
    {
        return context.ServerSettings.SingleOrDefaultAsync(x => x.Key == key)!;
    }

    public async Task<IEnumerable<ServerSetting>> GetSettingsAsync()
    {
        return await context.ServerSettings.ToListAsync();
    }
}