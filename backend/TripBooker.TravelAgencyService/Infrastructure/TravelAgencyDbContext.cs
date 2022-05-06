using Microsoft.EntityFrameworkCore;
using TripBooker.Common.TravelAgency.Model;
using TripBooker.TravelAgencyService.Model;

namespace TripBooker.TravelAgencyService.Infrastructure;

internal class TravelAgencyDbContext : DbContext
{
    // VIEWS
    public DbSet<TransportModel> TransportView { get; set; } = null!;

    public DbSet<HotelOccupationModel> HotelOccupationView { get; set; } = null!;

    public TravelAgencyDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HotelOccupationModel>()
            .HasKey(c => new { c.HotelId, c.Date });
    }
}