using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Quartz;
using TripBooker.Common;
using TripBooker.PaymentService.Consumers;

namespace TripBooker.PaymentService.Infrastructure;

internal static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<PaymentDbContext>(opt =>
                opt
                    .UseNpgsql(configuration.GetConnectionString("SqlDbContext"))
                    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                    .EnableSensitiveDataLogging())
            .AddBus(configuration)
            .AddQuartz();

    }

    private static IServiceCollection AddBus(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq")["Host"];

        return services.AddMassTransit(x =>
                {
                    // public
                    x.AddConsumer<NewPaymentConsumer>();
                    x.AddConsumer<PaymentCommandConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(host);
                            cfg.ConfigureEndpoints(context);
                        }
                    );
                }
            )
            .Configure<MassTransitHostOptions>(x => { x.WaitUntilStarted = true; });
    }

    //private static IServiceCollection AddQuartz(this IServiceCollection services)
    //{
    //    // configure job to create update view event every 15s
    //    return services.AddQuartz(q =>
    //    {
    //        q.UseMicrosoftDependencyInjectionJobFactory();
    //
    //        var jobKey = new JobKey(nameof(UpdateViewJob));
    //        q.AddJob<UpdateViewJob>(opt => opt.WithIdentity(jobKey));
    //        q.AddTrigger(opt => opt
    //            .ForJob(jobKey)
    //            .WithIdentity(jobKey + "-trigger")
    //            .WithSimpleSchedule(x => x
    //                .WithIntervalInSeconds(15)
    //                .RepeatForever()));
    //    })
    //        .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    //}
}
