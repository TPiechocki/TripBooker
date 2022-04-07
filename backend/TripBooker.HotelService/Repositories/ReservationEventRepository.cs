using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.HotelService.Infrastructure;
using TripBooker.HotelService.Model.Events;

namespace TripBooker.HotelService.Repositories;

internal interface IReservationEventRepository
{
    Task<Guid> AddNewAsync(NewReservationEventData reservationEvent, CancellationToken cancellationToken);

    Task AddAcceptedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken);

    Task AddRejectedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken);

    Task<ICollection<ReservationEvent>> GetReservationEvents(Guid streamId, CancellationToken cancellationToken);
}

internal class ReservationEventRepository : IReservationEventRepository
{
    private readonly HotelDbContext _dbContext;
    private readonly ILogger<ReservationEventRepository> _logger;
    private readonly IBus _bus;

    public ReservationEventRepository(
        HotelDbContext dbContext,
        ILogger<ReservationEventRepository> logger,
        IBus bus)
    {
        _dbContext = dbContext;
        _logger = logger;
        _bus = bus;
    }

    public async Task<Guid> AddNewAsync(NewReservationEventData reservationEvent, CancellationToken cancellationToken)
    {
        var guid = Guid.NewGuid();
        await _dbContext.ReservationEvent.AddAsync(new ReservationEvent(
                guid, 1, nameof(NewReservationEventData), reservationEvent),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new reservation event: {JsonConvert.SerializeObject(reservationEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        return guid;
    }

    public async Task AddAcceptedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken)
    {
        await _dbContext.ReservationEvent.AddAsync(new ReservationEvent(
                streamId, previousVersion + 1, nameof(ReservationAcceptedEventData), new ReservationAcceptedEventData()),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add an accepted reservation event: streamId={streamId}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddRejectedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken)
    {
        await _dbContext.ReservationEvent.AddAsync(new ReservationEvent(
                streamId, previousVersion + 1, nameof(ReservationRejectedEventData), new ReservationRejectedEventData()),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a rejected reservation event: streamId={streamId}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task<ICollection<ReservationEvent>> GetReservationEvents(Guid streamId, CancellationToken cancellationToken)
    {
        return await _dbContext.ReservationEvent
            .Where(x => x.StreamId == streamId)
            .OrderBy(x => x.Version)
            .ToListAsync(cancellationToken);
    }
}
