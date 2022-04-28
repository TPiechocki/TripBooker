using System;
using System.Collections.Generic;

namespace TripBooker.Common.Helpers;

public static class DateTimeHelpers
{
    public static IEnumerable<DateTime> GetDaysBetween(DateTime start, DateTime end)
    {
        for (var i = start; i <= end; i = i.AddDays(1))
        {
            yield return i;
        }
    }
}