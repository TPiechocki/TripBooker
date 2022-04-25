using TripBooker.HotelService.Model.Events.Hotel;
using TripBooker.HotelService.Model.Extensions;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelService
{
    Task AddNewHotelDay(DateTime day, CancellationToken cancellationToken);
}
internal class HotelService : IHotelService
{
    private readonly IHotelEventRepository _eventRepository;
    private readonly IHotelOptionRepository _hotelOptionRepository;

    public HotelService(IHotelEventRepository eventRepository, IHotelOptionRepository hotelOptionRepository)
    {
        _eventRepository = eventRepository;
        _hotelOptionRepository = hotelOptionRepository;
    }

    public async Task AddNewHotelDay(DateTime day, CancellationToken cancellationToken)
    {
        var hotels = await _hotelOptionRepository.QuerryAllAsync(cancellationToken);
        var events = new List<NewHotelDayEventData>();

        foreach (var hotel in hotels)
        {
            events.Add(HotelExtensions.MapToNewHotelDayEventData(day, hotel));
        }

        await _eventRepository.AddNewRangeAsync(events, cancellationToken);
    }
}
