using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Auditoria automática: intercepta o SaveChanges e grava um <see cref="RegistroAuditoria"/>
/// para cada inclusão/alteração/exclusão das entidades auditadas.
/// </summary>
public partial class AppDbContext
{
    /// <summary>Tipos cujas gravações geram registro de auditoria.</summary>
    private static readonly HashSet<Type> TiposAuditados = new()
    {
        typeof(Tutor),
        typeof(Colaborador),
        typeof(Fornecedor),
        typeof(ContaBancaria),
    };

    public override int SaveChanges()
    {
        NormalizarNomes();
        var pendentes = CapturarAuditoria();
        var resultado = base.SaveChanges();
        if (GravarAuditoria(pendentes))
            base.SaveChanges();
        return resultado;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizarNomes();
        var pendentes = await CapturarAuditoriaAsync(cancellationToken);
        var resultado = await base.SaveChangesAsync(cancellationToken);
        if (GravarAuditoria(pendentes))
            await base.SaveChangesAsync(cancellationToken);
        return resultado;
    }

    private List<EntityEntry> EntradasAuditaveis() =>
        ChangeTracker.Entries()
            .Where(e => e.Entity is not RegistroAuditoria
                        && TiposAuditados.Contains(e.Entity.GetType())
                        && (e.State == EntityState.Added
                            || e.State == EntityState.Modified
                            || e.State == EntityState.Deleted))
            .ToList();

    /// <summary>
    /// Percorre o ChangeTracker antes do save e captura o necessário de cada
    /// entidade auditada. Para alterações, busca no banco os valores atuais
    /// (as telas usam AsNoTracking + Update, então o OriginalValue rastreado
    /// é igual ao CurrentValue e não serviria para o diff).
    /// </summary>
    private List<AuditoriaPendente> CapturarAuditoria()
    {
        var pendentes = new List<AuditoriaPendente>();
        foreach (var entry in EntradasAuditaveis())
        {
            var original = entry.State == EntityState.Modified
                ? entry.GetDatabaseValues()
                : null;
            var pendente = MontarPendente(entry, original);
            if (pendente is not null)
                pendentes.Add(pendente);
        }
        return pendentes;
    }

    private async Task<List<AuditoriaPendente>> CapturarAuditoriaAsync(CancellationToken cancellationToken)
    {
        var pendentes = new List<AuditoriaPendente>();
        foreach (var entry in EntradasAuditaveis())
        {
            var original = entry.State == EntityState.Modified
                ? await entry.GetDatabaseValuesAsync(cancellationToken)
                : null;
            var pendente = MontarPendente(entry, original);
            if (pendente is not null)
                pendentes.Add(pendente);
        }
        return pendentes;
    }

    /// <summary>
    /// Monta o registro pendente de uma entidade. Retorna <c>null</c> quando a
    /// operação é uma "alteração" que, na prática, não mudou nenhum campo
    /// (ex.: as telas usam Update e marcam a entidade como Modified mesmo sem
    /// edição real) — nesse caso não faz sentido gerar registro de auditoria.
    /// </summary>
    private static AuditoriaPendente? MontarPendente(EntityEntry entry, PropertyValues? original)
    {
        var acao = entry.State switch
        {
            EntityState.Added => AcaoAuditoria.Inclusao,
            EntityState.Modified => AcaoAuditoria.Alteracao,
            _ => AcaoAuditoria.Exclusao
        };

        string? alteracoes = null;
        if (acao == AcaoAuditoria.Alteracao)
        {
            alteracoes = SerializarAlteracoes(entry, original);

            // Nenhum campo mudou de fato: ignora (não gera registro vazio).
            if (alteracoes is null)
                return null;
        }

        var pendente = new AuditoriaPendente
        {
            Entry = entry,
            Acao = acao,
            Entidade = entry.Entity.GetType().Name,
            Descricao = DescreverEntidade(entry.Entity),
            Alteracoes = alteracoes,
        };

        // Para alteração/exclusão a chave já está disponível; para inclusão ela é
        // gerada pelo banco e só é lida após o SaveChanges.
        if (acao != AcaoAuditoria.Inclusao)
            pendente.EntidadeId = ObterId(entry);

        return pendente;
    }

    /// <summary>
    /// Após o save principal, monta os registros de auditoria (agora com as
    /// chaves geradas) e os adiciona ao contexto. Retorna true se há algo a salvar.
    /// </summary>
    private bool GravarAuditoria(List<AuditoriaPendente> pendentes)
    {
        if (pendentes.Count == 0)
            return false;

        foreach (var p in pendentes)
        {
            var id = p.EntidadeId ?? ObterId(p.Entry);

            RegistrosAuditoria.Add(new RegistroAuditoria
            {
                UsuarioId = UsuarioAuditoriaId,
                UsuarioNome = string.IsNullOrWhiteSpace(UsuarioAuditoriaNome) ? "Sistema" : UsuarioAuditoriaNome,
                Entidade = p.Entidade,
                EntidadeId = id,
                Acao = p.Acao,
                Descricao = p.Descricao,
                Alteracoes = p.Alteracoes,
            });
        }

        return true;
    }

    private static int ObterId(EntityEntry entry)
    {
        var valor = entry.Property("Id").CurrentValue;
        return valor is int i ? i : 0;
    }

    private static string? DescreverEntidade(object entidade) => entidade switch
    {
        Tutor t => t.Nome,
        Colaborador c => c.Nome,
        Fornecedor f => f.NomeRazaoSocial,
        ContaBancaria cb => cb.NomeTitular,
        _ => null
    };

    /// <summary>
    /// Serializa os campos escalares que realmente mudaram (valor no banco vs.
    /// valor atual), no formato { "Campo": { "de": "...", "para": "..." } }.
    /// </summary>
    private static string? SerializarAlteracoes(EntityEntry entry, PropertyValues? original)
    {
        if (original is null)
            return null;

        var atual = entry.CurrentValues;
        var alteracoes = new Dictionary<string, Dictionary<string, string?>>();

        foreach (var prop in atual.Properties)
        {
            if (prop.IsPrimaryKey())
                continue;

            var de = original[prop.Name]?.ToString();
            var para = atual[prop.Name]?.ToString();

            if (de == para)
                continue;

            alteracoes[prop.Name] = new Dictionary<string, string?>
            {
                ["de"] = de,
                ["para"] = para
            };
        }

        return alteracoes.Count == 0
            ? null
            : JsonSerializer.Serialize(alteracoes);
    }

    /// <summary>Dados capturados de uma entidade antes do SaveChanges.</summary>
    private sealed class AuditoriaPendente
    {
        public required EntityEntry Entry { get; init; }
        public required AcaoAuditoria Acao { get; init; }
        public required string Entidade { get; init; }
        public string? Descricao { get; init; }
        public int? EntidadeId { get; set; }
        public string? Alteracoes { get; set; }
    }
}
