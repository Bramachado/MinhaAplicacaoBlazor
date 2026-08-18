using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models.Reports;

namespace MinhaAplicacaoBlazor.Services;

public class RelatorioFinanceiroService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public RelatorioFinanceiroService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<CompetenciaSelecaoDto>> ObterCompetenciasAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Competencias
            .AsNoTracking()
            .OrderByDescending(c => c.Ano).ThenByDescending(c => c.Mes)
            .Select(c => new CompetenciaSelecaoDto
            {
                Id = c.Id,
                Mes = c.Mes,
                Ano = c.Ano,
                Status = c.Status
            })
            .ToListAsync();
    }

    public async Task<RelatorioGeralDespesasDto> GerarRelatorioGeralDespesasAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var lancamentos = await ctx.LancamentosFinanceiros
            .AsNoTracking()
            .Where(l => l.CompetenciaId == competenciaId && l.Status != "Cancelado")
            .Include(l => l.Unidade)
            .Include(l => l.PlanoConta)
            .Include(l => l.Fornecedor)
            .Include(l => l.FormaPagamento)
            .OrderBy(l => l.Unidade!.Nome)
            .ThenBy(l => l.PlanoConta!.Nome)
            .ThenBy(l => l.DataVencimento)
            .ThenBy(l => l.Descricao)
            .Select(l => new RelatorioGeralDespesaItemDto
            {
                DataVencimento = l.DataVencimento,
                DataPagamento = l.DataPagamento,
                Unidade = l.Unidade != null ? l.Unidade.Nome : null,
                PlanoConta = l.PlanoConta!.Nome,
                Fornecedor = l.Fornecedor != null ? l.Fornecedor.NomeRazaoSocial : null,
                Descricao = l.Descricao,
                FormaPagamento = l.FormaPagamento != null ? l.FormaPagamento.Nome : null,
                TipoLancamento = l.TipoLancamento,
                TipoDespesa = l.TipoDespesa,
                Valor = l.Valor,
                Status = l.Status
            })
            .ToListAsync();

        return new RelatorioGeralDespesasDto
        {
            Itens = lancamentos,
            TotalEntradas = lancamentos.Where(l => l.TipoLancamento == "Entrada").Sum(l => l.Valor),
            TotalSaidas = lancamentos.Where(l => l.TipoLancamento == "Saida").Sum(l => l.Valor),
            TotalDespesasFixas = lancamentos.Where(l => l.TipoLancamento == "Saida" && l.TipoDespesa == "Fixa").Sum(l => l.Valor),
            TotalDespesasVariaveis = lancamentos.Where(l => l.TipoLancamento == "Saida" && l.TipoDespesa == "Variavel").Sum(l => l.Valor)
        };
    }

    public async Task<RelatorioPorUnidadeDto> GerarRelatorioPorUnidadeAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var dados = await ctx.LancamentosFinanceiros
            .AsNoTracking()
            .Where(l => l.CompetenciaId == competenciaId && l.Status != "Cancelado")
            .Include(l => l.Unidade)
            .GroupBy(l => l.Unidade != null ? l.Unidade.Nome : "(Sem unidade)")
            .Select(g => new RelatorioPorUnidadeLinhaDto
            {
                Unidade = g.Key,
                TotalEntradas = g.Where(x => x.TipoLancamento == "Entrada").Sum(x => (decimal?)x.Valor) ?? 0,
                TotalSaidas = g.Where(x => x.TipoLancamento == "Saida").Sum(x => (decimal?)x.Valor) ?? 0,
                QuantidadeLancamentos = g.Count()
            })
            .OrderBy(x => x.Unidade)
            .ToListAsync();

        return new RelatorioPorUnidadeDto
        {
            Linhas = dados,
            Quantidade = dados.Count,
            TotalEntradas = dados.Sum(x => x.TotalEntradas),
            TotalSaidas = dados.Sum(x => x.TotalSaidas)
        };
    }

    public async Task<RelatorioPorPlanoContaDto> GerarRelatorioPorPlanoContaAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var dados = await ctx.LancamentosFinanceiros
            .AsNoTracking()
            .Where(l => l.CompetenciaId == competenciaId && l.Status != "Cancelado")
            .Include(l => l.PlanoConta)
            .GroupBy(l => l.PlanoConta!.Nome)
            .Select(g => new RelatorioPorPlanoContaLinhaDto
            {
                PlanoConta = g.Key,
                TotalEntradas = g.Where(x => x.TipoLancamento == "Entrada").Sum(x => (decimal?)x.Valor) ?? 0,
                TotalSaidas = g.Where(x => x.TipoLancamento == "Saida").Sum(x => (decimal?)x.Valor) ?? 0,
                QuantidadeLancamentos = g.Count()
            })
            .OrderBy(x => x.PlanoConta)
            .ToListAsync();

        return new RelatorioPorPlanoContaDto
        {
            Linhas = dados,
            Quantidade = dados.Count,
            TotalEntradas = dados.Sum(x => x.TotalEntradas),
            TotalSaidas = dados.Sum(x => x.TotalSaidas)
        };
    }

    public async Task<RelatorioPorFormaPagamentoDto> GerarRelatorioPorFormaPagamentoAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var dados = await ctx.LancamentosFinanceiros
            .AsNoTracking()
            .Where(l => l.CompetenciaId == competenciaId && l.Status != "Cancelado")
            .Include(l => l.FormaPagamento)
            .GroupBy(l => l.FormaPagamento != null ? l.FormaPagamento.Nome : "(Sem forma de pagamento)")
            .Select(g => new RelatorioPorFormaPagamentoLinhaDto
            {
                FormaPagamento = g.Key,
                TotalEntradas = g.Where(x => x.TipoLancamento == "Entrada").Sum(x => (decimal?)x.Valor) ?? 0,
                TotalSaidas = g.Where(x => x.TipoLancamento == "Saida").Sum(x => (decimal?)x.Valor) ?? 0,
                QuantidadeLancamentos = g.Count()
            })
            .OrderBy(x => x.FormaPagamento)
            .ToListAsync();

        return new RelatorioPorFormaPagamentoDto
        {
            Linhas = dados,
            Quantidade = dados.Count,
            TotalEntradas = dados.Sum(x => x.TotalEntradas),
            TotalSaidas = dados.Sum(x => x.TotalSaidas)
        };
    }

    public async Task<RelatorioFolhaColaboradorDto?> GerarRelatorioFolhaColaboradoresAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folha = await ctx.FolhasColaboradores
            .AsNoTracking()
            .Include(f => f.Competencia)
            .Include(f => f.Itens)
                .ThenInclude(i => i.Colaborador!)
                    .ThenInclude(c => c.Unidade)
            .FirstOrDefaultAsync(f => f.CompetenciaId == competenciaId);

        if (folha is null) return null;

        return new RelatorioFolhaColaboradorDto
        {
            CompetenciaTexto = folha.Competencia is not null
                ? $"{folha.Competencia.Mes:00}/{folha.Competencia.Ano}"
                : string.Empty,
            Status = folha.Status,
            DataFechamento = folha.DataFechamento,
            ValorTotal = folha.ValorTotal,
            Itens = folha.Itens
                .OrderBy(i => i.Colaborador?.Nome)
                .Select(i => new RelatorioFolhaColaboradorItemDto
                {
                    Colaborador = i.Colaborador?.Nome ?? "(?)",
                    Unidade = i.Colaborador?.Unidade?.Nome,
                    Cargo = i.Cargo,
                    CargaHorariaSemanal = i.CargaHorariaSemanal,
                    SalarioBruto = i.SalarioBase,
                    TicketAlimentacao = i.TicketAlimentacao,
                    PremiacaoFixa = i.PremiacaoFixa,
                    PremiacaoVariavel = i.PremiacaoVariavel,
                    DecimoTerceiro = i.DecimoTerceiro,
                    FeriasUmTerco = i.FeriasUmTerco,
                    TotalProventos = i.TotalProventos,
                    DescontoINSS = i.DescontoINSS,
                    DescontoIR = i.DescontoIR,
                    PlanoSaude = i.PlanoSaude,
                    ValeConsignadoFGTS = i.ValeConsignadoFGTS,
                    TotalDescontos = i.TotalDescontos,
                    ValorTotal = i.ValorTotal,
                    ValorReceberPix = i.ValorReceberPix,
                    Observacao = i.Observacao
                })
                .ToList()
        };
    }

    public async Task<RelatorioFolhaTutorDto?> GerarRelatorioFolhaTutoresAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folha = await ctx.FolhasTutores
            .AsNoTracking()
            .Include(f => f.Competencia)
            .Include(f => f.Itens)
                .ThenInclude(i => i.Tutor!)
                    .ThenInclude(t => t.Unidade)
            .Include(f => f.Itens)
                .ThenInclude(i => i.Tutor!)
                    .ThenInclude(t => t.Titulacao)
            .FirstOrDefaultAsync(f => f.CompetenciaId == competenciaId);

        if (folha is null) return null;

        return new RelatorioFolhaTutorDto
        {
            CompetenciaTexto = folha.Competencia is not null
                ? $"{folha.Competencia.Mes:00}/{folha.Competencia.Ano}"
                : string.Empty,
            Status = folha.Status,
            DataFechamento = folha.DataFechamento,
            ValorTotal = folha.ValorTotal,
            Itens = folha.Itens
                .OrderBy(i => i.Tutor?.Nome)
                .Select(i => new RelatorioFolhaTutorItemDto
                {
                    Tutor = i.Tutor?.Nome ?? "(?)",
                    Unidade = i.Tutor?.Unidade?.Nome,
                    Titulacao = i.Tutor?.Titulacao?.Nome,
                    Cargo = i.Cargo,
                    StatusPagamento = i.StatusPagamento,
                    TotalHorasNormais = i.TotalHorasNormais,
                    TotalHorasPraticas = i.TotalHorasPraticas,
                    ValorHoraNormal = i.ValorHoraNormal,
                    ValorHoraPratica = i.ValorHoraPratica,
                    ValorAulaNormal = i.ValorAulaNormal,
                    ValorAulaPratica = i.ValorAulaPratica,
                    ValorTotalReceber = i.ValorTotalReceber,
                    Banco = i.Banco,
                    Agencia = i.Agencia,
                    Conta = i.Conta,
                    ChavePix = i.ChavePix,
                    NomeTitularConta = i.NomeTitularConta,
                    Observacao = i.Observacao
                })
                .ToList()
        };
    }

    public async Task<RelatorioFolhaFornecedorDto?> GerarRelatorioFolhaFornecedoresAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folha = await ctx.FolhasFornecedores
            .AsNoTracking()
            .Include(f => f.Competencia)
            .Include(f => f.Itens)
                .ThenInclude(i => i.Fornecedor)
            .Include(f => f.Itens)
                .ThenInclude(i => i.BancoPagador)
            .FirstOrDefaultAsync(f => f.CompetenciaId == competenciaId);

        if (folha is null) return null;

        return new RelatorioFolhaFornecedorDto
        {
            CompetenciaTexto = folha.Competencia is not null
                ? $"{folha.Competencia.Mes:00}/{folha.Competencia.Ano}"
                : string.Empty,
            Status = folha.Status,
            DataFechamento = folha.DataFechamento,
            ValorTotal = folha.ValorTotal,
            Itens = folha.Itens
                .OrderBy(i => i.Fornecedor != null ? i.Fornecedor.NomeRazaoSocial : string.Empty)
                .Select(i => new RelatorioFolhaFornecedorItemDto
                {
                    Fornecedor = i.Fornecedor?.NomeRazaoSocial ?? "(?)",
                    Descricao = i.Descricao,
                    NumeroDocumento = i.NumeroDocumento,
                    TipoPagamento = i.TipoPagamento,
                    BancoPagador = i.BancoPagador != null ? i.BancoPagador.NomeBanco : null,
                    DataVencimento = i.DataVencimento,
                    StatusPagamento = i.StatusPagamento,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    ValorTotalPagar = i.ValorTotalPagar,
                    Banco = i.Banco,
                    Agencia = i.Agencia,
                    Conta = i.Conta,
                    ChavePix = i.ChavePix,
                    NomeTitularConta = i.NomeTitularConta,
                    Observacao = i.Observacao
                })
                .ToList()
        };
    }

    public async Task<ResumoDespesaGeralDto> GerarResumoDespesaGeralAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var competencia = await ctx.Competencias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == competenciaId);

        // Bloco 1 — Entradas (entidade Entrada) da competência.
        var entradas = await ctx.Entradas
            .AsNoTracking()
            .Where(e => e.CompetenciaId == competenciaId)
            .Include(e => e.Fornecedor)
            .OrderBy(e => e.DataEmissao)
            .Select(e => new ResumoEntradaDto
            {
                Fornecedor = e.Fornecedor != null ? e.Fornecedor.NomeRazaoSocial : "(?)",
                Descricao = e.Descricao,
                DataEmissao = e.DataEmissao,
                DataPagamento = e.DataPagamento,
                ValorBruto = e.ValorBruto,
                Desconto = e.Desconto,
                ValorLiquido = e.ValorLiquido
            })
            .ToListAsync();

        // Blocos 2, 3 e 4 — Folhas da competência (reaproveitam os relatórios existentes).
        var folhaFornecedor = await GerarRelatorioFolhaFornecedoresAsync(competenciaId);
        var folhaTutor = await GerarRelatorioFolhaTutoresAsync(competenciaId);
        var folhaColaborador = await GerarRelatorioFolhaColaboradoresAsync(competenciaId);

        return new ResumoDespesaGeralDto
        {
            CompetenciaTexto = competencia is not null ? $"{competencia.Mes:00}/{competencia.Ano}" : string.Empty,
            Entradas = entradas,
            FolhaFornecedor = folhaFornecedor,
            FolhaTutor = folhaTutor,
            FolhaColaborador = folhaColaborador
        };
    }

    /// <summary>
    /// Relatório "Folha de Fornecedores 2": lista os itens da folha de fornecedores
    /// da competência informada, com dados do fornecedor, banco pagador e conta
    /// bancária. Permite filtrar por tipo de pagamento do fornecedor
    /// ("Todos", "Boleto" ou "Transferencia").
    /// </summary>
    public async Task<List<FolhaFornecedor2LinhaDto>> GerarFolhaFornecedor2Async(int competenciaId, string tipoPagamento)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folhas = await ctx.FolhasFornecedores
            .AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId)
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.ContaBancaria)
            .Include(f => f.Itens).ThenInclude(i => i.BancoPagador)
            .ToListAsync();

        var linhas = new List<FolhaFornecedor2LinhaDto>();
        foreach (var folha in folhas)
        {
            foreach (var item in folha.Itens)
            {
                var forn = item.Fornecedor;
                var conta = forn?.ContaBancaria;
                linhas.Add(new FolhaFornecedor2LinhaDto
                {
                    ItemId = item.Id,
                    Fornecedor = forn?.NomeRazaoSocial ?? "(?)",
                    CpfCnpjFornecedor = forn?.CpfCnpj,
                    TipoPagamento = forn?.TipoPagamento,
                    ValorTotalPagar = item.ValorTotalPagar,
                    BancoPagador = item.BancoPagador?.Descricao,
                    NomeTitular = conta?.NomeTitular,
                    CpfCnpjConta = conta?.CpfCnpj,
                    Forma = conta?.Forma.ToString(),
                    TipoPessoa = conta?.TipoPessoa.ToString(),
                    TipoChavePix = conta?.TipoChavePix.ToString(),
                    ChavePix = conta?.ChavePix,
                    CodigoBanco = conta?.CodigoBanco,
                    NomeBanco = conta?.NomeBanco,
                    Agencia = conta?.Agencia,
                    Conta = conta?.Conta
                });
            }
        }

        // Filtro por tipo de pagamento do fornecedor ("Todos" não filtra).
        if (tipoPagamento is "Boleto" or "Transferencia")
            linhas = linhas.Where(l => l.TipoPagamento == tipoPagamento).ToList();

        return linhas
            .OrderBy(l => l.Fornecedor)
            .ToList();
    }

    /// <summary>
    /// Relatório "Folha de Colaboradores 2": lista os itens da folha de colaboradores
    /// da competência informada, com dados do colaborador e da conta bancária.
    /// Segue o SQL de origem (INNER JOIN em ContasBancarias): apenas itens cujo
    /// colaborador possui conta bancária vinculada.
    /// </summary>
    public async Task<List<FolhaColaborador2LinhaDto>> GerarFolhaColaborador2Async(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folhas = await ctx.FolhasColaboradores
            .AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId)
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.ContaBancaria)
            .ToListAsync();

        var linhas = new List<FolhaColaborador2LinhaDto>();
        foreach (var folha in folhas)
        {
            foreach (var item in folha.Itens)
            {
                var colab = item.Colaborador;
                var conta = colab?.ContaBancaria;
                if (conta is null) continue; // INNER JOIN: ignora sem conta bancária

                linhas.Add(new FolhaColaborador2LinhaDto
                {
                    ItemId = item.Id,
                    Colaborador = colab?.Nome ?? "(?)",
                    Cpf = colab?.Cpf,
                    ValorTotal = item.ValorTotal,
                    ValorReceberPix = item.ValorReceberPix,
                    NomeTitular = conta.NomeTitular,
                    CpfCnpjConta = conta.CpfCnpj,
                    Forma = conta.Forma.ToString(),
                    TipoPessoa = conta.TipoPessoa.ToString(),
                    TipoChavePix = conta.TipoChavePix.ToString(),
                    ChavePix = conta.ChavePix,
                    CodigoBanco = conta.CodigoBanco,
                    NomeBanco = conta.NomeBanco,
                    Agencia = conta.Agencia,
                    Conta = conta.Conta
                });
            }
        }

        return linhas
            .OrderBy(l => l.Colaborador)
            .ToList();
    }

    /// <summary>
    /// Relatório "Folha de Tutores 2": lista os itens da folha de tutores da
    /// competência informada, com dados do tutor e da conta bancária.
    /// Segue o SQL de origem (INNER JOIN em ContasBancarias): apenas itens cujo
    /// tutor possui conta bancária vinculada.
    /// </summary>
    public async Task<List<FolhaTutor2LinhaDto>> GerarFolhaTutor2Async(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folhas = await ctx.FolhasTutores
            .AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId)
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.ContaBancaria)
            .ToListAsync();

        var linhas = new List<FolhaTutor2LinhaDto>();
        foreach (var folha in folhas)
        {
            foreach (var item in folha.Itens)
            {
                var tutor = item.Tutor;
                var conta = tutor?.ContaBancaria;
                if (conta is null) continue; // INNER JOIN: ignora sem conta bancária

                linhas.Add(new FolhaTutor2LinhaDto
                {
                    ItemId = item.Id,
                    Tutor = tutor?.Nome ?? "(?)",
                    Cpf = tutor?.Cpf,
                    ValorTotalReceber = item.ValorTotalReceber,
                    NomeTitular = conta.NomeTitular,
                    CpfCnpjConta = conta.CpfCnpj,
                    Forma = conta.Forma.ToString(),
                    TipoPessoa = conta.TipoPessoa.ToString(),
                    TipoChavePix = conta.TipoChavePix.ToString(),
                    ChavePix = conta.ChavePix,
                    CodigoBanco = conta.CodigoBanco,
                    NomeBanco = conta.NomeBanco,
                    Agencia = conta.Agencia,
                    Conta = conta.Conta
                });
            }
        }

        return linhas
            .OrderBy(l => l.Tutor)
            .ToList();
    }

    public async Task<RelatorioPagamentosBancariosDto> GerarRelatorioPagamentosBancariosAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var competencia = await ctx.Competencias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == competenciaId);
        var competenciaTexto = competencia is not null ? $"{competencia.Mes:00}/{competencia.Ano}" : string.Empty;

        var linhas = new List<PagamentoBancarioLinhaDto>();

        // === Folha de Colaboradores (apenas FECHADA) ===
        var folhasColab = await ctx.FolhasColaboradores
            .AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.ContaBancaria)
            .ToListAsync();

        foreach (var folha in folhasColab)
        {
            foreach (var i in folha.Itens)
            {
                var conta = i.Colaborador?.ContaBancaria;
                linhas.Add(new PagamentoBancarioLinhaDto
                {
                    Origem = "Colaborador",
                    Unidade = i.Colaborador?.Unidade?.Nome,
                    CompetenciaTexto = competenciaTexto,
                    Nome = i.Colaborador?.Nome ?? "(?)",
                    Cpf = i.Colaborador?.Cpf,
                    Valor = i.ValorReceberPix,
                    Forma = conta?.Forma.ToString(),
                    TipoPessoa = conta?.TipoPessoa.ToString(),
                    NomeTitular = conta?.NomeTitular,
                    ChavePix = conta?.ChavePix,
                    Banco = conta?.NomeBanco,
                    Codigo = conta?.CodigoBanco,
                    Agencia = conta?.Agencia,
                    Conta = conta?.Conta
                });
            }
        }

        // === Folha de Tutores (apenas FECHADA) ===
        var folhasTut = await ctx.FolhasTutores
            .AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.ContaBancaria)
            .ToListAsync();

        foreach (var folha in folhasTut)
        {
            foreach (var i in folha.Itens)
            {
                var conta = i.Tutor?.ContaBancaria;
                linhas.Add(new PagamentoBancarioLinhaDto
                {
                    Origem = "Tutor",
                    Unidade = i.Tutor?.Unidade?.Nome,
                    CompetenciaTexto = competenciaTexto,
                    Nome = i.Tutor?.Nome ?? "(?)",
                    Cpf = i.Tutor?.Cpf,
                    Valor = i.ValorTotalReceber,
                    Forma = conta?.Forma.ToString(),
                    TipoPessoa = conta?.TipoPessoa.ToString(),
                    NomeTitular = conta?.NomeTitular,
                    ChavePix = conta?.ChavePix,
                    Banco = conta?.NomeBanco,
                    Codigo = conta?.CodigoBanco,
                    Agencia = conta?.Agencia,
                    Conta = conta?.Conta
                });
            }
        }

        // === Folha de Fornecedores (apenas FECHADA) ===
        var folhasForn = await ctx.FolhasFornecedores
            .AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.ContaBancaria)
            .ToListAsync();

        foreach (var folha in folhasForn)
        {
            foreach (var i in folha.Itens)
            {
                var conta = i.Fornecedor?.ContaBancaria;
                linhas.Add(new PagamentoBancarioLinhaDto
                {
                    Origem = "Fornecedor",
                    Unidade = i.Fornecedor?.Unidade?.Nome,
                    CompetenciaTexto = competenciaTexto,
                    Nome = i.Fornecedor?.NomeRazaoSocial ?? "(?)",
                    Cpf = i.Fornecedor?.CpfCnpj,
                    Valor = i.ValorTotalPagar,
                    Forma = conta?.Forma.ToString(),
                    TipoPessoa = conta?.TipoPessoa.ToString(),
                    NomeTitular = conta?.NomeTitular,
                    ChavePix = conta?.ChavePix,
                    Banco = conta?.NomeBanco,
                    Codigo = conta?.CodigoBanco,
                    Agencia = conta?.Agencia,
                    Conta = conta?.Conta
                });
            }
        }

        return new RelatorioPagamentosBancariosDto
        {
            CompetenciaTexto = competenciaTexto,
            Linhas = linhas
                .OrderBy(l => l.Origem)
                .ThenBy(l => l.Unidade)
                .ThenBy(l => l.Nome)
                .ToList()
        };
    }
}
