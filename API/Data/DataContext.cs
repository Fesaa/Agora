using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MergeRooms>()
            .HasOne(mr => mr.Parent)
            .WithMany(mr => mr.ParentMergeRooms)
            .HasForeignKey(mr => mr.ParentRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MeetingRoom>()
            .HasMany(m => m.MergeRooms)
            .WithMany(mr => mr.MeetingRooms)
            .UsingEntity(j => j.ToTable("MeetingRoomMergeRooms"));
    }


    public DbSet<UserPreferences> AppUserPreferences { get; set; } = null!;
    public DbSet<ServerSetting> ServerSettings { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;
    public DbSet<MeetingRoom> MeetingRooms { get; set; } = null!;
    public DbSet<MergeRooms> MergeRooms { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
}