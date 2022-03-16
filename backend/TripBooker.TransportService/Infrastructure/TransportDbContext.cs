using Microsoft.EntityFrameworkCore;
using TripBooker.Common;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Infrastructure;

internal class TransportDbContext : DbContext
{
    public DbSet<TransportOption> TransportOption { get; set; } = null!;


    // EVENTS
    public DbSet<TransportEvent> TransportEvent { get; set; } = null!;

    public DbSet<ReservationEvent> ReservationEvent { get; set; } = null!;


    // VIEWS
    public DbSet<TransportModel> TransportView { get; set; } = null!;


    public TransportDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TransportEvent>()
            .Property(x => x.Timestamp)
            .HasDefaultValueSql("now() at time zone 'utc'");
        modelBuilder.Entity<ReservationEvent>()
            .Property(x => x.Timestamp)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }
}

internal class TransportEvent : BaseEvent
{
    public TransportEvent(Guid streamId, int version, string type, object data) 
        : base(streamId, version, type, data)
    {
    }

    public TransportEvent(Guid streamId, int version, string type, string data) 
        : base(streamId, version, type, data)
    {
    }
}

internal class ReservationEvent : BaseEvent
{
    public ReservationEvent(Guid streamId, int version, string type, object data)
        : base(streamId, version, type, data)
    {
    }

    public ReservationEvent(Guid streamId, int version, string type, string data) 
        : base(streamId, version, type, data)
    {
    }
}