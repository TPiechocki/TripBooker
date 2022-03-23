namespace TripBooker.TransportService.Repositories;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransportOptionRepository, TransportOptionRepository>()
            .AddScoped<ITransportEventRepository, TransportEventRepository>()
            .AddScoped<ITransportViewRepository, TransportViewRepository>()
            .AddScoped<IReservationEventRepository, ReservationEventRepository>()
            .AddScoped<IEventTimestampRepository, EventTimestampRepository>();
    }
}