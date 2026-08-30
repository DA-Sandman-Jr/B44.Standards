using System;

namespace InjectedTime;

internal static class Injected
{
    public static long Date(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        return timeProvider.GetUtcNow().UtcDateTime.Ticks;
    }

    public static long Elapsed(TimeProvider timeProvider, long startTimestamp)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        return timeProvider.GetElapsedTime(startTimestamp).Ticks;
    }

    public static int Roll(Func<int, int> randomSource, int exclusiveMaximum)
    {
        ArgumentNullException.ThrowIfNull(randomSource);
        return randomSource(exclusiveMaximum);
    }
}
