using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common;
using TripBooker.TravelAgencyService.EventConsumers.Public;
using TripBooker.TravelAgencyService.EventConsumers.Public.Query;
using TripBooker.TravelAgencyService.Order.State;

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
                x.AddConsumer<TransportViewContractConsumer>(cfg =>
                {
                    cfg.Options<BatchOptions>(opt =>
                    {
                        opt.ConcurrencyLimit = 1;
                        opt.MessageLimit = 500;
                    });
                });
                x.AddConsumer<HotelOccupationViewContractConsumer>(cfg =>
                {
                    cfg.Options<BatchOptions>(opt =>
                    {
                        opt.ConcurrencyLimit = 1;
                        opt.MessageLimit = 1000;
                    });
                });

                // queries
                x.AddConsumer<DestinationsQueryConsumer>();
                x.AddConsumer<TripsQueryConsumer>();

                // sagas
                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .MongoDbRepository(c =>
                    {
                        c.Connection = configuration.GetConnectionString("MongoDb");
                        c.DatabaseName = GlobalConstants.MongoDbName;
                        c.CollectionName = "OrderSaga";
                    });

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