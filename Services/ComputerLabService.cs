using LabBack.Data;
using LabBack.Models;
using Microsoft.EntityFrameworkCore;

namespace LabBack.Services;

public sealed class ComputerLabService : IComputerLabService
{
    private readonly AppDbContext _dbContext;

    public ComputerLabService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PC>> GetAllPcsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Pcs
            .AsNoTracking()
            .OrderBy(pc => pc.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Server>> GetAllServersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Servers
            .AsNoTracking()
            .OrderBy(server => server.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<PC?> GetPcAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Pcs.FindAsync([id], cancellationToken);
    }

    public async Task<Server?> GetServerAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Servers.FindAsync([id], cancellationToken);
    }

    public async Task<int> AddPcAsync(PC pc, CancellationToken cancellationToken = default)
    {
        _dbContext.Pcs.Add(pc);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return pc.Id;
    }

    public async Task<int> AddServerAsync(Server server, CancellationToken cancellationToken = default)
    {
        _dbContext.Servers.Add(server);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return server.Id;
    }

    public async Task<PC?> UpdatePcAsync(int id, PC updatedPc, CancellationToken cancellationToken = default)
    {
        var pc = await _dbContext.Pcs.FindAsync([id], cancellationToken);
        if (pc is null)
        {
            return null;
        }

        pc.ProcessorFrequency = updatedPc.ProcessorFrequency;
        pc.RamAmount = updatedPc.RamAmount;
        pc.UserShell = updatedPc.UserShell;
        pc.Os = updatedPc.Os;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return pc;
    }

    public async Task<Server?> UpdateServerAsync(int id, Server updatedServer, CancellationToken cancellationToken = default)
    {
        var server = await _dbContext.Servers.FindAsync([id], cancellationToken);
        if (server is null)
        {
            return null;
        }

        server.Update(
            updatedServer.ProcessorFrequency,
            updatedServer.RamAmount,
            updatedServer.MaxConnections,
            updatedServer.CurrentConnections);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return server;
    }

    public async Task<bool> DeletePcAsync(int id, CancellationToken cancellationToken = default)
    {
        var pc = await _dbContext.Pcs.FindAsync([id], cancellationToken);
        if (pc is null)
        {
            return false;
        }

        _dbContext.Pcs.Remove(pc);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteServerAsync(int id, CancellationToken cancellationToken = default)
    {
        var server = await _dbContext.Servers.FindAsync([id], cancellationToken);
        if (server is null)
        {
            return false;
        }

        _dbContext.Servers.Remove(server);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
