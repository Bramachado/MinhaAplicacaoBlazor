using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Services;

public class FolhaTutorService
{
    private const string OrigemLancamento = "FolhaTutor";
    private const string DescricaoLancamento = "Folha de Tutores";
    private const string PlanoContaPadrao = "FOLHA PAGAMENTO";

    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public FolhaTutorService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> ValidarFolhaAbertaAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var status = await ctx.FolhasTutores
            .AsNoTracking()
            .Where(f => f.Id == folhaId)
            .Select(f => f.Status)
            .FirstOrDefaultAsync();

        return status == "Aberta";
    }

    public async Task<decimal> RecalcularTotalAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var folha = await ctx.FolhasTutores.FirstOrDefaultAsync(f => f.Id == folhaId)
            ?? throw new InvalidOperationException("Folha não encontrada.");

        folha.ValorTotal = await ctx.FolhasTutoresItens
            .Where(i => i.FolhaTutorId == folhaId)
            .SumAsync(i => (decimal?)i.ValorTotalReceber) ?? 0m;

        await ctx.SaveChangesAsync();
        return folha.ValorTotal;
    }

    public async Task FecharFolhaAsync(int folhaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var folha = await ctx.FolhasTutores.FirstOrDefaultAsync(f => f.Id == folhaId)
            ?? throw new InvalidOperationException("Folha não encontrada.");

        if (folha.Status != "Aberta")
            throw new InvalidOperationException("Só é possível fechar folhas abertas.");

        folha.ValorTotal = await ctx.FolhasTutoresItens
            .Where(i => i.FolhaTutorId == folhaId)
            .SumAsync(i => (decimal?)i.ValorTotalReceber) ?? 0m;

        folha.Status = "Fechada";
        folha.DataFechamento = DateTime.Now;

        var planoContaId = await ObterOuCriarPlanoFolhaPagamentoAsync(ctx);

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

        var folha = await ctx.FolhasTutores.FirstOrDefaultAsync(f => f.Id == folhaId)
            ?? throw new InvalidOperationException("Folha não encontrada.");

        if (folha.Status == "Aberta")
            throw new InvalidOperationException("A folha já está aberta.");

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

    private static async Task<int> ObterOuCriarPlanoFolhaPagamentoAsync(AppDbContext ctx)
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
