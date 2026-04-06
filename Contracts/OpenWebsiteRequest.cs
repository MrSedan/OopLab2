namespace LabBack.Contracts;

/// <summary>
/// Request payload for opening a website on a PC.
/// </summary>
public sealed class OpenWebsiteRequest
{
    /// <summary>
    /// Gets or sets the website URL.
    /// </summary>
    public string Url { get; init; } = string.Empty;
}
