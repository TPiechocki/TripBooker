namespace TripBooker.HotelService.Repositories;

internal static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IHotelOptionRepository, HotelOptionRepository>()
            .AddScoped<IRoomOptionRepository, RoomOptionRepository>()
            .AddScoped<IHotelOccupationViewRepository, HotelOccupationViewRepository>()
            .AddScoped<IReservationEventRepository, ReservationEventRepository>()
            .AddScoped<IHotelEventRepository, HotelEventRepository>()
            .AddScoped<IEventTimestampRepository, EventTimestampRepository>();
    }
    
}
