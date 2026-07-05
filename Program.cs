using MinhaAplicacaoBlazor.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;
using MinhaAplicacaoBlazor.Models.Auth;
using MinhaAplicacaoBlazor.Models.Cnab;
using MinhaAplicacaoBlazor.Models.Crm;
using MinhaAplicacaoBlazor.Services;
using MinhaAplicacaoBlazor.Services.Auth;
using MinhaAplicacaoBlazor.Services.Cnab;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// O ASP.NET Core Identity precisa de um AppDbContext "scoped"; como usamos
// IDbContextFactory, criamos um a partir da factory por escopo.
builder.Services.AddScoped<AppDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

// === Autenticação (ASP.NET Core Identity com cookie) ===
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
}).AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

// Onde o middleware de autorização redireciona quando falta login/permissão.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/acesso-negado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// === Autorização baseada em permissões ===
builder.Services.AddScoped<PermissaoService>();
builder.Services.AddScoped<IdentitySeeder>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    // Por padrão, toda página exige usuário autenticado
    // (páginas públicas usam [AllowAnonymous]).
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<CategoriaFornecedorService>();
builder.Services.AddScoped<FolhaColaboradorService>();
builder.Services.AddScoped<FolhaTutorService>();
builder.Services.AddScoped<FolhaFornecedorService>();
builder.Services.AddScoped<RelatorioFinanceiroService>();
builder.Services.AddScoped<ICnabPagamentoService, CnabPagamentoService>();
builder.Services.AddScoped<MinhaAplicacaoBlazor.CnabBtg.CnabBtgGeracaoService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Cria perfil Administrador + usuário admin inicial.
    var identitySeeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await identitySeeder.SeedAsync();

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

    // === Seed CNAB ===
    if (!await dbCtx.ConfiguracoesCnab.AnyAsync(c => c.NomeConfiguracao == "BTG - EDUNORTE"))
    {
        dbCtx.ConfiguracoesCnab.Add(new ConfiguracaoCnab
        {
            NomeConfiguracao = "BTG - EDUNORTE",
            BancoCodigo = "208",
            BancoNome = "BTG Pactual",
            RazaoSocial = "EDUNORTE SERVICOS EDUCACIONAIS LTDA",
            CNPJ = "58262834000155",
            Agencia = "0050",
            Conta = "829434",
            ContaDV = "3",
            Layout = "FEBRABAN240",
            VersaoLayoutArquivo = "087",
            VersaoLayoutLote = "046",
            SequencialArquivo = 1,
            Ativa = true,
            DataCadastro = DateTime.Now
        });
        await dbCtx.SaveChangesAsync();
    }

    var formasCnab = new (string Codigo, string Nome, string Categoria, string Segmentos)[]
    {
        ("01", "Crédito em Conta Corrente/Salário", "Transferência", "A,B,C"),
        ("03", "DOC/TED",                            "Transferência", "A,B,C"),
        ("05", "Crédito em Conta Poupança",          "Transferência", "A,B,C"),
        ("41", "TED - Outra Titularidade",           "Transferência", "A,B,C"),
        ("43", "TED - Mesma Titularidade",           "Transferência", "A,B,C"),
        ("44", "TED para Conta Investimento",        "Transferência", "A,B,C"),
        ("45", "PIX Transferência",                  "Transferência", "A,B,C"),
        ("47", "PIX QR-CODE",                        "Transferência", "J,J-52,J-52Pix"),
        ("50", "Débito em Conta Corrente",           "Transferência", "A,B,C"),
        ("30", "Liquidação de Títulos do Próprio Banco", "Títulos",   "J,J-52"),
        ("31", "Pagamento de Títulos de Outros Bancos",  "Títulos",   "J,J-52"),
        ("11", "Pagamento de Contas e Tributos com Código de Barras", "Tributos", "O"),
        ("16", "Tributo - DARF Normal",              "Tributos",      "N2")
    };
    var existentesCnab = await dbCtx.FormasLancamentoCnab.Select(f => f.Codigo).ToListAsync();
    var novasCnab = formasCnab
        .Where(f => !existentesCnab.Contains(f.Codigo))
        .Select(f => new FormaLancamentoCnab
        {
            Codigo = f.Codigo,
            Nome = f.Nome,
            Categoria = f.Categoria,
            Segmentos = f.Segmentos,
            Ativa = true
        })
        .ToList();
    if (novasCnab.Count > 0)
    {
        dbCtx.FormasLancamentoCnab.AddRange(novasCnab);
        await dbCtx.SaveChangesAsync();
    }

    // === Seed CRM: etapas padrão do funil ===
    if (!await dbCtx.CrmEtapasFunil.AnyAsync())
    {
        var etapasFunil = new[]
        {
            new CrmEtapaFunil { Nome = "Novo",        Ordem = 1, Cor = "#6c757d", Ativa = true },
            new CrmEtapaFunil { Nome = "Contato feito", Ordem = 2, Cor = "#0dcaf0", Ativa = true },
            new CrmEtapaFunil { Nome = "Proposta",    Ordem = 3, Cor = "#0d6efd", Ativa = true },
            new CrmEtapaFunil { Nome = "Negociação",  Ordem = 4, Cor = "#ffc107", Ativa = true },
            new CrmEtapaFunil { Nome = "Ganho",       Ordem = 5, Cor = "#198754", Ativa = true },
            new CrmEtapaFunil { Nome = "Perdido",     Ordem = 6, Cor = "#dc3545", Ativa = true },
        };
        dbCtx.CrmEtapasFunil.AddRange(etapasFunil);
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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Libera os arquivos estáticos (CSS, JS, imagens, blazor.web.js) para acesso
// anônimo. Sem isto, a FallbackPolicy exigiria login até para carregar o CSS,
// deixando a própria tela de login sem estilo.
app.MapStaticAssets().AllowAnonymous();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoint de logout (POST). Blazor interativo não pode limpar o cookie de
// autenticação; por isso o logout é feito por um endpoint HTTP.
app.MapPost("/Account/Logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.Run();
