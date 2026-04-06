namespace LabBack.Contracts;

public sealed record CreateServerRequest(
    int ProcessorFrequency,
    int RamAmount,
    int MaxConnections,
    int CurrentConnections = 0);
