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
    private readonly IHotelStatisticsService _hotelService;
    private readonly ILogger<NewReservationStatisticsConsumer> _logger;
    private readonly IMapper _mapper;
    private readonly IReservationRepository _repository;
    private readonly ITransportStatisticsService _transportService;

    public NewReservationStatisticsConsumer(
        ILogger<NewReservationStatisticsConsumer> logger,
        IReservationRepository repository,
        IMapper mapper,
        IDestinationStatisticsService destinationService,
        IHotelStatisticsService hotelService,
        ITransportStatisticsService transportService)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _destinationService = destinationService;
        _hotelService = hotelService;
        _transportService = transportService;
    }

    public async Task Consume(ConsumeContext<NewReservationEvent> context)
    {
        _logger.LogInformation($"Storing statistics for order with id={context.Message.OrderId}");

        var reservation = context.Message;

        await _repository.AddNewReservation(
            _mapper.Map<ReservationModel>(reservation), context.CancellationToken);

        // send counter updates
        await _destinationService.UpdateCount(reservation.DestinationAirportCode, context.CancellationToken);
        await _hotelService.UpdateCount(reservation.DestinationAirportCode, reservation.HotelCode,
            context.CancellationToken);
        await _transportService.UpdateCount(reservation.DestinationAirportCode, context.CancellationToken);
    }
}