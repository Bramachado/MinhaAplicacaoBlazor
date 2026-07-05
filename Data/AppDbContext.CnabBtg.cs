using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.CnabBtg.Data;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Persistência do subsistema CNAB BTG (novo): lotes de geração, arquivos,
/// pagamentos incluídos e sequência de NSA por empresa pagadora.
/// </summary>
public partial class AppDbContext
{
    public DbSet<CnabBatch> CnabBatches => Set<CnabBatch>();
    public DbSet<CnabGeneratedFile> CnabGeneratedFiles => Set<CnabGeneratedFile>();
    public DbSet<CnabBatchPayment> CnabBatchPayments => Set<CnabBatchPayment>();
    public DbSet<CnabSequence> CnabSequences => Set<CnabSequence>();

    private static void ConfigurarCnabBtg(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CnabBatch>()
            .HasMany(b => b.Arquivos)
            .WithOne(a => a.CnabBatch!)
            .HasForeignKey(a => a.CnabBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CnabBatch>()
            .HasMany(b => b.Pagamentos)
            .WithOne(p => p.CnabBatch!)
            .HasForeignKey(p => p.CnabBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CnabBatch>()
            .HasIndex(b => b.GeradoEm);

        modelBuilder.Entity<CnabBatchPayment>()
            .HasIndex(p => new { p.Origem, p.OrigemId });

        modelBuilder.Entity<CnabSequence>()
            .HasIndex(s => s.EmpresaPagadora)
            .IsUnique();
    }
}
