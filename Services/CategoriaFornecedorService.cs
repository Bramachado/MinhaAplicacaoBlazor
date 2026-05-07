using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;

namespace MinhaAplicacaoBlazor.Services;

public class CategoriaFornecedorService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public CategoriaFornecedorService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<CategoriaFornecedor>> ListarAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.CategoriasFornecedores
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<List<CategoriaFornecedor>> ListarAtivasAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.CategoriasFornecedores
            .AsNoTracking()
            .Where(c => c.Ativa)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<CategoriaFornecedor?> ObterPorIdAsync(int id)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.CategoriasFornecedores
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<CategoriaFornecedor> CriarAsync(CategoriaFornecedor categoria)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        categoria.DataCadastro = DateTime.Now;
        categoria.DataAtualizacao = null;

        context.CategoriasFornecedores.Add(categoria);
        await context.SaveChangesAsync();

        return categoria;
    }

    public async Task AtualizarAsync(CategoriaFornecedor categoria)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var existente = await context.CategoriasFornecedores
            .FirstOrDefaultAsync(c => c.Id == categoria.Id)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        existente.Nome = categoria.Nome;
        existente.Descricao = categoria.Descricao;
        existente.Ativa = categoria.Ativa;
        existente.DataAtualizacao = DateTime.Now;

        await context.SaveChangesAsync();
    }

    public async Task AtivarInativarAsync(int id)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var categoria = await context.CategoriasFornecedores
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        categoria.Ativa = !categoria.Ativa;
        categoria.DataAtualizacao = DateTime.Now;

        await context.SaveChangesAsync();
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? idIgnorar = null)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.CategoriasFornecedores
            .AsNoTracking()
            .AnyAsync(c => c.Nome == nome && (idIgnorar == null || c.Id != idIgnorar));
    }

    public async Task<bool> PodeExcluirAsync(int id)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var temFornecedores = await context.Fornecedores
            .AsNoTracking()
            .AnyAsync(f => f.CategoriaFornecedorId == id);
        return !temFornecedores;
    }

    public async Task<int> ContarFornecedoresVinculadosAsync(int categoriaId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.Fornecedores
            .AsNoTracking()
            .CountAsync(f => f.CategoriaFornecedorId == categoriaId);
    }

    public async Task ExcluirAsync(int id)
    {
        if (!await PodeExcluirAsync(id))
            throw new InvalidOperationException("Esta categoria possui fornecedores vinculados e não pode ser excluída. Inative-a.");

        await using var context = await _dbFactory.CreateDbContextAsync();
        var categoria = await context.CategoriasFornecedores.FindAsync(id);

        if (categoria is not null)
        {
            context.CategoriasFornecedores.Remove(categoria);
            await context.SaveChangesAsync();
        }
    }

    public async Task SeedAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var categoriasIniciais = new[]
        {
            "Manutenção", "Limpeza", "Internet", "Energia", "Água",
            "Aluguel", "Professor/Tutor", "Funcionário", "Prestador de Serviço",
            "Material de Expediente", "Obra/Reforma", "Marketing", "Sistema/Software",
            "Contabilidade", "Jurídico", "Segurança", "Alimentação", "Transporte",
            "Eventos", "Outros"
        };

        var existentes = await context.CategoriasFornecedores
            .Select(c => c.Nome)
            .ToListAsync();

        var novas = categoriasIniciais
            .Where(n => !existentes.Contains(n))
            .Select(n => new CategoriaFornecedor
            {
                Nome = n,
                Ativa = true,
                DataCadastro = DateTime.Now
            })
            .ToList();

        if (novas.Count > 0)
        {
            context.CategoriasFornecedores.AddRange(novas);
            await context.SaveChangesAsync();
        }
    }
}
