namespace MinhaAplicacaoBlazor.Models.Reports;

/// <summary>
/// Resultado completo do dashboard da Home para uma competência (e,
/// opcionalmente, uma unidade). Consolida receitas (Entradas) e despesas
/// (folhas de Colaboradores, Tutores e Fornecedores).
/// </summary>
public class DashboardDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string UnidadeTexto { get; set; } = "Todas as unidades";

    // === KPIs do período selecionado ===
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo => TotalReceitas - TotalDespesas;
    public decimal MargemPercentual => TotalReceitas == 0 ? 0 : Math.Round(Saldo / TotalReceitas * 100, 1);

    // Composição das despesas por origem.
    public decimal DespesaColaboradores { get; set; }
    public decimal DespesaTutores { get; set; }
    public decimal DespesaFornecedores { get; set; }

    // Contas a pagar (folha de fornecedores em aberto).
    public decimal TotalAPagar { get; set; }
    public decimal TotalVencido { get; set; }
    public int QtdeContasAPagar { get; set; }

    // Comparação com a competência anterior (para as setas de tendência).
    public decimal? ReceitasAnterior { get; set; }
    public decimal? DespesasAnterior { get; set; }
    public decimal? SaldoAnterior { get; set; }

    // === Detalhamentos ===
    public List<DashboardUnidadeLinhaDto> PorUnidade { get; set; } = new();
    public List<DashboardPlanoContaLinhaDto> PorPlanoConta { get; set; } = new();
    public List<DashboardEvolucaoPontoDto> Evolucao { get; set; } = new();
    public List<DashboardVencimentoDto> ProximosVencimentos { get; set; } = new();
    public List<DashboardProventoLinhaDto> ComposicaoProventosColaboradores { get; set; } = new();
}

public class DashboardUnidadeLinhaDto
{
    public string Unidade { get; set; } = string.Empty;
    public decimal Receitas { get; set; }
    public decimal Despesas { get; set; }
    public decimal Saldo => Receitas - Despesas;
    public decimal MargemPercentual => Receitas == 0 ? 0 : Math.Round(Saldo / Receitas * 100, 1);
}

public class DashboardPlanoContaLinhaDto
{
    public string PlanoConta { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class DashboardEvolucaoPontoDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string Rotulo => $"{Mes:00}/{Ano}";
    public decimal Receitas { get; set; }
    public decimal Despesas { get; set; }
    public decimal Saldo => Receitas - Despesas;
}

/// <summary>
/// Somatório de um campo da folha de colaboradores (proventos) na
/// competência selecionada, usado no gráfico "Composição da Folha".
/// </summary>
public class DashboardProventoLinhaDto
{
    public string Rotulo { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class DashboardVencimentoDto
{
    public DateTime? DataVencimento { get; set; }
    public string Fornecedor { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public string? Status { get; set; }
    public bool Vencido => DataVencimento.HasValue && DataVencimento.Value.Date < DateTime.Today;
}
