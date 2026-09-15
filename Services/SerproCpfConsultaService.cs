using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace MinhaAplicacaoBlazor.Services;

/// <summary>Resultado de uma consulta de CPF na base da SERPRO.</summary>
public record ConsultaCpfResultado
{
    public bool Sucesso { get; init; }

    /// <summary>Preenchido quando <see cref="Sucesso"/> é true.</summary>
    public string? Nome { get; init; }

    /// <summary>Ex.: "REGULAR", "SUSPENSA", "CANCELADA" etc. (campo "situacao.descricao").</summary>
    public string? SituacaoCadastral { get; init; }

    public DateTime? DataNascimento { get; init; }

    /// <summary>Data de inscrição do CPF na Receita Federal (campo "dataInscricao").</summary>
    public DateTime? DataInscricao { get; init; }

    /// <summary>JSON bruto retornado pela API, para os campos que ainda não são
    /// mapeados explicitamente acima.</summary>
    public string? RespostaBruta { get; init; }

    /// <summary>Preenchido quando <see cref="Sucesso"/> é false (erro HTTP, CPF não
    /// encontrado, assinatura da API ausente etc.).</summary>
    public string? MensagemErro { get; init; }
}

/// <summary>
/// Consulta dados cadastrais de um CPF na API "Consulta CPF" da SERPRO
/// (https://gateway.apiserpro.serpro.gov.br, produto consulta-cpf-df/v3 — não a
/// versão trial), usada para validar se os dados informados no cadastro (CPF +
/// data de nascimento) correspondem a uma pessoa real antes de salvar. Autentica
/// via OAuth2 client_credentials e reaproveita o token entre chamadas (cache em
/// <see cref="SerproTokenCache"/>).
/// </summary>
public class SerproCpfConsultaService
{
    private readonly HttpClient _http;
    private readonly SerproTokenCache _tokenCache;
    private readonly string _consumerKey;
    private readonly string _consumerSecret;
    private readonly string _tokenPath;
    private readonly string _consultaCpfPath;

    public SerproCpfConsultaService(HttpClient http, SerproTokenCache tokenCache, IConfiguration config)
    {
        _http = http;
        _tokenCache = tokenCache;

        _http.BaseAddress ??= new Uri(config["SerproApi:BaseUrl"] ?? "https://gateway.apiserpro.serpro.gov.br");
        _consumerKey = config["SerproApi:ConsumerKey"] ?? string.Empty;
        _consumerSecret = config["SerproApi:ConsumerSecret"] ?? string.Empty;
        _tokenPath = config["SerproApi:TokenPath"] ?? "token";
        _consultaCpfPath = config["SerproApi:ConsultaCpfPath"] ?? "consulta-cpf-df/v3/cpf";
    }

    /// <summary>
    /// Compara o nome informado no cadastro com o nome retornado pela SERPRO,
    /// ignorando maiúsculas/minúsculas, acentos e espaços redundantes.
    /// </summary>
    public static bool NomesCorrespondem(string? nomeInformado, string? nomeApi)
    {
        if (string.IsNullOrWhiteSpace(nomeInformado) || string.IsNullOrWhiteSpace(nomeApi))
            return false;

        return NormalizarNome(nomeInformado) == NormalizarNome(nomeApi);
    }

    private static string NormalizarNome(string nome)
    {
        var semAcentos = nome.Normalize(NormalizationForm.FormD)
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray();

        var normalizado = new string(semAcentos).Normalize(NormalizationForm.FormC);
        var partes = normalizado.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return string.Join(' ', partes).ToUpperInvariant();
    }

    /// <summary>Consulta o CPF (11 dígitos) e a data de nascimento informados.</summary>
    public async Task<ConsultaCpfResultado> ConsultarAsync(string cpf, DateTime dataNascimento, CancellationToken ct = default)
    {
        var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpfLimpo.Length != 11)
        {
            return new ConsultaCpfResultado { Sucesso = false, MensagemErro = "CPF inválido para consulta (esperado 11 dígitos)." };
        }

        var dataFormatada = dataNascimento.ToString("ddMMyyyy");

        var resultado = await ExecutarConsultaAsync(cpfLimpo, dataFormatada, tentarNovoToken: false, ct);

        // Token pode ter expirado entre a leitura do cache e a chamada; tenta uma vez mais com token novo.
        if (!resultado.Sucesso && resultado.MensagemErro == MensagemTokenExpirado)
        {
            resultado = await ExecutarConsultaAsync(cpfLimpo, dataFormatada, tentarNovoToken: true, ct);
        }

        return resultado;
    }

    private const string MensagemTokenExpirado = "__token_expirado__";

    private async Task<ConsultaCpfResultado> ExecutarConsultaAsync(string cpf, string dataFormatada, bool tentarNovoToken, CancellationToken ct)
    {
        string token;
        try
        {
            token = await _tokenCache.ObterAsync(ObterNovoTokenAsync, forcarRenovacao: tentarNovoToken, ct);
        }
        catch (Exception ex)
        {
            return new ConsultaCpfResultado { Sucesso = false, MensagemErro = $"Falha ao autenticar na SERPRO: {ex.Message}" };
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_consultaCpfPath}/{cpf}/{dataFormatada}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, ct);
        }
        catch (Exception ex)
        {
            return new ConsultaCpfResultado { Sucesso = false, MensagemErro = $"Falha ao consultar a SERPRO: {ex.Message}" };
        }

        var corpo = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !tentarNovoToken)
        {
            return new ConsultaCpfResultado { Sucesso = false, MensagemErro = MensagemTokenExpirado };
        }

        if (!response.IsSuccessStatusCode)
        {
            return new ConsultaCpfResultado { Sucesso = false, MensagemErro = InterpretarErro(response.StatusCode, corpo) };
        }

        return InterpretarSucesso(corpo);
    }

    private static string InterpretarErro(System.Net.HttpStatusCode status, string corpo)
    {
        try
        {
            using var doc = JsonDocument.Parse(corpo);
            var raiz = doc.RootElement;
            var descricao = raiz.TryGetProperty("description", out var d) ? d.GetString()
                : raiz.TryGetProperty("message", out var m) ? m.GetString()
                : null;

            if (!string.IsNullOrWhiteSpace(descricao))
                return $"SERPRO ({(int)status}): {descricao}";
        }
        catch (JsonException)
        {
            // corpo não é JSON — cai no retorno genérico abaixo.
        }

        return $"SERPRO retornou erro {(int)status}.";
    }

    /// <remarks>
    /// Schema confirmado contra a API real (consulta-cpf-df/v3, não-trial):
    /// {"ni":"...","nome":"...","situacao":{"codigo":"0","descricao":"REGULAR"},
    ///  "nascimento":"ddMMyyyy","dataInscricao":"ddMMyyyy"}.
    /// </remarks>
    private static ConsultaCpfResultado InterpretarSucesso(string corpo)
    {
        string? nome = null;
        string? situacao = null;
        DateTime? nascimento = null;
        DateTime? dataInscricao = null;

        try
        {
            using var doc = JsonDocument.Parse(corpo);
            var raiz = doc.RootElement;

            if (raiz.TryGetProperty("nome", out var nomeEl))
                nome = nomeEl.GetString();

            if (raiz.TryGetProperty("situacao", out var situacaoEl) &&
                situacaoEl.ValueKind == JsonValueKind.Object &&
                situacaoEl.TryGetProperty("descricao", out var descEl))
            {
                situacao = descEl.GetString();
            }

            if (raiz.TryGetProperty("nascimento", out var nascEl) &&
                DateTime.TryParseExact(nascEl.GetString(), "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var nascDt))
            {
                nascimento = nascDt;
            }

            if (raiz.TryGetProperty("dataInscricao", out var inscEl) &&
                DateTime.TryParseExact(inscEl.GetString(), "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out var inscDt))
            {
                dataInscricao = inscDt;
            }
        }
        catch (JsonException)
        {
            // Mantém os campos nulos; RespostaBruta guarda o corpo para inspeção.
        }

        return new ConsultaCpfResultado
        {
            Sucesso = true,
            Nome = nome,
            SituacaoCadastral = situacao,
            DataNascimento = nascimento,
            DataInscricao = dataInscricao,
            RespostaBruta = corpo,
        };
    }

    private async Task<(string Token, int ExpiraEmSegundos)> ObterNovoTokenAsync(CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _tokenPath)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
            }),
        };

        var credenciais = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_consumerKey}:{_consumerSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credenciais);

        var response = await _http.SendAsync(request, ct);
        var corpo = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"SERPRO recusou a autenticação ({(int)response.StatusCode}): {corpo}");

        var token = JsonSerializer.Deserialize<SerproTokenResponse>(corpo)
            ?? throw new InvalidOperationException("Resposta de token da SERPRO vazia ou inválida.");

        return (token.AccessToken, token.ExpiresIn);
    }

    private sealed class SerproTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}

/// <summary>
/// Cache em memória (singleton) do token OAuth2 da SERPRO, compartilhado entre
/// todas as requisições para evitar gerar um token novo a cada consulta de CPF.
/// </summary>
public class SerproTokenCache
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _token;
    private DateTimeOffset _expiraEm = DateTimeOffset.MinValue;

    public async Task<string> ObterAsync(
        Func<CancellationToken, Task<(string Token, int ExpiraEmSegundos)>> obterNovoToken,
        bool forcarRenovacao,
        CancellationToken ct)
    {
        if (!forcarRenovacao && _token is not null && DateTimeOffset.UtcNow < _expiraEm)
            return _token;

        await _lock.WaitAsync(ct);
        try
        {
            if (!forcarRenovacao && _token is not null && DateTimeOffset.UtcNow < _expiraEm)
                return _token;

            var (token, expiraEmSegundos) = await obterNovoToken(ct);
            _token = token;
            // Margem de segurança de 30s para não usar um token na borda da expiração.
            _expiraEm = DateTimeOffset.UtcNow.AddSeconds(Math.Max(0, expiraEmSegundos - 30));
            return _token;
        }
        finally
        {
            _lock.Release();
        }
    }
}
