using LabBack.Models;

namespace LabBack.Services;

public interface IComputerLabService
{
    Task<IReadOnlyCollection<PC>> GetAllPcsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Server>> GetAllServersAsync(CancellationToken cancellationToken = default);
    Task<PC?> GetPcAsync(int id, CancellationToken cancellationToken = default);
    Task<Server?> GetServerAsync(int id, CancellationToken cancellationToken = default);
    Task<int> AddPcAsync(PC pc, CancellationToken cancellationToken = default);
    Task<int> AddServerAsync(Server server, CancellationToken cancellationToken = default);
    Task<PC?> UpdatePcAsync(int id, PC updatedPc, CancellationToken cancellationToken = default);
    Task<Server?> UpdateServerAsync(int id, Server updatedServer, CancellationToken cancellationToken = default);
    Task<bool> DeletePcAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteServerAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
