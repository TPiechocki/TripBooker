using Microsoft.EntityFrameworkCore;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.Services;

internal interface IDestinationsService
{
    Task<IEnumerable<DestinationContract>> GetAll(CancellationToken cancellationToken);
}

internal class DestinationsService : IDestinationsService
{
    private readonly IHotelOccupationViewRepository _hotelRepository;
    private readonly ITransportViewRepository _transportRepository;

    public DestinationsService(
        ITransportViewRepository transportRepository, 
        IHotelOccupationViewRepository hotelRepository)
    {
        _transportRepository = transportRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task<IEnumerable<DestinationContract>> GetAll(CancellationToken cancellationToken)
    {
        var allHotels = _hotelRepository.QueryAll();
        var airportCodes = await allHotels.Select(x => x.AirportCode).Distinct().ToListAsync(cancellationToken);

        // Get names of all airports
        var airportNames = await _transportRepository.QueryAll()
            .GroupBy(x => x.DestinationAirportCode, (_, values) => values.First())
            .ToDictionaryAsync(x => x.DestinationAirportCode, x => x.DestinationAirportName, cancellationToken);

        // Uses only destinations which have the available flights
        return airportCodes.Select(x =>
            {
                var name = airportNames.GetValueOrDefault(x);
                return name == null ? null : new DestinationContract(x, name);
            })
            .Where(x => x != null)!;
    }
}