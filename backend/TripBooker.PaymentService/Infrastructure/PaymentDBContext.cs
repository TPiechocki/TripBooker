using Microsoft.EntityFrameworkCore;
using TripBooker.Common;

namespace TripBooker.PaymentService.Infrastructure;

internal class PaymentDbContext : DbContext
{

    // EVENTS
    public DbSet<PaymentEvent> PaymentEvent { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentEvent>()
            .Property(x => x.Timestamp)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }

    public PaymentDbContext(DbContextOptions options) : base(options)
    {
    }
}

internal class PaymentEvent : BaseEvent
{
    public PaymentEvent(Guid streamId, int version, string type, object data)
        : base(streamId, version, type, data)
    {
    }

    public PaymentEvent(Guid streamId, int version, string type, string data)
        : base(streamId, version, type, data)
    {
    }
}
