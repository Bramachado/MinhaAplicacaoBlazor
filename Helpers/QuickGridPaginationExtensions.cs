using Microsoft.AspNetCore.Components.QuickGrid;

namespace MinhaAplicacaoBlazor.Helpers;

/// <summary>
/// Utilitários de paginação para o QuickGrid.
/// </summary>
public static class QuickGridPaginationExtensions
{
    /// <summary>
    /// Devolve o <see cref="PaginationState"/> apenas quando a coleção tem MAIS
    /// registros que o tamanho da página; caso contrário devolve <c>null</c>.
    /// <para>
    /// Passar <c>null</c> como paginação faz o QuickGrid renderizar somente as
    /// linhas existentes, eliminando as linhas "fantasmas" que ele reserva
    /// quando há menos registros do que <see cref="PaginationState.ItemsPerPage"/>.
    /// </para>
    /// <para>
    /// O parâmetro é tratado como <see cref="IEnumerable{T}"/>, então a contagem
    /// usa LINQ-to-Objects (em memória) — não gera consulta adicional ao banco
    /// para as coleções já materializadas usadas nas telas.
    /// </para>
    /// </summary>
    public static PaginationState? QuandoNecessario<T>(this PaginationState pagination, IEnumerable<T>? items)
        => items is not null && items.Count() > pagination.ItemsPerPage
            ? pagination
            : null;
}
