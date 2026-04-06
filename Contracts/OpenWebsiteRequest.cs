namespace LabBack.Contracts;

/// <summary>
/// Параметры запроса для открытия веб-сайта на ПК.
/// </summary>
public sealed class OpenWebsiteRequest
{
    /// <summary>
    /// URL веб-сайта.
    /// </summary>
    public string Url { get; init; } = string.Empty;
}
