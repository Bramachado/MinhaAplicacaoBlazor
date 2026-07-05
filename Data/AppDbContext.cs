using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Models;
using MinhaAplicacaoBlazor.Models.Auth;
using MinhaAplicacaoBlazor.Data;


namespace MinhaAplicacaoBlazor.Data;

public partial class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Id do usuário logado que originou a operação. Definido por operação
    /// (via CreateDbContextAsync(user)); usado pela auditoria automática.
    /// </summary>
    public string? UsuarioAuditoriaId { get; set; }

    /// <summary>Nome do usuário logado que originou a operação.</summary>
    public string? UsuarioAuditoriaNome { get; set; }

    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<PerfilPermissao> PerfilPermissoes => Set<PerfilPermissao>();
    public DbSet<UsuarioPermissao> UsuarioPermissoes => Set<UsuarioPermissao>();

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
    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<ContaBancaria> ContasBancarias => Set<ContaBancaria>();
    public DbSet<Arquivo> Arquivos => Set<Arquivo>();
    public DbSet<Entrada> Entradas => Set<Entrada>();
    public DbSet<LancamentoFinanceiro> LancamentosFinanceiros => Set<LancamentoFinanceiro>();
    public DbSet<FolhaColaborador> FolhasColaboradores => Set<FolhaColaborador>();
    public DbSet<FolhaColaboradorItem> FolhasColaboradoresItens => Set<FolhaColaboradorItem>();
    public DbSet<FolhaTutor> FolhasTutores => Set<FolhaTutor>();
    public DbSet<FolhaTutorItem> FolhasTutoresItens => Set<FolhaTutorItem>();
    public DbSet<FolhaFornecedor> FolhasFornecedores => Set<FolhaFornecedor>();
    public DbSet<FolhaFornecedorItem> FolhasFornecedoresItens => Set<FolhaFornecedorItem>();

    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Autenticação / Permissões ===
        modelBuilder.Entity<Perfil>()
            .HasIndex(x => x.Nome)
            .IsUnique();

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(x => x.Perfil)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.PerfilId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PerfilPermissao>()
            .HasIndex(x => new { x.PerfilId, x.Permissao })
            .IsUnique();

        modelBuilder.Entity<PerfilPermissao>()
            .HasOne(x => x.Perfil)
            .WithMany(x => x.Permissoes)
            .HasForeignKey(x => x.PerfilId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UsuarioPermissao>()
            .HasIndex(x => new { x.UsuarioId, x.Permissao })
            .IsUnique();

        modelBuilder.Entity<UsuarioPermissao>()
            .HasOne(x => x.Usuario)
            .WithMany(x => x.Permissoes)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<Fornecedor>()
            .HasOne(x => x.PlanoConta)
            .WithMany()
            .HasForeignKey(x => x.PlanoContaId)
            .OnDelete(DeleteBehavior.Restrict);

        // === Conta Bancária: enums gravados como texto ===
        modelBuilder.Entity<ContaBancaria>()
            .Property(x => x.TipoPessoa)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<ContaBancaria>()
            .Property(x => x.Forma)
            .HasConversion<string>()
            .HasMaxLength(10);

        modelBuilder.Entity<ContaBancaria>()
            .Property(x => x.TipoConta)
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasDefaultValue(TipoConta.Corrente);

        modelBuilder.Entity<ContaBancaria>()
            .Property(x => x.TipoChavePix)
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasDefaultValue(TipoChavePix.Nenhuma);

        // === Conta Bancária (uma conta por Tutor / Colaborador / Fornecedor) ===
        modelBuilder.Entity<Tutor>()
            .HasOne(x => x.ContaBancaria)
            .WithOne()
            .HasForeignKey<Tutor>(x => x.ContaBancariaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Colaborador>()
            .HasOne(x => x.ContaBancaria)
            .WithOne()
            .HasForeignKey<Colaborador>(x => x.ContaBancariaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Fornecedor>()
            .HasOne(x => x.ContaBancaria)
            .WithOne()
            .HasForeignKey<Fornecedor>(x => x.ContaBancariaId)
            .OnDelete(DeleteBehavior.SetNull);

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

        modelBuilder.Entity<FolhaFornecedor>()
            .Property(x => x.Status)
            .HasDefaultValue("Aberta");

        modelBuilder.Entity<FolhaFornecedor>()
            .Property(x => x.ValorTotal)
            .HasDefaultValue(0m);

        modelBuilder.Entity<FolhaFornecedor>()
            .HasOne(x => x.Competencia)
            .WithMany()
            .HasForeignKey(x => x.CompetenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FolhaFornecedor>()
            .HasOne(x => x.LancamentoFinanceiro)
            .WithMany()
            .HasForeignKey(x => x.LancamentoFinanceiroId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FolhaFornecedorItem>()
            .HasOne(x => x.FolhaFornecedor)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.FolhaFornecedorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FolhaFornecedorItem>()
            .HasOne(x => x.Fornecedor)
            .WithMany()
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FolhaFornecedorItem>()
            .HasOne(x => x.BancoPagador)
            .WithMany()
            .HasForeignKey(x => x.BancoPagadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Banco>()
            .HasIndex(x => x.NomeBanco)
            .IsUnique();

        modelBuilder.Entity<Arquivo>()
            .HasOne(x => x.FolhaFornecedorItem)
            .WithMany(x => x.Arquivos)
            .HasForeignKey(x => x.FolhaFornecedorItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Entrada>()
            .HasOne(x => x.Fornecedor)
            .WithMany()
            .HasForeignKey(x => x.FornecedorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Entrada>()
            .HasOne(x => x.Competencia)
            .WithMany()
            .HasForeignKey(x => x.CompetenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Entrada>()
            .HasOne(x => x.Banco)
            .WithMany()
            .HasForeignKey(x => x.BancoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Arquivo>()
            .HasOne(x => x.Entrada)
            .WithMany(x => x.Arquivos)
            .HasForeignKey(x => x.EntradaId)
            .OnDelete(DeleteBehavior.Cascade);

        // === Auditoria ===
        modelBuilder.Entity<RegistroAuditoria>()
            .Property(x => x.Acao)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<RegistroAuditoria>()
            .Property(x => x.DataHora)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<RegistroAuditoria>()
            .HasIndex(x => x.DataHora);

        modelBuilder.Entity<RegistroAuditoria>()
            .HasIndex(x => new { x.Entidade, x.EntidadeId });

        modelBuilder.Entity<RegistroAuditoria>()
            .HasIndex(x => x.UsuarioId);
    }
}