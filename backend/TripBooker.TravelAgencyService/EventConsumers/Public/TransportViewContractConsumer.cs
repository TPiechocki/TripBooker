using AutoMapper;
using MassTransit;
using TripBooker.Common.Transport.Contract;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.EventConsumers.Public;

internal class TransportViewContractConsumer : IConsumer<Batch<TransportViewContract>>
{
    private readonly ITransportViewRepository _repository;
    private readonly IMapper _mapper;

    public TransportViewContractConsumer(ITransportViewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<Batch<TransportViewContract>> context)
    {
        var transports = _mapper.Map<IEnumerable<TransportModel>>(context.Message.Select(x => x.Message));

        var distinctTransports = transports.GroupBy(x => x.Id).Select(x => x.Last());

        await _repository.AddOrUpdateManyAsync(distinctTransports, context.CancellationToken);
    }
}