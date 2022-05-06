using MassTransit;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.TravelAgencyService.Services;

namespace TripBooker.TravelAgencyService.EventConsumers.Public.Query;

internal class TripQueryConsumer : IConsumer<TripQueryContract>
{
    private readonly ILogger<TripQueryConsumer> _logger;
    private readonly ITripsService _service;

    public TripQueryConsumer(
        ILogger<TripQueryConsumer> logger, 
        ITripsService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Consume(ConsumeContext<TripQueryContract> context)
    {
        _logger.LogInformation("Trip query received.");

        var result = await _service.GetTrip(context.Message, context.CancellationToken);

        await context.RespondAsync(result);

        _logger.LogInformation("Trip query handled.");
    }
}