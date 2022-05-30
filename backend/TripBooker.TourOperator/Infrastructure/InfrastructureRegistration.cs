using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.TourOperator.Consumers;
using TripBooker.TourOperator.EventConsumers.Public;

namespace TripBooker.TourOperator.Infrastructure;

internal static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TourOperatorDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)))
            )
            .AddBus(configuration);
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
            {
                // PUBLIC
                x.AddConsumer<TourOperatorReportConsumer>();

                // View updates
                x.AddConsumer<TourOperatorHotelOccupationViewContractConsumer>(cfg =>
                {
                    cfg.Options<BatchOptions>(opt =>
                    {
                        opt.ConcurrencyLimit = 1;
                        opt.MessageLimit = 1000;
                    });
                });

                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(host);
                        cfg.ConfigureEndpoints(context);
                    }
                );
            })
            .Configure<MassTransitHostOptions>(x => { x.WaitUntilStarted = true; });
        ;
    }
}