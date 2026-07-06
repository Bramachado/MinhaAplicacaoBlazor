using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.CnabBtg.Generation;
using MinhaAplicacaoBlazor.CnabBtg.Payments;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.CnabBtg;

/// <summary>
/// Serviço de CONFERÊNCIA da tela "Gerenciar CNAB". Carrega TODOS os pagamentos
/// das folhas fechadas da competência (transferências E boletos), classifica cada
/// um, normaliza/valida as transferências e calcula os totais para conferência.
/// A geração em si continua no <see cref="CnabBtgGeracaoService"/> — este serviço
/// não gera arquivo nem grava nada; apenas prepara os dados da tela.
/// Boletos aparecem e entram nos totais, mas nunca no arquivo .rem.
/// </summary>
public class CnabBtgGerenciamentoService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public CnabBtgGerenciamentoService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<(int Id, string Texto)>> ObterCompetenciasAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Competencias.AsNoTracking()
            .OrderByDescending(c => c.Ano).ThenByDescending(c => c.Mes)
            .Select(c => new ValueTuple<int, string>(c.Id, $"{c.Mes:00}/{c.Ano} - {c.Status}"))
            .ToListAsync();
    }

    /// <summary>Carrega e classifica todos os pagamentos da competência para conferência.</summary>
    /// <param name="permitirRegerar">
    /// Quando true, itens já em CNAB ativo continuam selecionáveis (modo teste),
    /// mantendo a marcação para exibição. Quando false, ficam bloqueados.
    /// </param>
    public async Task<CnabGerenciarResumoDto> ObterResumoAsync(int competenciaId, bool permitirRegerar = false)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var competencia = await ctx.Competencias.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == competenciaId);

        var resumo = new CnabGerenciarResumoDto
        {
            CompetenciaId = competenciaId,
            CompetenciaTexto = competencia is not null ? $"{competencia.Mes:00}/{competencia.Ano}" : string.Empty
        };

        if (competencia is null)
            return resumo;

        var pagamentos = new List<CnabGerenciarPagamentoDto>();

        // --- Colaboradores (sempre transferência via conta bancária) ---
        var colab = await ctx.FolhasColaboradores.AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.ContaBancaria)
            .ToListAsync();
        foreach (var f in colab)
            foreach (var i in f.Itens)
                pagamentos.Add(Classificar("Colaborador", i.Id, i.Colaborador?.Unidade?.Nome,
                    i.Colaborador?.Nome, i.Colaborador?.Cpf, i.ValorReceberPix,
                    i.Colaborador?.ContaBancaria, ehBoleto: false, bancoCadastro: null));

        // --- Tutores (sempre transferência via conta bancária) ---
        var tut = await ctx.FolhasTutores.AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.ContaBancaria)
            .ToListAsync();
        foreach (var f in tut)
            foreach (var i in f.Itens)
                pagamentos.Add(Classificar("Tutor", i.Id, i.Tutor?.Unidade?.Nome,
                    i.Tutor?.Nome, i.Tutor?.Cpf, i.ValorTotalReceber,
                    i.Tutor?.ContaBancaria, ehBoleto: false, bancoCadastro: null));

        // --- Fornecedores (boleto OU transferência, conforme TipoPagamento) ---
        var forn = await ctx.FolhasFornecedores.AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.ContaBancaria)
            .Include(f => f.Itens).ThenInclude(i => i.BancoPagador)
            .ToListAsync();
        foreach (var f in forn)
            foreach (var i in f.Itens)
            {
                var tipoPagamento = !string.IsNullOrWhiteSpace(i.TipoPagamento)
                    ? i.TipoPagamento
                    : (i.Fornecedor?.TipoPagamento ?? "Boleto");
                var ehBoleto = EhBoleto(tipoPagamento);

                pagamentos.Add(Classificar("Fornecedor", i.Id, i.Fornecedor?.Unidade?.Nome,
                    i.Fornecedor?.NomeRazaoSocial, i.Fornecedor?.CpfCnpj, i.ValorTotalPagar,
                    i.Fornecedor?.ContaBancaria, ehBoleto, bancoCadastro: i.BancoPagador?.NomeBanco));
            }

        // --- Marca os que já estão em um CNAB ativo ---
        var chavesEmCnab = await ctx.CnabBatchPayments.AsNoTracking()
            .Where(bp => bp.CnabBatch!.Status != "CANCELADO" && bp.StatusCnab == "CNAB_GERADO")
            .Select(bp => new { bp.Origem, bp.OrigemId })
            .ToListAsync();
        var set = chavesEmCnab.Select(x => $"{x.Origem}:{x.OrigemId}").ToHashSet();

        foreach (var p in pagamentos)
        {
            if (set.Contains(p.Chave))
            {
                p.JaEmCnab = true;
                if (permitirRegerar)
                {
                    // Modo teste: mantém a marcação, mas segue selecionável/gerável.
                    p.Alertas.Insert(0, "Já em CNAB ativo — será REGERADO (modo teste).");
                }
                else
                {
                    p.Selecionavel = false;
                    p.StatusValidacao = "JaGerado";
                    p.Alertas.Insert(0, "Já incluído em um CNAB ativo.");
                }
            }
        }

        resumo.Pagamentos = pagamentos
            .OrderBy(p => p.TipoOperacaoCnab)
            .ThenBy(p => p.Origem).ThenBy(p => p.Unidade).ThenBy(p => p.Nome)
            .ToList();

        // --- Totais da competência ---
        resumo.TotalPagamentos = pagamentos.Count;
        resumo.ValorTotalCompetencia = pagamentos.Sum(p => p.Valor);

        var transferencias = pagamentos.Where(p => p.EhTransferencia).ToList();
        resumo.TotalTransferencias = transferencias.Count;
        resumo.ValorTransferencias = transferencias.Sum(p => p.Valor);

        var pix = transferencias.Where(p => p.TipoTransferencia == "PIX").ToList();
        resumo.TotalPix = pix.Count;
        resumo.ValorPix = pix.Sum(p => p.Valor);

        var ted = transferencias.Where(p => p.TipoTransferencia == "TED").ToList();
        resumo.TotalTed = ted.Count;
        resumo.ValorTed = ted.Sum(p => p.Valor);

        var boletos = pagamentos.Where(p => p.EhBoleto).ToList();
        resumo.TotalBoletos = boletos.Count;
        resumo.ValorBoletos = boletos.Sum(p => p.Valor);

        var jaGerados = pagamentos.Where(p => p.JaEmCnab).ToList();
        resumo.TotalJaGerados = jaGerados.Count;
        resumo.ValorJaGerados = jaGerados.Sum(p => p.Valor);

        var invalidos = pagamentos.Where(p => p.EhTransferencia && p.Invalido).ToList();
        resumo.TotalInvalidos = invalidos.Count;
        resumo.ValorInvalidos = invalidos.Sum(p => p.Valor);

        return resumo;
    }

    /// <summary>Valida os pagamentos selecionados e devolve o detalhamento por item.</summary>
    public async Task<CnabValidacaoSelecionadosDto> ValidarSelecionadosAsync(int competenciaId, IEnumerable<string> chavesSelecionadas, bool permitirRegerar = false)
    {
        var resumo = await ObterResumoAsync(competenciaId, permitirRegerar);
        var selecionadas = chavesSelecionadas.ToHashSet();
        var itens = resumo.Pagamentos.Where(p => selecionadas.Contains(p.Chave)).ToList();

        var dto = new CnabValidacaoSelecionadosDto
        {
            TotalSelecionados = itens.Count,
            ValorSelecionado = itens.Sum(p => p.Valor)
        };

        foreach (var p in itens)
        {
            dto.Itens.Add(new CnabValidacaoItemDto
            {
                Chave = p.Chave,
                Origem = p.Origem,
                OrigemId = p.OrigemId,
                Nome = p.Nome,
                Valor = p.Valor,
                Status = p.StatusValidacao,
                Observacoes = p.Alertas.ToList()
            });
        }

        var validos = itens.Where(p => p.GeravelNoCnab && p.StatusValidacao is "Valido").ToList();
        var corrigidos = itens.Where(p => p.GeravelNoCnab && p.StatusValidacao is "Corrigido" or "Pendente").ToList();
        var invalidos = itens.Where(p => p.EhTransferencia && p.Invalido).ToList();
        var boletos = itens.Where(p => p.EhBoleto).ToList();

        dto.TotalValidos = validos.Count;
        dto.ValorValidos = validos.Sum(p => p.Valor);
        dto.TotalCorrigidos = corrigidos.Count;
        dto.ValorCorrigidos = corrigidos.Sum(p => p.Valor);
        dto.TotalInvalidos = invalidos.Count;
        dto.ValorInvalidos = invalidos.Sum(p => p.Valor);
        dto.TotalBoletosBloqueados = boletos.Count;
        dto.ValorBoletosBloqueados = boletos.Sum(p => p.Valor);

        if (boletos.Count > 0)
            dto.Alertas.Add($"{boletos.Count} boleto(s) selecionado(s) serão IGNORADOS na geração — módulo de Segmentos J/J-52 não implementado.");
        if (invalidos.Count > 0)
            dto.Alertas.Add($"{invalidos.Count} pagamento(s) inválido(s) não entram no arquivo (ou bloqueiam a geração, conforme a opção escolhida).");
        var jaGeradosSelecionados = itens.Count(p => p.JaEmCnab);
        if (jaGeradosSelecionados > 0)
            dto.Alertas.Add(permitirRegerar
                ? $"{jaGeradosSelecionados} pagamento(s) já em CNAB ativo serão REGERADOS (modo teste)."
                : $"{jaGeradosSelecionados} selecionado(s) já estão em um CNAB ativo e serão ignorados.");

        return dto;
    }

    // ------------------------------------------------------------ helpers

    private static bool EhBoleto(string? tipo)
    {
        var t = (tipo ?? string.Empty).Trim().ToUpperInvariant();
        return t is "BOLETO" or "TITULO" or "TÍTULO" or "CODIGO DE BARRAS" or "CÓDIGO DE BARRAS";
    }

    /// <summary>
    /// Monta o DTO de conferência de um pagamento: infere a forma, normaliza/valida
    /// (transferências) e coleta os alertas. Nunca inventa dados — o que falta é
    /// marcado como inválido/pendente.
    /// </summary>
    private static CnabGerenciarPagamentoDto Classificar(
        string origem, int origemId, string? unidade, string? nome, string? cpf,
        decimal valor, ContaBancaria? conta, bool ehBoleto, string? bancoCadastro)
    {
        var forma = ehBoleto ? null : InferirForma(conta);

        var input = new PaymentInput
        {
            Id = origemId,
            Origem = origem,
            Nome = nome ?? string.Empty,
            NomeTitular = conta?.NomeTitular ?? nome,
            CpfCnpj = conta?.CpfCnpj ?? cpf,
            Valor = valor,
            Forma = forma,
            TipoPessoa = conta?.TipoPessoa.ToString(),
            ChavePix = conta?.ChavePix,
            TipoChavePix = conta is { TipoChavePix: not TipoChavePix.Nenhuma } ? conta.TipoChavePix.ToString() : null,
            BancoNome = conta?.NomeBanco,
            CodigoBanco = conta?.CodigoBanco,
            Agencia = conta?.Agencia,
            Conta = conta?.Conta,
            DigitoConta = string.IsNullOrWhiteSpace(conta?.DigitoConta) ? null : conta!.DigitoConta,
            TipoConta = conta?.TipoConta.ToString()
        };

        var dto = new CnabGerenciarPagamentoDto
        {
            Origem = origem,
            OrigemId = origemId,
            Unidade = unidade,
            Nome = nome ?? string.Empty,
            CpfCnpj = conta?.CpfCnpj ?? cpf,
            Valor = valor,
            TipoOperacaoCnab = ehBoleto ? "Boleto" : "Transferencia",
            Forma = forma,
            BancoCadastro = bancoCadastro,
            BancoFavorecido = conta?.NomeBanco,
            CodigoBanco = conta?.CodigoBanco,
            Agencia = conta?.Agencia,
            Conta = conta?.Conta,
            DigitoConta = conta?.DigitoConta,
            ChavePix = conta?.ChavePix,
            Selecionavel = true,
            Input = input
        };

        if (ehBoleto)
        {
            dto.StatusValidacao = "Boleto";
            dto.Alertas.Add("Boleto — geração via Segmentos J/J-52 não implementada; não entra no arquivo .rem.");
            return dto;
        }

        // Transferência: normaliza e valida com opções de conferência (data = hoje).
        var options = new CnabGenerationOptions
        {
            DataPagamento = DateTime.Today,
            GeradoEm = DateTime.Now,
            TipoPagamentoPrincipal = null
        };
        var normalizado = PaymentNormalizer.Normalizar(input, options);
        PaymentValidator.Validar(normalizado, options);

        dto.Input = input;
        dto.StatusValidacao = normalizado.Status switch
        {
            StatusPagamentoCnab.Valido => "Valido",
            StatusPagamentoCnab.CorrigidoAutomaticamente => "Corrigido",
            StatusPagamentoCnab.PendenteDeConfirmacao => "Pendente",
            _ => "Invalido"
        };
        dto.Alertas.AddRange(normalizado.Erros);
        dto.Alertas.AddRange(normalizado.Avisos);
        dto.Alertas.AddRange(normalizado.Correcoes);

        return dto;
    }

    /// <summary>PIX→45; poupança→05; mesma instituição (208)→01; outro banco→41 (TED).</summary>
    private static string? InferirForma(ContaBancaria? conta)
    {
        if (conta is null)
            return null;
        if (conta.Forma == Forma.PIX)
            return FormaLancamento.PixTransferencia;
        if (conta.TipoConta == TipoConta.Poupanca)
            return FormaLancamento.CreditoPoupanca;
        return conta.CodigoBanco == "208"
            ? FormaLancamento.CreditoContaCorrente
            : FormaLancamento.TedOutraTitularidade;
    }
}
