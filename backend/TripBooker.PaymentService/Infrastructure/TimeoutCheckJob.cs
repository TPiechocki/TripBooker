using MassTransit;
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

    public TimeoutCheckJob(IBus bus,
                           ITimeoutTimestampRepository timestampRepository,
                           IPaymentEventRepository eventRepository)
    {
        _bus = bus;
        _timestampRepository = timestampRepository;
        _eventRepository = eventRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var timestamps = await _timestampRepository.QueryAll(context.CancellationToken);

        foreach (var timestamp in timestamps)
        {
            if (timestamp.Timestamp.AddMinutes(1) < DateTime.UtcNow)
            {
                var events = await _eventRepository.GetPaymentEvents(timestamp.Id, context.CancellationToken);
                var payment = PaymentBuilder.Build(events);

                if (payment.Status != PaymentStatus.Accepted)
                {
                    await _eventRepository.AddTimeoutAsync(payment.Id, payment.Version, context.CancellationToken);
                    await _bus.Publish(new PaymentTimeout(payment.Id));
                }

                _timestampRepository.Remove(timestamp);
            }
        }
    }
}
