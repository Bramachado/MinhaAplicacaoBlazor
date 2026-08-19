using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Isolamento multi-empresa (multi-tenant). Concentra num só lugar:
///  - a FK <c>EmpresaId</c> de cada entidade <see cref="IEntidadeEmpresa"/>;
///  - o filtro global de consulta por empresa (toda leitura já vem filtrada);
///  - o carimbo automático do <c>EmpresaId</c> em novos registros no SaveChanges.
/// As telas e serviços não precisam saber que existe multi-empresa.
/// </summary>
public partial class AppDbContext
{
    private void ConfigurarMultiEmpresa(ModelBuilder modelBuilder)
    {
        // Para cada entidade do tenant: cria a FK para Empresa e o filtro global
        // que compara EmpresaId com a empresa da operação atual (EmpresaIdAtual).
        void Tenant<T>() where T : class, IEntidadeEmpresa
        {
            modelBuilder.Entity<T>()
                .HasOne<Empresa>()
                .WithMany()
                .HasForeignKey(e => e.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<T>()
                .HasQueryFilter(e => e.EmpresaId == EmpresaIdAtual);
        }

        Tenant<Unidade>();
        Tenant<Tutor>();
        Tenant<Colaborador>();
        Tenant<Fornecedor>();
        Tenant<Titulacao>();
        Tenant<Curso>();
        Tenant<CategoriaFornecedor>();
        Tenant<PlanoConta>();
        Tenant<FormaPagamento>();
        Tenant<Competencia>();
        Tenant<LancamentoFinanceiro>();
        Tenant<Entrada>();
        Tenant<FolhaColaborador>();
        Tenant<FolhaTutor>();
        Tenant<FolhaFornecedor>();
        Tenant<FolhaColaboradorItem>();
        Tenant<FolhaTutorItem>();
        Tenant<FolhaFornecedorItem>();
        Tenant<FolhaFornecedorItemNota>();
        Tenant<ContaBancaria>();
        Tenant<EscalaAulaPratica>();
        Tenant<Arquivo>();
    }

    /// <summary>
    /// Carimba o <c>EmpresaId</c> da operação atual em todo registro novo do tenant.
    /// Garante que nenhum registro nasça sem empresa ou na empresa errada, mesmo
    /// que a tela esqueça de preencher. Chamado no início do SaveChanges.
    /// </summary>
    private void AplicarEmpresa()
    {
        if (EmpresaIdAtual is not int empresaId)
            return;

        foreach (var entry in ChangeTracker.Entries<IEntidadeEmpresa>())
        {
            if (entry.State == EntityState.Added && entry.Entity.EmpresaId == 0)
                entry.Entity.EmpresaId = empresaId;
        }
    }
}
