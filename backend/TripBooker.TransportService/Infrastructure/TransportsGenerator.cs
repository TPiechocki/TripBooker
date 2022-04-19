using TripBooker.Common.Helpers;
using TripBooker.TransportService.Contract;
using TripBooker.TransportService.Model;

namespace TripBooker.TransportService.Infrastructure
{
    internal static class TransportsGenerator
    {
        internal static IEnumerable<NewTransportContract> GenerateTransports(IEnumerable<TransportOption> options)
        {
            var random = new Random();
            var saturdays = DateTimeHelpers.GetDaysBetween(
                    DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, 06, 01), DateTimeKind.Utc),
                    DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, 08, 31), DateTimeKind.Utc))
                .Where(x => x.DayOfWeek == DayOfWeek.Saturday)
                .ToList();

            var transports = new List<NewTransportContract>();

            foreach (var option in options)
            {
                var newTransports = saturdays.Select(x =>
                    new NewTransportContract(
                        option.Id,
                        x,
                        random.Next(50, 150),
                        50 + option.Duration / 2));

                transports.AddRange(newTransports);
            }

            return transports;
        }
    }
}
