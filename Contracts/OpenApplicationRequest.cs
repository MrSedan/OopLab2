namespace LabBack.Contracts;

/// <summary>
/// Параметры запроса для открытия настольного приложения на ПК.
/// </summary>
public sealed class OpenApplicationRequest
{
    /// <summary>
    /// Название приложения.
    /// </summary>
    public string AppName { get; init; } = string.Empty;
}
