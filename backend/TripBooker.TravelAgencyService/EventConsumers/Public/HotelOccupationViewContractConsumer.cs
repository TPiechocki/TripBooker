using AutoMapper;
using MassTransit;
using TripBooker.Common.Hotel.Contract;
using TripBooker.Common.Hubs;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.EventConsumers.Public;

internal class HotelOccupationViewContractConsumer : IConsumer<Batch<HotelOccupationViewContract>>
{
    private readonly IHotelOccupationViewRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<HotelOccupationViewContractConsumer> _logger;

    public HotelOccupationViewContractConsumer(IHotelOccupationViewRepository repository, 
        IMapper mapper, 
        ILogger<HotelOccupationViewContractConsumer> logger)
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
        
        await context.Publish(
            new HotelViewUpdated(distinctData.Select(x => x.Id)),
            context.CancellationToken);
    }
}
