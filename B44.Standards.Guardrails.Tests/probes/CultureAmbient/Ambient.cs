using System;

namespace CultureAmbient;

internal static class Ambient
{
    public static bool SameId(string a, string b) => string.Equals(a, b, StringComparison.InvariantCulture);

    public static int OrderIds(string a, string b) => string.Compare(a, b);

    public static bool IsSaveToken(string token) => token.StartsWith("sav-");

    public static string NormalizeKey(string key) => key.ToUpper();

    public static int ParseCount(string raw) => int.Parse(raw);

    public static string FormatCount(double count) => count.ToString();
}
