using AutoMapper;
using MassTransit;
using TripBooker.Common.Hotel.Contract;
using TripBooker.TourOperator.Model;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public;

internal class TourOperatorHotelOccupationViewContractConsumer : IConsumer<Batch<HotelOccupationViewContract>>
{
    private readonly IHotelOccupationViewRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourOperatorHotelOccupationViewContractConsumer> _logger;

    public TourOperatorHotelOccupationViewContractConsumer(IHotelOccupationViewRepository repository,
        IMapper mapper,
        ILogger<TourOperatorHotelOccupationViewContractConsumer> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Batch<HotelOccupationViewContract>> context)
    {
        var data = _mapper.Map<IEnumerable<HotelOccupationModel>>(
            context.Message.Select(x => x.Message));

        var distinctData = data.GroupBy(x =>
            new { x.HotelId, x.Date }).Select(x => x.Last()).ToList();

        _logger.LogInformation($"Received updates for hotel view (Count={distinctData.Count})");

        await _repository.AddOrUpdateManyAsync(distinctData, context.CancellationToken);
    }
}
