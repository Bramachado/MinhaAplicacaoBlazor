namespace MinhaAplicacaoBlazor.Models.Auth;

/// <summary>
/// Uma ação possível dentro de um módulo (Ver/Criar/Editar/Excluir).
/// </summary>
public record AcaoPermissao(string Chave, string Rotulo);

/// <summary>
/// Um módulo do sistema com suas ações. A chave completa da permissão é
/// "{ModuloChave}.{AcaoChave}", ex.: "Fornecedores.Editar".
/// </summary>
public record ModuloPermissao(string Chave, string Rotulo, string Grupo, IReadOnlyList<AcaoPermissao> Acoes)
{
    public IEnumerable<string> ChavesCompletas => Acoes.Select(a => $"{Chave}.{a.Chave}");
}

/// <summary>
/// Catálogo central de permissões. Toda permissão do sistema nasce aqui.
/// A UI de perfis/usuários e as policies de autorização usam esta lista.
/// </summary>
public static class Permissoes
{
    public const string Ver = "Ver";
    public const string Criar = "Criar";
    public const string Editar = "Editar";
    public const string Excluir = "Excluir";
    public const string VerTodos = "VerTodos";

    // Ações padrão de um módulo CRUD.
    private static readonly AcaoPermissao[] Crud =
    {
        new(Ver, "Ver"),
        new(Criar, "Criar"),
        new(Editar, "Editar"),
        new(Excluir, "Excluir"),
    };

    // CRUD + "Ver de todos": sem a chave VerTodos, o usuário só enxerga os
    // próprios registros (filtro por responsável); com ela, vê os de todos.
    private static readonly AcaoPermissao[] CrudComVerTodos =
    {
        new(Ver, "Ver"),
        new(Criar, "Criar"),
        new(Editar, "Editar"),
        new(Excluir, "Excluir"),
        new(VerTodos, "Ver de todos os usuários"),
    };

    // Módulos que só têm consulta (ex.: relatórios).
    private static readonly AcaoPermissao[] SomenteVer =
    {
        new(Ver, "Ver"),
    };

    /// <summary>Todos os módulos do sistema, na ordem de exibição.</summary>
    public static readonly IReadOnlyList<ModuloPermissao> Modulos = new List<ModuloPermissao>
    {
        // Geral
        new("Unidades", "Unidades", "Geral", Crud),
        new("Competencias", "Competências", "Geral", Crud),

        // Acadêmico
        new("Cursos", "Cursos", "Acadêmico", Crud),
        new("Titulacoes", "Titulações", "Acadêmico", Crud),
        new("EscalasAulasPraticas", "Escalas de Aula Prática", "Acadêmico", Crud),
        new("Tutores", "Tutores", "Acadêmico", Crud),

        // Pessoas / Fornecedores
        new("Colaboradores", "Colaboradores", "Pessoas", Crud),
        new("Fornecedores", "Fornecedores", "Pessoas", Crud),
        new("CategoriasFornecedores", "Categorias de Fornecedores", "Pessoas", Crud),
        new("ContasBancarias", "Contas Bancárias", "Pessoas", Crud),

        // Financeiro
        new("PlanosContas", "Planos de Contas", "Financeiro", Crud),
        new("FormasPagamento", "Formas de Pagamento", "Financeiro", Crud),
        new("Entradas", "Entradas", "Financeiro", Crud),
        new("LancamentosFinanceiros", "Lançamentos Financeiros", "Financeiro", Crud),
        new("Arquivos", "Arquivos", "Financeiro", Crud),
        new("RelatoriosFinanceiros", "Relatórios Financeiros", "Financeiro", SomenteVer),

        // Folhas
        new("FolhasColaboradores", "Folhas de Colaboradores", "Folhas", Crud),
        new("FolhasTutores", "Folhas de Tutores", "Folhas", Crud),
        new("FolhasFornecedores", "Folhas de Fornecedores", "Folhas", Crud),

        // CNAB
        new("Cnab", "CNAB (Remessas/Retornos)", "CNAB", Crud),

        // CRM
        new("CrmContatos", "Contatos (CRM)", "CRM", CrudComVerTodos),
        new("CrmOportunidades", "Oportunidades (CRM)", "CRM", CrudComVerTodos),
        new("CrmFunil", "Funil / Etapas (CRM)", "CRM", Crud),

        // Administração
        new("Usuarios", "Usuários", "Administração", Crud),
        new("Perfis", "Perfis de Acesso", "Administração", Crud),
        new("Auditoria", "Auditoria / Logs", "Administração", SomenteVer),
    };

    /// <summary>Todas as chaves de permissão válidas do sistema.</summary>
    public static readonly IReadOnlySet<string> Todas =
        Modulos.SelectMany(m => m.ChavesCompletas).ToHashSet();

    /// <summary>Agrupa os módulos por grupo, preservando a ordem de declaração.</summary>
    public static IEnumerable<IGrouping<string, ModuloPermissao>> PorGrupo() =>
        Modulos.GroupBy(m => m.Grupo);

    public static bool Existe(string permissao) => Todas.Contains(permissao);
}
