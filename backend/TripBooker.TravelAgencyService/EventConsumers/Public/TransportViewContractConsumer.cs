using AutoMapper;
using MassTransit;
using TripBooker.Common.Transport.Contract;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.EventConsumers.Public;

internal class ManyTransportsViewContractConsumer : IConsumer<ManyTransportsViewContract>
{
    private readonly ITransportViewRepository _repository;
    private readonly IMapper _mapper;

    public ManyTransportsViewContractConsumer(ITransportViewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ManyTransportsViewContract> context)
    {
        var transports = _mapper.Map<IEnumerable<TransportModel>>(context.Message.Transports);
        await _repository.AddOrUpdateManyAsync(transports, context.CancellationToken);
    }
}