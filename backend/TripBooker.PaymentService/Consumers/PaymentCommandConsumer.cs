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

internal class PaymentCommandConsumer : IConsumer<PaymentCommand>
{
    private readonly ILogger<PaymentCommandConsumer> _logger;
    private readonly IBus _bus;
    private readonly IPaymentEventRepository _repository;

    public PaymentCommandConsumer(
        ILogger<PaymentCommandConsumer> logger, 
        IBus bus, 
        IPaymentEventRepository repository)
    {
        _logger = logger;
        _bus = bus;
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<PaymentCommand> context)
    {
        _logger.LogInformation($"Received payment action for order (OrderId={context.Message.CorrelationId}).");

        var result = await AddInProgress(context.Message.CorrelationId, context.CancellationToken);
        if (result == false)
        {
            _logger.LogInformation($"Payment is already in progress, was completed or timed out. (OrderId={context.Message.CorrelationId}).");
            return;
        }

        var rd = new Random();
        await Task.Delay(rd.Next(2, 10) * 1000, context.CancellationToken);

        if (rd.NextDouble() < 0.2)
        {
            await AddRejectedAsync(context.Message.CorrelationId, context.CancellationToken);
            _logger.LogInformation($"Payment rejected for order (OrderId={context.Message.CorrelationId}).");
        }
        else
        {
            await AddAcceptedAsync(context.Message.CorrelationId, context.CancellationToken);
            _logger.LogInformation($"Payment accepted for order (OrderId={context.Message.CorrelationId}).");
        }
    }

    private async Task<bool> AddInProgress(Guid reservationId, CancellationToken cancellationToken)
    {
        var tryTransaction = true;
        while (tryTransaction)
        {
            tryTransaction = false;

            var payment = await GetModel(reservationId, cancellationToken);

            if (payment.Status is PaymentStatus.InProgress or PaymentStatus.Accepted or PaymentStatus.Timeout)
            {
                return false;
            }

            try
            {
                await _repository.AddInProgressAsync(payment.Id, payment.Version,
                    cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
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

        return true;
    }

    private async Task AddRejectedAsync(Guid reservationId, CancellationToken cancellationToken)
    {
        var tryTransaction = true;
        while (tryTransaction)
        {
            tryTransaction = false;

            var payment = await GetModel(reservationId, cancellationToken);

            if (payment.Status == PaymentStatus.Timeout)
            {
                // skip for timed out payment
                return;
            }

            if (payment.Status != PaymentStatus.InProgress)
            {
                throw new InvalidOperationException($"Cannot reject payment which is not in progress (OrderId={reservationId}.");
            }

            try
            {
                await _repository.AddRejectedAsync(payment.Id, payment.Version,
                    cancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
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
    }

    private async Task AddAcceptedAsync(Guid reservationId, CancellationToken cancellationToken)
    {
        var tryTransaction = true;
        while (tryTransaction)
        {
            tryTransaction = false;

            var payment = await GetModel(reservationId, cancellationToken);

            if (payment.Status == PaymentStatus.Timeout)
            {
                // skip for timed out payment
                return;
            }

            if (payment.Status != PaymentStatus.InProgress)
            {
                throw new InvalidOperationException($"Cannot accept payment which is not in progress (OrderId={reservationId}.");
            }

            try
            {
                await _repository.AddAcceptedAsync(payment.Id, payment.Version,
                    cancellationToken);
                await _bus.Publish(new PaymentAccepted(reservationId), cancellationToken);

            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
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
    }

    private async Task<PaymentModel> GetModel(Guid id, CancellationToken cancellationToken)
    {
        var paymentEvents =
            await _repository.GetPaymentEvents(id, cancellationToken);
        return PaymentBuilder.Build(paymentEvents);
    }
}