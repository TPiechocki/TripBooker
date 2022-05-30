using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;
using TripBooker.Common;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Model.Events.Hotel;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.EventConsumers.Public;

internal class NewHotelUpdateEventConsumer : IConsumer<HotelUpdateContract>
{
    private readonly IHotelEventRepository _eventRepository;
    private readonly IHotelOccupationViewRepository _occupationRepository;
    private readonly ILogger<NewHotelReservationEventConsumer> _logger;

    public NewHotelUpdateEventConsumer(
        IHotelEventRepository hotelRepository,
        IHotelOccupationViewRepository occupationRepository,
        ILogger<NewHotelReservationEventConsumer> logger)
    {
        _eventRepository = hotelRepository;
        _occupationRepository = occupationRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HotelUpdateContract> context)
    {
        _logger.LogInformation($"Hotel Update Contract recieved (HotelId = {context.Message.HotelId}, number of days = {context.Message.Days.Count})");

        bool transactionSuccesfull;
        do
        {
            transactionSuccesfull = true;

            var hotelOccupations = new List<HotelOccupationModel>();

            foreach (var day in context.Message.Days)
            {
                var hotelDay = await _occupationRepository.GetByHotelIdAndDateAsync(context.Message.HotelId, day, context.CancellationToken);
                if (hotelDay != null)
                    hotelOccupations.Add(hotelDay);
                else
                    _logger.LogInformation($"Could not locate HotelOccupationModel for given values: HotelId = {context.Message.HotelId}, DayTime = {day}");
            }

            try
            {
                await ValidateHotelUpdateTransaction(context.Message, hotelOccupations, context.CancellationToken);
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException { SqlState: GlobalConstants.PostgresUniqueViolationCode })
                {
                    // repeat if there was version violation, so the db read and business logic
                    // does not need to be inside transaction
                    transactionSuccesfull = false;
                }
                else
                {
                    throw;
                }
            }

        }
        while (!transactionSuccesfull);

        // TODO: request/response to Rabbit with change confirmation

        _logger.LogInformation($"Hotel Update Contract consumed (HotelId = {context.Message.HotelId}, number of days = {context.Message.Days.Count})");
    }

    private async Task ValidateHotelUpdateTransaction(HotelUpdateContract contract, IEnumerable<HotelOccupationModel> hotelOccupations, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var updateEvent = new OccupatonUpdateEvent
        {
            ReservationEventId = Guid.Empty,
            PriceModifierFactor = contract.PriceModifierFactor,
            RoomsStudio = contract.RoomsStudioChange,
            RoomsSmall = contract.RoomsSmallChange,
            RoomsMedium = contract.RoomsMediumChange,
            RoomsLarge = contract.RoomsLargeChange,
            RoomsApartment = contract.RoomsApartmentChange,
        };

        await _eventRepository.AddToManyAsync(updateEvent, hotelOccupations.Select(x => x.Id),
            hotelOccupations.Select(x => x.Version), cancellationToken);

        transaction.Complete();
    } 
}
