using Microsoft.EntityFrameworkCore;
using TripBooker.TourOperator.Model;

namespace TripBooker.TourOperator.Infrastructure;

internal class TourOperatorDbContext : DbContext
{
    // Views
    public DbSet<HotelOccupationModel> HotelOccupationView { get; set; } = null!;

    public TourOperatorDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HotelOccupationModel>()
            .HasKey(c => new { c.HotelId, c.Date });
    }
}
