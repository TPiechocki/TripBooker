using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Quartz;
using TripBooker.Common;
using TripBooker.Common.Order.Payment;
using TripBooker.PaymentService.Model.Events;
using TripBooker.PaymentService.Repositories;

namespace TripBooker.PaymentService.Infrastructure;

internal class TimeoutCheckJob : IJob
{
    private readonly IBus _bus;
    private readonly IPaymentEventRepository _eventRepository;
    private readonly ITimeoutTimestampRepository _timestampRepository;
    private readonly ILogger<TimeoutCheckJob> _logger;

    public TimeoutCheckJob(IBus bus,
                           ITimeoutTimestampRepository timestampRepository,
                           IPaymentEventRepository eventRepository, 
                           ILogger<TimeoutCheckJob> logger)
    {
        _bus = bus;
        _timestampRepository = timestampRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var timestamps = await _timestampRepository.QueryAllOlderThan1Minute(context.CancellationToken);

        foreach (var timestamp in timestamps)
        {
            var tryTransaction = true;
            while (tryTransaction)
            {
                tryTransaction = false;

                var events = await _eventRepository.GetPaymentEvents(timestamp.Id, context.CancellationToken);
                var payment = PaymentBuilder.Build(events);

                if (payment.Status != PaymentStatus.Accepted)
                {
                    try
                    {
                        if (payment.Status != PaymentStatus.Accepted)
                        {
                            await _eventRepository.AddTimeoutAsync(payment.Id, payment.Version,
                                context.CancellationToken);
                            await _bus.Publish(new PaymentTimeout(payment.Id));
                            _logger.LogInformation($"Payment timed out (OrderId={payment.Id})");
                        }
                    }

                    catch (DbUpdateException e)
                    {
                        if (e.GetBaseException() is PostgresException
                            {
                                SqlState: GlobalConstants.PostgresUniqueViolationCode
                            })
                        {
                            // repeat if there was version violation, so the db read and business logic
                            // does not need to be inside transaction
                            tryTransaction = true;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                _timestampRepository.Remove(timestamp);
            }
        }
    }
}
