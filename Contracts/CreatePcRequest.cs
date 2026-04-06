namespace LabBack.Contracts;

/// <summary>
/// Request payload for creating a new PC.
/// </summary>
public sealed class CreatePcRequest
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
    /// Gets or sets the default user shell. Falls back to <c>XFCE</c> when omitted.
    /// </summary>
    public string? UserShell { get; init; }

    /// <summary>
    /// Gets or sets the operating system. Falls back to <c>Linux</c> when omitted.
    /// </summary>
    public string? Os { get; init; }
}
