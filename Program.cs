using MinhaAplicacaoBlazor.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinhaAplicacaoBlazor.Data;
using MinhaAplicacaoBlazor.Models;
using MinhaAplicacaoBlazor.Models.Auth;
using MinhaAplicacaoBlazor.Services;
using MinhaAplicacaoBlazor.Services.Auth;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// === Multi-empresa (tenant) ===
// A empresa do usuário logado é resolvida por escopo e carimbada em todo contexto.
// Registrado DEPOIS do AddDbContextFactory para substituir a fábrica padrão:
// assim todo serviço que injeta IDbContextFactory<AppDbContext> passa a receber
// contextos já isolados por empresa, sem alterar os serviços existentes.
builder.Services.AddScoped<MinhaAplicacaoBlazor.Services.Auth.TenantContext>();
builder.Services.AddScoped<IDbContextFactory<AppDbContext>, TenantAwareDbContextFactory>();

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
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<MinhaAplicacaoBlazor.CnabBtg.CnabBtgGeracaoService>();
builder.Services.AddScoped<MinhaAplicacaoBlazor.CnabBtg.CnabBtgGerenciamentoService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var dbFactory = sp.GetRequiredService<IDbContextFactory<AppDbContext>>();

    // 1) Garante a empresa (tenant) padrão e define-a como empresa deste escopo de
    //    seeding — assim todos os cadastros iniciais nascem carimbados nela.
    int empresaPadraoId;
    await using (var dbInit = await dbFactory.CreateDbContextAsync())
    {
        var empresa = await dbInit.Empresas.FirstOrDefaultAsync();
        if (empresa is null)
        {
            empresa = new Empresa { Nome = "Empresa Padrão", Ativa = true };
            dbInit.Empresas.Add(empresa);
            await dbInit.SaveChangesAsync();
        }
        empresaPadraoId = empresa.Id;
    }
    sp.GetRequiredService<MinhaAplicacaoBlazor.Services.Auth.TenantContext>().EmpresaId = empresaPadraoId;

    // 2) Cria perfil Administrador + usuário admin inicial (vinculado à empresa padrão).
    var identitySeeder = sp.GetRequiredService<IdentitySeeder>();
    await identitySeeder.SeedAsync(empresaPadraoId);

    var seed = sp.GetRequiredService<CategoriaFornecedorService>();
    await seed.SeedAsync();

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

// Serve os arquivos físicos de wwwroot (incluindo assets de RCL em _content/**,
// como o JS do Blazor-ApexCharts usado no dashboard) pelo middleware clássico.
// Isto complementa o MapStaticAssets(): em alguns ambientes publicados / atrás de
// proxy, o caminho "simples" (não fingerprinted) de _content pode dar 404 apenas
// pelo pipeline novo; o UseStaticFiles garante a entrega desses arquivos.
app.UseStaticFiles();

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
