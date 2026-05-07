using MinhaAplicacaoBlazor.Components;
using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;
using MinhaAplicacaoBlazor.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CategoriaFornecedorService>();
builder.Services.AddScoped<FolhaColaboradorService>();
builder.Services.AddScoped<FolhaTutorService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<CategoriaFornecedorService>();
    await seed.SeedAsync();

    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var dbCtx = await dbFactory.CreateDbContextAsync();

    var planosIniciais = new[]
    {
        "ALUGUEL", "INTERNET", "ENERGIA", "ÁGUA", "MARKETING",
        "FOLHA PAGAMENTO", "IMPOSTO", "PRESTAÇÃO SERVIÇO", "OUTRAS COMPRAS"
    };

    var planosExistentes = await dbCtx.PlanosContas.Select(p => p.Nome).ToListAsync();
    var planosNovos = planosIniciais
        .Where(n => !planosExistentes.Contains(n))
        .Select(n => new PlanoConta { Nome = n, Ativo = true })
        .ToList();

    if (planosNovos.Count > 0)
    {
        dbCtx.PlanosContas.AddRange(planosNovos);
        await dbCtx.SaveChangesAsync();
    }

    var formasIniciais = new[] { "BOLETO", "PIX", "DEPÓSITO", "CONTA SIMPLES" };

    var formasExistentes = await dbCtx.FormasPagamento.Select(f => f.Nome).ToListAsync();
    var formasNovas = formasIniciais
        .Where(n => !formasExistentes.Contains(n))
        .Select(n => new FormaPagamento { Nome = n, Ativa = true })
        .ToList();

    if (formasNovas.Count > 0)
    {
        dbCtx.FormasPagamento.AddRange(formasNovas);
        await dbCtx.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
