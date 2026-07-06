namespace MinhaAplicacaoBlazor.CnabBtg.Audit;

/// <summary>Relatório de auditoria da geração (cabeçalho + arquivos + linhas por pagamento).</summary>
public sealed class CnabAuditReport
{
    public string DataHoraGeracao { get; set; } = string.Empty;
    public string? Usuario { get; set; }
    public string Ambiente { get; set; } = string.Empty;

    public string EmpresaPagadora { get; set; } = string.Empty;
    public string Banco { get; set; } = string.Empty;
    public string Agencia { get; set; } = string.Empty;
    public string Conta { get; set; } = string.Empty;
    public string Convenio { get; set; } = string.Empty;

    public int NsaInicial { get; set; }
    public int NsaFinal { get; set; }

    public int TotalSelecionados { get; set; }
    public int TotalValidos { get; set; }
    public int TotalCorrigidos { get; set; }
    public int TotalPendentes { get; set; }
    public int TotalInvalidos { get; set; }
    public int TotalGerados { get; set; }

    public decimal ValorTotalGeral { get; set; }

    public bool Bloqueado { get; set; }
    public string? MotivoBloqueio { get; set; }

    public string ObservacaoValidacao { get; set; } =
        "Arquivo gerado conforme regras estruturais CNAB. A validação final deve ser feita no ambiente BTG.";

    public List<CnabAuditArquivo> Arquivos { get; set; } = new();
    public List<CnabAuditRow> Pagamentos { get; set; } = new();
}

public sealed class CnabAuditArquivo
{
    public string Arquivo { get; set; } = string.Empty;
    public int Nsa { get; set; }
    public int Operacoes { get; set; }
    public int QuantidadeRegistros { get; set; }
    public int QuantidadeLotes { get; set; }
    public decimal ValorTotal { get; set; }
    public bool TodasLinhas240 { get; set; }
}
