using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Fornecedor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome/razão social é obrigatório.")]
    [MaxLength(200)]
    public string NomeRazaoSocial { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NomeFantasia { get; set; }

    [MaxLength(20)]
    public string? CpfCnpj { get; set; }

        [MaxLength(250)]
    public string? Endereco { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(150)]
    public string? NomeContato { get; set; }

    public bool Ativo { get; set; } = true;

    public int UnidadeId { get; set; }

    public Unidade? Unidade { get; set; }

    public int CategoriaFornecedorId { get; set; }

    public CategoriaFornecedor? CategoriaFornecedor { get; set; }

    /// <summary>
    /// Plano de contas associado ao fornecedor. Usado para classificar
    /// contabilmente as despesas (folha de fornecedores) por conta.
    /// </summary>
    public int? PlanoContaId { get; set; }

    public PlanoConta? PlanoConta { get; set; }

    public int? ContaBancariaId { get; set; }

    public ContaBancaria? ContaBancaria { get; set; }

    /// <summary>
    /// Tipo de pagamento do fornecedor: "Transferencia" quando tem conta bancária
    /// vinculada, "Boleto" quando não tem. Coluna persistida (para filtro/ordenação
    /// via SQL), mantida em sincronia automaticamente no SaveChanges a partir de
    /// <see cref="ContaBancariaId"/>.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string TipoPagamento { get; set; } = "Boleto";
}
