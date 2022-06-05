using MassTransit;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.Common.TourOperator.Contract.Query;
using TripBooker.TourOperator.Model.Extensions;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public.Query;

internal class HotelUpdateQueryConsumer : IConsumer<HotelUpdateQuery>
{
    private readonly IHotelOccupationViewRepository _hotelRepository;
    private readonly IUpdatesRepository _updatesRepository;
    private readonly ILogger<HotelUpdateQueryConsumer> _logger;
    private readonly IRequestClient<HotelUpdateContract> _hotelRequestClient;
    
    public HotelUpdateQueryConsumer(
        IHotelOccupationViewRepository hotelRepository,
        IUpdatesRepository updatesRepository,
        ILogger<HotelUpdateQueryConsumer> logger, 
        IRequestClient<HotelUpdateContract> hotelRequestClient)
    {
        _hotelRepository = hotelRepository;
        _updatesRepository = updatesRepository;
        _logger = logger;
        _hotelRequestClient = hotelRequestClient;
    }

    public async Task Consume(ConsumeContext<HotelUpdateQuery> context)
    {
        _logger.LogInformation("Query for hotel update received");

        var hotelDays = await _hotelRepository.GetByHotelIdAndDatesAsync(
            context.Message.HotelId,
            context.Message.StartDate,
            context.Message.StartDate.AddDays(context.Message.Length),
            context.CancellationToken);

        if(hotelDays == null || !hotelDays.Any())
        {
            _logger.LogError($"Could not locate hotel days with HotelId = {context.Message.HotelId} and dates from {context.Message.StartDate} to {context.Message.StartDate.AddDays(context.Message.Length)}!");
            return;
        }

        var minvals = hotelDays.Reduce();

        var update = new HotelUpdateContract
        {
            HotelId = context.Message.HotelId,
            HotelDays = hotelDays.Select(x => x.Id).ToList(),
            PriceModifierFactor = context.Message.PriceModifierFactor,
            RoomsStudioChange = Math.Max(context.Message.RoomsStudioChange, -minvals.RoomsStudio),
            RoomsSmallChange = Math.Max(context.Message.RoomsSmallChange, -minvals.RoomsSmall),
            RoomsMediumChange = Math.Max(context.Message.RoomsMediumChange, -minvals.RoomsMedium),
            RoomsLargeChange = Math.Max(context.Message.RoomsLargeChange, -minvals.RoomsLarge),
            RoomsApartmentChange = Math.Max(context.Message.RoomsApartmentChange, -minvals.RoomsApartment),
        };

        var updateResponse = await _hotelRequestClient.GetResponse<HotelUpdateResponse>(update);
        await _updatesRepository.AddAsync(update.Describe(context.Message.StartDate, updateResponse.Message),
            context.CancellationToken);

        _logger.LogInformation($"Query for hotel update consumed, update for: HotelId = {context.Message.HotelId}, StartDate = {context.Message.StartDate}, Length = {context.Message.Length}");
    }
}
