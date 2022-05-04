using Microsoft.EntityFrameworkCore;
using TripBooker.Common.Extensions;
using TripBooker.Common.Helpers;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.Services;

internal interface ITripsService
{
    Task<IEnumerable<TripDescription>> GetTrips(TripsQueryContract query, CancellationToken cancellationToken);
}

internal class TripsService : ITripsService
{
    private readonly IHotelOccupationViewRepository _hotelRepository;
    private readonly ITransportViewRepository _transportRepository;

    public TripsService(
        IHotelOccupationViewRepository hotelRepository, 
        ITransportViewRepository transportRepository)
    {
        _hotelRepository = hotelRepository;
        _transportRepository = transportRepository;
    }

    public async Task<IEnumerable<TripDescription>> GetTrips(TripsQueryContract query,
        CancellationToken cancellationToken)
    {
        var allDates = DateTimeHelpers.GetDaysBetween(
                query.DepartureDate,
                DateTime.SpecifyKind(query.DepartureDate + TimeSpan.FromDays(query.NumberOfDays), DateTimeKind.Utc))
            .ToList();

        // find and validate both transports
        TransportModel? flight = null, returnFlight = null;
        if (query.DepartureAirportCode != null)
        {
            flight = await GetFlights(query, cancellationToken);
            if (flight == null || flight.AvailablePlaces < query.NumberOfOccupiedSeats())
            {
                return Enumerable.Empty<TripDescription>();
            }

            returnFlight = await GetReturnFlight(query, allDates.Last(), cancellationToken);
            if (returnFlight == null || returnFlight.AvailablePlaces < query.NumberOfOccupiedSeats())
            {
                return Enumerable.Empty<TripDescription>();
            }
        }

        var availableHotels = await GetAvailableHotels(query, allDates, cancellationToken);
        if (!availableHotels.Any())
        {
            // set error message with the empty reason
            return Enumerable.Empty<TripDescription>();
        }

        // TODO: include minimal price for hotel in the offer
        var minimalPrice = 
            (flight?.TicketPrice ?? 0 + returnFlight?.TicketPrice ?? 0) * query.NumberOfOccupiedSeats();

        return availableHotels.Select(x => new TripDescription(x.HotelCode, x.HotelName, minimalPrice)).ToList();
    }

    private async Task<TransportModel?> GetReturnFlight(TripsQueryContract query, DateTime returnDate,
        CancellationToken cancellationToken)
    {
        return await _transportRepository.QueryAll()
            .Where(x => x.DepartureDate == returnDate &&
                        x.DestinationAirportCode == query.DepartureAirportCode &&
                        x.DepartureAirportCode == query.AirportCode)
            .SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<TransportModel?> GetFlights(TripsQueryContract query, CancellationToken cancellationToken)
    {
        return await _transportRepository.QueryAll()
            .Where(x => x.DepartureDate == query.DepartureDate &&
                        x.DestinationAirportCode == query.AirportCode &&
                        x.DepartureAirportCode == query.DepartureAirportCode)
            .SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<IEnumerable<HotelOccupationModel>> GetAvailableHotels(TripsQueryContract query, IEnumerable<DateTime> allDates,
        CancellationToken cancellationToken)
    {
        // find hotels
        var hotels = await _hotelRepository.QueryAll()
            .Where(x => x.AirportCode == query.AirportCode && allDates.Contains(x.Date))
            .ToListAsync(cancellationToken);

        // Set minimal values for rooms occupation
        var availableHotels = new List<HotelOccupationModel>();
        foreach (var group in hotels.GroupBy(x => x.HotelId))
        {
            var hotel = group.First();
            foreach (var day in group.ToList())
            {
                hotel.RoomsLarge = Math.Min(hotel.RoomsLarge, day.RoomsLarge);
                hotel.RoomsMedium = Math.Min(hotel.RoomsMedium, day.RoomsMedium);
                hotel.RoomsSmall = Math.Min(hotel.RoomsSmall, day.RoomsSmall);
                hotel.RoomsStudio = Math.Min(hotel.RoomsStudio, day.RoomsStudio);
            }
            if (hotel.MaxNumberOfPeople >= query.NumberOfHotelPlaces())
                availableHotels.Add(hotel);
        }

        return availableHotels;
    }
}