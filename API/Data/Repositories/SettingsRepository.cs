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
    private readonly ServerSettingKey[] OpenIdConnectKeys = 
        [ServerSettingKey.OpenIdAuthority, ServerSettingKey.OpenIdClientId, ServerSettingKey.OpenIdClientSecret]; 
    
    public async Task<bool> CompleteOpenIdConnectSettingsAsync()
    {
        return await context.ServerSettings
            .CountAsync(s => OpenIdConnectKeys.Contains(s.Key) && !string.IsNullOrEmpty(s.Value)) == 3;
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