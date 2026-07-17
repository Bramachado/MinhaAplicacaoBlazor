namespace MinhaAplicacaoBlazor.Models;

/// <summary>
/// Marca uma entidade como pertencente a uma empresa (tenant). O <see cref="EmpresaId"/>
/// é carimbado automaticamente na gravação e filtrado automaticamente em toda consulta
/// (ver AppDbContext: carimbo no SaveChanges e HasQueryFilter no OnModelCreating).
/// </summary>
public interface IEntidadeEmpresa
{
    int EmpresaId { get; set; }
}
