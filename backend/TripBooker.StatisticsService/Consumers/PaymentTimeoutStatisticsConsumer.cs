using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.StatisticsService.Repository;
using TripBooker.StatisticsService.Services;

namespace TripBooker.StatisticsService.Consumers;

internal class PaymentTimeoutStatisticsConsumer : IConsumer<PaymentTimeout>
{
    private readonly IDestinationStatisticsService _destinationService;
    private readonly IHotelStatisticsService _hotelService;
    private readonly ILogger<PaymentTimeoutStatisticsConsumer> _logger;
    private readonly IReservationRepository _repository;
    private readonly ITransportStatisticsService _transportService;

    public PaymentTimeoutStatisticsConsumer(
        ILogger<PaymentTimeoutStatisticsConsumer> logger,
        IReservationRepository repository,
        IDestinationStatisticsService destinationService,
        IHotelStatisticsService hotelService,
        ITransportStatisticsService transportService)
    {
        _logger = logger;
        _repository = repository;
        _destinationService = destinationService;
        _hotelService = hotelService;
        _transportService = transportService;
    }

    public async Task Consume(ConsumeContext<PaymentTimeout> context)
    {
        _logger.LogInformation("Removing order with expired payment from statistics " +
                               $"(id={context.Message.CorrelationId})");

        var reservation =
            await _repository.Remove(context.Message.CorrelationId, context.CancellationToken);

        await _destinationService.UpdateCount(reservation.DestinationAirportCode, context.CancellationToken);
        await _hotelService.UpdateCount(reservation.DestinationAirportCode, reservation.HotelCode,
            context.CancellationToken);
        await _transportService.UpdateCount(reservation.DestinationAirportCode, context.CancellationToken);
    }
}