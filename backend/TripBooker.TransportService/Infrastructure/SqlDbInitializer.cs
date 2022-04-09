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

        var transportIds = new List<Guid>();
        // Transports
        if (!transportContext.TransportEvent.Any())
        {
            var transports = TransportsGenerator.GenerateTransports(
                transportContext.TransportOption.Select(x => x).ToList());

            foreach (var transport in transports)
            {
                var guid = transportService.AddNewTransport(transport, default).GetAwaiter().GetResult();
                transportIds.Add(guid);
            }
        }

        // Reservations
        if (!transportContext.ReservationEvent.Any())
        {
            // correct reservation example
            reservationService.AddNewReservation(
                    new NewReservationContract(new Guid(), transportIds[1], 7),
                    default)
                .GetAwaiter().GetResult();
            // reject reservation example
            reservationService.AddNewReservation(
                    new NewReservationContract(new Guid(), transportIds[1], 500),
                    default)
                .GetAwaiter().GetResult();

            // cancel reservation example
            var reservation = reservationService.AddNewReservation(
                    new NewReservationContract(new Guid(), transportIds[0], 4),
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