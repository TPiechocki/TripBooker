using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.Common;
using TripBooker.Common.Order;
using TripBooker.Common.Order.Hotel;
using TripBooker.Common.Order.Payment;
using TripBooker.Common.Order.Transport;
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
                x.AddConsumer<TripOptionsQueryConsumer>();
                x.AddConsumer<TripQueryConsumer>();

                // sagas
                x.AddSagaStateMachine<OrderStateMachine, OrderState>(cfg =>
                        // workaround to prevent reading single event multiple times
                        // for the first saga
                    {

                        cfg.UseConcurrentMessageLimit(4);
                        cfg.UseInMemoryOutbox();

                        var partition = cfg.CreatePartitioner(4);
                        cfg.Message<SubmitOrder>(s => s.UsePartitioner(partition, m => m.Message.Order.OrderId));

                        cfg.Message<TransportReservationAccepted>(s =>
                            s.UsePartitioner(partition, m => m.Message.CorrelationId));
                        cfg.Message<TransportReservationRejected>(s =>
                            s.UsePartitioner(partition, m => m.Message.CorrelationId));

                        cfg.Message<HotelReservationAccepted>(s =>
                            s.UsePartitioner(partition, m => m.Message.CorrelationId));
                        cfg.Message<HotelReservationRejected>(s =>
                            s.UsePartitioner(partition, m => m.Message.CorrelationId));

                        cfg.Message<PaymentAccepted>(s =>
                            s.UsePartitioner(partition, m => m.Message.CorrelationId));
                        cfg.Message<PaymentTimeout>(s =>
                            s.UsePartitioner(partition, m => m.Message.CorrelationId));
                    })
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