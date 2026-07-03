using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Models.Crm;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Subsistema de CRM. Tabelas mapeadas no schema SQL "crm" e sem nenhuma chave
/// estrangeira para o núcleo do sistema — o CRM é autocontido (só há FKs entre
/// as próprias tabelas Crm). Isso permite, se necessário, extrair o módulo para
/// outro banco/serviço sem impacto no restante da aplicação.
/// </summary>
public partial class AppDbContext
{
    public DbSet<CrmContato> CrmContatos => Set<CrmContato>();
    public DbSet<CrmEtapaFunil> CrmEtapasFunil => Set<CrmEtapaFunil>();
    public DbSet<CrmOportunidade> CrmOportunidades => Set<CrmOportunidade>();

    /// <summary>Configuração do modelo do CRM (chamada a partir do OnModelCreating).</summary>
    private static void ConfigurarCrm(ModelBuilder modelBuilder)
    {
        const string schema = "crm";

        // === Contato ===
        modelBuilder.Entity<CrmContato>(e =>
        {
            e.ToTable("Contatos", schema);
            e.Property(x => x.CriadoEm).HasDefaultValueSql("GETDATE()");
            e.HasIndex(x => x.Nome);
            e.HasIndex(x => x.ResponsavelUserId);
        });

        // === Etapa do Funil ===
        modelBuilder.Entity<CrmEtapaFunil>(e =>
        {
            e.ToTable("EtapasFunil", schema);
            e.HasIndex(x => x.Ordem);
        });

        // === Oportunidade ===
        modelBuilder.Entity<CrmOportunidade>(e =>
        {
            e.ToTable("Oportunidades", schema);

            e.Property(x => x.Valor).HasPrecision(18, 2);

            e.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            e.Property(x => x.CriadoEm).HasDefaultValueSql("GETDATE()");

            e.HasIndex(x => x.ResponsavelUserId);
            e.HasIndex(x => x.CrmEtapaFunilId);

            // FKs internas do CRM (Restrict: impede apagar contato/etapa em uso).
            e.HasOne(x => x.Contato)
                .WithMany()
                .HasForeignKey(x => x.CrmContatoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Etapa)
                .WithMany()
                .HasForeignKey(x => x.CrmEtapaFunilId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
