using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;
using TripBooker.Common;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Model.Events.Transport;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.EventConsumers.Public;

internal class TransportUpdateEventConsumer : IConsumer<TransportUpdateContract>
{
    private readonly ILogger<NewTransportReservationEventConsumer> _logger;
    private readonly ITransportEventRepository _eventRepository;
    private readonly ITransportOptionRepository _transportOptionRepository;

    public TransportUpdateEventConsumer(
        ILogger<NewTransportReservationEventConsumer> logger,
        ITransportEventRepository eventRepository, 
        ITransportOptionRepository transportOptionRepository)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _transportOptionRepository = transportOptionRepository;
    }

    public async Task Consume(ConsumeContext<TransportUpdateContract> context)
    {
        _logger.LogInformation($"Transport Update Contract recieved (TransportId = {context.Message.Id})");

        var tryTransaction = true;
        var transportDescription = string.Empty;
        while (tryTransaction)
        {
            tryTransaction = false;

            var transportEvents =
                await _eventRepository.GetTransportEventsAsync(context.Message.Id, context.CancellationToken);
            if (transportEvents == null || transportEvents.Count == 0)
            {
                _logger.LogInformation($"Could not locate Transport with Id = {context.Message.Id}");
                break;
            }

            var transportItem = TransportBuilder.Build(transportEvents);

            try
            {
                await ValidateTransportUpdateTransaction(context.Message, transportItem, context.CancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
                {
                    // repeat if there was version violation, so the db read and business logic
                    // does not need to be inside transaction
                    tryTransaction = true;
                }
                else
                {
                    throw;
                }
            }
            
            var transportOption = await _transportOptionRepository.GetById(transportItem.TransportOptionId);
            transportDescription =
                $"from {transportOption!.DepartureAirportName} to {transportOption.DestinationAirportName} " +
                $"on {transportItem.DepartureDate:yyyy-MM-dd}";

        }
        
        _logger.LogInformation($"Transport Update Contract consumed (TransportId = {context.Message.Id}");

        await context.RespondAsync(new TransportUpdateResponse(transportDescription));
    }

    private async Task ValidateTransportUpdateTransaction(TransportUpdateContract contract, TransportModel transport,
        CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var transportEvent = new TransportPlaceUpdateEvent(
            transport.AvailablePlaces + contract.AvailablePlacesChange,
            contract.AvailablePlacesChange,
            Guid.Empty);

        await _eventRepository.AddAsync(transportEvent, transport.Id, transport.Version, cancellationToken, !contract.PriceChangedFlag);

        if (contract.PriceChangedFlag)
        {
            var priceEvent = new TicketPriceUpdateEvent(contract.NewTicketPrice);

            await _eventRepository.AddAsync(priceEvent, transport.Id, transport.Version + 1, cancellationToken);
        }

        transaction.Complete();
    }
}
