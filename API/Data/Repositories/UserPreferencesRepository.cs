using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public interface IUserPreferencesRepository
{
    Task<UserPreferences> GetByUserIdAsync(string userId);
    Task<string?> GetLocaleAsync(string userId);
}

public class UserPreferencesRepository(DataContext context, IMapper mapper): IUserPreferencesRepository
{

    private UserPreferences DefaultUserPreferences(string userId)
    {
        return new UserPreferences()
        {
            ExternalId = userId,
        };
    }


    public async Task<UserPreferences> GetByUserIdAsync(string userId)
    {
        var pref = await context.AppUserPreferences
            .FirstOrDefaultAsync(pref => pref.ExternalId == userId);

        if (pref == null)
        {
            pref = DefaultUserPreferences(userId);
            await context.AppUserPreferences.AddAsync(pref);
            await context.SaveChangesAsync();
        }

        return pref;
    }

    public async Task<string?> GetLocaleAsync(string userId)
    {
        return await context.AppUserPreferences
            .Where(pref => pref.ExternalId == userId)
            .Select(pref => pref.Language)
            .FirstOrDefaultAsync();
    }
}