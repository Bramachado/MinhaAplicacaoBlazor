using System.IO.Compression;
using System.Text;
using MinhaAplicacaoBlazor.CnabBtg.Generation;

namespace MinhaAplicacaoBlazor.CnabBtg.Audit;

/// <summary>
/// Empacota os arquivos .rem e as auditorias (JSON/CSV) em um único .zip.
/// Os .rem são gravados em ASCII sem BOM; as auditorias em UTF-8 sem BOM.
/// </summary>
public static class CnabZipPacker
{
    private static readonly UTF8Encoding Utf8SemBom = new(encoderShouldEmitUTF8Identifier: false);

    public static byte[] Empacotar(CnabGenerationResult resultado, string auditoriaJson, string auditoriaCsv, string nomeBase)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var arquivo in resultado.Arquivos)
                GravarEntrada(zip, arquivo.FileName, RemBytes(arquivo.Conteudo));

            GravarEntrada(zip, $"{nomeBase}_auditoria.json", Utf8SemBom.GetBytes(auditoriaJson));
            GravarEntrada(zip, $"{nomeBase}_auditoria.csv", Utf8SemBom.GetBytes(auditoriaCsv));
        }
        return ms.ToArray();
    }

    /// <summary>Bytes do .rem em ASCII, sem BOM (caracteres fora de ASCII já foram removidos na normalização).</summary>
    public static byte[] RemBytes(string conteudo) => Encoding.ASCII.GetBytes(conteudo);

    private static void GravarEntrada(ZipArchive zip, string nome, byte[] conteudo)
    {
        var entrada = zip.CreateEntry(nome, CompressionLevel.Optimal);
        using var stream = entrada.Open();
        stream.Write(conteudo, 0, conteudo.Length);
    }
}
