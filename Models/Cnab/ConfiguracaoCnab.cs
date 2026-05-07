using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.Models.Cnab;

public class ConfiguracaoCnab
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string NomeConfiguracao { get; set; } = string.Empty;

    [Required, MaxLength(3)]
    public string BancoCodigo { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string BancoNome { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required, MaxLength(14)]
    public string CNPJ { get; set; } = string.Empty;

    [Required, MaxLength(5)]
    public string Agencia { get; set; } = string.Empty;

    [MaxLength(1)]
    public string? AgenciaDV { get; set; }

    [Required, MaxLength(12)]
    public string Conta { get; set; } = string.Empty;

    [MaxLength(1)]
    public string? ContaDV { get; set; }

    [MaxLength(20)]
    public string? Convenio { get; set; }

    [Required, MaxLength(30)]
    public string Layout { get; set; } = "FEBRABAN240";

    [MaxLength(3)]
    public string VersaoLayoutArquivo { get; set; } = "087";

    [MaxLength(3)]
    public string VersaoLayoutLote { get; set; } = "046";

    public int SequencialArquivo { get; set; } = 1;

    public bool Ativa { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.Now;
}
