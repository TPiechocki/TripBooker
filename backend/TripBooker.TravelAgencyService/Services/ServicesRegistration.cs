namespace TripBooker.TravelAgencyService.Services;

internal static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IDestinationsService, DestinationsService>();
    }
}