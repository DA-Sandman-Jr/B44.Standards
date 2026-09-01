namespace StaticOwnedState;

/// <summary>The same values, each with an owner CA2211 can see.</summary>
public static class OwnedState
{
    /// <summary>Compile-time constant: nothing to reassign.</summary>
    public const int StartingTurn = 1;

    /// <summary>Assigned once at type initialization.</summary>
    public static readonly string DefaultProfileId = "default";

    private static int _turnNumber = StartingTurn;

    /// <summary>Readable by callers, writable only through the owning type.</summary>
    public static int TurnNumber => _turnNumber;

    /// <summary>The single sanctioned way the value moves.</summary>
    public static void AdvanceTurn() => _turnNumber++;
}
