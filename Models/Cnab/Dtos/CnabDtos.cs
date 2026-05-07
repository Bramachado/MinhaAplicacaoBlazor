namespace MinhaAplicacaoBlazor.Models.Cnab.Dtos;

public class CriarRemessaCnabRequest
{
    public int ConfiguracaoCnabId { get; set; }
    public int? CompetenciaId { get; set; }
    public DateTime DataPagamento { get; set; }
    public int FormaLancamentoCnabId { get; set; }
    public string Origem { get; set; } = "LancamentoFinanceiro";
    public List<CriarRemessaCnabItemRequest> ItensSelecionados { get; set; } = new();
    public string? Observacao { get; set; }
}

public class CriarRemessaCnabItemRequest
{
    public int? LancamentoFinanceiroId { get; set; }
    public int? FolhaColaboradorItemId { get; set; }
    public int? FolhaTutorItemId { get; set; }
    public int? FornecedorId { get; set; }
    public string NomeFavorecido { get; set; } = string.Empty;
    public string? CPF_CNPJ_Favorecido { get; set; }
    public string? BancoFavorecido { get; set; }
    public string? AgenciaFavorecido { get; set; }
    public string? AgenciaDVFavorecido { get; set; }
    public string? ContaFavorecido { get; set; }
    public string? ContaDVFavorecido { get; set; }
    public string? ChavePix { get; set; }
    public string? TipoChavePix { get; set; }
    public string? CodigoBarras { get; set; }
    public decimal ValorPagamento { get; set; }
}

public class PagamentoPendenteCnabDto
{
    public string Origem { get; set; } = string.Empty;
    public int OrigemId { get; set; }
    public string NomeFavorecido { get; set; } = string.Empty;
    public string? CPF_CNPJ_Favorecido { get; set; }
    public string? Banco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? ChavePix { get; set; }
    public decimal Valor { get; set; }
    public DateTime? DataVencimento { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Selecionado { get; set; }
    public List<string> ErrosValidacao { get; set; } = new();
    public bool JaEmRemessa { get; set; }
}

public class ValidacaoCnabResultadoDto
{
    public bool Valido { get; set; }
    public List<string> ErrosGerais { get; set; } = new();
    public List<ItemValidacaoDto> ItensComErro { get; set; } = new();
    public int QuantidadeValidada { get; set; }
    public decimal ValorTotal { get; set; }
}

public class ItemValidacaoDto
{
    public int Indice { get; set; }
    public string NomeFavorecido { get; set; } = string.Empty;
    public List<string> Erros { get; set; } = new();
}

public class RemessaCnabResumoDto
{
    public int Id { get; set; }
    public int NumeroSequencial { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataGeracao { get; set; }
    public string? Competencia { get; set; }
    public int QuantidadePagamentos { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RemessaCnabItemDto
{
    public int Id { get; set; }
    public string NomeFavorecido { get; set; } = string.Empty;
    public string? CPF_CNPJ_Favorecido { get; set; }
    public string? FormaLancamento { get; set; }
    public decimal ValorPagamento { get; set; }
    public DateTime DataPagamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CodigoOcorrencia { get; set; }
    public string? MensagemOcorrencia { get; set; }
}

public class ResultadoImportacaoRetornoCnabDto
{
    public bool Sucesso { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalProcessados { get; set; }
    public int TotalAceitos { get; set; }
    public int TotalRejeitados { get; set; }
    public List<string> Mensagens { get; set; } = new();
    public int? RetornoCnabId { get; set; }
}
