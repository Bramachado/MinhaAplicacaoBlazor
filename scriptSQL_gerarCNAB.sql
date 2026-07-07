SELECT f.NomeRazaoSocial, f.CpfCnpj, f.TipoPagamento, item.ValorTotalPagar, 
banco.Descricao, 
c.NomeTitular, c.CpfCnpj, c.Forma, c.TipoPessoa, c.TipoChavePix, c.ChavePix, c.CodigoBanco, c.NomeBanco, c.Agencia, c.Conta
FROM FolhasFornecedoresItens AS item
INNER JOIN FolhasFornecedores AS folhaF
    ON folhaF.Id = item.FolhaFornecedorId
INNER JOIN Fornecedores AS f
    ON item.FornecedorId = f.Id
INNER JOIN ContasBancarias AS c
    ON f.ContaBancariaId = c.Id
	inner join CategoriasFornecedores as cat
	on f.CategoriaFornecedorId = cat.Id
	inner join Bancos as banco
	on banco.Id = item.BancoPagadorId
WHERE folhaF.CompetenciaId = 2;
;

SELECT p.Nome, p.Cpf, item.ValorTotal, item.ValorReceberPix , 
c.NomeTitular, c.CpfCnpj, c.Forma, c.TipoPessoa, c.TipoChavePix, c.ChavePix, c.CodigoBanco, c.NomeBanco, c.Agencia, c.Conta
FROM FolhasColaboradoresItens AS item
INNER JOIN FolhasColaboradores AS folhaC
    ON folhaC.Id = item.FolhaColaboradorId
INNER JOIN Colaboradores AS p
    ON item.ColaboradorId = p.Id
INNER JOIN ContasBancarias AS c
    ON p.ContaBancariaId = c.Id
WHERE folhaC.CompetenciaId = 2;

SELECT t.Nome, t.Cpf, item.ValorTotalReceber, c.NomeTitular, 
c.CpfCnpj, c.Forma, c.TipoPessoa, c.TipoChavePix, c.ChavePix, c.CodigoBanco, c.NomeBanco, c.Agencia, c.Conta
FROM FolhasTutoresItens AS item
INNER JOIN FolhasTutores AS folhaT
    ON folhaT.Id = item.FolhaTutorId
INNER JOIN Tutores AS t
    ON item.TutorId = t.Id
INNER JOIN ContasBancarias AS c
    ON t.ContaBancariaId = c.Id
WHERE folhaT.CompetenciaId = 2;