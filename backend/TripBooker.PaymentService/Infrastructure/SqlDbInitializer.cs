namespace TripBooker.PaymentService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(PaymentDbContext context)
    {
        context.Database.EnsureCreated();
    }
}
    