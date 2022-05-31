using Microsoft.EntityFrameworkCore;
using TripBooker.StatisticsService.Model;

namespace TripBooker.StatisticsService.Infrastructure;

internal class StatisticsDbContext : DbContext
{
    public DbSet<ReservationModel> Reservation { get; set; } = null!;

    public StatisticsDbContext(DbContextOptions options) : base(options)
    {
    }
}