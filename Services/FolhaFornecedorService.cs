using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Services;

/// <summary>Resultado de <see cref="FolhaFornecedorService.LancarNotaComParcelasAsync"/>.</summary>
public class ResultadoLancamentoParcelado
{
    public bool Sucesso { get; set; }

    /// <summary>Competências (MM/AAAA) que precisam ser cadastradas antes de lançar as parcelas.</summary>
    public List<string> CompetenciasFaltantes { get; set; } = new();

    /// <summary>Competências cuja folha de fornecedores já existe mas está fechada.</summary>
    public List<string> CompetenciasFechadas { get; set; } = new();

    public bool TemPendencias => CompetenciasFaltantes.Count > 0 || CompetenciasFechadas.Count > 0;
}

public class FolhaFornecedorService
{
    private const string OrigemLancamento = "FolhaFornecedor";
    private const string DescricaoLancamento = "Folha de Fornecedores";
    private const string PlanoContaPadrao = "PRESTAÇÃO SERVIÇO";

    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public FolhaFornecedorService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> ValidarFolhaAbertaAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var status = await ctx.FolhasFornecedores
            .AsNoTracking()
            .Where(f => f.Id == folhaId)
            .Select(f => f.Status)
            .FirstOrDefaultAsync();

        return status == "Aberta";
    }

    public async Task<decimal> RecalcularTotalAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folha = await ctx.FolhasFornecedores.FirstOrDefaultAsync(f => f.Id == folhaId)
            ?? throw new InvalidOperationException("Folha não encontrada.");

        folha.ValorTotal = await ctx.FolhasFornecedoresItens
            .Where(i => i.FolhaFornecedorId == folhaId)
            .SumAsync(i => (decimal?)i.ValorTotalPagar) ?? 0m;

        await ctx.SaveChangesAsync();
        return folha.ValorTotal;
    }

    /// <summary>
    /// Lança uma nota parcelada: a 1ª parcela fica no item de origem (competência atual)
    /// e as demais são criadas automaticamente nas competências seguintes, no pagamento
    /// consolidado do mesmo fornecedor (criando a folha/o item daquela competência se
    /// ainda não existirem). Se alguma competência futura não existir, ou já existir com
    /// a folha fechada, nada é gravado e o chamador deve orientar o usuário a resolver
    /// a pendência antes de tentar novamente.
    /// </summary>
    public async Task<ResultadoLancamentoParcelado> LancarNotaComParcelasAsync(
        int itemOrigemId, string? numeroDocumento, string? descricao, decimal valorParcela, int totalParcelas)
    {
        if (totalParcelas < 2)
            throw new ArgumentOutOfRangeException(nameof(totalParcelas), "Uma nota parcelada precisa de pelo menos 2 parcelas.");

        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var itemOrigem = await ctx.FolhasFornecedoresItens
            .Include(i => i.FolhaFornecedor!).ThenInclude(f => f.Competencia)
            .FirstOrDefaultAsync(i => i.Id == itemOrigemId)
            ?? throw new InvalidOperationException("Item não encontrado.");

        var competenciaOrigem = itemOrigem.FolhaFornecedor!.Competencia!;

        // Monta a lista de (Mes, Ano) das parcelas 2..N (a 1ª fica na competência atual).
        var mesesNecessarios = new List<(int Mes, int Ano)>();
        int mes = competenciaOrigem.Mes, ano = competenciaOrigem.Ano;
        for (int p = 2; p <= totalParcelas; p++)
        {
            mes++;
            if (mes > 12) { mes = 1; ano++; }
            mesesNecessarios.Add((mes, ano));
        }

        var anosEnvolvidos = mesesNecessarios.Select(m => m.Ano).Distinct().ToList();
        var competenciasExistentes = await ctx.Competencias
            .Where(c => anosEnvolvidos.Contains(c.Ano))
            .ToListAsync();

        var resultado = new ResultadoLancamentoParcelado();

        foreach (var (m, a) in mesesNecessarios)
        {
            var competencia = competenciasExistentes.FirstOrDefault(c => c.Mes == m && c.Ano == a);
            if (competencia is null)
            {
                resultado.CompetenciasFaltantes.Add($"{m:00}/{a}");
                continue;
            }

            var folhaExistente = await ctx.FolhasFornecedores
                .FirstOrDefaultAsync(f => f.CompetenciaId == competencia.Id);
            if (folhaExistente is not null && folhaExistente.Status != "Aberta")
                resultado.CompetenciasFechadas.Add($"{m:00}/{a}");
        }

        if (resultado.TemPendencias)
            return resultado;

        // Tudo certo: grava a 1ª parcela no item de origem e cria as demais.
        ctx.FolhasFornecedoresItensNotas.Add(new FolhaFornecedorItemNota
        {
            FolhaFornecedorItemId = itemOrigemId,
            NumeroDocumento = numeroDocumento,
            Descricao = descricao,
            Valor = valorParcela,
            NumeroParcela = 1,
            TotalParcelas = totalParcelas
        });

        var itensAfetadosIds = new List<int> { itemOrigemId };

        foreach (var (m, a) in mesesNecessarios)
        {
            var competencia = competenciasExistentes.First(c => c.Mes == m && c.Ano == a);

            var folha = await ctx.FolhasFornecedores.FirstOrDefaultAsync(f => f.CompetenciaId == competencia.Id);
            if (folha is null)
            {
                folha = new FolhaFornecedor { CompetenciaId = competencia.Id, Status = "Aberta", ValorTotal = 0 };
                ctx.FolhasFornecedores.Add(folha);
                await ctx.SaveChangesAsync();
            }

            var item = await ctx.FolhasFornecedoresItens
                .FirstOrDefaultAsync(i => i.FolhaFornecedorId == folha.Id && i.FornecedorId == itemOrigem.FornecedorId);
            if (item is null)
            {
                item = new FolhaFornecedorItem
                {
                    FolhaFornecedorId = folha.Id,
                    FornecedorId = itemOrigem.FornecedorId,
                    BancoPagadorId = itemOrigem.BancoPagadorId,
                    TipoPagamento = itemOrigem.TipoPagamento,
                    StatusPagamento = "Pendente",
                    Banco = itemOrigem.Banco,
                    Agencia = itemOrigem.Agencia,
                    Conta = itemOrigem.Conta,
                    ChavePix = itemOrigem.ChavePix,
                    NomeTitularConta = itemOrigem.NomeTitularConta,
                    ValorTotalPagar = 0
                };
                ctx.FolhasFornecedoresItens.Add(item);
                await ctx.SaveChangesAsync();
            }

            var numeroParcela = mesesNecessarios.IndexOf((m, a)) + 2; // a 1ª já foi gravada acima
            ctx.FolhasFornecedoresItensNotas.Add(new FolhaFornecedorItemNota
            {
                FolhaFornecedorItemId = item.Id,
                NumeroDocumento = numeroDocumento,
                Descricao = descricao,
                Valor = valorParcela,
                NumeroParcela = numeroParcela,
                TotalParcelas = totalParcelas
            });

            itensAfetadosIds.Add(item.Id);
        }

        await ctx.SaveChangesAsync();

        // Recalcula o total de cada item afetado e o total de cada folha envolvida.
        foreach (var itemId in itensAfetadosIds.Distinct())
        {
            var item = await ctx.FolhasFornecedoresItens.FindAsync(itemId);
            if (item is null) continue;

            item.ValorTotalPagar = await ctx.FolhasFornecedoresItensNotas
                .Where(n => n.FolhaFornecedorItemId == itemId)
                .SumAsync(n => (decimal?)n.Valor) ?? 0m;
        }
        await ctx.SaveChangesAsync();

        var folhasAfetadasIds = await ctx.FolhasFornecedoresItens
            .Where(i => itensAfetadosIds.Contains(i.Id))
            .Select(i => i.FolhaFornecedorId)
            .Distinct()
            .ToListAsync();

        foreach (var folhaId in folhasAfetadasIds)
        {
            var folha = await ctx.FolhasFornecedores.FindAsync(folhaId);
            if (folha is null) continue;

            folha.ValorTotal = await ctx.FolhasFornecedoresItens
                .Where(i => i.FolhaFornecedorId == folhaId)
                .SumAsync(i => (decimal?)i.ValorTotalPagar) ?? 0m;
        }
        await ctx.SaveChangesAsync();

        resultado.Sucesso = true;
        return resultado;
    }

    public async Task FecharFolhaAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var folha = await ctx.FolhasFornecedores.FirstOrDefaultAsync(f => f.Id == folhaId)
            ?? throw new InvalidOperationException("Folha não encontrada.");

        if (folha.Status != "Aberta")
            throw new InvalidOperationException("Só é possível fechar folhas abertas.");

        folha.ValorTotal = await ctx.FolhasFornecedoresItens
            .Where(i => i.FolhaFornecedorId == folhaId)
            .SumAsync(i => (decimal?)i.ValorTotalPagar) ?? 0m;

        folha.Status = "Fechada";
        folha.DataFechamento = DateTime.Now;

        var planoContaId = await ObterOuCriarPlanoServicoAsync(ctx);

        var lancamento = await ctx.LancamentosFinanceiros
            .FirstOrDefaultAsync(l => l.Origem == OrigemLancamento && l.OrigemId == folhaId);

        if (lancamento is null)
        {
            lancamento = new LancamentoFinanceiro
            {
                CompetenciaId = folha.CompetenciaId,
                PlanoContaId = planoContaId,
                TipoLancamento = "Saida",
                TipoDespesa = "Variavel",
                Origem = OrigemLancamento,
                OrigemId = folhaId,
                Descricao = DescricaoLancamento,
                Valor = folha.ValorTotal,
                Status = "Aberto",
                CriadoEm = DateTime.Now
            };
            ctx.LancamentosFinanceiros.Add(lancamento);
        }
        else
        {
            lancamento.CompetenciaId = folha.CompetenciaId;
            lancamento.PlanoContaId = planoContaId;
            lancamento.Valor = folha.ValorTotal;
            lancamento.Descricao = DescricaoLancamento;
            if (lancamento.Status == "Cancelado")
                lancamento.Status = "Aberto";
        }

        await ctx.SaveChangesAsync();

        folha.LancamentoFinanceiroId = lancamento.Id;
        await ctx.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task ReabrirFolhaAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var folha = await ctx.FolhasFornecedores.FirstOrDefaultAsync(f => f.Id == folhaId)
            ?? throw new InvalidOperationException("Folha não encontrada.");

        if (folha.Status == "Aberta")
            throw new InvalidOperationException("A folha já está aberta.");

        var statusCompetencia = await ctx.Competencias
            .Where(c => c.Id == folha.CompetenciaId)
            .Select(c => c.Status)
            .FirstOrDefaultAsync();

        if (statusCompetencia != "Aberta" && statusCompetencia != "Em Processamento")
            throw new InvalidOperationException(
                $"Não é possível reabrir ou alterar folhas já fechadas: a competência está com status \"{statusCompetencia}\". " +
                "A reabertura só é permitida quando a competência está \"Aberta\" ou \"Em Processamento\".");

        var lancamento = await ctx.LancamentosFinanceiros
            .FirstOrDefaultAsync(l => l.Origem == OrigemLancamento && l.OrigemId == folhaId);

        if (lancamento is not null && lancamento.Status == "Pago")
            throw new InvalidOperationException(
                "O lançamento financeiro vinculado já foi pago. Cancele/estornie o pagamento antes de reabrir a folha.");

        folha.Status = "Aberta";
        folha.DataFechamento = null;

        if (lancamento is not null && lancamento.Status != "Cancelado")
            lancamento.Status = "Cancelado";

        await ctx.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private static async Task<int> ObterOuCriarPlanoServicoAsync(AppDbContext ctx)
    {
        var plano = await ctx.PlanosContas.FirstOrDefaultAsync(p => p.Nome == PlanoContaPadrao);
        if (plano is null)
        {
            plano = new PlanoConta { Nome = PlanoContaPadrao, Ativo = true };
            ctx.PlanosContas.Add(plano);
            await ctx.SaveChangesAsync();
        }
        return plano.Id;
    }
}
