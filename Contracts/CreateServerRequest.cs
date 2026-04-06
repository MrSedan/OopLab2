namespace LabBack.Contracts;

/// <summary>
/// Параметры запроса для создания нового сервера.
/// </summary>
public sealed class CreateServerRequest
{
    /// <summary>
    /// Частота процессора в МГц.
    /// </summary>
    public int ProcessorFrequency { get; init; }

    /// <summary>
    /// Объем оперативной памяти в МБ.
    /// </summary>
    public int RamAmount { get; init; }

    /// <summary>
    /// Максимальное число одновременных подключений.
    /// </summary>
    public int MaxConnections { get; init; }

    /// <summary>
    /// Текущее число активных подключений.
    /// </summary>
    public int CurrentConnections { get; init; } = 0;
}
