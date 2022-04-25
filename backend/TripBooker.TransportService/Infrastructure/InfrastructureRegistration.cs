using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Quartz;
using TripBooker.Common;
using TripBooker.TransportService.EventConsumers.Internal;
using TripBooker.TransportService.EventConsumers.Public;

namespace TripBooker.TransportService.Infrastructure;

internal static class ServicesRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TransportDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSimpleConsole(opt =>
                    {
                        opt.TimestampFormat = "[HH:mm:ss.fff] ";
                    })))
                    .EnableSensitiveDataLogging())
            .AddBus(configuration)
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
            })
            .AddQuartz();
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
                {
                    // public
                    x.AddConsumer<NewReservationEventConsumer>();
                    x.AddConsumer<CancelReservationEventConsumer>();

                    // internal
                    x.AddConsumer<TransportViewUpdateEventConsumer>(opt =>
                    {
                        // do not reload the view concurrently
                        opt.UseConcurrentMessageLimit(1);
                    });

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

    private static IServiceCollection AddQuartz(this IServiceCollection services)
    {
        // configure job to create update view event every 15s
        return services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var jobKey = new JobKey(nameof(UpdateViewJob));
                q.AddJob<UpdateViewJob>(opt => opt.WithIdentity(jobKey));
                q.AddTrigger(opt => opt
                    .ForJob(jobKey)
                    .WithIdentity(jobKey + "-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(600)
                        .RepeatForever()));
            })
            .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}