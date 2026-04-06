namespace LabBack.Contracts.Responses;

/// <summary>
/// Краткое описание ПК, возвращаемое API.
/// </summary>
public sealed class PcSummaryResponse
{
    /// <summary>
    /// Идентификатор ПК.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Частота процессора в МГц.
    /// </summary>
    public int ProcessorFrequency { get; init; }

    /// <summary>
    /// Объем оперативной памяти в МБ.
    /// </summary>
    public int RamAmount { get; init; }

    /// <summary>
    /// Пользовательская оболочка.
    /// </summary>
    public string UserShell { get; init; } = string.Empty;

    /// <summary>
    /// Операционная система.
    /// </summary>
    public string Os { get; init; } = string.Empty;
}

/// <summary>
/// Краткое описание сервера, возвращаемое API.
/// </summary>
public sealed class ServerSummaryResponse
{
    /// <summary>
    /// Идентификатор сервера.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Частота процессора в МГц.
    /// </summary>
    public int ProcessorFrequency { get; init; }

    /// <summary>
    /// Объем оперативной памяти в МБ.
    /// </summary>
    public int RamAmount { get; init; }

    /// <summary>
    /// Максимальное количество одновременных подключений.
    /// </summary>
    public int MaxConnections { get; init; }

    /// <summary>
    /// Текущее количество активных подключений.
    /// </summary>
    public int CurrentConnections { get; init; }
}

/// <summary>
/// Стандартный ответ с сообщением.
/// </summary>
public sealed class OperationMessageResponse
{
    /// <summary>
    /// Сообщение ответа.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Ответ после создания ПК или сервера.
/// </summary>
public sealed class CreatedResourceResponse
{
    /// <summary>
    /// Идентификатор созданного ресурса.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Текст статуса.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Ответ при изменении счетчика подключений сервера.
/// </summary>
public sealed class ConnectionStatusResponse
{
    /// <summary>
    /// Текст статуса.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Текущее количество активных подключений.
    /// </summary>
    public int CurrentConnections { get; init; }

    /// <summary>
    /// Максимальное количество одновременных подключений.
    /// </summary>
    public int MaxConnections { get; init; }
}
