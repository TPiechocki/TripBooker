using AutoMapper;
using MassTransit;
using TripBooker.Common.Transport.Contract;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.EventConsumers.Public;

internal class TransportViewContractConsumer : IConsumer<TransportViewContract>
{
    private readonly ITransportViewRepository _repository;
    private readonly IMapper _mapper;

    public TransportViewContractConsumer(ITransportViewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<TransportViewContract> context)
    {
        await _repository.AddOrUpdateAsync(_mapper.Map<TransportModel>(context.Message), context.CancellationToken);
    }
}