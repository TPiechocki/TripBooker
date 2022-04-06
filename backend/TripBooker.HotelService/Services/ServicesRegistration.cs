namespace TripBooker.HotelService.Services;

internal static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IHotelReservationService, HotelReservationService>();
    }
}
