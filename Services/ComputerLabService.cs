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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
