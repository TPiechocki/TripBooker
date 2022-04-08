using MassTransit;
using TripBooker.Common.TravelAgency.Folder.Query;
using TripBooker.TravelAgencyService.Services;

namespace TripBooker.TravelAgencyService.EventConsumers.Public.Query;

internal class DestinationsQueryConsumer : IConsumer<DestinationsQueryContract>
{
    private readonly ILogger<DestinationsQueryConsumer> _logger;
    private readonly IDestinationsService _service;

    public DestinationsQueryConsumer(
        ILogger<DestinationsQueryConsumer> logger, 
        IDestinationsService service)
    {
        _logger = logger;
        _service = service;
    }

    public async Task Consume(ConsumeContext<DestinationsQueryContract> context)
    {
        _logger.LogInformation("Destinations query received.");

        var destinations = await _service.GetAll(context.CancellationToken);

        await context.RespondAsync(new DestinationsQueryResultContract(
            destinations
        ));

        _logger.LogInformation("Destinations query handled.");
    }
}