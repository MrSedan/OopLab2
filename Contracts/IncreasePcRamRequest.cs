namespace LabBack.Contracts;

/// <summary>
/// Параметры запроса для увеличения объема оперативной памяти на ПК.
/// </summary>
public sealed class IncreasePcRamRequest
{
    /// <summary>
    /// Количество оперативной памяти в МБ, на которое нужно увеличить текущее значение.
    /// </summary>
    public int Amount { get; init; }
}
