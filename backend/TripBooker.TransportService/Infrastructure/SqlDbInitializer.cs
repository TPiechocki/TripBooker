using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TripBooker.Common.Transport;
using TripBooker.Common.Transport.Contract.Command;
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
            AddTransportOptions(transportContext.TransportOption);
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
            // correct reservation example
            reservationService.AddNewReservation(
                    new NewReservationContract(new Guid(), transportId[1], 7),
                    default)
                .GetAwaiter().GetResult();
            // reject reservation example
            reservationService.AddNewReservation(
                    new NewReservationContract(new Guid(), transportId[1], 500),
                    default)
                .GetAwaiter().GetResult();

            // cancel reservation example
            var reservation = reservationService.AddNewReservation(
                    new NewReservationContract(new Guid(), transportId[0], 4),
                    default)
                .GetAwaiter().GetResult();
            reservationService.Cancel(reservation.Id, default).GetAwaiter().GetResult();

        }

        transportContext.SaveChanges();
    }

    private static void AddTransportOptions(DbSet<TransportOption> dbSet)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var streamReader = File.OpenText("flights.csv");
        using var csvReader = new CsvReader(streamReader, csvConfig);

        var flightOptions = csvReader.GetRecords<TransportOption>()
            .DistinctBy(x => (x.DepartureAirportCode, x.DestinationAirportCode))
            .ToList();
        flightOptions.ForEach(x => x.Type = TransportType.Flight);

        dbSet.AddRange(flightOptions);
    }
}