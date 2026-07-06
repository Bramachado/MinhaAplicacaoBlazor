namespace MinhaAplicacaoBlazor.CnabBtg.Payments;

/// <summary>
/// Entrada de pagamento para o gerador CNAB, independente da origem dos dados
/// (folhas, lançamentos, importação). O gerador nunca inventa dados: o que não
/// vier preenchido é validado/rejeitado, nunca fabricado.
/// </summary>
public sealed class PaymentInput
{
    /// <summary>Id de origem (para reconciliação/auditoria). Não precisa ser único global.</summary>
    public int Id { get; set; }

    /// <summary>Origem lógica: "Colaborador", "Tutor", "Fornecedor", "Lancamento".</summary>
    public string Origem { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public decimal Valor { get; set; }

    /// <summary>Forma de lançamento CNAB (código FEBRABAN: 01, 03, 05, 41, 43, 45). Pode vir textual (PIX/TED).</summary>
    public string? Forma { get; set; }

    /// <summary>"Fisica" / "Juridica" (ou vazio para inferir por tamanho do documento).</summary>
    public string? TipoPessoa { get; set; }

    public string? NomeTitular { get; set; }
    public string? ChavePix { get; set; }

    /// <summary>Tipo da chave PIX quando informado explicitamente: Telefone/Email/CpfCnpj/Aleatoria. Vazio = inferir.</summary>
    public string? TipoChavePix { get; set; }

    public string? BancoNome { get; set; }
    public string? CodigoBanco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? DigitoConta { get; set; }

    /// <summary>"Corrente" / "Poupanca" quando aplicável.</summary>
    public string? TipoConta { get; set; }

    public DateTime? DataPagamento { get; set; }
    public string? Observacao { get; set; }
}
