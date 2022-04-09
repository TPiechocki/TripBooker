﻿namespace TripBooker.TravelAgencyService.Repositories;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ITransportViewRepository, TransportViewRepository>();
    }
}