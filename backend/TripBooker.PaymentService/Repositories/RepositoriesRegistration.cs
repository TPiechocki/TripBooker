namespace TripBooker.PaymentService.Repositories;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IPaymentEventRepository, PaymentEventRepository>()
            .AddScoped<ITimeoutTimestampRepository, TimeoutTimestampRepository>();
    }

}
