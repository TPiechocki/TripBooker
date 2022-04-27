namespace TripBooker.TravelAgencyService.Repositories;

internal static class ServicesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransportViewRepository, TransportViewRepository>()
            .AddScoped<IHotelOccupationViewRepository, HotelOccupationViewRepository>();
    }
}