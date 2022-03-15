using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransportService, TransportService>()
            .AddScoped<ITransportReservationService, TransportReservationService>();
    }
}