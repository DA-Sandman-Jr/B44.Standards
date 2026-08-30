using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CulturePresentation;

internal static class Presentation
{
    public static string Score(double score) => score.ToString("N0", CultureInfo.CurrentCulture);

    public static string Shout(string label) => label.ToUpper(CultureInfo.CurrentCulture);

    public static List<string> ForDisplay(IEnumerable<string> names) =>
        names.OrderBy(name => name, StringComparer.CurrentCulture).ToList();

    public static string Localized(DateTimeOffset when) =>
        when.ToString("D", CultureInfo.CurrentCulture);
}
