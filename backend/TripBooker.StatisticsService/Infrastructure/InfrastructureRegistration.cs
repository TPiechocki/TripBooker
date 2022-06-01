using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using TripBooker.StatisticsService.Consumers;
using TripBooker.StatisticsService.Consumers.Queries;

namespace TripBooker.StatisticsService.Infrastructure;

internal static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddBus(configuration)
            .AddDbContext<StatisticsDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)))
            );
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
            {
                // PUBLIC
                x.AddConsumer<NewReservationStatisticsConsumer>();
                x.AddConsumer<PaymentAcceptedStatisticsConsumer>();
                x.AddConsumer<PaymentTimeoutStatisticsConsumer>();

                x.AddConsumer<GetDestinationCountsQueryConsumer>();
                x.AddConsumer<GetHotelCountsQueryConsumer>();
                x.AddConsumer<GetTransportCountsQueryConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(host);
                        cfg.ConfigureEndpoints(context);
                    }
                );
            })
            .Configure<MassTransitHostOptions>(x => { x.WaitUntilStarted = true; })
            .AddQuartz();
    }

    private static IServiceCollection AddQuartz(this IServiceCollection services)
    {
        //configure job to create update view event every 15s
        return services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var jobKey = new JobKey(nameof(StatisticsTimeoutJob));
                q.AddJob<StatisticsTimeoutJob>(opt => opt.WithIdentity(jobKey));
                q.AddTrigger(opt => opt
                    .ForJob(jobKey)
                    .WithIdentity(jobKey + "-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever()));
            })
            .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}