using MassTransit;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.Common.TourOperator.Contract.Query;
using TripBooker.TourOperator.Model.Extensions;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public;

internal class HotelUpdateQueryConsumer : IConsumer<HotelUpdateQuery>
{
    private readonly IHotelOccupationViewRepository _repository;
    private readonly ILogger<HotelUpdateQueryConsumer> _logger; 
    private readonly IBus _bus;

    public HotelUpdateQueryConsumer(IHotelOccupationViewRepository repository, ILogger<HotelUpdateQueryConsumer> logger, IBus bus)
    {
        _repository = repository;
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<HotelUpdateQuery> context)
    {
        _logger.LogInformation("Query for hotel update received");

        var hotelDays = await _repository.GetByHotelIdAndDatesAsync(
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

        await _bus.Publish(new HotelUpdateContract()
        {
            HotelId = context.Message.HotelId,
            HotelDays = hotelDays.Select(x => x.Id).ToList(),
            PriceModifierFactor = context.Message.PriceModifierFactor,
            RoomsStudioChange = Math.Max(context.Message.RoomsStudioChange, -minvals.RoomsStudio),
            RoomsSmallChange = Math.Max(context.Message.RoomsSmallChange, -minvals.RoomsSmall),
            RoomsMediumChange = Math.Max(context.Message.RoomsMediumChange, -minvals.RoomsMedium),
            RoomsLargeChange = Math.Max(context.Message.RoomsLargeChange, -minvals.RoomsLarge),
            RoomsApartmentChange = Math.Max(context.Message.RoomsApartmentChange, -minvals.RoomsApartment),
        }, context.CancellationToken);

        _logger.LogInformation($"Query for hotel update consumed, update for: HotelId = {context.Message.HotelId}, StartDate = {context.Message.StartDate}, Length = {context.Message.Length}");
    }
}
