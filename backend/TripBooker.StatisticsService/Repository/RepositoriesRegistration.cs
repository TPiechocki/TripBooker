namespace TripBooker.StatisticsService.Repository;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IReservationRepository, ReservationRepository>();
    }
}