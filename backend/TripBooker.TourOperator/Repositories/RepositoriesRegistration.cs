namespace TripBooker.TourOperator.Repositories;

internal static class ServicesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IHotelOccupationViewRepository, HotelOccupationViewRepository>();
    }
}
