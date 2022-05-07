using System.Collections.Generic;

namespace TripBooker.Common.Payment;

public static class Discount
{
    private static readonly Dictionary<string, double> Codes = new()
    {
        { "JoMama", 0.99 },
        { "PGRules", 0.95 },
        { "RSWW", 0.9 }
    };

    public static bool IsViable(string code) => Codes.ContainsKey(code);
    
    public static double Apply(string code, double price) => Codes[code] * price;
}
