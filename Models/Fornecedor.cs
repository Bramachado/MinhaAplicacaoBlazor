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

    [MaxLength(150)]
    public string? ChavePix { get; set; }

    [MaxLength(100)]
    public string? Banco { get; set; }

    [MaxLength(20)]
    public string? CodigoBanco { get; set; }

    [MaxLength(30)]
    public string? Agencia { get; set; }

    [MaxLength(50)]
    public string? Conta { get; set; }

    [MaxLength(200)]
    public string? NomeTitularConta { get; set; }

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
}
