using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TripBooker.Common.Transport;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Services;

namespace TripBooker.TransportService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(
        TransportDbContext transportContext, 
        ITransportService transportService)
    {
        transportContext.Database.EnsureCreated();

        // Transport options
        if (!transportContext.TransportOption.Any())
        {
            AddTransportOptions(transportContext.TransportOption);
        }
        transportContext.SaveChanges();

        // Transports
        if (!transportContext.TransportEvent.Any())
        {
            var transports = TransportsGenerator.GenerateTransports(
                transportContext.TransportOption.Select(x => x).ToList());

            transportService.AddManyNewTransports(transports, default).GetAwaiter().GetResult();
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