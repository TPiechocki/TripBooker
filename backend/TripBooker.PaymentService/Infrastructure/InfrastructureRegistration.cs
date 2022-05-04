using MassTransit;
using TripBooker.Common.Order.Payment;
using TripBooker.PaymentService.Consumers;

namespace TripBooker.PaymentService.Infrastructure;

internal static class ServicesRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            // TODO: Add DB context with data about payments using EVENT SOURCING pattern
            // .AddDbContext<PaymentDbContext>(opt =>
            //     opt
            //         .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
            //         .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSimpleConsole(opt =>
            //         {
            //             opt.TimestampFormat = "[HH:mm:ss.fff] ";
            //         })))
            //         .EnableSensitiveDataLogging())
            .AddBus(configuration);
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
            {
                // PUBLIC

                // consumers
                x.AddConsumer<NewPaymentConsumer>();
                x.AddConsumer<PaymentCommandConsumer>();
                
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