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
