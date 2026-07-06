using System.ComponentModel.DataAnnotations;

namespace MinhaAplicacaoBlazor.CnabBtg.Data;

/// <summary>Sequência de NSA por empresa pagadora (controle do próximo número de arquivo).</summary>
public class CnabSequence
{
    public int Id { get; set; }

    [MaxLength(30)]
    public string EmpresaPagadora { get; set; } = string.Empty;

    public int UltimoNsa { get; set; }

    public DateTime UpdatedAt { get; set; }
}
