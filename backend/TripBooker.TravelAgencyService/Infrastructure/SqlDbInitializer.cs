namespace TripBooker.TravelAgencyService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(TravelAgencyDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        // TODO: create initial database state by querying services for views
    }
}