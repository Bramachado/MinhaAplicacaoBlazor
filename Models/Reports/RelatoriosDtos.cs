namespace MinhaAplicacaoBlazor.Models.Reports;

public class CompetenciaSelecaoDto
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TextoExibicao => $"{Mes:00}/{Ano} - {Status}";
}

public class ResumoFinanceiroCardDto
{
    public string Titulo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string CssClass { get; set; } = "border-secondary";
    public string? Descricao { get; set; }
}

public class RelatorioGeralDespesasDto
{
    public List<RelatorioGeralDespesaItemDto> Itens { get; set; } = new();
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public decimal TotalDespesasFixas { get; set; }
    public decimal TotalDespesasVariaveis { get; set; }
}

public class RelatorioGeralDespesaItemDto
{
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string? Unidade { get; set; }
    public string PlanoConta { get; set; } = string.Empty;
    public string? Fornecedor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? FormaPagamento { get; set; }
    public string TipoLancamento { get; set; } = string.Empty;
    public string? TipoDespesa { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RelatorioAgrupadoDto<TLinha>
{
    public List<TLinha> Linhas { get; set; } = new();
    public int Quantidade { get; set; }
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
}

public class RelatorioPorUnidadeDto : RelatorioAgrupadoDto<RelatorioPorUnidadeLinhaDto> { }

public class RelatorioPorUnidadeLinhaDto
{
    public string Unidade { get; set; } = string.Empty;
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public int QuantidadeLancamentos { get; set; }
}

public class RelatorioPorPlanoContaDto : RelatorioAgrupadoDto<RelatorioPorPlanoContaLinhaDto> { }

public class RelatorioPorPlanoContaLinhaDto
{
    public string PlanoConta { get; set; } = string.Empty;
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public int QuantidadeLancamentos { get; set; }
}

public class RelatorioPorFormaPagamentoDto : RelatorioAgrupadoDto<RelatorioPorFormaPagamentoLinhaDto> { }

public class RelatorioPorFormaPagamentoLinhaDto
{
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal Saldo => TotalEntradas - TotalSaidas;
    public int QuantidadeLancamentos { get; set; }
}

public class RelatorioFolhaColaboradorDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DataFechamento { get; set; }
    public decimal ValorTotal { get; set; }
    public List<RelatorioFolhaColaboradorItemDto> Itens { get; set; } = new();
    public int QuantidadeColaboradores => Itens.Count;
    public decimal TotalProventos => Itens.Sum(i => i.TotalProventos);
    public decimal TotalDescontos => Itens.Sum(i => i.TotalDescontos);
    public decimal TotalLiquido => Itens.Sum(i => i.ValorTotal);
    public decimal TotalReceberPix => Itens.Sum(i => i.ValorReceberPix);
}

public class RelatorioFolhaColaboradorItemDto
{
    public string Colaborador { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string? Cargo { get; set; }
    public decimal? CargaHorariaSemanal { get; set; }
    public decimal SalarioBruto { get; set; }
    public decimal TicketAlimentacao { get; set; }
    public decimal PremiacaoFixa { get; set; }
    public decimal PremiacaoVariavel { get; set; }
    public decimal DecimoTerceiro { get; set; }
    public decimal FeriasUmTerco { get; set; }
    public decimal TotalProventos { get; set; }
    public decimal DescontoINSS { get; set; }
    public decimal DescontoIR { get; set; }
    public decimal PlanoSaude { get; set; }
    public decimal ValeConsignadoFGTS { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorReceberPix { get; set; }
    public string? Observacao { get; set; }
}

public class RelatorioFolhaTutorDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DataFechamento { get; set; }
    public decimal ValorTotal { get; set; }
    public List<RelatorioFolhaTutorItemDto> Itens { get; set; } = new();
    public int QuantidadeTutores => Itens.Count;
    public decimal TotalHorasNormais => Itens.Sum(i => i.TotalHorasNormais);
    public decimal TotalHorasPraticas => Itens.Sum(i => i.TotalHorasPraticas);
    public decimal TotalHoras => TotalHorasNormais + TotalHorasPraticas;
    public decimal TotalAPagar => Itens.Sum(i => i.ValorTotalReceber);
}

public class RelatorioFolhaTutorItemDto
{
    public string Tutor { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string? Titulacao { get; set; }
    public string? Cargo { get; set; }
    public string? StatusPagamento { get; set; }
    public decimal TotalHorasNormais { get; set; }
    public decimal TotalHorasPraticas { get; set; }
    public decimal ValorHoraNormal { get; set; }
    public decimal ValorHoraPratica { get; set; }
    public decimal ValorAulaNormal { get; set; }
    public decimal ValorAulaPratica { get; set; }
    public decimal ValorTotalReceber { get; set; }
    public string? Banco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? ChavePix { get; set; }
    public string? NomeTitularConta { get; set; }
    public string? Observacao { get; set; }
}

public class RelatorioPagamentosBancariosDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public List<PagamentoBancarioLinhaDto> Linhas { get; set; } = new();
    public decimal ValorTotal => Linhas.Sum(l => l.Valor);
}

public class PagamentoBancarioLinhaDto
{
    public string Origem { get; set; } = string.Empty; // Colaborador / Tutor / Fornecedor
    public string? Unidade { get; set; }
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public decimal Valor { get; set; }
    public string? Forma { get; set; }
    public string? TipoPessoa { get; set; }
    public string? NomeTitular { get; set; }
    public string? ChavePix { get; set; }
    public string? Banco { get; set; }
    public string? Codigo { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
}

public class ResumoDespesaGeralDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;

    // Bloco 1 — Entradas (entidade Entrada)
    public List<ResumoEntradaDto> Entradas { get; set; } = new();
    public decimal TotalEntradas => Entradas.Sum(e => e.ValorLiquido);

    // Bloco 2 — Fornecedores (Folha de Fornecedores)
    public RelatorioFolhaFornecedorDto? FolhaFornecedor { get; set; }
    public decimal TotalFornecedores => FolhaFornecedor?.ValorTotal ?? 0m;

    // Bloco 3 — Folha de Tutores
    public RelatorioFolhaTutorDto? FolhaTutor { get; set; }
    public decimal TotalTutores => FolhaTutor?.ValorTotal ?? 0m;

    // Bloco 4 — Folha de Colaboradores
    public RelatorioFolhaColaboradorDto? FolhaColaborador { get; set; }
    public decimal TotalColaboradores => FolhaColaborador?.ValorTotal ?? 0m;

    // Bloco 5 — Resumo
    public decimal TotalSaidas => TotalFornecedores + TotalTutores + TotalColaboradores;
    public decimal SaldoFinal => TotalEntradas - TotalSaidas;
}

public class ResumoEntradaDto
{
    public string Fornecedor { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataEmissao { get; set; }
    public DateTime? DataPagamento { get; set; }
    public decimal ValorBruto { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorLiquido { get; set; }
}

public class RelatorioFolhaFornecedorDto
{
    public string CompetenciaTexto { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DataFechamento { get; set; }
    public decimal ValorTotal { get; set; }
    public List<RelatorioFolhaFornecedorItemDto> Itens { get; set; } = new();
    public int QuantidadeFornecedores => Itens.Count;
}

/// <summary>
/// Linha da grade do relatório "Folha de Fornecedores 2". Reúne dados do
/// fornecedor, do item da folha, do banco pagador e da conta bancária do
/// fornecedor (mesmas colunas da consulta SQL de origem).
/// </summary>
public class FolhaFornecedor2LinhaDto
{
    /// <summary>Marcação do usuário para exportar (ou não) esta linha para o Excel.</summary>
    public bool Selecionado { get; set; } = true;

    /// <summary>Id do FolhaFornecedorItem de origem, usado para abrir os detalhes.</summary>
    public int ItemId { get; set; }

    public string Fornecedor { get; set; } = string.Empty;
    public string? CpfCnpjFornecedor { get; set; }
    public string? TipoPagamento { get; set; }
    public decimal ValorTotalPagar { get; set; }

    /// <summary>Descrição do banco pagador (Bancos.Descricao).</summary>
    public string? BancoPagador { get; set; }

    // Conta bancária do fornecedor.
    public string? NomeTitular { get; set; }
    public string? CpfCnpjConta { get; set; }
    public string? Forma { get; set; }
    public string? TipoPessoa { get; set; }
    public string? TipoChavePix { get; set; }
    public string? ChavePix { get; set; }
    public string? CodigoBanco { get; set; }
    public string? NomeBanco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
}

/// <summary>
/// Linha da grade do relatório "Folha de Colaboradores 2". Reúne dados do
/// colaborador, do item da folha e da conta bancária do colaborador
/// (mesmas colunas da consulta SQL de origem).
/// </summary>
public class FolhaColaborador2LinhaDto
{
    /// <summary>Marcação do usuário para exportar (ou não) esta linha para o Excel.</summary>
    public bool Selecionado { get; set; } = true;

    /// <summary>Id do FolhaColaboradorItem de origem, usado para abrir os detalhes.</summary>
    public int ItemId { get; set; }

    public string Colaborador { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorReceberPix { get; set; }

    // Conta bancária do colaborador.
    public string? NomeTitular { get; set; }
    public string? CpfCnpjConta { get; set; }
    public string? Forma { get; set; }
    public string? TipoPessoa { get; set; }
    public string? TipoChavePix { get; set; }
    public string? ChavePix { get; set; }
    public string? CodigoBanco { get; set; }
    public string? NomeBanco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
}

/// <summary>
/// Linha da grade do relatório "Folha de Tutores 2". Reúne dados do tutor,
/// do item da folha e da conta bancária do tutor (mesmas colunas da consulta
/// SQL de origem).
/// </summary>
public class FolhaTutor2LinhaDto
{
    /// <summary>Marcação do usuário para exportar (ou não) esta linha para o Excel.</summary>
    public bool Selecionado { get; set; } = true;

    /// <summary>Id do FolhaTutorItem de origem, usado para abrir os detalhes.</summary>
    public int ItemId { get; set; }

    public string Tutor { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public decimal ValorTotalReceber { get; set; }

    // Conta bancária do tutor.
    public string? NomeTitular { get; set; }
    public string? CpfCnpjConta { get; set; }
    public string? Forma { get; set; }
    public string? TipoPessoa { get; set; }
    public string? TipoChavePix { get; set; }
    public string? ChavePix { get; set; }
    public string? CodigoBanco { get; set; }
    public string? NomeBanco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
}

public class RelatorioFolhaFornecedorItemDto
{
    public string Fornecedor { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    /// <summary>Números das notas que compõem este pagamento, ex.: "NF001, NF002".</summary>
    public string? DocumentosResumo { get; set; }
    public int QuantidadeNotas { get; set; }

    public string? TipoPagamento { get; set; }
    public string? BancoPagador { get; set; }
    public DateTime? DataVencimento { get; set; }
    public string? StatusPagamento { get; set; }
    public decimal ValorTotalPagar { get; set; }
    public string? Banco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? ChavePix { get; set; }
    public string? NomeTitularConta { get; set; }
    public string? Observacao { get; set; }
}
