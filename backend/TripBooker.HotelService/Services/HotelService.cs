using TripBooker.HotelService.Model.Extensions;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelService
{
    Task AddNewHotelDay(DateTime day, CancellationToken cancellationToken);
}
internal class HotelService : IHotelService
{
    private readonly HotelEventRepository _eventRepository;
    private readonly HotelOptionRepository _hotelOptionRepository;

    public HotelService(HotelEventRepository eventRepository, HotelOptionRepository hotelOptionRepository)
    {
        _eventRepository = eventRepository;
        _hotelOptionRepository = hotelOptionRepository;
    }

    public async Task AddNewHotelDay(DateTime day, CancellationToken cancellationToken)
    {
        var hotels = await _hotelOptionRepository.QuerryAllAsync(cancellationToken);

        foreach (var hotel in hotels)
        {
            var newHotelDayEvent = HotelExtensions.MapToNewHotelDayEventData(day, hotel);
            await _eventRepository.AddNewAsync(newHotelDayEvent, cancellationToken);
        }
    }
}
