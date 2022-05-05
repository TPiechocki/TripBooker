﻿using MassTransit;
using TripBooker.Common.Order.Transport;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class ConfirmReservationEventConsumer : IConsumer<ConfirmTransportReservation>
{
    private readonly ITransportReservationService _reservationService;
    private readonly ILogger<ConfirmReservationEventConsumer> _logger;

    public ConfirmReservationEventConsumer(
        ITransportReservationService reservationService, 
        ILogger<ConfirmReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ConfirmTransportReservation> context)
    {
        _logger.LogInformation(
            $"Received confirm transport reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");

        await _reservationService.Confirm(context.Message.ReservationId, context.CancellationToken);

        _logger.LogInformation(
            $"Consumed confirm transport reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");
    }
}