using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.Extensions;
using TripBooker.Common.Helpers;
using TripBooker.Common.Hotel;
using TripBooker.Common.Hotel.Contract;
using TripBooker.Common.Payment;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.Common.TravelAgency.Model;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.Services;

internal interface ITripsService
{
    Task<TripQueryResponse> GetTrip(TripQueryContract query, CancellationToken cancellationToken);

    Task<TripOptionsQueryResponse> GetTripOptions(TripOptionsQueryContract query, CancellationToken cancellationToken);

    Task<IEnumerable<TripDescription>> GetTrips(TripsQueryContract query, CancellationToken cancellationToken);
}

internal class TripsService : ITripsService
{
    private readonly IHotelOccupationViewRepository _hotelRepository;
    private readonly ITransportViewRepository _transportRepository;
    private readonly IMapper _mapper;

    public TripsService(
        IHotelOccupationViewRepository hotelRepository, 
        ITransportViewRepository transportRepository, 
        IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _transportRepository = transportRepository;
        _mapper = mapper;
    }

    public async Task<TripQueryResponse> GetTrip(TripQueryContract query, CancellationToken cancellationToken)
    {
        var result = new TripQueryResponse
        {
            IsAvailable = false
        };
        var price = 0.0;

        if (!query.EnoughPlacesInConfiguration())
        {
            result.ValidationError = "Configuration of rooms contains too little places for the number of people";
            return result;
        }

        // FLIGHTS
        var seats = query.NumberOfOccupiedSeats();

        var flights = await _transportRepository.QueryAll()
            .Where(x => query.Flights.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (flights.Any(x => x.AvailablePlaces < seats))
        {
            result.ValidationError = "One of the flights has not enough seats";
            return result;
        }
        price += flights.Sum(x => x.TicketPrice) * seats;


        // HOTEL
        var hotelDays = await _hotelRepository.QueryAll()
            .Where(x => query.HotelDays.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (hotelDays.Any(x =>
                x.RoomsSmall < query.NumberOfSmallRooms ||
                x.RoomsMedium < query.NumberOfMediumRooms ||
                x.RoomsLarge < query.NumberOfLargeRooms ||
                x.RoomsApartment < query.NumberOfApartments ||
                x.RoomsStudio < query.NumberOfStudios))
        {
            result.ValidationError = "There is not enough rooms";
            return result;
        }

        if (query.MealOption == MealOption.AllInclusive && hotelDays.Any(x => x.AllInclusive == false))
        {
            result.ValidationError = "Hotel does not offer all inclusive";
            return result;
        }

        price += query.NumberOfSmallRooms * hotelDays.Sum(x => x.SmallPrice);
        price += query.NumberOfMediumRooms * hotelDays.Sum(x => x.MediumPrice);
        price += query.NumberOfLargeRooms * hotelDays.Sum(x => x.LargePrice);
        price += query.NumberOfApartments * hotelDays.Sum(x => x.ApartmentPrice);
        price += query.NumberOfStudios * hotelDays.Sum(x => x.StudioPrice);

        var mealPriceMultiplier = query.NumberOfAdults
                                  + query.NumberOfChildrenUpTo18
                                  * HotelConstants.MealChildren10To18PriceFactor
                                  + (query.NumberOfChildrenUpTo3 + query.NumberOfChildrenUpTo10)
                                  * HotelConstants.MealChildrenUnder10PriceFactor;
        price += query.MealOption switch
        {
            MealOption.ContinentalBreakfeast => mealPriceMultiplier * hotelDays.Sum(x => x.BreakfastPrice),
            MealOption.AllInclusive => mealPriceMultiplier * hotelDays.Sum(x => x.AllInclusivePrice),
            _ => 0
        };

        // DISCOUNT
        if (query.DiscountCode != null && Discount.IsViable(query.DiscountCode))
            price = Discount.Apply(query.DiscountCode, price);

        result.IsAvailable = true;
        result.FinalPrice = Math.Round(price, 2);

        return result;
    }

    public async Task<TripOptionsQueryResponse> GetTripOptions(TripOptionsQueryContract query, CancellationToken cancellationToken)
    {
        var result = new TripOptionsQueryResponse();

        var allDates = DateTimeHelpers.GetDaysBetween(
                query.DepartureDate,
                DateTime.SpecifyKind(query.DepartureDate + TimeSpan.FromDays(query.NumberOfDays), DateTimeKind.Utc))
            .ToList();

        // GET HOTEL INFO
        var hotelDays = await _hotelRepository.QueryAll()
            .Where(x => x.HotelCode == query.HotelCode && allDates.SkipLast(1).Contains(x.Date))
            .ToListAsync(cancellationToken);
        result.HotelDays = hotelDays.Select(x => x.Id);
        
        result.HotelAvailability = _mapper.Map<HotelOccupationViewContract>(hotelDays.ReduceHotelOccupations());

        // GET POSSIBLE FLIGHTS
        result.TransportOptions = await _transportRepository.QueryAll()
            .Where(x => x.DepartureAirportCode == hotelDays.First().AirportCode &&
                        x.DepartureDate == allDates.First())
            .ToListAsync(cancellationToken);

        // GET POSSIBLE RETURN FLIGHTS
        result.ReturnTransportOptions = await _transportRepository.QueryAll()
            .Where(x => x.DepartureAirportCode == hotelDays.First().AirportCode &&
                        x.DepartureDate == allDates.Last())
            .ToListAsync(cancellationToken);

        return result;
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
            flight = await GetFlight(query, cancellationToken);
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

        var minimalFlightPrice = 
            (flight?.TicketPrice ?? 0 + returnFlight?.TicketPrice ?? 0) * query.NumberOfOccupiedSeats();

        return availableHotels.Select(x => new TripDescription(
            x.HotelCode, x.HotelName, Math.Round(
                minimalFlightPrice + x.GetMinPrice(query.NumberOfHotelPlaces(), query.NumberOfDays), 2
            ))).ToList();
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

    private async Task<TransportModel?> GetFlight(TripsQueryContract query, CancellationToken cancellationToken)
    {
        return await _transportRepository.QueryAll()
            .Where(x => x.DepartureDate == query.DepartureDate &&
                        x.DestinationAirportCode == query.AirportCode &&
                        x.DepartureAirportCode == query.DepartureAirportCode)
            .SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<IEnumerable<HotelOccupationModel>> GetAvailableHotels(
        TripsQueryContract query, IEnumerable<DateTime> allDates, CancellationToken cancellationToken)
    {
        // find hotels
        var hotels = await _hotelRepository.QueryAll()
            .Where(x => x.AirportCode == query.AirportCode && allDates.SkipLast(1).Contains(x.Date))
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
                hotel.RoomsApartment = Math.Min(hotel.RoomsApartment, day.RoomsApartment);
            }
            if (hotel.MaxNumberOfPeople >= query.NumberOfHotelPlaces())
                availableHotels.Add(hotel);
        }

        return availableHotels;
    }
}