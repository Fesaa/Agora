using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<UserPreferences> AppUserPreferences { get; set; } = null!;
    public DbSet<ServerSetting> ServerSettings { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;
}