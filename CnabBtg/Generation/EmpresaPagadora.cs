namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Dados fixos da empresa pagadora (conta debitada) usados nos Headers de Arquivo
/// e de Lote. Configurável por appsettings (seção "CnabBtg:Empresas") ou pelos
/// valores padrão de <see cref="Padrao"/>. Nunca inventar estes dados: divergências
/// devem ser registradas na auditoria.
/// </summary>
public sealed class EmpresaPagadora
{
    /// <summary>Chave lógica: EDUNORTE, FADUC.</summary>
    public string Codigo { get; set; } = string.Empty;

    public string RazaoSocial { get; set; } = string.Empty;

    /// <summary>Banco pagador (208 = BTG).</summary>
    public string Banco { get; set; } = "208";

    public string NomeBanco { get; set; } = "BANCO BTG PACTUAL";

    /// <summary>1 = CPF, 2 = CNPJ.</summary>
    public string TipoInscricao { get; set; } = "2";

    public string Inscricao { get; set; } = string.Empty; // CNPJ só dígitos

    public string Agencia { get; set; } = string.Empty;
    public string AgenciaDv { get; set; } = string.Empty;
    public string Conta { get; set; } = string.Empty;
    public string ContaDv { get; set; } = string.Empty;
    public string Convenio { get; set; } = string.Empty;

    /// <summary>1 = Remessa.</summary>
    public string CodigoRemessa { get; set; } = "1";

    public string VersaoLayoutArquivo { get; set; } = "103";
    public string VersaoLayoutLote { get; set; } = "046";

    /// <summary>Empresas fixas conforme especificação BTG (fallback quando não há config em appsettings).</summary>
    public static IReadOnlyList<EmpresaPagadora> Padrao { get; } = new List<EmpresaPagadora>
    {
        new()
        {
            Codigo = "EDUNORTE",
            RazaoSocial = "EDUNORTE EDUCACAO E SERVICOS LTDA",
            Banco = "208", NomeBanco = "BANCO BTG PACTUAL",
            TipoInscricao = "2", Inscricao = "58262834000155",
            Agencia = "0050", AgenciaDv = "",
            Conta = "829434", ContaDv = "3",
            Convenio = "1778341975974",
            CodigoRemessa = "1", VersaoLayoutArquivo = "103", VersaoLayoutLote = "046"
        },
        new()
        {
            Codigo = "FADUC",
            RazaoSocial = "CARAJAS CENTRO DE ENSINO SUPER",
            Banco = "208", NomeBanco = "BANCO BTG PACTUAL",
            TipoInscricao = "2", Inscricao = "48132114000111",
            Agencia = "0050", AgenciaDv = "",
            Conta = "534630", ContaDv = "2",
            Convenio = "1778800268932",
            CodigoRemessa = "1", VersaoLayoutArquivo = "103", VersaoLayoutLote = "046"
        }
    };

    public static EmpresaPagadora? Obter(string? codigo, IEnumerable<EmpresaPagadora>? configuradas = null)
    {
        var fonte = configuradas is not null && configuradas.Any() ? configuradas : Padrao;
        return fonte.FirstOrDefault(e =>
            string.Equals(e.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
    }
}
