﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Quartz;
using TripBooker.Common;
using TripBooker.TransportService.EventConsumers.Internal;
using TripBooker.TransportService.EventConsumers.Public;

namespace TripBooker.TransportService.Infrastructure;

internal static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TransportDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)))
            )
            .AddBus(configuration)
            .AddSingleton(s =>
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                var settings = MongoClientSettings.FromConnectionString(connectionString);
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
                    x.AddConsumer<NewTransportReservationEventConsumer>();
                    x.AddConsumer<CancelTransportReservationEventConsumer>();
                    x.AddConsumer<ConfirmTransportReservationEventConsumer>();
                    x.AddConsumer<TransportUpdateEventConsumer>();

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
                        .WithIntervalInSeconds(15)
                        .RepeatForever()));
            })
            .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}