using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model.Extensions;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal interface ITransportService
{
    Task<Guid> AddNewTransport(NewTransportContract transport, CancellationToken cancellationToken);
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

    public async Task<Guid> AddNewTransport(NewTransportContract transport, CancellationToken cancellationToken)
    {
        var transportOption = await _optionRepository.GetById(transport.TransportOptionId);
        if (transportOption == null)
        {
            throw new ArgumentException($"Option id for transport does not exist (optionId={transport.TransportOptionId}",
                nameof(transport));
        }

        var newTransportEvent = transport.MapToNewTransportEventData(transportOption);

        var guid = await _eventRepository.AddNewAsync(
            newTransportEvent, cancellationToken);

        // TODO: update view based on event table with internal bus

        return guid;
    }
}