﻿using MassTransit;
using Quartz;
using TripBooker.HotelService.Model.Events;

namespace TripBooker.HotelService.Infrastructure;

internal class UpdateViewJob : IJob
{
    private readonly IBus _bus;

    public UpdateViewJob(IBus bus)
    {
        _bus = bus;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _bus.Publish(new OccupationViewUpdateEvent());
        return Task.CompletedTask;
    }
}
