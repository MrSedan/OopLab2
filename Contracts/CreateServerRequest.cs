namespace LabBack.Contracts;

/// <summary>
/// Request payload for creating a new server.
/// </summary>
public sealed class CreateServerRequest
{
    /// <summary>
    /// Gets or sets the processor frequency in MHz.
    /// </summary>
    public int ProcessorFrequency { get; init; }

    /// <summary>
    /// Gets or sets the RAM size in MB.
    /// </summary>
    public int RamAmount { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of simultaneous connections.
    /// </summary>
    public int MaxConnections { get; init; }

    /// <summary>
    /// Gets or sets the current number of active connections.
    /// </summary>
    public int CurrentConnections { get; init; } = 0;
}
