using Microsoft.EntityFrameworkCore;
using TripBooker.TravelAgencyService.Model;

namespace TripBooker.TravelAgencyService.Infrastructure;

internal class TravelAgencyDbContext : DbContext
{
    // VIEWS
    public DbSet<TransportModel> TransportView { get; set; } = null!;

    public TravelAgencyDbContext(DbContextOptions options) : base(options)
    {
    }
}