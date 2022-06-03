using AutoMapper;
using MassTransit;
using TripBooker.Common.TourOperator.Contract.Query;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.EventConsumers.Public.Query;

internal class UpdatesQueryConsumer : IConsumer<UpdatesQueryContract>
{
    private readonly IUpdatesRepository _updatesRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdatesQueryConsumer> _logger;

    public UpdatesQueryConsumer(
        IUpdatesRepository updatesRepository,
        IMapper mapper,
        ILogger<UpdatesQueryConsumer> logger)
    {
        _updatesRepository = updatesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdatesQueryContract> context)
    {
        _logger.LogInformation("Updates query received.");

        var updates = await _updatesRepository.QueryLast10Async(context.CancellationToken);

        await context.RespondAsync(new UpdatesQueryResultContract(
            _mapper.Map<IEnumerable<UpdateContract>>(updates)
        ));

        _logger.LogInformation("Updates query handled.");
    }
}
