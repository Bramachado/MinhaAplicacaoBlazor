using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

/// <summary>
/// Tipo de operação registrada na auditoria.
/// </summary>
public enum AcaoAuditoria
{
    Inclusao,
    Alteracao,
    Exclusao
}

/// <summary>
/// Registro imutável de uma operação (inclusão/alteração/exclusão) feita sobre
/// uma entidade auditada (Tutor, Colaborador, Fornecedor, ContaBancaria).
/// Os dados do usuário são desnormalizados de propósito: o log deve sobreviver
/// mesmo que o usuário seja excluído posteriormente.
/// </summary>
public class RegistroAuditoria
{
    public int Id { get; set; }

    /// <summary>Quando a operação ocorreu (preenchido pelo banco com GETDATE()).</summary>
    public DateTime DataHora { get; set; }

    /// <summary>Id do usuário (ApplicationUser) que realizou a operação, se conhecido.</summary>
    [MaxLength(450)]
    public string? UsuarioId { get; set; }

    /// <summary>Nome do usuário no momento da operação (desnormalizado).</summary>
    [MaxLength(200)]
    public string UsuarioNome { get; set; } = "Sistema";

    /// <summary>Nome da entidade afetada, ex.: "Tutor", "ContaBancaria".</summary>
    [Required]
    [MaxLength(80)]
    public string Entidade { get; set; } = string.Empty;

    /// <summary>Chave primária do registro afetado.</summary>
    public int EntidadeId { get; set; }

    /// <summary>Operação realizada.</summary>
    public AcaoAuditoria Acao { get; set; }

    /// <summary>Rótulo legível do registro afetado (ex.: nome do tutor).</summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// JSON com os campos alterados no formato { "Campo": { "de": "...", "para": "..." } }.
    /// Preenchido apenas em alterações.
    /// </summary>
    public string? Alteracoes { get; set; }
}
