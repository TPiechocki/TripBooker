using AutoMapper;
using MassTransit;
using Newtonsoft.Json;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Model.Events;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.EventConsumers;

internal class NewTransportEventConsumer : IConsumer<NewTransportEvent>
{
    private readonly ILogger<NewTransportEventConsumer> _logger;
    private readonly ITransportViewUpdateRepository _viewRepository;
    private readonly IMapper _mapper;

    public NewTransportEventConsumer(
        ILogger<NewTransportEventConsumer> logger,
        ITransportViewUpdateRepository viewRepository, IMapper mapper)
    {
        _logger = logger;
        _viewRepository = viewRepository;
        _mapper = mapper;
    }


    public async Task Consume(ConsumeContext<NewTransportEvent> context)
    {
        _logger.LogInformation($"NewTransportEvent consumed: {JsonConvert.SerializeObject(context.Message)}");

        await _viewRepository.AddNewTransportAsync(_mapper.Map<TransportView>(context.Message), default);
    }
}