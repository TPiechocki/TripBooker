using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common.TravelAgency;
using TripBooker.TravelAgencyService.EventConsumers.Public;
using TripBooker.TravelAgencyService.EventConsumers.Public.Query;
using TripBooker.TravelAgencyService.Order.State;
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
                // PUBLIC

                // view updates
                x.AddConsumer<TransportViewContractConsumer>();

                // queries
                x.AddConsumer<DestinationsQueryConsumer>();

                // sagas
                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .InMemoryRepository();      // TODO: persist saga in database

                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(host);
                        cfg.ConfigureEndpoints(context);
                    }
                );
            })
            .Configure<MassTransitHostOptions>(x =>
            {
                x.WaitUntilStarted = true;
            }); ;
    }
}