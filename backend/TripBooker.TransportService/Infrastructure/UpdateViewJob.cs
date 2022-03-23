using MassTransit;
using Quartz;
using TripBooker.TransportService.Model.Events;

namespace TripBooker.TransportService.Infrastructure;

internal class UpdateViewJob : IJob
{
    private readonly IBus _bus;

    public UpdateViewJob(IBus bus)
    {
        _bus = bus;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _bus.Publish(new TransportViewUpdateEvent());
        return Task.CompletedTask;
    }
}