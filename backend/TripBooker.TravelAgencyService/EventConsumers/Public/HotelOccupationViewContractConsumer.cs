using AutoMapper;
using MassTransit;
using TripBooker.Common.Hotel.Contract;
using TripBooker.TravelAgencyService.Model;
using TripBooker.TravelAgencyService.Repositories;

namespace TripBooker.TravelAgencyService.EventConsumers.Public;

internal class HotelOccupationViewContractConsumer : IConsumer<Batch<HotelOccupationViewContract>>
{
    private readonly IHotelOccupationViewRepository _repository;
    private readonly IMapper _mapper;

    public HotelOccupationViewContractConsumer(IHotelOccupationViewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<Batch<HotelOccupationViewContract>> context)
    {
        var data = _mapper.Map<IEnumerable<HotelOccupationModel>>(
            context.Message.Select(x => x.Message));

        var distinctData = data.GroupBy(x =>
            new { x.HotelId, x.Date }).Select(x => x.Last());

        await _repository.AddOrUpdateManyAsync(distinctData, context.CancellationToken);
    }
}
