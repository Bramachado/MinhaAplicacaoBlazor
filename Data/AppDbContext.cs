using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Models;
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

        modelBuilder.Entity<EscalaAulaPratica>()
            .HasOne(x => x.Tutor)
            .WithMany()
            .HasForeignKey(x => x.TutorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}