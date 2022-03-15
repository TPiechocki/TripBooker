namespace TripBooker.TransportService.Repositories;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransportOptionRepository, TransportOptionRepository>()
            .AddScoped<ITransportCommandRepository, TransportRepository>()
            .AddScoped<ITransportViewUpdateRepository, TransportViewRepository>()
            .AddScoped<ITransportReservationRepository, TransportReservationRepository>()
            .AddScoped<ITransportReservationsRepository, MongoTransportReservationsRepository>();
    }
}