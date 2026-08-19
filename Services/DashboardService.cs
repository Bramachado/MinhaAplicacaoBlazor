using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models.Reports;

namespace MinhaAplicacaoBlazor.Services;

/// <summary>
/// Consolida os números financeiros da Home. As RECEITAS vêm da entidade
/// <c>Entrada</c> (valor líquido) e as DESPESAS das três folhas
/// (Colaboradores, Tutores e Fornecedores). A atribuição por unidade usa a
/// unidade do Colaborador / Tutor / Fornecedor de cada linha.
/// </summary>
public class DashboardService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    private const string SemUnidade = "(Sem unidade)";
    private const string SemPlano = "(Sem plano de contas)";

    public DashboardService(IDbContextFactory<AppDbContext> dbFactory)
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

    public async Task<List<Models.Unidade>> ObterUnidadesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Unidades
            .AsNoTracking()
            .Where(u => u.Ativa)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<DashboardDto> ObterDashboardAsync(int competenciaId, int? unidadeId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var competencia = await ctx.Competencias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == competenciaId);

        if (competencia is null)
            return new DashboardDto();

        var unidadeNome = unidadeId is null
            ? "Todas as unidades"
            : (await ctx.Unidades.AsNoTracking().Where(u => u.Id == unidadeId).Select(u => u.Nome).FirstOrDefaultAsync() ?? "(?)");

        var dto = new DashboardDto
        {
            CompetenciaTexto = $"{competencia.Mes:00}/{competencia.Ano}",
            UnidadeTexto = unidadeNome
        };

        // === KPIs e detalhamento por unidade do período selecionado ===
        await PreencherPeriodoAsync(ctx, competenciaId, unidadeId, dto);

        // === Comparação com a competência anterior ===
        var anterior = await ObterCompetenciaAnteriorIdAsync(ctx, competencia.Ano, competencia.Mes);
        if (anterior is not null)
        {
            var (recAnt, despAnt) = await ObterTotaisAsync(ctx, anterior.Value, unidadeId);
            dto.ReceitasAnterior = recAnt;
            dto.DespesasAnterior = despAnt;
            dto.SaldoAnterior = recAnt - despAnt;
        }

        // === Evolução das últimas 12 competências ===
        dto.Evolucao = await ObterEvolucaoAsync(ctx, competencia.Ano, competencia.Mes, unidadeId);

        // === Despesas por plano de contas (folha de fornecedores) ===
        dto.PorPlanoConta = await ObterPorPlanoContaAsync(ctx, competenciaId, unidadeId);

        // === Próximos vencimentos (folha de fornecedores em aberto) ===
        dto.ProximosVencimentos = await ObterProximosVencimentosAsync(ctx, competenciaId, unidadeId);

        // === Composição da folha de colaboradores por tipo de provento ===
        dto.ComposicaoProventosColaboradores = await ObterComposicaoProventosAsync(ctx, competenciaId, unidadeId);

        return dto;
    }

    private static async Task PreencherPeriodoAsync(AppDbContext ctx, int competenciaId, int? unidadeId, DashboardDto dto)
    {
        var mapa = new Dictionary<string, DashboardUnidadeLinhaDto>();

        DashboardUnidadeLinhaDto Linha(string unidade)
        {
            if (!mapa.TryGetValue(unidade, out var linha))
            {
                linha = new DashboardUnidadeLinhaDto { Unidade = unidade };
                mapa[unidade] = linha;
            }
            return linha;
        }

        // --- Receitas (Entradas) por unidade do fornecedor ---
        var receitas = await ctx.Entradas
            .AsNoTracking()
            .Where(e => e.CompetenciaId == competenciaId)
            .Where(e => unidadeId == null || e.Fornecedor!.UnidadeId == unidadeId)
            .GroupBy(e => e.Fornecedor!.Unidade!.Nome)
            .Select(g => new { Unidade = g.Key, Total = g.Sum(x => (decimal?)x.ValorLiquido) ?? 0 })
            .ToListAsync();
        foreach (var r in receitas)
            Linha(r.Unidade ?? SemUnidade).Receitas += r.Total;

        // --- Despesa: Colaboradores ---
        var colab = await ctx.FolhasColaboradoresItens
            .AsNoTracking()
            .Where(i => i.FolhaColaborador!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Colaborador!.UnidadeId == unidadeId)
            .GroupBy(i => i.Colaborador!.Unidade!.Nome)
            .Select(g => new { Unidade = g.Key, Total = g.Sum(x => (decimal?)x.ValorTotal) ?? 0 })
            .ToListAsync();
        foreach (var c in colab)
        {
            Linha(c.Unidade ?? SemUnidade).Despesas += c.Total;
            dto.DespesaColaboradores += c.Total;
        }

        // --- Despesa: Tutores ---
        var tut = await ctx.FolhasTutoresItens
            .AsNoTracking()
            .Where(i => i.FolhaTutor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Tutor!.UnidadeId == unidadeId)
            .GroupBy(i => i.Tutor!.Unidade!.Nome)
            .Select(g => new { Unidade = g.Key, Total = g.Sum(x => (decimal?)x.ValorTotalReceber) ?? 0 })
            .ToListAsync();
        foreach (var t in tut)
        {
            Linha(t.Unidade ?? SemUnidade).Despesas += t.Total;
            dto.DespesaTutores += t.Total;
        }

        // --- Despesa: Fornecedores ---
        var forn = await ctx.FolhasFornecedoresItens
            .AsNoTracking()
            .Where(i => i.FolhaFornecedor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Fornecedor!.UnidadeId == unidadeId)
            .GroupBy(i => i.Fornecedor!.Unidade!.Nome)
            .Select(g => new { Unidade = g.Key, Total = g.Sum(x => (decimal?)x.ValorTotalPagar) ?? 0 })
            .ToListAsync();
        foreach (var f in forn)
        {
            Linha(f.Unidade ?? SemUnidade).Despesas += f.Total;
            dto.DespesaFornecedores += f.Total;
        }

        dto.PorUnidade = mapa.Values
            .OrderByDescending(l => l.Saldo)
            .ToList();

        dto.TotalReceitas = dto.PorUnidade.Sum(l => l.Receitas);
        dto.TotalDespesas = dto.PorUnidade.Sum(l => l.Despesas);

        // --- Contas a pagar (itens de folha de fornecedores não pagos) ---
        var aPagar = await ctx.FolhasFornecedoresItens
            .AsNoTracking()
            .Where(i => i.FolhaFornecedor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Fornecedor!.UnidadeId == unidadeId)
            .Where(i => i.StatusPagamento != "Pago" && i.StatusPagamento != "Cancelado")
            .Select(i => new { i.ValorTotalPagar, i.DataVencimento })
            .ToListAsync();

        var hoje = DateTime.Today;
        dto.TotalAPagar = aPagar.Sum(x => x.ValorTotalPagar);
        dto.TotalVencido = aPagar.Where(x => x.DataVencimento.HasValue && x.DataVencimento.Value.Date < hoje).Sum(x => x.ValorTotalPagar);
        dto.QtdeContasAPagar = aPagar.Count;
    }

    private static async Task<(decimal receitas, decimal despesas)> ObterTotaisAsync(AppDbContext ctx, int competenciaId, int? unidadeId)
    {
        var receitas = await ctx.Entradas
            .AsNoTracking()
            .Where(e => e.CompetenciaId == competenciaId)
            .Where(e => unidadeId == null || e.Fornecedor!.UnidadeId == unidadeId)
            .SumAsync(e => (decimal?)e.ValorLiquido) ?? 0;

        var dColab = await ctx.FolhasColaboradoresItens
            .AsNoTracking()
            .Where(i => i.FolhaColaborador!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Colaborador!.UnidadeId == unidadeId)
            .SumAsync(i => (decimal?)i.ValorTotal) ?? 0;

        var dTut = await ctx.FolhasTutoresItens
            .AsNoTracking()
            .Where(i => i.FolhaTutor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Tutor!.UnidadeId == unidadeId)
            .SumAsync(i => (decimal?)i.ValorTotalReceber) ?? 0;

        var dForn = await ctx.FolhasFornecedoresItens
            .AsNoTracking()
            .Where(i => i.FolhaFornecedor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Fornecedor!.UnidadeId == unidadeId)
            .SumAsync(i => (decimal?)i.ValorTotalPagar) ?? 0;

        return (receitas, dColab + dTut + dForn);
    }

    private static async Task<int?> ObterCompetenciaAnteriorIdAsync(AppDbContext ctx, int ano, int mes)
    {
        return await ctx.Competencias
            .AsNoTracking()
            .Where(c => c.Ano < ano || (c.Ano == ano && c.Mes < mes))
            .OrderByDescending(c => c.Ano).ThenByDescending(c => c.Mes)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }

    private static async Task<List<DashboardEvolucaoPontoDto>> ObterEvolucaoAsync(AppDbContext ctx, int ano, int mes, int? unidadeId)
    {
        // As 12 competências mais recentes até (e incluindo) a selecionada.
        var competencias = await ctx.Competencias
            .AsNoTracking()
            .Where(c => c.Ano < ano || (c.Ano == ano && c.Mes <= mes))
            .OrderByDescending(c => c.Ano).ThenByDescending(c => c.Mes)
            .Take(12)
            .Select(c => new { c.Id, c.Ano, c.Mes })
            .ToListAsync();

        var ids = competencias.Select(c => c.Id).ToList();

        var receitas = (await ctx.Entradas
            .AsNoTracking()
            .Where(e => ids.Contains(e.CompetenciaId))
            .Where(e => unidadeId == null || e.Fornecedor!.UnidadeId == unidadeId)
            .GroupBy(e => e.CompetenciaId)
            .Select(g => new { Id = g.Key, Total = g.Sum(x => (decimal?)x.ValorLiquido) ?? 0 })
            .ToListAsync())
            .ToDictionary(x => x.Id, x => x.Total);

        var dColab = (await ctx.FolhasColaboradoresItens
            .AsNoTracking()
            .Where(i => ids.Contains(i.FolhaColaborador!.CompetenciaId))
            .Where(i => unidadeId == null || i.Colaborador!.UnidadeId == unidadeId)
            .GroupBy(i => i.FolhaColaborador!.CompetenciaId)
            .Select(g => new { Id = g.Key, Total = g.Sum(x => (decimal?)x.ValorTotal) ?? 0 })
            .ToListAsync())
            .ToDictionary(x => x.Id, x => x.Total);

        var dTut = (await ctx.FolhasTutoresItens
            .AsNoTracking()
            .Where(i => ids.Contains(i.FolhaTutor!.CompetenciaId))
            .Where(i => unidadeId == null || i.Tutor!.UnidadeId == unidadeId)
            .GroupBy(i => i.FolhaTutor!.CompetenciaId)
            .Select(g => new { Id = g.Key, Total = g.Sum(x => (decimal?)x.ValorTotalReceber) ?? 0 })
            .ToListAsync())
            .ToDictionary(x => x.Id, x => x.Total);

        var dForn = (await ctx.FolhasFornecedoresItens
            .AsNoTracking()
            .Where(i => ids.Contains(i.FolhaFornecedor!.CompetenciaId))
            .Where(i => unidadeId == null || i.Fornecedor!.UnidadeId == unidadeId)
            .GroupBy(i => i.FolhaFornecedor!.CompetenciaId)
            .Select(g => new { Id = g.Key, Total = g.Sum(x => (decimal?)x.ValorTotalPagar) ?? 0 })
            .ToListAsync())
            .ToDictionary(x => x.Id, x => x.Total);

        decimal Get(Dictionary<int, decimal> d, int id) => d.TryGetValue(id, out var v) ? v : 0;

        return competencias
            .OrderBy(c => c.Ano).ThenBy(c => c.Mes)
            .Select(c => new DashboardEvolucaoPontoDto
            {
                Ano = c.Ano,
                Mes = c.Mes,
                Receitas = Get(receitas, c.Id),
                Despesas = Get(dColab, c.Id) + Get(dTut, c.Id) + Get(dForn, c.Id)
            })
            .ToList();
    }

    private static async Task<List<DashboardPlanoContaLinhaDto>> ObterPorPlanoContaAsync(AppDbContext ctx, int competenciaId, int? unidadeId)
    {
        return await ctx.FolhasFornecedoresItens
            .AsNoTracking()
            .Where(i => i.FolhaFornecedor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Fornecedor!.UnidadeId == unidadeId)
            .GroupBy(i => i.Fornecedor!.PlanoConta != null ? i.Fornecedor.PlanoConta.Nome : SemPlano)
            .Select(g => new DashboardPlanoContaLinhaDto
            {
                PlanoConta = g.Key,
                Total = g.Sum(x => (decimal?)x.ValorTotalPagar) ?? 0
            })
            .Where(x => x.Total > 0)
            .OrderByDescending(x => x.Total)
            .ToListAsync();
    }

    /// <summary>
    /// Soma, por tipo de provento (Salário Base, Premiações, 13º, Férias 1/3
    /// etc.), os itens da folha de colaboradores da competência selecionada.
    /// Usado no gráfico "Composição da Folha de Colaboradores" da Home.
    /// </summary>
    private static async Task<List<DashboardProventoLinhaDto>> ObterComposicaoProventosAsync(AppDbContext ctx, int competenciaId, int? unidadeId)
    {
        var agregado = await ctx.FolhasColaboradoresItens
            .AsNoTracking()
            .Where(i => i.FolhaColaborador!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Colaborador!.UnidadeId == unidadeId)
            .GroupBy(i => 1)
            .Select(g => new
            {
                SalarioBase = g.Sum(x => x.SalarioBase),
                PremiacaoFixa = g.Sum(x => x.PremiacaoFixa),
                PremiacaoVariavel = g.Sum(x => x.PremiacaoVariavel),
                OutrosProventos = g.Sum(x => x.OutrosProventos),
                SalarioFamilia = g.Sum(x => x.SalarioFamilia),
                DecimoTerceiro = g.Sum(x => x.DecimoTerceiro),
                FeriasUmTerco = g.Sum(x => x.FeriasUmTerco),
                PlanoSaude = g.Sum(x => x.PlanoSaude)
            })
            .FirstOrDefaultAsync();

        if (agregado is null)
            return new();

        var lista = new List<DashboardProventoLinhaDto>
        {
            new() { Rotulo = "Salário Base", Total = agregado.SalarioBase },
            new() { Rotulo = "Premiação Fixa", Total = agregado.PremiacaoFixa },
            new() { Rotulo = "Premiação Variável", Total = agregado.PremiacaoVariavel },
            new() { Rotulo = "Outros Proventos", Total = agregado.OutrosProventos },
            new() { Rotulo = "Salário Família", Total = agregado.SalarioFamilia },
            new() { Rotulo = "13º Salário", Total = agregado.DecimoTerceiro },
            new() { Rotulo = "Férias 1/3", Total = agregado.FeriasUmTerco },
            new() { Rotulo = "Plano de Saúde", Total = agregado.PlanoSaude },
        };

        return lista.OrderByDescending(x => x.Total).ToList();
    }

    private static async Task<List<DashboardVencimentoDto>> ObterProximosVencimentosAsync(AppDbContext ctx, int competenciaId, int? unidadeId)
    {
        return await ctx.FolhasFornecedoresItens
            .AsNoTracking()
            .Where(i => i.FolhaFornecedor!.CompetenciaId == competenciaId)
            .Where(i => unidadeId == null || i.Fornecedor!.UnidadeId == unidadeId)
            .Where(i => i.StatusPagamento != "Pago" && i.StatusPagamento != "Cancelado")
            .OrderBy(i => i.DataVencimento ?? DateTime.MaxValue)
            .Take(10)
            .Select(i => new DashboardVencimentoDto
            {
                DataVencimento = i.DataVencimento,
                Fornecedor = i.Fornecedor != null ? i.Fornecedor.NomeRazaoSocial : "(?)",
                Unidade = i.Fornecedor != null && i.Fornecedor.Unidade != null ? i.Fornecedor.Unidade.Nome : null,
                Descricao = i.Descricao,
                Valor = i.ValorTotalPagar,
                Status = i.StatusPagamento
            })
            .ToListAsync();
    }
}
