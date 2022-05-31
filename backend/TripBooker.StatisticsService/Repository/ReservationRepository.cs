using Microsoft.EntityFrameworkCore;
using TripBooker.StatisticsService.Infrastructure;
using TripBooker.StatisticsService.Model;

namespace TripBooker.StatisticsService.Repository;

internal interface IReservationRepository
{
    Task AddNewReservation(ReservationModel reservation, CancellationToken cancellationToken);

    IQueryable<ReservationModel> QueryAll();

    Task<ICollection<ReservationModel>> RemoveOlderThan(DateTime threshold, CancellationToken cancellationToken);

    Task UpdateTimeStamp(Guid orderId, CancellationToken cancellationToken);
    Task<ReservationModel> Remove(Guid orderId, CancellationToken cancellationToken);
}

internal class ReservationRepository : IReservationRepository
{
    private readonly StatisticsDbContext _context;

    public ReservationRepository(StatisticsDbContext context)
    {
        _context = context;
    }

    public async Task AddNewReservation(ReservationModel reservation, CancellationToken cancellationToken)
    {
        await _context.Reservation.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<ReservationModel> QueryAll()
    {
        return _context.Reservation.Select(x => x);
    }

    public async Task<ReservationModel> Remove(Guid orderId, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Reservation.FindAsync(new object?[] {orderId}, cancellationToken);

        _context.Remove(entity!);
        await _context.SaveChangesAsync(cancellationToken);

        return entity!;
    }

    public async Task<ICollection<ReservationModel>> RemoveOlderThan(DateTime threshold,
        CancellationToken cancellationToken)
    {
        var entities = await _context.Reservation.Where(x => x.TimeStamp < threshold)
            .ToListAsync(cancellationToken);

        _context.RemoveRange(entities);
        await _context.SaveChangesAsync(cancellationToken);

        return entities;
    }

    public async Task UpdateTimeStamp(Guid orderId, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Reservation.FindAsync(new object?[] {orderId}, cancellationToken);

        entity!.TimeStamp = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}