using TripBooker.HotelService.Model.Events.Hotel;
using TripBooker.HotelService.Model.Extensions;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelService
{
    Task AddNewHotelDay(DateTime day, CancellationToken cancellationToken, int days = 1);
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

    public async Task AddNewHotelDay(DateTime day, CancellationToken cancellationToken, int days = 1)
    {
        var hotels = await _hotelOptionRepository.QuerryAllAsync(cancellationToken);
        var events = new List<NewHotelDayEventData>();

        for (int i = 0; i < days; i++)
        {
            foreach (var hotel in hotels)
            {
                events.Add(HotelExtensions.MapToNewHotelDayEventData(day, hotel));
            }

            day = day.AddDays(1);
        }

        await _eventRepository.AddNewRangeAsync(events, cancellationToken);
    }
}
