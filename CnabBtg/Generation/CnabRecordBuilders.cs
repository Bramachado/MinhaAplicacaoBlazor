using MinhaAplicacaoBlazor.CnabBtg.Payments;

namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Montagem dos registros FEBRABAN 240 do BTG (208) por posição fixa, para
/// pagamentos via Segmentos A e B. Posições conforme especificação do layout.
/// </summary>
public static class CnabRecordBuilders
{
    // Serviço de pagamento e operação padrão do Header de Lote.
    private const string TipoServicoPagamento = "20";
    private const string OperacaoLote = "C";

    public static string HeaderArquivo(CnabGenerationOptions o, int nsa)
    {
        var e = o.Empresa;
        return new Cnab240LineBuilder()
            .Num(1, 3, e.Banco)
            .Num(4, 4, "0")                       // lote 0000
            .Num(8, 1, "0")                       // tipo registro
            .Brancos(9, 9)
            .Num(18, 1, e.TipoInscricao)
            .Num(19, 14, e.Inscricao)
            .Alfa(33, 20, o.ConvenioEfetivo)
            .Num(53, 5, e.Agencia)
            .Alfa(58, 1, e.AgenciaDv)
            .Num(59, 12, e.Conta)
            .Alfa(71, 1, e.ContaDv)
            .Brancos(72, 1)                        // DV Ag/Conta
            .Alfa(73, 30, e.RazaoSocial)
            .Alfa(103, 30, e.NomeBanco)
            .Brancos(133, 10)
            .Num(143, 1, e.CodigoRemessa)
            .Num(144, 8, o.GeradoEm.ToString("ddMMyyyy"))
            .Num(152, 6, o.GeradoEm.ToString("HHmmss"))
            .Num(158, 6, nsa)
            .Num(164, 3, e.VersaoLayoutArquivo)
            .Zeros(167, 5)                         // densidade
            .Brancos(172, 69)                      // reservado banco/empresa/FEBRABAN
            .Build();
    }

    public static string HeaderLote(CnabGenerationOptions o, int lote, string forma)
    {
        var e = o.Empresa;
        return new Cnab240LineBuilder()
            .Num(1, 3, e.Banco)
            .Num(4, 4, lote)
            .Num(8, 1, "1")                        // tipo registro
            .Alfa(9, 1, OperacaoLote)
            .Num(10, 2, TipoServicoPagamento)
            .Num(12, 2, forma)
            .Num(14, 3, e.VersaoLayoutLote)
            .Brancos(17, 1)
            .Num(18, 1, e.TipoInscricao)
            .Num(19, 14, e.Inscricao)
            .Alfa(33, 20, o.ConvenioEfetivo)
            .Num(53, 5, e.Agencia)
            .Alfa(58, 1, e.AgenciaDv)
            .Num(59, 12, e.Conta)
            .Alfa(71, 1, e.ContaDv)
            .Brancos(72, 1)
            .Alfa(73, 30, e.RazaoSocial)
            .Brancos(103, 40)                      // mensagem
            .Brancos(143, 80)                      // endereço da empresa (não utilizado)
            .Num(223, 2, FormaLancamento.ExigeContaBancaria(forma) ? "01" : "00") // indicativo forma pagto
            .Brancos(225, 16)
            .Build();
    }

    public static string SegmentoA(CnabGenerationOptions o, NormalizedPayment p, int lote, int seqNoLote, string seuNumero)
    {
        return new Cnab240LineBuilder()
            .Num(1, 3, o.Empresa.Banco)
            .Num(4, 4, lote)
            .Num(8, 1, "3")                        // tipo registro
            .Num(9, 5, seqNoLote)
            .Alfa(14, 1, "A")
            .Num(15, 1, "0")                       // tipo movimento
            .Num(16, 2, "00")                      // código instrução
            .Num(18, 3, FormaLancamento.Camara(p.Forma))
            .Num(21, 3, FormaLancamento.EhPix(p.Forma) ? "0" : p.CodigoBanco)
            .Num(24, 5, p.Agencia)
            .Alfa(29, 1, p.AgenciaDv)
            .Num(30, 12, p.Conta)
            .Alfa(42, 1, p.ContaDv)
            .Brancos(43, 1)                        // DV Ag/Conta
            .Alfa(44, 30, p.Nome)
            .Alfa(74, 20, seuNumero)
            .Num(94, 8, p.DataPagamento.ToString("ddMMyyyy"))
            .Alfa(102, 3, "BRL")
            .Zeros(105, 15)                        // quantidade moeda
            .Valor(120, 15, p.Valor)
            .Brancos(135, 20)                      // nosso número
            .Zeros(155, 8)                         // data real
            .Zeros(163, 15)                        // valor real
            .Brancos(178, 40)                      // informação 2
            .Brancos(218, 12)
            .Num(230, 1, "0")                      // aviso
            .Brancos(231, 10)                      // ocorrências
            .Build();
    }

    public static string SegmentoB(CnabGenerationOptions o, NormalizedPayment p, int lote, int seqNoLote)
    {
        var b = new Cnab240LineBuilder()
            .Num(1, 3, o.Empresa.Banco)
            .Num(4, 4, lote)
            .Num(8, 1, "3")
            .Num(9, 5, seqNoLote)
            .Alfa(14, 1, "B")
            .Alfa(15, 3, FormaLancamento.EhPix(p.Forma) ? (p.FormaIniciacaoPix ?? "004") : "   ")
            .Num(18, 1, p.TipoInscricao)
            .Num(19, 14, p.CpfCnpj);

        if (FormaLancamento.EhPix(p.Forma))
        {
            b.Alfa(33, 35, p.ChavePix)             // Informação 10 = chave PIX
             .Brancos(68, 60)                      // Informação 11
             .Brancos(128, 99);                    // Informação 12
        }
        else
        {
            b.Brancos(33, 35)
             .Brancos(68, 60)
             .Brancos(128, 99);
        }

        return b
            .Zeros(227, 6)                         // código UG
            .Brancos(233, 8)                       // ISPB
            .Build();
    }

    public static string TrailerLote(CnabGenerationOptions o, int lote, int quantidadeRegistrosLote, decimal somaValores)
    {
        return new Cnab240LineBuilder()
            .Num(1, 3, o.Empresa.Banco)
            .Num(4, 4, lote)
            .Num(8, 1, "5")                        // tipo registro
            .Brancos(9, 9)
            .Num(18, 6, quantidadeRegistrosLote)
            .Valor(24, 18, somaValores)
            .Zeros(42, 18)                         // somatória quantidade moeda
            .Zeros(60, 6)                          // número aviso débito
            .Brancos(66, 165)
            .Brancos(231, 10)                      // ocorrências
            .Build();
    }

    public static string TrailerArquivo(CnabGenerationOptions o, int quantidadeLotes, int quantidadeRegistrosArquivo)
    {
        return new Cnab240LineBuilder()
            .Num(1, 3, o.Empresa.Banco)
            .Num(4, 4, "9999")
            .Num(8, 1, "9")                        // tipo registro
            .Brancos(9, 9)
            .Num(18, 6, quantidadeLotes)
            .Num(24, 6, quantidadeRegistrosArquivo)
            .Zeros(30, 6)                          // contas para conciliação
            .Brancos(36, 205)
            .Build();
    }
}
