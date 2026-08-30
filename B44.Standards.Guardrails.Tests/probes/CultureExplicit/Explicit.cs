using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CultureExplicit;

internal static class Explicit
{
    public static bool SameId(string a, string b) => string.Equals(a, b, StringComparison.Ordinal);

    public static int OrderIds(string a, string b) => string.CompareOrdinal(a, b);

    public static bool IsSaveToken(string token) => token.StartsWith("sav-", StringComparison.Ordinal);

    public static string NormalizeKey(string key) => key.ToUpperInvariant();

    public static int ParseCount(string raw) => int.Parse(raw, CultureInfo.InvariantCulture);

    public static string FormatCount(double count) => count.ToString(CultureInfo.InvariantCulture);

    public static List<string> Ordered(IEnumerable<string> ids) =>
        ids.OrderBy(id => id, StringComparer.Ordinal).ToList();
}
