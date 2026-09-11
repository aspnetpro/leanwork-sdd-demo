using Microsoft.EntityFrameworkCore;

namespace Encurtador.Web.Dados;

public class EncurtadorDbContext(DbContextOptions<EncurtadorDbContext> options) : DbContext(options)
{
    public DbSet<Link> Links => Set<Link>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>(entity =>
        {
            entity.Property(l => l.Codigo)
                .IsRequired()
                .HasMaxLength(7);

            entity.Property(l => l.UrlDestino)
                .IsRequired()
                .HasMaxLength(2048);

            // SQLite não tem tipo nativo de data: o DateTimeKind se perde no round-trip (risco registrado em T-02).
            // Gravação já é sempre UTC (RN-09); a conversão de leitura restaura o Kind para refletir isso.
            entity.Property(l => l.CriadoEm)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.HasIndex(l => l.Codigo)
                .IsUnique();
        });
    }
}
