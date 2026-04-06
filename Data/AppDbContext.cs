using LabBack.Models;
using Microsoft.EntityFrameworkCore;

namespace LabBack.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<PC> Pcs => Set<PC>();
    public DbSet<Server> Servers => Set<Server>();
}
