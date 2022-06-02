using MassTransit;
using Quartz;
using Microsoft.EntityFrameworkCore;
using TripBooker.TourOperator.EventConsumers.Public;

namespace TripBooker.TourOperator.Infrastructure;

internal static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TourOperatorDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning)))
            )
            .AddBus(configuration)
            .AddQuartz();
    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
            {
                // PUBLIC
                x.AddConsumer<TourOperatorReportConsumer>();
                x.AddConsumer<HotelUpdateQueryConsumer>();
                x.AddConsumer<TransportUpdateQueryConsumer>();

                // View updates
                x.AddConsumer<TourOperatorTransportViewContractConsumer>(cfg =>
                {
                    cfg.Options<BatchOptions>(opt =>
                    {
                        opt.ConcurrencyLimit = 1;
                        opt.MessageLimit = 500;
                    });
                });
                x.AddConsumer<TourOperatorHotelOccupationViewContractConsumer>(cfg =>
                {
                    cfg.Options<BatchOptions>(opt =>
                    {
                        opt.ConcurrencyLimit = 1;
                        opt.MessageLimit = 1000;
                    });
                });

                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(host);
                        cfg.ConfigureEndpoints(context);
                    }
                );
            })
            .Configure<MassTransitHostOptions>(x => { x.WaitUntilStarted = true; });
        ;
    }

    private static IServiceCollection AddQuartz(this IServiceCollection services)
    {
        // configure job to create job that updates Hotels and Transports every minute
        return services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey(nameof(UpdateHotelsAndTransportsJob));
            q.AddJob<UpdateHotelsAndTransportsJob>(opt => opt.WithIdentity(jobKey));
            q.AddTrigger(opt => opt
                .ForJob(jobKey)
                .WithIdentity(jobKey + "-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(45)
                    .RepeatForever()));
        })
            .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}