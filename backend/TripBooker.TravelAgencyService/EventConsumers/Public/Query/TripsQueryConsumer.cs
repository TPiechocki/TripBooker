using MassTransit;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.TravelAgencyService.Services;

namespace TripBooker.TravelAgencyService.EventConsumers.Public.Query;

internal class TripsQueryConsumer : IConsumer<TripsQueryContract>
{
    private readonly ILogger<TripsQueryConsumer> _logger;
    private readonly ITripsService _service;

    public TripsQueryConsumer(
        ILogger<TripsQueryConsumer> logger,
        ITripsService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Consume(ConsumeContext<TripsQueryContract> context)
    {
        _logger.LogInformation("Trips query received.");

        var result = await _service.GetTrips(context.Message, context.CancellationToken);

        await context.RespondAsync(new TripsQueryResult(result));
        
        _logger.LogInformation("Trips query handled.");
    }
}