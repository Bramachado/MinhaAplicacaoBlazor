using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models;

public class Arquivo : IEntidadeEmpresa
{
    public int Id { get; set; }

    /// <summary>Empresa (tenant) dona do registro; carimbada automaticamente.</summary>
    public int EmpresaId { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [MaxLength(260)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long TamanhoBytes { get; set; }

    // Conteúdo binário do arquivo armazenado no banco (varbinary(max)).
    [Required]
    public byte[] Conteudo { get; set; } = Array.Empty<byte>();

    // Vínculo opcional a um item da folha de fornecedores (anexo).
    public int? FolhaFornecedorItemId { get; set; }
    public FolhaFornecedorItem? FolhaFornecedorItem { get; set; }

    // Vínculo opcional a uma entrada financeira (anexo).
    public int? EntradaId { get; set; }
    public Entrada? Entrada { get; set; }
}
