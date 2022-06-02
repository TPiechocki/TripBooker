using AutoMapper;
using MassTransit;
using TripBooker.Common.Transport.Contract;
using TripBooker.TourOperator.Model;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public;

internal class TourOperatorTransportViewContractConsumer : IConsumer<Batch<TransportViewContract>>
{
    private readonly ITransportViewRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourOperatorTransportViewContractConsumer> _logger;

    public TourOperatorTransportViewContractConsumer(
        ITransportViewRepository repository,
        IMapper mapper,
        ILogger<TourOperatorTransportViewContractConsumer> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Batch<TransportViewContract>> context)
    {
        var transports = _mapper.Map<IEnumerable<TransportModel>>(context.Message.Select(x => x.Message));

        var distinctTransports = transports.GroupBy(x => x.Id).Select(x => x.Last()).ToList();

        _logger.LogInformation($"Received updates for transport view (Count={distinctTransports.Count})");

        await _repository.AddOrUpdateManyAsync(distinctTransports, context.CancellationToken);
    }
}
