namespace LabBack.Contracts;

/// <summary>
/// Параметры запроса для создания нового ПК.
/// </summary>
public sealed class CreatePcRequest
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
    /// Пользовательская оболочка. Если поле не указано, используется <c>XFCE</c>.
    /// </summary>
    public string? UserShell { get; init; }

    /// <summary>
    /// Операционная система. Если поле не указано, используется <c>Linux</c>.
    /// </summary>
    public string? Os { get; init; }
}
