﻿using Newtonsoft.Json;
using TripBooker.Common;
using TripBooker.Common.Payment;
using TripBooker.PaymentService.Model.Events.Payment;

namespace TripBooker.PaymentService.Model.Events;

internal static class PaymentBuilder
{
    public static PaymentModel Build(IEnumerable<BaseEvent> events)
    {
        var item = new PaymentModel();

        foreach (var @event in events)
        {
            switch (@event.Type)
            {
                case nameof(NewPaymentEventData):
                    item.ApplyNew(@event);
                    break;

                case nameof(PaymentAcceptedEventData):
                    item.ApplyAccepted(@event);
                    break;

                case nameof(PaymentRejectedEventData):
                    item.ApplyRejected(@event);
                    break;

                case nameof(PaymentTimeoutEventData):
                    item.ApplyTimeout(@event);
                    break;

                case nameof(PaymentInProgressEventData):
                    item.ApplyInProgress(@event);
                    break;
            }
        }

        return item;
    }

    private static void ApplyNew(this PaymentModel item, BaseEvent @event)
    {
        var data = JsonConvert.DeserializeObject<NewPaymentEventData>(@event.Data)!;

        item.Id = @event.StreamId;
        item.Version = @event.Version;

        item.Price = data.Price;

        item.Status = PaymentStatus.New;
    }

    private static void ApplyAccepted(this PaymentModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = PaymentStatus.Accepted;
    }

    private static void ApplyRejected(this PaymentModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = PaymentStatus.Rejected;
    }

    private static void ApplyTimeout(this PaymentModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = PaymentStatus.Timeout;
    }

    private static void ApplyInProgress(this PaymentModel item, BaseEvent @event)
    {
        item.Version = @event.Version;

        item.Status = PaymentStatus.InProgress;
    }
}
