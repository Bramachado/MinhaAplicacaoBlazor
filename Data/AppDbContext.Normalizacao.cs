using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Helpers;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Data;

/// <summary>
/// Normalização automática de campos de nome antes da gravação. Roda no início
/// do SaveChanges (antes da captura de auditoria), então o valor persistido — e
/// registrado na auditoria — já sai em Title Case, independentemente de como o
/// usuário digitou (inclusive tudo em maiúsculas).
/// </summary>
public partial class AppDbContext
{
    private void NormalizarNomes()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            switch (entry.Entity)
            {
                case Tutor t:
                    t.Nome = TextoBr.ParaTitulo(t.Nome) ?? t.Nome;
                    break;
                case Colaborador c:
                    c.Nome = TextoBr.ParaTitulo(c.Nome) ?? c.Nome;
                    break;
                case Fornecedor f:
                    f.NomeRazaoSocial = TextoBr.ParaTitulo(f.NomeRazaoSocial) ?? f.NomeRazaoSocial;
                    // Tipo de pagamento derivado: com conta vinculada → Transferência, senão → Boleto.
                    f.TipoPagamento = f.ContaBancariaId.HasValue ? "Transferencia" : "Boleto";
                    break;
            }
        }
    }
}
