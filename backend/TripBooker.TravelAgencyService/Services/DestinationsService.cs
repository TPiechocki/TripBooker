using Microsoft.EntityFrameworkCore;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.Services;

internal interface IDestinationsService
{
    Task<IEnumerable<string>> GetAll(CancellationToken cancellationToken);
}

internal class DestinationsService : IDestinationsService
{
    private readonly ITransportViewRepository _repository;

    public DestinationsService(ITransportViewRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<string>> GetAll(CancellationToken cancellationToken)
    {
        var allTransports = _repository.QueryAll();
        // TODO: consider hotel localizations 
        return await allTransports.Select(x => x.DestinationAirportName).Distinct().ToListAsync(cancellationToken);
    }
}