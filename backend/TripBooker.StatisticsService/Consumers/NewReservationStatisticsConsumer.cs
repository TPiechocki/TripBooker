using AutoMapper;
using MassTransit;
using TripBooker.Common.Statistics;
using TripBooker.StatisticsService.Model;
using TripBooker.StatisticsService.Repository;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers;

internal class NewReservationStatisticsConsumer : IConsumer<NewReservationEvent>
{
    private readonly IDestinationStatisticsService _destinationService;
    private readonly ILogger<NewReservationStatisticsConsumer> _logger;
    private readonly IMapper _mapper;
    private readonly IReservationRepository _repository;

    public NewReservationStatisticsConsumer(
        ILogger<NewReservationStatisticsConsumer> logger,
        IReservationRepository repository,
        IMapper mapper,
        IDestinationStatisticsService destinationService)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _destinationService = destinationService;
    }

    public async Task Consume(ConsumeContext<NewReservationEvent> context)
    {
        _logger.LogInformation($"Storing statistics for order with id={context.Message.OrderId}");

        await _repository.AddNewReservation(
            _mapper.Map<ReservationModel>(context.Message), context.CancellationToken);

        // send destination counter update
        await _destinationService.UpdateCount(context.Message.DestinationAirportCode, context.CancellationToken);
    }
}