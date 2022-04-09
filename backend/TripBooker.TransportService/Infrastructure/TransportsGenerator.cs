using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Infrastructure
{
    internal static class TransportsGenerator
    {
        internal static IEnumerable<NewTransportContract> GenerateTransports(IEnumerable<TransportOption> options)
        {
            var random = new Random();

            foreach (var option in options)
            {
                yield return new NewTransportContract(
                    option.Id,
                    DateTime.SpecifyKind(new DateTime(2022, 06, 01), DateTimeKind.Utc),
                    random.Next(50, 150)
                );
            }
        }
    }
}
