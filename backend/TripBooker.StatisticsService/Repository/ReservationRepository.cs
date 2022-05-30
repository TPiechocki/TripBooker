using TripBooker.StatisticsService.Infrastructure;
using TripBooker.StatisticsService.Model;

namespace TripBooker.StatisticsService.Repository;

internal interface IReservationRepository
{
    Task AddNewReservation(ReservationModel reservation, CancellationToken cancellationToken);

    IQueryable<ReservationModel> QueryAll();
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
}