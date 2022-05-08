using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TripBooker.PaymentService.Infrastructure;
using TripBooker.PaymentService.Model.Events.Payment;

namespace TripBooker.PaymentService.Repositories;

internal interface IPaymentEventRepository
{
    Task<Guid> AddNewAsync(NewPaymentEventData paymentEvent, CancellationToken cancellationToken);

    Task AddAcceptedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken);

    Task AddRejectedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken);

    Task AddTimeoutAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken);

    Task<ICollection<PaymentEvent>> GetPaymentEvents(Guid streamId, CancellationToken cancellationToken);
}

internal class PaymentEventRepository : IPaymentEventRepository
{
    private readonly PaymentDbContext _dbContext;
    private readonly ILogger<PaymentEventRepository> _logger;
    private readonly IBus _bus;

    public PaymentEventRepository(
        PaymentDbContext dbContext,
        ILogger<PaymentEventRepository> logger,
        IBus bus)
    {
        _dbContext = dbContext;
        _logger = logger;
        _bus = bus;
    }

    public async Task<Guid> AddNewAsync(NewPaymentEventData paymentEvent, CancellationToken cancellationToken)
    {
        var guid = Guid.NewGuid();
        await _dbContext.PaymentEvent.AddAsync(new PaymentEvent(
                guid, 1, nameof(NewPaymentEventData), paymentEvent),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a new payment event: {JsonConvert.SerializeObject(paymentEvent)}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }

        return guid;
    }

    public async Task AddAcceptedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken)
    {
        await _dbContext.PaymentEvent.AddAsync(new PaymentEvent(
                streamId, previousVersion + 1, nameof(PaymentAcceptedEventData), new PaymentAcceptedEventData()),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add an accept payment event: streamId={streamId}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddRejectedAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken)
    {
        await _dbContext.PaymentEvent.AddAsync(new PaymentEvent(
                streamId, previousVersion + 1, nameof(PaymentRejectedEventData), new PaymentRejectedEventData()),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a reject payment event: streamId={streamId}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task AddTimeoutAsync(Guid streamId, int previousVersion, CancellationToken cancellationToken)
    {
        await _dbContext.PaymentEvent.AddAsync(new PaymentEvent(
                streamId, previousVersion + 1, nameof(PaymentRejectedEventData), new PaymentTimeoutEventData()),
            cancellationToken);

        var status = await _dbContext.SaveChangesAsync(cancellationToken);
        if (status == 0)
        {
            var message = $"Could not add a timeout payment event: streamId={streamId}";
            _logger.LogError(message);
            throw new DbUpdateException(message);
        }
    }

    public async Task<ICollection<PaymentEvent>> GetPaymentEvents(Guid streamId, CancellationToken cancellationToken)
    {
        return await _dbContext.PaymentEvent
            .Where(x => x.StreamId == streamId)
            .OrderBy(x => x.Version)
            .ToListAsync(cancellationToken);
    }
}
