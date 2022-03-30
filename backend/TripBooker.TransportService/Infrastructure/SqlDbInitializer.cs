using TripBooker.Common.Transport;
using TripBooker.Common.Transport.Contract;
using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(
        TransportDbContext transportContext, 
        ITransportService transportService,
        ITransportReservationService reservationService)
    {
        transportContext.Database.EnsureCreated();

        // Transport options
        if (!transportContext.TransportOption.Any())
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
            transportContext.TransportOption.AddRange(transportOptions);
        }
        transportContext.SaveChanges();

        var transportId = new List<Guid>();
        // Transports
        if (!transportContext.TransportEvent.Any())
        {
            transportId.Add(transportService.AddNewTransport(
                    new NewTransportContract(
                        DateTime.SpecifyKind(new DateTime(2022, 07, 01), DateTimeKind.Utc).Date,
                        false, 100, 1),
                    default)
                .GetAwaiter().GetResult());

            transportId.Add(transportService.AddNewTransport(
                    new NewTransportContract(
                        DateTime.SpecifyKind(new DateTime(2022, 07, 08), DateTimeKind.Utc).Date, 
                        true, 200, 1),
                    default)
                .GetAwaiter().GetResult());

            transportId.Add(transportService.AddNewTransport(
                    new NewTransportContract(
                        DateTime.SpecifyKind(new DateTime(2022, 08, 01), DateTimeKind.Utc).Date, 
                        false, 300, 1),
                    default)
                .GetAwaiter().GetResult());
        }

        // Reservations
        if (!transportContext.ReservationEvent.Any())
        {
            reservationService.AddNewReservation(
                    new NewReservationContract(transportId[1], 7),
                    default)
                .GetAwaiter().GetResult();
            reservationService.AddNewReservation(
                    new NewReservationContract(transportId[1], 500),
                    default)
                .GetAwaiter().GetResult();
        }

        transportContext.SaveChanges();
    }
}