namespace API.Entities;

public class AppUserPreferences
{
    public int Id { get; set; }

    public AppUser AppUser { get; set; } = null!;
    public int AppUserId { get; set; }
}