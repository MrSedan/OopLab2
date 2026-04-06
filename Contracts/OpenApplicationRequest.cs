namespace LabBack.Contracts;

/// <summary>
/// Request payload for opening a desktop application on a PC.
/// </summary>
public sealed class OpenApplicationRequest
{
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string AppName { get; init; } = string.Empty;
}
