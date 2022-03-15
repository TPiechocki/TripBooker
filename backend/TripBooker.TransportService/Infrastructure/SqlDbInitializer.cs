using TripBooker.Common.Transport;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(SqlDbContext context, ITransportService transportService,
        ITransportReservationService reservationService)
    {
        context.Database.EnsureCreated();


        // Transport options
        if (!context.TransportOption.Any())
        {
            var transportOptions = new[]
            {
                new TransportOption
                {
                    DeparturePlace = "Gdańsk",
                    Destination = "Split",  
                    Type = TransportType.Flight,
                    Carrier = "Enter Air"
                }
            };
            context.TransportOption.AddRange(transportOptions);
        }
        context.SaveChanges();


        // Transports
        if (!context.Transport.Any())
        {
            transportService.AddNewTransport(new Transport
            {
                OptionId = 1,
                DepartureDate = new DateOnly(2022, 07, 01),
                IsReturn = false,
                NumberOfSeats = 180
            }, default).GetAwaiter().GetResult();

            transportService.AddNewTransport(new Transport
            {
                OptionId = 1,
                DepartureDate = new DateOnly(2022, 07, 08),
                IsReturn = true,
                NumberOfSeats = 180
            }, default).GetAwaiter().GetResult();

            transportService.AddNewTransport(new Transport
            {
                OptionId = 1,
                DepartureDate = new DateOnly(2022, 08, 01),
                IsReturn = false,
                NumberOfSeats = 180
            }, default).GetAwaiter().GetResult();
        }

        // Reservations
        if (!context.TransportReservation.Any())
        {
            reservationService.AddNewReservation(new TransportReservation
            {
                TranportId = 2,
                Places = 7
            }, default).GetAwaiter().GetResult();
        }

        context.SaveChanges();
    }
}