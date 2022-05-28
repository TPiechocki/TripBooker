using MassTransit;
using TripBooker.Common.TravelAgency.Contract.Query;
using TripBooker.WebApi.Consumers;

namespace TripBooker.WebApi.Infrastructure
{
    internal static class ServicesRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddBus(configuration);
        }

        private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration.GetSection("RabbitMq")["Host"];

            return services
                .AddMassTransit(x =>
                    {
                        // public
                        x.AddConsumer<PurchasedOfferNotificationConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                            {
                                cfg.Host(host);
                                cfg.ConfigureEndpoints(context);
                            }
                        );
                    }
                )
                .Configure<MassTransitHostOptions>(x =>
                {
                    x.WaitUntilStarted = true;
                });
        }
    }
}