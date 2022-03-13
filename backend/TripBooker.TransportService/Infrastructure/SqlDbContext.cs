using Microsoft.EntityFrameworkCore;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Infrastructure;

internal class SqlDbContext : DbContext
{
    public DbSet<TransportOption> TransportOptions { get; set; } = null!;

    public DbSet<Transport> Transport { get; set; } = null!;

    public DbSet<TransportView> TransportView { get; set; } = null!;


    public SqlDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransportOption>().ToTable("TransportOptions");
        modelBuilder.Entity<Transport>().ToTable("Transport");
        modelBuilder.Entity<TransportView>().ToTable("TransportView");
    }
}