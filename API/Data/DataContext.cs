using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class DataContext: IdentityDbContext<AppUser, AppRole, int, 
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    
    public DbSet<AppUser> AppUsers { get; set; } = null!;
    public DbSet<AppUserPreferences> AppUserPreferences { get; set; } = null!;
    public DbSet<ServerSettings> ServerSettings { get; set; } = null!;
}