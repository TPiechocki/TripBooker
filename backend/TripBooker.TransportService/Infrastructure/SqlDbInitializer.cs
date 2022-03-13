using TripBooker.Common.Transport;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(SqlDbContext context, ITransportCommandRepository repository)
    {
        context.Database.EnsureCreated();

        if (!context.TransportOptions.Any())
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
            context.TransportOptions.AddRange(transportOptions);
        }
        context.SaveChanges();

        if (!context.Transport.Any())
        {
            repository.AddTransport(new Transport
            {
                OptionId = 1,
                DepartureDate = new DateOnly(2022, 07, 01),
                IsReturn = false,
                NumberOfSeats = 180
            });

            repository.AddTransport(new Transport
            {
                OptionId = 1,
                DepartureDate = new DateOnly(2022, 07, 08),
                IsReturn = true,
                NumberOfSeats = 180
            });

            repository.AddTransport(new Transport
            {
                OptionId = 1,
                DepartureDate = new DateOnly(2022, 08, 01),
                IsReturn = false,
                NumberOfSeats = 180
            });
        }

        context.SaveChanges();
    }
}