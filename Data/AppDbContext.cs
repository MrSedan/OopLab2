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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Computer>(entity =>
        {
            entity.ToTable("computers");

            entity.UseTptMappingStrategy();

            entity.HasKey(computer => computer.Id);

            entity.Property(computer => computer.Id)
                .HasColumnName("id");

            entity.Property(computer => computer.ProcessorFrequency)
                .HasColumnName("processor_frequency");

            entity.Property(computer => computer.RamAmount)
                .HasColumnName("ram_amount");
        });

        modelBuilder.Entity<PC>(entity =>
        {
            entity.ToTable("pcs");

            entity.Property(pc => pc.Id)
                .HasColumnName("id");

            entity.Property(pc => pc.UserShell)
                .HasColumnName("user_shell")
                .HasMaxLength(100);

            entity.Property(pc => pc.Os)
                .HasColumnName("os")
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.ToTable("servers");

            entity.Property(server => server.Id)
                .HasColumnName("id");

            entity.Property(server => server.MaxConnections)
                .HasColumnName("max_connections");

            entity.Property(server => server.CurrentConnections)
                .HasColumnName("current_connections");
        });
    }
}
