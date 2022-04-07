﻿using MassTransit;
using TripBooker.Common.TravelAgency;
using TripBooker.Common.TravelAgency.Folder.Query;

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

                        // request client needs defined endpoint to work properly
                        x.AddRequestClient<DestinationsQueryContract>(
                            new Uri($"rabbitmq://{host}/{TravelAgencyConstants.Endpoints.Destinations}"));

                        x.UsingRabbitMq((context, cfg) =>
                            {
                                cfg.Host(host);
                                cfg.ConfigureEndpoints(context);
                                cfg.UseRawJsonSerializer();
                            }
                        );
                    }
                )
                .AddMassTransitHostedService(true);
        }
    }
}