namespace LabBack.Contracts.Responses;

/// <summary>
/// Summary view of a PC returned by the API.
/// </summary>
public sealed class PcSummaryResponse
{
    /// <summary>
    /// Gets or sets the PC identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the processor frequency in MHz.
    /// </summary>
    public int ProcessorFrequency { get; init; }

    /// <summary>
    /// Gets or sets the RAM size in MB.
    /// </summary>
    public int RamAmount { get; init; }

    /// <summary>
    /// Gets or sets the user shell.
    /// </summary>
    public string UserShell { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the operating system.
    /// </summary>
    public string Os { get; init; } = string.Empty;
}

/// <summary>
/// Summary view of a server returned by the API.
/// </summary>
public sealed class ServerSummaryResponse
{
    /// <summary>
    /// Gets or sets the server identifier.
    /// </summary>
    public int Id { get; init; }

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
    public int CurrentConnections { get; init; }
}

/// <summary>
/// Standard message response.
/// </summary>
public sealed class OperationMessageResponse
{
    /// <summary>
    /// Gets or sets the response message.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Response returned after creating a PC or server.
/// </summary>
public sealed class CreatedResourceResponse
{
    /// <summary>
    /// Gets or sets the created resource identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Response returned when the server connection counter changes.
/// </summary>
public sealed class ConnectionStatusResponse
{
    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the current number of active connections.
    /// </summary>
    public int CurrentConnections { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of simultaneous connections.
    /// </summary>
    public int MaxConnections { get; init; }
}
