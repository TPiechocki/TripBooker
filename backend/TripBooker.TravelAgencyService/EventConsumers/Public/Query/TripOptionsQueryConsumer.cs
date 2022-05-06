using MassTransit;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.TravelAgencyService.Services;

namespace TripBooker.TravelAgencyService.EventConsumers.Public.Query;

internal class TripOptionsQueryConsumer : IConsumer<TripOptionsQueryContract>
{
    private readonly ILogger<TripOptionsQueryConsumer> _logger;
    private readonly ITripsService _service;

    public TripOptionsQueryConsumer(
        ILogger<TripOptionsQueryConsumer> logger, 
        ITripsService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Consume(ConsumeContext<TripOptionsQueryContract> context)
    {
        _logger.LogInformation("Trip options query received.");

        var result = await _service.GetTripOptions(context.Message, context.CancellationToken);

        await context.RespondAsync(result);

        _logger.LogInformation("Trip options query handled.");
    }
}