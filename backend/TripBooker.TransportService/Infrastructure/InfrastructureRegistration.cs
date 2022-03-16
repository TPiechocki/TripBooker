using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using TripBooker.Common;
using TripBooker.TransportService.EventConsumers;

namespace TripBooker.TransportService.Infrastructure;

internal static class ServicesRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TransportDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                    .EnableSensitiveDataLogging())
            .AddBus(configuration)

            // mongoDB TODO: remove if not needed
            .AddSingleton(s =>
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        var logger = s.GetRequiredService<ILogger<IMongoClient>>();
                        logger.LogInformation($"MongoLog: {e.CommandName} - {e.Command.ToJson()}");
                    });
                };

                var mongoClient = new MongoClient(settings);
                return mongoClient.GetDatabase(GlobalConstants.MongoDbName);
            });
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