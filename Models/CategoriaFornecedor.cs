using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class CategoriaFornecedor : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
    public string? Descricao { get; set; }

    public bool Ativa { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public DateTime? DataAtualizacao { get; set; }
}
