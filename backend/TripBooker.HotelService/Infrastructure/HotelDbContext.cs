using Microsoft.EntityFrameworkCore;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Infrastructure;

internal class HotelDbContext : DbContext
{
    public DbSet<HotelOption> HotelOption { get; set; } = null!;

    public DbSet<RoomOption> RoomOption { get; set; } = null!;

    // VIEWS
    public DbSet<HotelOccupationModel> HotelOccupationModels { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoomOption>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }

    public HotelDbContext(DbContextOptions options) : base(options)
    {
    }
}
