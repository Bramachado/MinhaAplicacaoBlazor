using System.Text;
using MinhaAplicacaoBlazor.Models.Cnab;

namespace MinhaAplicacaoBlazor.Services.Cnab;

public static class CnabHeaderLoteBuilder
{
    public static string Build(ConfiguracaoCnab cfg, RemessaCnab remessa, FormaLancamentoCnab forma)
    {
        var sb = new StringBuilder(240);
        sb.Append(Cnab240Formatter.Num(cfg.BancoCodigo, 3));     // 001-003
        sb.Append(Cnab240Formatter.Num("1", 4));                 // 004-007 lote 0001
        sb.Append('1');                                          // 008
        sb.Append('C');                                          // 009 operação Crédito
        sb.Append(Cnab240Formatter.Num("20", 2));                // 010-011 serviço 20=Pgto Fornecedor
        sb.Append(Cnab240Formatter.Num(forma.Codigo, 2));        // 012-013 forma de lançamento
        sb.Append(Cnab240Formatter.Num(cfg.VersaoLayoutLote, 3));// 014-016
        sb.Append(' ');                                          // 017
        sb.Append('2');                                          // 018 CNPJ
        sb.Append(Cnab240Formatter.Num(cfg.CNPJ, 14));           // 019-032
        sb.Append(Cnab240Formatter.Alfa(cfg.Convenio, 20));      // 033-052
        sb.Append(Cnab240Formatter.Num(cfg.Agencia, 5));         // 053-057
        sb.Append(Cnab240Formatter.Alfa(cfg.AgenciaDV, 1));      // 058
        sb.Append(Cnab240Formatter.Num(cfg.Conta, 12));          // 059-070
        sb.Append(Cnab240Formatter.Alfa(cfg.ContaDV, 1));        // 071
        sb.Append(' ');                                          // 072
        sb.Append(Cnab240Formatter.Alfa(cfg.RazaoSocial, 30));   // 073-102
        sb.Append(Cnab240Formatter.Brancos(40));                 // 103-142 mensagem 1
        sb.Append(Cnab240Formatter.Brancos(30));                 // 143-172 endereço
        sb.Append(Cnab240Formatter.Num("0", 5));                 // 173-177 número
        sb.Append(Cnab240Formatter.Brancos(15));                 // 178-192 complemento
        sb.Append(Cnab240Formatter.Brancos(20));                 // 193-212 cidade
        sb.Append(Cnab240Formatter.Num("0", 8));                 // 213-220 cep
        sb.Append(Cnab240Formatter.Brancos(2));                  // 221-222 uf
        sb.Append(Cnab240Formatter.Brancos(8));                  // 223-230
        sb.Append(Cnab240Formatter.Brancos(10));                 // 231-240 reservado banco
        var linha = sb.ToString();
        Cnab240Formatter.ValidarLinha240(linha, "Header de Lote");
        return linha;
    }
}
