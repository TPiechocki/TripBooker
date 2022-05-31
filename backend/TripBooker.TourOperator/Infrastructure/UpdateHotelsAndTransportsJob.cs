﻿using MassTransit;
using Quartz;
using TripBooker.Common.TourOperator.Contract;
using TripBooker.TourOperator.Repositories;

namespace TripBooker.TourOperator.Infrastructure;

internal class UpdateHotelsAndTransportsJob : IJob
{
    private readonly IBus _bus;
    private readonly IHotelOccupationViewRepository _hotelRepository;

    public UpdateHotelsAndTransportsJob(IBus bus, IHotelOccupationViewRepository hotelRepository)
    {
        _bus = bus;
        _hotelRepository = hotelRepository;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var rnd = new Random();

        // Hotels
        var hotels = _hotelRepository.QueryAll().GroupBy(x => x.HotelId).ToList();
        var hotelDays = hotels[rnd.Next(hotels.Count)].OrderBy(x => x.Date).ToList();

        var startDate = rnd.Next(hotelDays.Count);
        var length = rnd.Next(hotelDays.Count - startDate);
        hotelDays = hotelDays.Skip(startDate).Take(length).ToList();

        var minvals = hotelDays.First();
        foreach (var hotelDay in hotelDays)
        {
            minvals.PriceModifier = Math.Min(minvals.PriceModifier, hotelDay.PriceModifier);
            minvals.RoomsStudio = Math.Min(minvals.RoomsStudio, hotelDay.RoomsStudio);
            minvals.RoomsSmall = Math.Min(minvals.RoomsSmall, hotelDay.RoomsSmall);
            minvals.RoomsMedium = Math.Min(minvals.RoomsMedium, hotelDay.RoomsMedium);
            minvals.RoomsLarge = Math.Min(minvals.RoomsLarge, hotelDay.RoomsLarge);
            minvals.RoomsApartment = Math.Min(minvals.RoomsApartment, hotelDay.RoomsApartment);
        }

        var updateRoll = rnd.NextDouble();
        // If roll > 0.5 some rooms get taken and prices go up else price goes down no rooms taken
        _bus.Publish(new HotelUpdateContract()
        {
            HotelId = minvals.HotelId,
            HotelDays = hotelDays.Select(x => x.Id).ToList(),
            PriceModifierFactor = updateRoll + 0.5,
            RoomsStudioChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsStudio),
            RoomsSmallChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsSmall),
            RoomsMediumChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsMedium),
            RoomsLargeChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsLarge),
            RoomsApartmentChange = updateRoll < 0.5 ? 0 : -rnd.Next(minvals.RoomsApartment),
        });

        // Transports

        // TODO
        
        return Task.CompletedTask;
    }
}
