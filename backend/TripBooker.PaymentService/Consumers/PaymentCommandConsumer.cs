using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.Common.Payment;

namespace TripBooker.PaymentService.Consumers;

public class PaymentCommandConsumer : IConsumer<PaymentCommand>
{
    private readonly ILogger<PaymentCommandConsumer> _logger;
    private readonly IBus _bus;

    public PaymentCommandConsumer(
        ILogger<PaymentCommandConsumer> logger, 
        IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<PaymentCommand> context)
    {
        _logger.LogInformation($"Received payment action for order (OrderId={context.Message.CorrelationId}).");

        // TODO: simulate payment process and reject it sometimes and persist the new status in the database

        await _bus.Publish(new PaymentAccepted(context.Message.CorrelationId, 0), context.CancellationToken);

        _logger.LogInformation($"Accepted payment action for order (OrderId={context.Message.CorrelationId}).");
    }
}