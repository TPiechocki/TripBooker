namespace TripBooker.StatisticsService.Services;

internal static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IDestinationStatisticsService, DestinationStatisticsService>()
            .AddScoped<IHotelStatisticsService, HotelStatisticsService>();
    }
}