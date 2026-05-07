using MinhaAplicacaoBlazor.Models.Cnab;
using MinhaAplicacaoBlazor.Models.Cnab.Dtos;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public interface ICnabPagamentoService
{
    Task<List<ConfiguracaoCnab>> ListarConfiguracoesAsync();
    Task<List<FormaLancamentoCnab>> ListarFormasLancamentoAsync();
    Task<List<PagamentoPendenteCnabDto>> ListarPagamentosPendentesAsync(int competenciaId, string origem);
    Task<ValidacaoCnabResultadoDto> ValidarRemessaAsync(CriarRemessaCnabRequest request);
    Task<RemessaCnab> CriarRemessaAsync(CriarRemessaCnabRequest request);
    Task<string> GerarArquivoRemessaAsync(int remessaId);
    Task<byte[]> BaixarArquivoRemessaAsync(int remessaId);
    Task CancelarRemessaAsync(int remessaId);
    Task MarcarComoEnviadaAsync(int remessaId);
    Task<ResultadoImportacaoRetornoCnabDto> ImportarRetornoAsync(Stream arquivo, string nomeArquivo, int? remessaCnabId);
    Task AtualizarStatusPagamentoPeloRetornoAsync(int retornoId);
}
