using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MinhaAplicacaoBlazor.CnabBtg.Audit;
using MinhaAplicacaoBlazor.CnabBtg.Data;
using MinhaAplicacaoBlazor.CnabBtg.Generation;
using MinhaAplicacaoBlazor.CnabBtg.Payments;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.CnabBtg;

/// <summary>
/// Liga o relatório de Pagamentos Bancários (folhas fechadas + ContaBancaria) ao
/// gerador CNAB BTG: lista candidatos, gera o(s) .rem, monta auditoria e ZIP,
/// persiste o lote e controla o NSA por empresa. Não marca pagamentos como pagos.
/// </summary>
public class CnabBtgGeracaoService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IReadOnlyList<EmpresaPagadora> _empresas;
    private readonly string _storagePath;

    public CnabBtgGeracaoService(
        IDbContextFactory<AppDbContext> dbFactory,
        IConfiguration config,
        IHostEnvironment env)
    {
        _dbFactory = dbFactory;

        var configuradas = config.GetSection("CnabBtg:Empresas").Get<List<EmpresaPagadora>>();
        _empresas = configuradas is { Count: > 0 } ? configuradas : EmpresaPagadora.Padrao;

        var pasta = config["CnabBtg:StoragePath"] ?? "App_Data/CnabBtg";
        _storagePath = Path.IsPathRooted(pasta) ? pasta : Path.Combine(env.ContentRootPath, pasta);
    }

    public IReadOnlyList<EmpresaPagadora> Empresas => _empresas;

    // ---------------------------------------------------------------- Candidatos

    /// <summary>Lista os pagamentos das folhas FECHADAS da competência (candidatos ao CNAB).</summary>
    public async Task<List<CnabBtgPagamentoDto>> ObterPagamentosAsync(int competenciaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var lista = new List<CnabBtgPagamentoDto>();

        var colab = await ctx.FolhasColaboradores.AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Colaborador!).ThenInclude(c => c.ContaBancaria)
            .ToListAsync();
        foreach (var f in colab)
            foreach (var i in f.Itens)
                lista.Add(Mapear("Colaborador", i.Id, i.Colaborador?.Unidade?.Nome,
                    i.Colaborador?.Nome, i.Colaborador?.Cpf, i.ValorReceberPix, i.Colaborador?.ContaBancaria));

        var tut = await ctx.FolhasTutores.AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Tutor!).ThenInclude(t => t.ContaBancaria)
            .ToListAsync();
        foreach (var f in tut)
            foreach (var i in f.Itens)
                lista.Add(Mapear("Tutor", i.Id, i.Tutor?.Unidade?.Nome,
                    i.Tutor?.Nome, i.Tutor?.Cpf, i.ValorTotalReceber, i.Tutor?.ContaBancaria));

        var forn = await ctx.FolhasFornecedores.AsNoTracking()
            .Where(f => f.CompetenciaId == competenciaId && f.Status == "Fechada")
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.Unidade)
            .Include(f => f.Itens).ThenInclude(i => i.Fornecedor!).ThenInclude(fr => fr.ContaBancaria)
            .ToListAsync();
        foreach (var f in forn)
            foreach (var i in f.Itens)
                lista.Add(Mapear("Fornecedor", i.Id, i.Fornecedor?.Unidade?.Nome,
                    i.Fornecedor?.NomeRazaoSocial, i.Fornecedor?.CpfCnpj, i.ValorTotalPagar, i.Fornecedor?.ContaBancaria));

        // Marca os que já estão em um CNAB ativo (gerado, não cancelado).
        var chavesEmCnab = await ctx.CnabBatchPayments.AsNoTracking()
            .Where(bp => bp.CnabBatch!.Status != "CANCELADO" && bp.StatusCnab == "CNAB_GERADO")
            .Select(bp => new { bp.Origem, bp.OrigemId })
            .ToListAsync();
        var set = chavesEmCnab.Select(x => $"{x.Origem}:{x.OrigemId}").ToHashSet();

        foreach (var p in lista)
            p.JaEmCnab = set.Contains(p.Chave);

        return lista
            .OrderBy(p => p.Origem).ThenBy(p => p.Unidade).ThenBy(p => p.Nome)
            .ToList();
    }

    private static CnabBtgPagamentoDto Mapear(
        string origem, int origemId, string? unidade, string? nome, string? cpf, decimal valor, ContaBancaria? conta)
    {
        var forma = InferirForma(conta);
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
            Conta = conta?.Conta,      // dígito abaixo (ou separado na normalização se vazio)
            DigitoConta = string.IsNullOrWhiteSpace(conta?.DigitoConta) ? null : conta!.DigitoConta,
            TipoConta = conta?.TipoConta.ToString()
        };

        return new CnabBtgPagamentoDto
        {
            Origem = origem,
            OrigemId = origemId,
            Unidade = unidade,
            Nome = nome ?? string.Empty,
            CpfCnpj = conta?.CpfCnpj ?? cpf,
            Valor = valor,
            Forma = forma,
            Banco = conta?.CodigoBanco,
            Agencia = conta?.Agencia,
            Conta = conta?.Conta,
            ChavePix = conta?.ChavePix,
            Input = input
        };
    }

    /// <summary>
    /// Deriva a forma FEBRABAN a partir da conta: PIX→45; conta poupança→05;
    /// conta corrente no mesmo banco (208)→01 (crédito em conta); em outro banco→41 (TED).
    /// </summary>
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

    // ---------------------------------------------------------------- Geração

    public async Task<CnabBtgGeracaoResultadoDto> GerarAsync(CnabBtgGerarRequest req)
    {
        var empresa = EmpresaPagadora.Obter(req.EmpresaPagadora, _empresas)
            ?? throw new InvalidOperationException($"Empresa pagadora '{req.EmpresaPagadora}' não configurada.");

        var candidatos = await ObterPagamentosAsync(req.CompetenciaId);
        var selecionadas = req.ChavesSelecionadas.ToHashSet();
        var escolhidos = candidatos
            .Where(p => selecionadas.Contains(p.Chave) && !p.JaEmCnab)
            .ToList();

        if (escolhidos.Count == 0)
            throw new InvalidOperationException("Nenhum pagamento válido selecionado (ou todos já estão em um CNAB ativo).");

        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        // NSA
        var seq = await ctx.CnabSequences.FirstOrDefaultAsync(s => s.EmpresaPagadora == empresa.Codigo);
        var nsaInicial = req.UsarNsaAutomatico
            ? (seq?.UltimoNsa ?? 0) + 1
            : req.NsaInicial ?? 1;

        var options = new CnabGenerationOptions
        {
            Empresa = empresa,
            DataPagamento = req.DataPagamento,
            NsaInicial = nsaInicial,
            NomeBaseArquivo = string.IsNullOrWhiteSpace(req.NomeArquivo) ? "pagamentos" : req.NomeArquivo.Trim(),
            TipoPagamentoPrincipal = req.TipoPagamentoPrincipal,
            Ambiente = string.Equals(req.Ambiente, "PRODUCAO", StringComparison.OrdinalIgnoreCase)
                ? AmbienteCnab.Producao : AmbienteCnab.Teste,
            TratamentoInvalidos = req.BloquearSeHouverInvalidos ? TratamentoInvalidos.Bloquear : TratamentoInvalidos.Remover,
            SepararLotesPorForma = req.SepararLotesPorForma,
            ConvenioOverride = req.Convenio,
            MaxOperacoesPorArquivo = 50,
            GeradoEm = DateTime.Now,
            GeradoPor = req.GeradoPor
        };

        var generator = new CnabBtgPaymentGenerator();
        var resultado = generator.Gerar(escolhidos.Select(e => e.Input).ToList(), options);

        var auditReport = CnabAuditWriter.Montar(resultado, options);
        var json = CnabAuditWriter.ParaJson(auditReport);
        var csv = CnabAuditWriter.ParaCsv(auditReport);

        var origemPorId = escolhidos.ToDictionary(e => e.OrigemId, e => e.Origem);

        var batch = new CnabBatch
        {
            CompetenciaId = req.CompetenciaId,
            EmpresaPagadora = empresa.Codigo,
            Ambiente = options.Ambiente.ToString().ToUpperInvariant(),
            Convenio = options.ConvenioEfetivo,
            NsaInicial = resultado.NsaInicial,
            NsaFinal = resultado.NsaFinal,
            NomeBaseArquivo = options.NomeBaseArquivo,
            TipoPagamentoPrincipal = req.TipoPagamentoPrincipal,
            SeparadoPorForma = req.SepararLotesPorForma,
            TratamentoInvalidos = options.TratamentoInvalidos.ToString(),
            TotalSelecionados = resultado.TotalSelecionados,
            TotalValidos = resultado.TotalValidos,
            TotalCorrigidos = resultado.TotalCorrigidos,
            TotalPendentes = resultado.TotalPendentes,
            TotalInvalidos = resultado.TotalInvalidos,
            ValorTotal = resultado.ValorTotal,
            Status = resultado.Bloqueado ? "BLOQUEADO" : "GERADO",
            GeradoEm = options.GeradoEm,
            GeradoPor = req.GeradoPor,
            AuditoriaJson = json
        };

        foreach (var a in resultado.Arquivos)
        {
            batch.Arquivos.Add(new CnabGeneratedFile
            {
                FileName = a.FileName,
                Conteudo = a.Conteudo,
                Nsa = a.Nsa,
                QuantidadeOperacoes = a.Operacoes,
                QuantidadeRegistros = a.QuantidadeRegistros,
                ValorTotal = a.ValorTotal,
                TodasLinhas240 = a.TodasLinhas240,
                CriadoEm = options.GeradoEm
            });
        }

        foreach (var p in resultado.Pagamentos)
        {
            batch.Pagamentos.Add(new CnabBatchPayment
            {
                Origem = origemPorId.TryGetValue(p.Id, out var o) ? o : p.Origem,
                OrigemId = p.Id,
                Nome = p.Nome,
                CpfCnpj = p.CpfCnpj,
                Valor = p.Valor,
                ValorCentavos = p.ValorCentavos,
                FormaLancamento = string.IsNullOrEmpty(p.Forma) ? null : p.Forma,
                DataCnab = p.DataPagamento == default ? options.DataPagamento : p.DataPagamento,
                Status = p.Status.ToString(),
                SegmentosGerados = p.Geravel ? p.SegmentosGerados : null,
                ArquivoDestino = p.ArquivoDestino,
                Observacoes = string.Join(" | ", p.Correcoes.Concat(p.Erros).Concat(p.Avisos)),
                StatusCnab = p.Geravel && !resultado.Bloqueado ? "CNAB_GERADO" : "PENDENTE"
            });
        }

        ctx.CnabBatches.Add(batch);

        // Atualiza NSA apenas quando gerou de fato.
        if (!resultado.Bloqueado && resultado.Arquivos.Count > 0)
        {
            if (seq is null)
            {
                seq = new CnabSequence { EmpresaPagadora = empresa.Codigo, UltimoNsa = resultado.NsaFinal, UpdatedAt = options.GeradoEm };
                ctx.CnabSequences.Add(seq);
            }
            else
            {
                seq.UltimoNsa = resultado.NsaFinal;
                seq.UpdatedAt = options.GeradoEm;
            }
        }

        await ctx.SaveChangesAsync();

        // Salva o ZIP no storage (best-effort).
        if (!resultado.Bloqueado && resultado.Arquivos.Count > 0)
        {
            try
            {
                Directory.CreateDirectory(_storagePath);
                var zipBytes = CnabZipPacker.Empacotar(resultado, json, csv, options.NomeBaseArquivo);
                var caminho = Path.Combine(_storagePath, $"CNAB_{batch.Id}_{options.NomeBaseArquivo}.zip");
                await File.WriteAllBytesAsync(caminho, zipBytes);
                batch.CaminhoZip = caminho;
                await ctx.SaveChangesAsync();
            }
            catch
            {
                // Falha ao persistir em disco não invalida a geração — o ZIP é reconstruível.
            }
        }

        await tx.CommitAsync();

        return new CnabBtgGeracaoResultadoDto
        {
            BatchId = batch.Id,
            Status = batch.Status,
            MotivoBloqueio = resultado.MotivoBloqueio,
            NsaInicial = resultado.NsaInicial,
            NsaFinal = resultado.NsaFinal,
            TotalSelecionados = resultado.TotalSelecionados,
            TotalValidos = resultado.TotalValidos,
            TotalCorrigidos = resultado.TotalCorrigidos,
            TotalPendentes = resultado.TotalPendentes,
            TotalInvalidos = resultado.TotalInvalidos,
            ValorTotal = resultado.ValorTotal,
            TodasLinhas240 = resultado.TodasLinhas240,
            Arquivos = resultado.Arquivos.Select(a => new CnabBtgArquivoResumoDto
            {
                FileName = a.FileName,
                Nsa = a.Nsa,
                Operacoes = a.Operacoes,
                QuantidadeRegistros = a.QuantidadeRegistros,
                ValorTotal = a.ValorTotal,
                TodasLinhas240 = a.TodasLinhas240
            }).ToList(),
            Pagamentos = resultado.Pagamentos.Select(p => new CnabBtgPagamentoResultadoDto
            {
                OrigemId = p.Id,
                Origem = origemPorId.TryGetValue(p.Id, out var o) ? o : p.Origem,
                Nome = p.Nome,
                Valor = p.Valor,
                Status = p.Status.ToString(),
                ArquivoDestino = p.ArquivoDestino,
                Observacoes = string.Join(" | ", p.Erros.Concat(p.Avisos))
            }).ToList()
        };
    }

    // ---------------------------------------------------------------- Download / Histórico

    /// <summary>Retorna o ZIP do lote (reconstruído a partir do que está persistido).</summary>
    public async Task<(byte[] Bytes, string FileName)?> ObterZipAsync(int batchId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var batch = await ctx.CnabBatches.AsNoTracking()
            .Include(b => b.Arquivos)
            .FirstOrDefaultAsync(b => b.Id == batchId);
        if (batch is null || batch.Arquivos.Count == 0)
            return null;

        // Se o ZIP em disco existir, usa-o; senão reconstrói (rem + auditoria JSON).
        if (!string.IsNullOrEmpty(batch.CaminhoZip) && File.Exists(batch.CaminhoZip))
            return (await File.ReadAllBytesAsync(batch.CaminhoZip), Path.GetFileName(batch.CaminhoZip));

        using var ms = new MemoryStream();
        using (var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            foreach (var a in batch.Arquivos)
            {
                var entry = zip.CreateEntry(a.FileName, System.IO.Compression.CompressionLevel.Optimal);
                await using var s = entry.Open();
                var bytes = CnabZipPacker.RemBytes(a.Conteudo);
                await s.WriteAsync(bytes);
            }
            if (!string.IsNullOrEmpty(batch.AuditoriaJson))
            {
                var entry = zip.CreateEntry($"{batch.NomeBaseArquivo}_auditoria.json", System.IO.Compression.CompressionLevel.Optimal);
                await using var s = entry.Open();
                var bytes = new System.Text.UTF8Encoding(false).GetBytes(batch.AuditoriaJson);
                await s.WriteAsync(bytes);
            }
        }
        return (ms.ToArray(), $"CNAB_{batch.Id}_{batch.NomeBaseArquivo}.zip");
    }

    public async Task<CnabBtgBatchDetalheDto?> ObterDetalheAsync(int batchId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var b = await ctx.CnabBatches.AsNoTracking()
            .Include(x => x.Arquivos)
            .Include(x => x.Pagamentos)
            .FirstOrDefaultAsync(x => x.Id == batchId);
        if (b is null)
            return null;

        return new CnabBtgBatchDetalheDto
        {
            Cabecalho = new CnabBtgHistoricoDto
            {
                BatchId = b.Id,
                EmpresaPagadora = b.EmpresaPagadora,
                Ambiente = b.Ambiente,
                GeradoEm = b.GeradoEm,
                GeradoPor = b.GeradoPor,
                NsaInicial = b.NsaInicial,
                NsaFinal = b.NsaFinal,
                QuantidadeArquivos = b.Arquivos.Count,
                TotalGerados = b.Pagamentos.Count(p => p.StatusCnab == "CNAB_GERADO"),
                ValorTotal = b.ValorTotal,
                Status = b.Status
            },
            CompetenciaId = b.CompetenciaId,
            NomeBaseArquivo = b.NomeBaseArquivo,
            Convenio = b.Convenio,
            TotalValidos = b.TotalValidos,
            TotalCorrigidos = b.TotalCorrigidos,
            TotalPendentes = b.TotalPendentes,
            TotalInvalidos = b.TotalInvalidos,
            Arquivos = b.Arquivos.Select(a => new CnabBtgArquivoResumoDto
            {
                FileName = a.FileName,
                Nsa = a.Nsa,
                Operacoes = a.QuantidadeOperacoes,
                QuantidadeRegistros = a.QuantidadeRegistros,
                ValorTotal = a.ValorTotal,
                TodasLinhas240 = a.TodasLinhas240
            }).ToList(),
            Pagamentos = b.Pagamentos.Select(p => new CnabBtgBatchPagamentoDto
            {
                Origem = p.Origem,
                OrigemId = p.OrigemId,
                Nome = p.Nome,
                CpfCnpj = p.CpfCnpj,
                Valor = p.Valor,
                Forma = p.FormaLancamento,
                Status = p.Status,
                StatusCnab = p.StatusCnab,
                ArquivoDestino = p.ArquivoDestino,
                Observacoes = p.Observacoes
            }).ToList()
        };
    }

    public async Task<List<CnabBtgHistoricoDto>> ObterHistoricoAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.CnabBatches.AsNoTracking()
            .OrderByDescending(b => b.GeradoEm)
            .Select(b => new CnabBtgHistoricoDto
            {
                BatchId = b.Id,
                EmpresaPagadora = b.EmpresaPagadora,
                Ambiente = b.Ambiente,
                GeradoEm = b.GeradoEm,
                GeradoPor = b.GeradoPor,
                NsaInicial = b.NsaInicial,
                NsaFinal = b.NsaFinal,
                QuantidadeArquivos = b.Arquivos.Count,
                TotalGerados = b.Pagamentos.Count(p => p.StatusCnab == "CNAB_GERADO"),
                ValorTotal = b.ValorTotal,
                Status = b.Status
            })
            .ToListAsync();
    }
}
