namespace TripBooker.TransportService.Repositories;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransportCommandRepository, TransportCommandRepository>()
            .AddScoped<ITransportViewUpdateRepository, TransportViewUpdateRepository>();
    }
}