﻿using Microsoft.EntityFrameworkCore;
using TripBooker.Common;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Infrastructure;

internal class HotelDbContext : DbContext
{
    public DbSet<HotelOption> HotelOption { get; set; } = null!;

    public DbSet<RoomOption> RoomOption { get; set; } = null!;

    // EVENTS
    public DbSet<ReservationEvent> ReservationEvent { get; set; } = null!;

    public DbSet<HotelEvent> HotelEvent { get; set; } = null!;

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

        modelBuilder.Entity<HotelOccupationModel>()
            .HasKey(c => new { c.HotelId, c.Date });
    }

    public HotelDbContext(DbContextOptions options) : base(options)
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

internal class HotelEvent : BaseEvent
{
    public HotelEvent(Guid streamId, int version, string type, object data)
        : base(streamId, version, type, data)
    {
    }

    public HotelEvent(Guid streamId, int version, string type, string data)
        : base(streamId, version, type, data)
    {
    }
}
