﻿using MassTransit;
using TripBooker.Common.Order.Hotel;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class ConfirmReservationEventConsumer : IConsumer<ConfirmHotelReservation>
{
    private readonly IHotelReservationService _reservationService;
    private readonly ILogger<ConfirmReservationEventConsumer> _logger;

    public ConfirmReservationEventConsumer(
        IHotelReservationService reservationService, 
        ILogger<ConfirmReservationEventConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ConfirmHotelReservation> context)
    {
        _logger.LogInformation(
            $"Received confirm hotel reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");

        await _reservationService.Confirm(context.Message.ReservationId, context.CancellationToken);

        _logger.LogInformation(
            $"Consumed confirm hotel reservation for order (OrderId={context.Message.CorrelationId}, " +
            $"ReservationId={context.Message.ReservationId}).");
    }
}
