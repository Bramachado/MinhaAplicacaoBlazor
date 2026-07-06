using System.Text;

namespace MinhaAplicacaoBlazor.CnabBtg.Generation;

/// <summary>
/// Montador de uma linha CNAB de exatamente 240 posições, por posição fixa
/// (1-based, como na documentação FEBRABAN). Numéricos alinhados à direita com
/// zeros à esquerda; alfanuméricos alinhados à esquerda com espaços à direita.
/// Lança <see cref="CnabLayoutException"/> quando um campo obrigatório excede o
/// tamanho sem regra de truncamento.
/// </summary>
public sealed class Cnab240LineBuilder
{
    private readonly char[] _buffer;

    public Cnab240LineBuilder(int tamanho = 240)
    {
        Tamanho = tamanho;
        _buffer = new char[tamanho];
        Array.Fill(_buffer, ' ');
    }

    public int Tamanho { get; }

    /// <summary>Campo numérico: só dígitos, alinhado à direita com zeros à esquerda.</summary>
    public Cnab240LineBuilder Num(int posicaoInicial1Based, int tamanho, string? valor)
    {
        var digitos = CnabText.ApenasDigitos(valor);
        if (digitos.Length > tamanho)
            throw new CnabLayoutException(
                $"Campo numérico na posição {posicaoInicial1Based} excede {tamanho} dígitos: '{digitos}'.");

        var texto = digitos.PadLeft(tamanho, '0');
        Escrever(posicaoInicial1Based, tamanho, texto);
        return this;
    }

    /// <summary>Campo numérico a partir de um inteiro.</summary>
    public Cnab240LineBuilder Num(int posicaoInicial1Based, int tamanho, long valor)
        => Num(posicaoInicial1Based, tamanho, valor.ToString());

    /// <summary>
    /// Campo alfanumérico: normalizado (sem acento, caixa alta, ASCII), alinhado à
    /// esquerda com espaços à direita. Trunca à direita apenas quando
    /// <paramref name="permitirTruncar"/> (default true, pois nomes longos são
    /// truncados no CNAB); caso contrário lança exceção se exceder.
    /// </summary>
    public Cnab240LineBuilder Alfa(int posicaoInicial1Based, int tamanho, string? valor, bool permitirTruncar = true)
    {
        var texto = CnabText.NormalizarAlfa(valor);
        if (texto.Length > tamanho)
        {
            if (!permitirTruncar)
                throw new CnabLayoutException(
                    $"Campo alfanumérico na posição {posicaoInicial1Based} excede {tamanho} caracteres sem permissão de truncamento: '{texto}'.");
            texto = texto[..tamanho];
        }

        Escrever(posicaoInicial1Based, tamanho, texto.PadRight(tamanho, ' '));
        return this;
    }

    /// <summary>Valor monetário em centavos, alinhado à direita com zeros à esquerda.</summary>
    public Cnab240LineBuilder Valor(int posicaoInicial1Based, int tamanho, decimal valorReais)
    {
        var centavos = (long)Math.Round(valorReais * 100m, MidpointRounding.AwayFromZero);
        if (centavos < 0)
            throw new CnabLayoutException($"Valor negativo não permitido na posição {posicaoInicial1Based}: {valorReais}.");
        return Num(posicaoInicial1Based, tamanho, centavos);
    }

    /// <summary>Preenche a faixa com zeros.</summary>
    public Cnab240LineBuilder Zeros(int posicaoInicial1Based, int tamanho)
        => Num(posicaoInicial1Based, tamanho, 0);

    /// <summary>Preenche a faixa com brancos (espaços).</summary>
    public Cnab240LineBuilder Brancos(int posicaoInicial1Based, int tamanho)
    {
        Escrever(posicaoInicial1Based, tamanho, new string(' ', tamanho));
        return this;
    }

    private void Escrever(int posicaoInicial1Based, int tamanho, string texto)
    {
        var inicio0 = posicaoInicial1Based - 1;
        if (inicio0 < 0 || inicio0 + tamanho > Tamanho)
            throw new CnabLayoutException(
                $"Faixa {posicaoInicial1Based}..{posicaoInicial1Based + tamanho - 1} fora dos {Tamanho} caracteres da linha.");
        if (texto.Length != tamanho)
            throw new CnabLayoutException(
                $"Texto '{texto}' com {texto.Length} caracteres não cabe exatamente em {tamanho} posições.");

        for (var i = 0; i < tamanho; i++)
            _buffer[inicio0 + i] = texto[i];
    }

    public string Build()
    {
        var linha = new string(_buffer);
        if (linha.Length != Tamanho)
            throw new CnabLayoutException($"Linha gerada com {linha.Length} caracteres (esperado {Tamanho}).");
        return linha;
    }
}

/// <summary>Erro estrutural de montagem do CNAB (campo excedido, linha fora de 240, etc.).</summary>
public sealed class CnabLayoutException : Exception
{
    public CnabLayoutException(string message) : base(message) { }
}
