using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public interface IThemeRepository
{
    void Add(Theme theme);
    void Remove(Theme theme);
    void Update(Theme theme);
    
    Task<IEnumerable<ThemeDto>> GetThemesAsDtoAsync();
    Task<Theme?> GetThemeByIdAsync(int id);
    Task<Theme?> GetThemeByNameAsync(string name);
}

public class ThemeRepository(DataContext context, IMapper mapper): IThemeRepository
{
    public void Add(Theme theme)
    {
        context.Themes.Add(theme);
    }

    public void Remove(Theme theme)
    {
        context.Themes.Remove(theme);
    }

    public void Update(Theme theme)
    {
        context.Themes.Entry(theme).State = EntityState.Modified;
    }

    public async Task<IEnumerable<ThemeDto>> GetThemesAsDtoAsync()
    {
        return await context.Themes
            .ProjectTo<ThemeDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Theme?> GetThemeByIdAsync(int id)
    {
        return await context.Themes
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Theme?> GetThemeByNameAsync(string name)
    {
        return await context.Themes
            .Where(t => t.Name == name)
            .FirstOrDefaultAsync();
    }
}