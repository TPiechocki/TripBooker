using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooker.TransportService.EventConsumers;

namespace TripBooker.TransportService.Infrastructure;

internal static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<SqlDbContext>(opt =>
            opt
                .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableSensitiveDataLogging())
            .AddBus(configuration);
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
                {
                    x.AddConsumer<NewTransportEventConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(host);
                            cfg.ConfigureEndpoints(context);
                        }
                    );
                }
            )
            .AddMassTransitHostedService(true);
    }
}