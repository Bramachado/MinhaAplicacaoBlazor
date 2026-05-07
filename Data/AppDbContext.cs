using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Models;
using MinhaAplicacaoBlazor.Models.Cnab;
using MinhaAplicacaoBlazor.Data;


namespace MinhaAplicacaoBlazor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Titulacao> Titulacoes => Set<Titulacao>();
    public DbSet<EscalaAulaPratica> EscalasAulasPraticas => Set<EscalaAulaPratica>();
    public DbSet<Unidade> Unidades => Set<Unidade>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<CategoriaFornecedor> CategoriasFornecedores => Set<CategoriaFornecedor>();
    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();
    public DbSet<Competencia> Competencias => Set<Competencia>();
    public DbSet<PlanoConta> PlanosContas => Set<PlanoConta>();
    public DbSet<FormaPagamento> FormasPagamento => Set<FormaPagamento>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();
    public DbSet<FolhaColaborador> FolhasColaboradores => Set<FolhaColaborador>();
    public DbSet<FolhaColaboradorItem> FolhasColaboradoresItens => Set<FolhaColaboradorItem>();
    public DbSet<FolhaTutor> FolhasTutores => Set<FolhaTutor>();
    public DbSet<FolhaTutorItem> FolhasTutoresItens => Set<FolhaTutorItem>();
    public DbSet<ConfiguracaoCnab> ConfiguracoesCnab => Set<ConfiguracaoCnab>();
    public DbSet<FormaLancamentoCnab> FormasLancamentoCnab => Set<FormaLancamentoCnab>();
    public DbSet<RemessaCnab> RemessasCnab => Set<RemessaCnab>();
    public DbSet<RemessaCnabItem> RemessasCnabItens => Set<RemessaCnabItem>();
    public DbSet<RetornoCnab> RetornosCnab => Set<RetornoCnab>();
    public DbSet<RetornoCnabItem> RetornosCnabItens => Set<RetornoCnabItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Titulacao>()
            .Property(x => x.ValorHoraAulaNormal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Titulacao>()
            .Property(x => x.ValorHoraAulaPratica)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Tutor>()
            .HasIndex(x => x.Cpf)
            .IsUnique();

        modelBuilder.Entity<Tutor>()
            .HasOne(x => x.Titulacao)
            .WithMany(x => x.Tutores)
            .HasForeignKey(x => x.TitulacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Unidade>()
            .HasIndex(x => x.Codigo)
            .IsUnique();

        modelBuilder.Entity<Tutor>()
            .HasOne(x => x.Unidade)
            .WithMany(x => x.Tutores)
            .HasForeignKey(x => x.UnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tutor>()
            .HasOne(x => x.Curso)
            .WithMany(x => x.Tutores)
            .HasForeignKey(x => x.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EscalaAulaPratica>()
            .HasOne(x => x.Tutor)
            .WithMany()
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CategoriaFornecedor>()
            .HasIndex(x => x.Nome)
            .IsUnique();

        modelBuilder.Entity<Colaborador>()
            .HasIndex(x => x.Cpf)
            .IsUnique();

        modelBuilder.Entity<Colaborador>()
            .HasOne(x => x.Unidade)
            .WithMany()
            .HasForeignKey(x => x.UnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Competencia>()
            .Property(x => x.DataInicio)
            .HasColumnType("date");

        modelBuilder.Entity<Competencia>()
            .Property(x => x.DataFim)
            .HasColumnType("date");

        modelBuilder.Entity<Competencia>()
            .Property(x => x.Status)
            .HasDefaultValue("Aberta");

        modelBuilder.Entity<Competencia>()
            .HasIndex(x => new { x.Mes, x.Ano })
            .IsUnique();

        modelBuilder.Entity<PlanoConta>()
            .Property(x => x.Ativo)
            .HasDefaultValue(true);

        modelBuilder.Entity<PlanoConta>()
            .HasIndex(x => x.Nome)
            .IsUnique();

        modelBuilder.Entity<FormaPagamento>()
            .Property(x => x.Ativa)
            .HasDefaultValue(true);

        modelBuilder.Entity<FormaPagamento>()
            .HasIndex(x => x.Nome)
            .IsUnique();

        modelBuilder.Entity<Fornecedor>()
            .Property(x => x.Ativo)
            .HasDefaultValue(true);

        modelBuilder.Entity<Fornecedor>()
            .HasOne(x => x.Unidade)
            .WithMany()
            .HasForeignKey(x => x.UnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Fornecedor>()
            .HasOne(x => x.CategoriaFornecedor)
            .WithMany()
            .HasForeignKey(x => x.CategoriaFornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LancamentoFinanceiro>()
            .Property(x => x.Origem)
            .HasDefaultValue("Manual");

        modelBuilder.Entity<LancamentoFinanceiro>()
            .Property(x => x.Status)
            .HasDefaultValue("Aberto");

        modelBuilder.Entity<LancamentoFinanceiro>()
            .Property(x => x.CriadoEm)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<LancamentoFinanceiro>()
            .Property(x => x.DataVencimento)
            .HasColumnType("date");

        modelBuilder.Entity<LancamentoFinanceiro>()
            .Property(x => x.DataPagamento)
            .HasColumnType("date");

        modelBuilder.Entity<LancamentoFinanceiro>()
            .HasOne(x => x.Competencia)
            .WithMany()
            .HasForeignKey(x => x.CompetenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LancamentoFinanceiro>()
            .HasOne(x => x.Unidade)
            .WithMany()
            .HasForeignKey(x => x.UnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LancamentoFinanceiro>()
            .HasOne(x => x.PlanoConta)
            .WithMany()
            .HasForeignKey(x => x.PlanoContaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LancamentoFinanceiro>()
            .HasOne(x => x.FormaPagamento)
            .WithMany()
            .HasForeignKey(x => x.FormaPagamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LancamentoFinanceiro>()
            .Property(x => x.FornecedorId)
            .IsRequired(false);

        modelBuilder.Entity<LancamentoFinanceiro>()
            .HasOne(x => x.Fornecedor)
            .WithMany()
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FolhaColaborador>()
            .Property(x => x.Status)
            .HasDefaultValue("Aberta");

        modelBuilder.Entity<FolhaColaborador>()
            .Property(x => x.ValorTotal)
            .HasDefaultValue(0m);

        modelBuilder.Entity<FolhaColaborador>()
            .HasOne(x => x.Competencia)
            .WithMany()
            .HasForeignKey(x => x.CompetenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FolhaColaborador>()
            .HasOne(x => x.LancamentoFinanceiro)
            .WithMany()
            .HasForeignKey(x => x.LancamentoFinanceiroId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FolhaColaboradorItem>()
            .HasOne(x => x.FolhaColaborador)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.FolhaColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FolhaColaboradorItem>()
            .HasOne(x => x.Colaborador)
            .WithMany()
            .HasForeignKey(x => x.ColaboradorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FolhaTutor>()
            .Property(x => x.Status)
            .HasDefaultValue("Aberta");

        modelBuilder.Entity<FolhaTutor>()
            .Property(x => x.ValorTotal)
            .HasDefaultValue(0m);

        modelBuilder.Entity<FolhaTutor>()
            .HasOne(x => x.Competencia)
            .WithMany()
            .HasForeignKey(x => x.CompetenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FolhaTutor>()
            .HasOne(x => x.LancamentoFinanceiro)
            .WithMany()
            .HasForeignKey(x => x.LancamentoFinanceiroId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FolhaTutorItem>()
            .HasOne(x => x.FolhaTutor)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.FolhaTutorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FolhaTutorItem>()
            .HasOne(x => x.Tutor)
            .WithMany()
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Restrict);

        // === CNAB ===
        modelBuilder.Entity<ConfiguracaoCnab>()
            .HasIndex(x => x.NomeConfiguracao)
            .IsUnique();

        modelBuilder.Entity<FormaLancamentoCnab>()
            .HasIndex(x => x.Codigo)
            .IsUnique();

        modelBuilder.Entity<RemessaCnab>()
            .HasIndex(x => x.NumeroSequencial);

        modelBuilder.Entity<RemessaCnab>()
            .HasIndex(x => x.NomeArquivo);

        modelBuilder.Entity<RemessaCnab>()
            .HasOne(x => x.ConfiguracaoCnab)
            .WithMany()
            .HasForeignKey(x => x.ConfiguracaoCnabId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RemessaCnab>()
            .HasOne(x => x.Competencia)
            .WithMany()
            .HasForeignKey(x => x.CompetenciaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasIndex(x => x.SeuNumero);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasOne(x => x.RemessaCnab)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.RemessaCnabId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasOne(x => x.FormaLancamentoCnab)
            .WithMany()
            .HasForeignKey(x => x.FormaLancamentoCnabId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasOne(x => x.LancamentoFinanceiro)
            .WithMany()
            .HasForeignKey(x => x.LancamentoFinanceiroId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasOne(x => x.FolhaColaboradorItem)
            .WithMany()
            .HasForeignKey(x => x.FolhaColaboradorItemId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasOne(x => x.FolhaTutorItem)
            .WithMany()
            .HasForeignKey(x => x.FolhaTutorItemId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RemessaCnabItem>()
            .HasOne(x => x.Fornecedor)
            .WithMany()
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RetornoCnab>()
            .HasOne(x => x.RemessaCnab)
            .WithMany()
            .HasForeignKey(x => x.RemessaCnabId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RetornoCnabItem>()
            .HasOne(x => x.RetornoCnab)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.RetornoCnabId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RetornoCnabItem>()
            .HasOne(x => x.RemessaCnabItem)
            .WithMany()
            .HasForeignKey(x => x.RemessaCnabItemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}