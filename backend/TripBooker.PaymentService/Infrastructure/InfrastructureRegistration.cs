using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
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
            )
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
                    x.AddConsumer<PaymentStatusQueryConsumer>();

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

    private static IServiceCollection AddQuartz(this IServiceCollection services)
    {
        //configure job to create update view event every 15s
        return services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey(nameof(TimeoutCheckJob));
            q.AddJob<TimeoutCheckJob>(opt => opt.WithIdentity(jobKey));
            q.AddTrigger(opt => opt
                .ForJob(jobKey)
                .WithIdentity(jobKey + "-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever()));
        })
            .AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
