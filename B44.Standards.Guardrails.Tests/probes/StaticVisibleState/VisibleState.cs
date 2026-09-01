namespace StaticVisibleState;

/// <summary>Shared mutable state any caller can reassign, with no owner.</summary>
public static class VisibleState
{
    /// <summary>Public: reassignable by anyone, from anywhere, in any order.</summary>
    public static int TurnNumber;

    /// <summary>Protected-equivalent reach through a public type.</summary>
    public static string? ActiveProfileId;
}
