using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TripBooker.Common;
using TripBooker.Common.Order.Payment;
using TripBooker.Common.Payment;
using TripBooker.PaymentService.Model;
using TripBooker.PaymentService.Model.Events;
using TripBooker.PaymentService.Repositories;

namespace TripBooker.PaymentService.Consumers;

internal class PaymentStatusQueryConsumer : IConsumer<PaymentStatusQuery>
{
    private readonly ILogger<PaymentStatusQueryConsumer> _logger;
    private readonly IPaymentEventRepository _repository;

    public PaymentStatusQueryConsumer(
        ILogger<PaymentStatusQueryConsumer> logger, 
        IPaymentEventRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<PaymentStatusQuery> context)
    {
        _logger.LogInformation($"Received payment action for order (OrderId={context.Message.CorrelationId}).");

        var model = await GetModel(context.Message.CorrelationId, context.CancellationToken);
        await context.RespondAsync(model);
       
        _logger.LogInformation($"Accepted payment action for order (OrderId={context.Message.CorrelationId}).");
    }
    
    private async Task<PaymentModel> GetModel(Guid id, CancellationToken cancellationToken)
    {
        var paymentEvents =
            await _repository.GetPaymentEvents(id, cancellationToken);
        return PaymentBuilder.Build(paymentEvents);
    }
}