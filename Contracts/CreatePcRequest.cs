namespace LabBack.Contracts;

public sealed record CreatePcRequest(
    int ProcessorFrequency,
    int RamAmount,
    string? UserShell,
    string? Os);
