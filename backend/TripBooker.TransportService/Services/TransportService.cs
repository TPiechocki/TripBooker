using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model.Extensions;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal interface ITransportService
{
    Task AddManyNewTransports(IEnumerable<NewTransportContract> transports, CancellationToken cancellationToken);
}

internal class TransportService : ITransportService
{
    private readonly ITransportEventRepository _eventRepository;
    private readonly ITransportOptionRepository _optionRepository;

    public TransportService(
        ITransportOptionRepository optionRepository, 
        ITransportEventRepository eventRepository)
    {
        _optionRepository = optionRepository;
        _eventRepository = eventRepository;
    }

    public async Task AddManyNewTransports(IEnumerable<NewTransportContract> transports, CancellationToken cancellationToken)
    {
        var transportOptionsIds = transports.Select(x => x.TransportOptionId).Distinct().ToList();

        var transportOptions = await _optionRepository.GetByIds(
            transportOptionsIds, cancellationToken);
        if (transportOptionsIds.Count > transportOptions.Count())
        {
            throw new ArgumentException("Option id for any of the new transports does not exist",
                nameof(transports));
        }

        var newTransportEvents = transports.Select(x => x.MapToNewTransportEventData());

        var chunks = newTransportEvents.Chunk(500);

        // add in chunks so data is partially available earlier
        foreach (var chunk in chunks)
        {
            await _eventRepository.AddManyNewAsync(
                chunk, cancellationToken);
        }
    }
}