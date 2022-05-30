namespace TripBooker.TourOperator.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(TourOperatorDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        // TODO optional: create initial database state by querying services for views
    }
}
