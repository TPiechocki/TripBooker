﻿using MassTransit;
using TripBooker.Common.Order.Transport;
using TripBooker.Common;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class NewTransportReservationEventConsumer : IConsumer<NewTransportReservation>
{
    private readonly ITransportReservationService _reservationService;
    private readonly IBus _bus;

    public NewTransportReservationEventConsumer(
        ITransportReservationService reservationService, 
        IBus bus)
    {
        _reservationService = reservationService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<NewTransportReservation> context)
    {
        try
        {
            var result =
                await _reservationService.AddNewReservation(context.Message, context.CancellationToken);

            if (result.Status == ReservationStatus.Accepted)
            {
                await context.Publish(
                    new TransportReservationAccepted(context.Message.Order.OrderId, result.Price, result.Id),
                    context.CancellationToken);
                await _bus.Publish(new TransportViewUpdateEvent(), context.CancellationToken);
            }
            else
            {
                await context.Publish(new TransportReservationRejected(context.Message.Order.OrderId, result.Id), context.CancellationToken);
            }
        }
        catch
        {
            await context.Publish(new TransportReservationRejected(context.Message.Order.OrderId, null), context.CancellationToken);
        }
    }
}