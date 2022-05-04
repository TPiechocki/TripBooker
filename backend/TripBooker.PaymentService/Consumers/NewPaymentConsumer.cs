using MassTransit;
using TripBooker.Common.Order.Payment;

namespace TripBooker.PaymentService.Consumers;

internal class NewPaymentConsumer : IConsumer<NewPayment>
{
    private readonly ILogger<NewPaymentConsumer> _logger;

    public NewPaymentConsumer(ILogger<NewPaymentConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NewPayment> context)
    {
        _logger.LogInformation($"Received new payment for order (OrderId={context.Message.CorrelationId}).");

        // TODO: save new payment in the database

        _logger.LogInformation($"New payment persisted in database for order (OrderId={context.Message.CorrelationId}).");
        return Task.CompletedTask;
    }
}