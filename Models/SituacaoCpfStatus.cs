namespace MinhaAplicacaoBlazor.Models;

/// <summary>
/// Resultado da comparação entre o Nome informado no cadastro e o nome retornado
/// pela consulta de CPF na SERPRO. Usado em <see cref="Tutor.SituacaoCpf"/> e
/// <see cref="Colaborador.SituacaoCpf"/>, calculado tanto ao salvar um registro
/// individualmente (FormTutor/FormColaborador) quanto na conferência em lote
/// (FolhaTutores2/FolhaColaboradores2).
/// </summary>
public enum SituacaoCpfStatus
{
    OK,
    DIVERGENTE
}
