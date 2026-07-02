using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models.Cnab;
using MinhaAplicacaoBlazor.Models.Cnab.Dtos;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public class CnabPagamentoService : ICnabPagamentoService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly GeradorCnabPagamentoBtg _gerador;

    private static readonly string[] StatusEmRemessaAtiva =
        { "Gerada", "Enviada", "Aceita", "Processada" };

    public CnabPagamentoService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        _gerador = new GeradorCnabPagamentoBtg();
    }

    public async Task<List<ConfiguracaoCnab>> ListarConfiguracoesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.ConfiguracoesCnab.AsNoTracking()
            .OrderByDescending(c => c.Ativa).ThenBy(c => c.NomeConfiguracao)
            .ToListAsync();
    }

    public async Task<List<FormaLancamentoCnab>> ListarFormasLancamentoAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.FormasLancamentoCnab.AsNoTracking()
            .OrderBy(f => f.Codigo)
            .ToListAsync();
    }

    public async Task<List<PagamentoPendenteCnabDto>> ListarPagamentosPendentesAsync(int competenciaId, string origem)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var idsEmRemessaAtivaLanc = await ctx.RemessasCnabItens
            .Where(i => i.RemessaCnab!.Status != "Cancelada" && i.LancamentoFinanceiroId != null)
            .Select(i => i.LancamentoFinanceiroId!.Value)
            .ToListAsync();

        var idsEmRemessaColab = await ctx.RemessasCnabItens
            .Where(i => i.RemessaCnab!.Status != "Cancelada" && i.FolhaColaboradorItemId != null)
            .Select(i => i.FolhaColaboradorItemId!.Value)
            .ToListAsync();

        var idsEmRemessaTutor = await ctx.RemessasCnabItens
            .Where(i => i.RemessaCnab!.Status != "Cancelada" && i.FolhaTutorItemId != null)
            .Select(i => i.FolhaTutorItemId!.Value)
            .ToListAsync();

        var resultado = new List<PagamentoPendenteCnabDto>();

        if (origem == "LancamentoFinanceiro")
        {
            var lancs = await ctx.LancamentosFinanceiros.AsNoTracking()
                .Include(l => l.Fornecedor).ThenInclude(f => f!.ContaBancaria)
                .Where(l => l.CompetenciaId == competenciaId
                            && l.TipoLancamento == "Saida"
                            && l.Status != "Cancelado"
                            && l.Status != "Pago"
                            && l.Valor > 0)
                .OrderBy(l => l.DataVencimento)
                .ToListAsync();

            foreach (var l in lancs)
            {
                resultado.Add(new PagamentoPendenteCnabDto
                {
                    Origem = "LancamentoFinanceiro",
                    OrigemId = l.Id,
                    NomeFavorecido = l.Fornecedor?.NomeRazaoSocial ?? l.Descricao,
                    CPF_CNPJ_Favorecido = l.Fornecedor?.CpfCnpj,
                    Banco = l.Fornecedor?.ContaBancaria?.CodigoBanco,
                    Agencia = l.Fornecedor?.ContaBancaria?.Agencia,
                    Conta = l.Fornecedor?.ContaBancaria?.Conta,
                    ChavePix = l.Fornecedor?.ContaBancaria?.ChavePix,
                    Valor = l.Valor,
                    DataVencimento = l.DataVencimento,
                    Descricao = l.Descricao,
                    JaEmRemessa = idsEmRemessaAtivaLanc.Contains(l.Id)
                });
            }
        }
        else if (origem == "FolhaColaborador")
        {
            var itens = await ctx.FolhasColaboradoresItens.AsNoTracking()
                .Include(i => i.FolhaColaborador)
                .Include(i => i.Colaborador).ThenInclude(c => c!.ContaBancaria)
                .Where(i => i.FolhaColaborador!.CompetenciaId == competenciaId
                            && i.ValorReceberPix > 0)
                .ToListAsync();

            foreach (var i in itens)
            {
                resultado.Add(new PagamentoPendenteCnabDto
                {
                    Origem = "FolhaColaborador",
                    OrigemId = i.Id,
                    NomeFavorecido = i.Colaborador?.Nome ?? "",
                    CPF_CNPJ_Favorecido = i.Colaborador?.Cpf,
                    Banco = i.Colaborador?.ContaBancaria?.CodigoBanco,
                    Agencia = i.Colaborador?.ContaBancaria?.Agencia,
                    Conta = i.Colaborador?.ContaBancaria?.Conta,
                    ChavePix = i.Colaborador?.ContaBancaria?.ChavePix,
                    Valor = i.ValorReceberPix,
                    Descricao = $"Folha Colab. {i.FolhaColaborador!.Status}",
                    JaEmRemessa = idsEmRemessaColab.Contains(i.Id)
                });
            }
        }
        else if (origem == "FolhaTutor")
        {
            var itens = await ctx.FolhasTutoresItens.AsNoTracking()
                .Include(i => i.FolhaTutor)
                .Include(i => i.Tutor).ThenInclude(t => t!.ContaBancaria)
                .Where(i => i.FolhaTutor!.CompetenciaId == competenciaId
                            && i.ValorTotalReceber > 0)
                .ToListAsync();

            foreach (var i in itens)
            {
                resultado.Add(new PagamentoPendenteCnabDto
                {
                    Origem = "FolhaTutor",
                    OrigemId = i.Id,
                    NomeFavorecido = i.Tutor?.Nome ?? "",
                    CPF_CNPJ_Favorecido = i.Tutor?.Cpf,
                    Banco = i.Banco ?? i.Tutor?.ContaBancaria?.CodigoBanco,
                    Agencia = i.Agencia ?? i.Tutor?.ContaBancaria?.Agencia,
                    Conta = i.Conta ?? i.Tutor?.ContaBancaria?.Conta,
                    ChavePix = i.ChavePix ?? i.Tutor?.ContaBancaria?.ChavePix,
                    Valor = i.ValorTotalReceber,
                    Descricao = $"Folha Tutor {i.FolhaTutor!.Status}",
                    JaEmRemessa = idsEmRemessaTutor.Contains(i.Id)
                });
            }
        }

        return resultado;
    }

    public async Task<ValidacaoCnabResultadoDto> ValidarRemessaAsync(CriarRemessaCnabRequest request)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var resultado = new ValidacaoCnabResultadoDto();

        var forma = await ctx.FormasLancamentoCnab.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.FormaLancamentoCnabId);

        if (forma is null)
        {
            resultado.ErrosGerais.Add("Selecione uma forma de lançamento CNAB.");
            return resultado;
        }

        if (request.ItensSelecionados.Count == 0)
        {
            resultado.ErrosGerais.Add("Nenhum pagamento selecionado.");
            return resultado;
        }

        if (request.DataPagamento == default)
            resultado.ErrosGerais.Add("Informe a data de pagamento.");

        var indice = 0;
        foreach (var item in request.ItensSelecionados)
        {
            var erros = ValidarItem(item, forma.Codigo);
            if (erros.Count > 0)
            {
                resultado.ItensComErro.Add(new ItemValidacaoDto
                {
                    Indice = indice,
                    NomeFavorecido = item.NomeFavorecido,
                    Erros = erros
                });
            }
            indice++;
        }

        resultado.QuantidadeValidada = request.ItensSelecionados.Count;
        resultado.ValorTotal = request.ItensSelecionados.Sum(i => i.ValorPagamento);
        resultado.Valido = resultado.ErrosGerais.Count == 0 && resultado.ItensComErro.Count == 0;
        return resultado;
    }

    private static List<string> ValidarItem(CriarRemessaCnabItemRequest item, string codigoForma)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(item.NomeFavorecido))
            erros.Add("Nome do favorecido é obrigatório.");
        if (item.ValorPagamento <= 0)
            erros.Add("Valor do pagamento deve ser maior que zero.");

        var creditoConta = new[] { "01", "03", "05", "41", "43" };
        var pix = new[] { "45" };
        var titulosTributos = new[] { "30", "31", "11", "16" };

        if (creditoConta.Contains(codigoForma))
        {
            if (string.IsNullOrWhiteSpace(item.BancoFavorecido)) erros.Add("Banco do favorecido obrigatório.");
            if (string.IsNullOrWhiteSpace(item.AgenciaFavorecido)) erros.Add("Agência do favorecido obrigatória.");
            if (string.IsNullOrWhiteSpace(item.ContaFavorecido)) erros.Add("Conta do favorecido obrigatória.");
            if (string.IsNullOrWhiteSpace(item.CPF_CNPJ_Favorecido)) erros.Add("CPF/CNPJ do favorecido obrigatório.");
        }
        else if (pix.Contains(codigoForma))
        {
            if (string.IsNullOrWhiteSpace(item.ChavePix)) erros.Add("Chave PIX obrigatória.");
            if (string.IsNullOrWhiteSpace(item.TipoChavePix)) erros.Add("Tipo da chave PIX obrigatório.");
            if (string.IsNullOrWhiteSpace(item.CPF_CNPJ_Favorecido)) erros.Add("CPF/CNPJ do favorecido obrigatório.");
        }
        else if (titulosTributos.Contains(codigoForma))
        {
            if (string.IsNullOrWhiteSpace(item.CodigoBarras)) erros.Add("Código de barras obrigatório.");
        }

        return erros;
    }

    public async Task<RemessaCnab> CriarRemessaAsync(CriarRemessaCnabRequest request)
    {
        var validacao = await ValidarRemessaAsync(request);
        if (!validacao.Valido)
            throw new InvalidOperationException("Há erros de validação na remessa. Corrija antes de prosseguir.");

        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var cfg = await ctx.ConfiguracoesCnab.FirstOrDefaultAsync(c => c.Id == request.ConfiguracaoCnabId)
            ?? throw new InvalidOperationException("Configuração CNAB não encontrada.");

        if (!cfg.Ativa)
            throw new InvalidOperationException("Configuração CNAB inativa.");

        var formaId = request.FormaLancamentoCnabId;

        // Verificar duplicidade
        foreach (var item in request.ItensSelecionados)
        {
            if (item.LancamentoFinanceiroId is int lid)
            {
                var ja = await ctx.RemessasCnabItens.AnyAsync(x =>
                    x.LancamentoFinanceiroId == lid && x.RemessaCnab!.Status != "Cancelada");
                if (ja) throw new InvalidOperationException($"Lançamento #{lid} já está em outra remessa ativa.");
            }
            if (item.FolhaColaboradorItemId is int cid)
            {
                var ja = await ctx.RemessasCnabItens.AnyAsync(x =>
                    x.FolhaColaboradorItemId == cid && x.RemessaCnab!.Status != "Cancelada");
                if (ja) throw new InvalidOperationException($"Item de folha colab. #{cid} já está em outra remessa ativa.");
            }
            if (item.FolhaTutorItemId is int tid)
            {
                var ja = await ctx.RemessasCnabItens.AnyAsync(x =>
                    x.FolhaTutorItemId == tid && x.RemessaCnab!.Status != "Cancelada");
                if (ja) throw new InvalidOperationException($"Item de folha tutor #{tid} já está em outra remessa ativa.");
            }
        }

        var nsa = cfg.SequencialArquivo;
        var dataGer = DateTime.Now;
        var nomeArquivo = $"CNAB_BTG_{dataGer:yyyyMMdd_HHmmss}_{nsa:000000}.rem";

        var remessa = new RemessaCnab
        {
            ConfiguracaoCnabId = cfg.Id,
            CompetenciaId = request.CompetenciaId,
            NumeroSequencial = nsa,
            NomeArquivo = nomeArquivo,
            DataGeracao = dataGer,
            QuantidadePagamentos = request.ItensSelecionados.Count,
            ValorTotal = request.ItensSelecionados.Sum(i => i.ValorPagamento),
            Status = "Gerada",
            Observacao = request.Observacao,
            Itens = request.ItensSelecionados.Select((i, idx) => new RemessaCnabItem
            {
                LancamentoFinanceiroId = i.LancamentoFinanceiroId,
                FolhaColaboradorItemId = i.FolhaColaboradorItemId,
                FolhaTutorItemId = i.FolhaTutorItemId,
                FornecedorId = i.FornecedorId,
                FormaLancamentoCnabId = formaId,
                NomeFavorecido = i.NomeFavorecido,
                CPF_CNPJ_Favorecido = i.CPF_CNPJ_Favorecido,
                BancoFavorecido = i.BancoFavorecido,
                AgenciaFavorecido = i.AgenciaFavorecido,
                AgenciaDVFavorecido = i.AgenciaDVFavorecido,
                ContaFavorecido = i.ContaFavorecido,
                ContaDVFavorecido = i.ContaDVFavorecido,
                ChavePix = i.ChavePix,
                TipoChavePix = i.TipoChavePix,
                CodigoBarras = i.CodigoBarras,
                DataPagamento = request.DataPagamento,
                ValorPagamento = i.ValorPagamento,
                SeuNumero = $"REM{nsa:000000}{idx + 1:00000}",
                Status = "EmRemessa"
            }).ToList()
        };

        ctx.RemessasCnab.Add(remessa);
        cfg.SequencialArquivo = nsa + 1;
        await ctx.SaveChangesAsync();

        // Gerar conteúdo do arquivo
        var forma = await ctx.FormasLancamentoCnab.FirstAsync(f => f.Id == formaId);
        var remessaCompleta = await ctx.RemessasCnab
            .Include(r => r.ConfiguracaoCnab)
            .Include(r => r.Itens)
            .FirstAsync(r => r.Id == remessa.Id);

        var conteudo = _gerador.Gerar(remessaCompleta.ConfiguracaoCnab!, remessaCompleta, forma);
        remessaCompleta.ConteudoArquivo = conteudo;
        remessaCompleta.QuantidadeRegistros = conteudo
            .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length;

        await ctx.SaveChangesAsync();
        await tx.CommitAsync();

        return remessaCompleta;
    }

    public async Task<string> GerarArquivoRemessaAsync(int remessaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var remessa = await ctx.RemessasCnab
            .Include(r => r.ConfiguracaoCnab)
            .Include(r => r.Itens).ThenInclude(i => i.FormaLancamentoCnab)
            .FirstOrDefaultAsync(r => r.Id == remessaId)
            ?? throw new InvalidOperationException("Remessa não encontrada.");

        var forma = remessa.Itens.FirstOrDefault()?.FormaLancamentoCnab
            ?? throw new InvalidOperationException("Remessa sem forma de lançamento.");

        var conteudo = _gerador.Gerar(remessa.ConfiguracaoCnab!, remessa, forma);
        remessa.ConteudoArquivo = conteudo;
        await ctx.SaveChangesAsync();
        return conteudo;
    }

    public async Task<byte[]> BaixarArquivoRemessaAsync(int remessaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var remessa = await ctx.RemessasCnab.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == remessaId)
            ?? throw new InvalidOperationException("Remessa não encontrada.");

        var conteudo = remessa.ConteudoArquivo
            ?? await GerarArquivoRemessaAsync(remessaId);

        return Encoding.ASCII.GetBytes(conteudo);
    }

    public async Task CancelarRemessaAsync(int remessaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var remessa = await ctx.RemessasCnab
            .Include(r => r.Itens)
            .FirstOrDefaultAsync(r => r.Id == remessaId)
            ?? throw new InvalidOperationException("Remessa não encontrada.");

        if (remessa.Status == "Processada")
            throw new InvalidOperationException("Não é possível cancelar uma remessa já processada.");
        if (remessa.Status == "Cancelada")
            return;

        remessa.Status = "Cancelada";
        foreach (var item in remessa.Itens)
        {
            item.Status = "Cancelado";
        }

        await ctx.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task MarcarComoEnviadaAsync(int remessaId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var remessa = await ctx.RemessasCnab.FirstOrDefaultAsync(r => r.Id == remessaId)
            ?? throw new InvalidOperationException("Remessa não encontrada.");

        if (remessa.Status is "Cancelada" or "Processada")
            throw new InvalidOperationException("Remessa não pode ser marcada como enviada.");

        remessa.Status = "Enviada";
        await ctx.SaveChangesAsync();
    }

    public async Task<ResultadoImportacaoRetornoCnabDto> ImportarRetornoAsync(
        Stream arquivo, string nomeArquivo, int? remessaCnabId)
    {
        using var reader = new StreamReader(arquivo, Encoding.ASCII);
        var conteudo = await reader.ReadToEndAsync();
        var linhas = conteudo
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var retorno = new RetornoCnab
        {
            RemessaCnabId = remessaCnabId,
            NomeArquivo = nomeArquivo,
            DataImportacao = DateTime.Now,
            ConteudoArquivo = conteudo,
            QuantidadeRegistros = linhas.Count,
            Status = "Importado"
        };

        var msgs = new List<string>();
        var aceitos = 0;
        var rejeitados = 0;
        var processados = 0;

        foreach (var linha in linhas)
        {
            if (linha.Length < 240) continue;
            var tipoRegistro = linha[7];
            if (tipoRegistro != '3') continue; // só detalhes
            var segmento = linha[13];
            if (segmento != 'A') continue;

            var seuNumero = linha.Substring(73, 20).Trim();
            var nome = linha.Substring(43, 30).Trim();
            var valorStr = linha.Substring(119, 15);
            var dataPgtoStr = linha.Substring(149, 8);
            var ocorrencia = linha.Substring(230, 10).Trim();

            var valor = decimal.Zero;
            if (long.TryParse(valorStr, out var centavos))
                valor = centavos / 100m;

            DateTime? dtPgto = null;
            if (dataPgtoStr != "00000000" &&
                DateTime.TryParseExact(dataPgtoStr, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                dtPgto = dt;

            var statusOcorrencia = string.IsNullOrWhiteSpace(ocorrencia) || ocorrencia == "BD" ? "Aceito" : "Rejeitado";
            if (statusOcorrencia == "Aceito") aceitos++; else rejeitados++;
            processados++;

            int? remessaItemId = null;
            if (!string.IsNullOrWhiteSpace(seuNumero))
            {
                remessaItemId = await ctx.RemessasCnabItens
                    .Where(i => i.SeuNumero == seuNumero)
                    .Select(i => (int?)i.Id)
                    .FirstOrDefaultAsync();
            }

            retorno.Itens.Add(new RetornoCnabItem
            {
                RemessaCnabItemId = remessaItemId,
                SeuNumero = seuNumero,
                NomeFavorecido = nome,
                ValorPagamento = valor,
                DataPagamento = dtPgto,
                CodigoOcorrencia = ocorrencia,
                MensagemOcorrencia = statusOcorrencia == "Rejeitado"
                    ? $"Ocorrência: {ocorrencia}"
                    : null,
                StatusProcessamento = statusOcorrencia
            });
        }

        retorno.QuantidadeItensProcessados = processados;
        ctx.RetornosCnab.Add(retorno);
        await ctx.SaveChangesAsync();
        await tx.CommitAsync();

        msgs.Add($"Arquivo {nomeArquivo} importado.");
        msgs.Add($"{processados} pagamentos processados ({aceitos} aceitos, {rejeitados} rejeitados).");

        return new ResultadoImportacaoRetornoCnabDto
        {
            Sucesso = true,
            TotalRegistros = linhas.Count,
            TotalProcessados = processados,
            TotalAceitos = aceitos,
            TotalRejeitados = rejeitados,
            Mensagens = msgs,
            RetornoCnabId = retorno.Id
        };
    }

    public async Task AtualizarStatusPagamentoPeloRetornoAsync(int retornoId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        var retorno = await ctx.RetornosCnab
            .Include(r => r.Itens).ThenInclude(i => i.RemessaCnabItem!).ThenInclude(ri => ri.LancamentoFinanceiro)
            .FirstOrDefaultAsync(r => r.Id == retornoId)
            ?? throw new InvalidOperationException("Retorno não encontrado.");

        foreach (var ri in retorno.Itens.Where(i => i.RemessaCnabItem is not null))
        {
            var item = ri.RemessaCnabItem!;
            if (ri.StatusProcessamento == "Aceito")
            {
                item.Status = "Pago";
                if (item.LancamentoFinanceiro is not null)
                {
                    item.LancamentoFinanceiro.Status = "Pago";
                    item.LancamentoFinanceiro.DataPagamento = ri.DataPagamento ?? item.LancamentoFinanceiro.DataPagamento;
                }
            }
            else
            {
                item.Status = "Rejeitado";
                item.CodigoOcorrencia = ri.CodigoOcorrencia;
                item.MensagemOcorrencia = ri.MensagemOcorrencia;
            }
        }

        retorno.Status = "Processado";

        if (retorno.RemessaCnabId is int remId)
        {
            var rem = await ctx.RemessasCnab.FirstOrDefaultAsync(r => r.Id == remId);
            if (rem is not null) rem.Status = "Processada";
        }

        await ctx.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
