using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabSegmentoBBuilder
{
    public static string Build(ConfiguracaoCnab cfg, FormaLancamentoCnab forma, RemessaCnabItem item, int seqLote)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));     // 001-003
        sb.Append(Cnab240Formatter.Num("1", 4));                 // 004-007
        sb.Append('3');                                          // 008
        sb.Append(Cnab240Formatter.Num(seqLote, 5));             // 009-013
        sb.Append('B');                                          // 014
        sb.Append(Cnab240Formatter.Brancos(3));                  // 015-017

        var tipoInsc = (item.CPF_CNPJ_Favorecido ?? "").Length > 11 ? "2" : "1";
        sb.Append(tipoInsc);                                     // 018
        sb.Append(Cnab240Formatter.Num(item.CPF_CNPJ_Favorecido, 14)); // 019-032

        if (forma.Codigo == "45")
        {
            // Layout PIX: chave PIX nos campos seguintes
            sb.Append(Cnab240Formatter.Alfa(item.ChavePix, 99));  // 033-131 chave PIX
            sb.Append(Cnab240Formatter.Brancos(6));               // 132-137
            sb.Append(Cnab240Formatter.Num("0", 8));              // 138-145 data vencimento
            sb.Append(Cnab240Formatter.Valor(item.ValorPagamento, 15)); // 146-160
            sb.Append(Cnab240Formatter.Num("0", 15));             // 161-175 abatimento
            sb.Append(Cnab240Formatter.Num("0", 15));             // 176-190 desconto
            sb.Append(Cnab240Formatter.Num("0", 15));             // 191-205 mora
            sb.Append(Cnab240Formatter.Num("0", 15));             // 206-220 multa
            sb.Append(Cnab240Formatter.Brancos(15));              // 221-235
            sb.Append(Cnab240Formatter.Brancos(5));               // 236-240
        }
        else
        {
            sb.Append(Cnab240Formatter.Alfa(string.Empty, 30));   // 033-062 logradouro
            sb.Append(Cnab240Formatter.Num("0", 5));              // 063-067 número
            sb.Append(Cnab240Formatter.Alfa(string.Empty, 15));   // 068-082 complemento
            sb.Append(Cnab240Formatter.Alfa(string.Empty, 15));   // 083-097 bairro
            sb.Append(Cnab240Formatter.Alfa(string.Empty, 20));   // 098-117 cidade
            sb.Append(Cnab240Formatter.Num("0", 5));              // 118-122 cep
            sb.Append(Cnab240Formatter.Alfa(string.Empty, 3));    // 123-125 compl cep
            sb.Append(Cnab240Formatter.Brancos(2));               // 126-127 uf
            sb.Append(Cnab240Formatter.Data(item.DataPagamento)); // 128-135 dt vencimento
            sb.Append(Cnab240Formatter.Valor(item.ValorPagamento, 15)); // 136-150
            sb.Append(Cnab240Formatter.Num("0", 15));             // 151-165 abatimento
            sb.Append(Cnab240Formatter.Num("0", 15));             // 166-180 desconto
            sb.Append(Cnab240Formatter.Num("0", 15));             // 181-195 mora
            sb.Append(Cnab240Formatter.Num("0", 15));             // 196-210 multa
            sb.Append(Cnab240Formatter.Alfa(string.Empty, 15));   // 211-225 cód doc favorecido
            sb.Append(Cnab240Formatter.Brancos(5));               // 226-230
            sb.Append(Cnab240Formatter.Brancos(10));              // 231-240
        }

        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, $"Segmento B item #{item.Id}");
        return linha;
    }
}
