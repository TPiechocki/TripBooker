using AutoMapper;
using MassTransit;
using TripBooker.Common.Transport.Contract;
using TripBooker.Common.TravelAgency.Model;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.EventConsumers.Public;

internal class TransportViewContractConsumer : IConsumer<Batch<TransportViewContract>>
{
    private readonly ITransportViewRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TransportViewContractConsumer> _logger;

    public TransportViewContractConsumer(
        ITransportViewRepository repository, 
        IMapper mapper, 
        ILogger<TransportViewContractConsumer> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Batch<TransportViewContract>> context)
    {
        var transports = _mapper.Map<IEnumerable<TransportModel>>(context.Message.Select(x => x.Message));

        var distinctTransports = transports.GroupBy(x => x.Id).Select(x => x.Last()).ToList();

        _logger.LogInformation($"Received updates for hotel view (Count={distinctTransports.Count()})");

        await _repository.AddOrUpdateManyAsync(distinctTransports, context.CancellationToken);
    }
}