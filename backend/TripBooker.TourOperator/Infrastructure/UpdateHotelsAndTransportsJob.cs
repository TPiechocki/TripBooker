using MassTransit;
using Quartz;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.TourOperator.Model.Extensions;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.Infrastructure;

internal class UpdateHotelsAndTransportsJob : IJob
{
    private readonly IBus _bus;
    private readonly IHotelOccupationViewRepository _hotelRepository;
    private readonly ITransportViewRepository _transportRepository;
    private readonly IUpdatesRepository _updatesRepository;
    private readonly ILogger<UpdateHotelsAndTransportsJob> _logger;


    public UpdateHotelsAndTransportsJob(
        IBus bus,
        IHotelOccupationViewRepository hotelRepository,
        ITransportViewRepository transportRepository,
        IUpdatesRepository updateRepository,
        ILogger<UpdateHotelsAndTransportsJob> logger)
    {
        _bus = bus;
        _hotelRepository = hotelRepository;
        _transportRepository = transportRepository;
        _updatesRepository = updateRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Updating Hotels and Transports");

        var rnd = new Random();

        // Hotels
        var hotels = _hotelRepository.QueryAll().GroupBy(x => x.HotelId).ToList();
        var hotelDays = hotels[rnd.Next(hotels.Count)].OrderBy(x => x.Date).ToList();

        var startDate = rnd.Next(hotelDays.Count - 1);
        var length = rnd.Next(hotelDays.Count - startDate) + 1;
        hotelDays = hotelDays.Skip(startDate).Take(length).ToList();

        var minvals = hotelDays.Reduce();

        var updateRoll = rnd.NextDouble();
        // If roll > 0.5 some rooms get taken and prices go up else price goes down no rooms taken
        var hotelUpdate = new HotelUpdateContract()
        {
            HotelId = minvals.HotelId,
            HotelDays = hotelDays.Select(x => x.Id).ToList(),
            PriceModifierFactor = updateRoll + 0.5,
            RoomsStudioChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsStudio),
            RoomsSmallChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsSmall),
            RoomsMediumChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsMedium),
            RoomsLargeChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsLarge),
            RoomsApartmentChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsApartment),
        };
        await _bus.Publish(hotelUpdate, context.CancellationToken);
        await _updatesRepository.AddAsync(hotelUpdate.Describe(), context.CancellationToken);
        _logger.LogInformation($"Updated Hotel: HotelId = {minvals.HotelId}, StartDate = {minvals.Date}, length = {hotelDays.Count}, updateRoll = {updateRoll}");

        // Transports
        var transports = _transportRepository.QueryAll().ToList();
        var transport = transports[rnd.Next(transports.Count)];

        updateRoll = rnd.NextDouble();
        // if roll > 0.5 some places are taken and the price goes up else price goes down
        var transportUpdate = new TransportUpdateContract()
        {
            Id = transport.Id,
            AvailablePlacesChange = 
                updateRoll > 0.5 ? -rnd.Next((int)((transport.AvailablePlaces - 1) * updateRoll) + 1) : 0,
            PriceChangedFlag = true,
            NewTicketPrice = Math.Max((int)((updateRoll + 0.5) * transport.TicketPrice), 1)
        };
        await _bus.Publish(transportUpdate, context.CancellationToken);
        await _updatesRepository.AddAsync(transportUpdate.Describe(transport.TicketPrice),
                                          context.CancellationToken);
        _logger.LogInformation($"Updated Transport: Id = {transport.Id}, updateRoll = {updateRoll}");
    }
}
