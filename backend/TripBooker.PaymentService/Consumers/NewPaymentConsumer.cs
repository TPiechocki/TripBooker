using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.PaymentService.Repositories;
using TripBooker.PaymentService.Model.Events.Payment;

namespace TripBooker.PaymentService.Consumers;

internal class NewPaymentConsumer : IConsumer<NewPayment>
{
    private readonly ILogger<NewPaymentConsumer> _logger;
    private readonly IPaymentEventRepository _paymentEventRepository;

    public NewPaymentConsumer(
        IPaymentEventRepository paymentEventRepository,
        ILogger<NewPaymentConsumer> logger)
    {
        _paymentEventRepository = paymentEventRepository;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NewPayment> context)
    {
        _logger.LogInformation($"Received new payment for order (OrderId={context.Message.CorrelationId}).");

        var data = new NewPaymentEventData(context.Message.Price);
        _paymentEventRepository.AddNewAsync(data, context.CancellationToken);

        _logger.LogInformation($"New payment persisted in database for order (OrderId={context.Message.CorrelationId}).");
        return Task.CompletedTask;
    }
}