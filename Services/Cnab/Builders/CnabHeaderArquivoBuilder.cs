using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabHeaderArquivoBuilder
{
    public static string Build(ConfiguracaoCnab cfg, RemessaCnab remessa)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));     // 001-003
        sb.Append(Cnab240Formatter.Num("0", 4));                 // 004-007 lote 0000
        sb.Append('0');                                          // 008
        sb.Append(Cnab240Formatter.Brancos(9));                  // 009-017
        sb.Append('2');                                          // 018 CNPJ
        sb.Append(Cnab240Formatter.Num(cfg.CNPJ, 14));           // 019-032
        sb.Append(Cnab240Formatter.Alfa(cfg.Convenio, 20));      // 033-052
        sb.Append(Cnab240Formatter.Num(cfg.Agencia, 5));         // 053-057
        sb.Append(Cnab240Formatter.Alfa(cfg.AgenciaDV, 1));      // 058
        sb.Append(Cnab240Formatter.Num(cfg.Conta, 12));          // 059-070
        sb.Append(Cnab240Formatter.Alfa(cfg.ContaDV, 1));        // 071
        sb.Append(' ');                                          // 072 DV ag/conta
        sb.Append(Cnab240Formatter.Alfa(cfg.RazaoSocial, 30));   // 073-102
        sb.Append(Cnab240Formatter.Alfa(cfg.BancoNome, 30));     // 103-132
        sb.Append(Cnab240Formatter.Brancos(10));                 // 133-142
        sb.Append('1');                                          // 143 remessa
        sb.Append(Cnab240Formatter.Data(remessa.DataGeracao));   // 144-151
        sb.Append(Cnab240Formatter.Hora(remessa.DataGeracao));   // 152-157
        sb.Append(Cnab240Formatter.Num(remessa.NumeroSequencial, 6)); // 158-163
        sb.Append(Cnab240Formatter.Num(cfg.VersaoLayoutArquivo, 3));  // 164-166
        sb.Append(Cnab240Formatter.Num("0", 5));                 // 167-171 densidade
        sb.Append(Cnab240Formatter.Brancos(20));                 // 172-191 reservado banco
        sb.Append(Cnab240Formatter.Brancos(20));                 // 192-211 reservado empresa
        sb.Append(Cnab240Formatter.Brancos(29));                 // 212-240
        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, "Header de Arquivo");
        return linha;
    }
}
