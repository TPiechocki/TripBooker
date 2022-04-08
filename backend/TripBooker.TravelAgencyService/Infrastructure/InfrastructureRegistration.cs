using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.TravelAgency;
using TripBooker.TravelAgencyService.EventConsumers.Public;
using TripBooker.TravelAgencyService.EventConsumers.Public.Query;
using TripBooker.TravelAgencyService.Services;

namespace TripBooker.TravelAgencyService.Infrastructure;

internal static class ServicesRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TravelAgencyDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSimpleConsole(opt =>
                    {
                        opt.TimestampFormat = "[HH:mm:ss.fff] ";
                    })))
                    .EnableSensitiveDataLogging())
            .AddBus(configuration);
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
            {
                // public
                x.AddConsumer<TransportViewContractConsumer>();

                x.AddConsumer<DestinationsQueryConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(host);
                        cfg.ConfigureEndpoints(context);
                    }
                );
            })
            .AddMassTransitHostedService(true);
    }
}