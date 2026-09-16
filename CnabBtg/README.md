# Subsistema CNAB BTG (Pagamentos — FEBRABAN 240)

Gera arquivos de remessa de pagamento no padrão **FEBRABAN 240** para o **Banco BTG
Pactual (208)**, a partir dos pagamentos das folhas fechadas da aplicação. Núcleo
puro (sem EF), independente da origem dos dados: recebe uma lista de `PaymentInput`.

## Fluxo do usuário

Cada relatório de folha (**Fornecedores**, **Tutores**, **Colaboradores**) tem seu
próprio botão **CNAB** que abre um modal inline de exportação: seleciona os
pagamentos do tipo Transferência, informa empresa pagadora/data/ambiente e gera
o `.rem` + auditoria + ZIP na hora, usando `CnabBtgGeracaoService.Empresas` e o
gerador diretamente (sem persistir lote/histórico).

As telas **Gerenciar CNAB** (`/cnab-btg/gerenciar`) e **Histórico CNAB**
(`/cnab-btg/historico`) — um fluxo alternativo de conferência/geração/histórico
por competência, com persistência em `CnabBatch` e telas dedicadas — foram
**removidas** por estarem sem uso. Os métodos que as serviam
(`CnabBtgGeracaoService.GerarAsync/ObterPagamentosAsync/ObterHistoricoAsync/
ObterDetalheAsync/ObterZipAsync`) e as tabelas `CnabBatch`/`CnabBatchPayment`/
`CnabGeneratedFile`/`CnabSequence` continuam no banco (histórico preservado),
mas hoje não têm nenhuma tela chamando-os.

Resta a tela **Configurações CNAB** (`/cnab-btg/configuracoes`), só leitura das
empresas pagadoras configuradas.

## Estrutura de código (`/CnabBtg`)

| Pasta | Conteúdo |
|---|---|
| `Generation/` | `Cnab240LineBuilder` (linha de 240), `CnabText`, `FormaLancamento`, `EmpresaPagadora`, `CnabGenerationOptions/Result`, `CnabRecordBuilders` (Header Arquivo/Lote, Segmento A/B, Trailer Lote/Arquivo), `CnabBtgPaymentGenerator` |
| `Payments/` | `PaymentInput`, `NormalizedPayment`, `PaymentNormalizer`, `PaymentValidator` |
| `Audit/` | `CnabAuditReport/Row`, `CnabAuditWriter` (JSON/CSV), `CnabZipPacker` |
| `Data/` | `CnabBatch` (+ campos de conferência), `CnabGeneratedFile`, `CnabBatchPayment`, `CnabSequence` |
| (raiz) | `CnabBtgGeracaoService` (geração + persistência; `Empresas` usado pelas telas de Folha e por `ConfiguracoesCnabBtg.razor`), DTOs (`CnabBtgDtos`) |

Tela: `Components/Pages/Cnab/ConfiguracoesCnabBtg.razor`.

O gerador é usado pelo `CnabBtgGeracaoService`, mas pode ser chamado isoladamente:

```csharp
var options = new CnabGenerationOptions { Empresa = ..., DataPagamento = ..., NsaInicial = 100, ... };
var result = new CnabBtgPaymentGenerator().Gerar(listaDePaymentInput, options);
```

## Regras implementadas

- Linha com **exatamente 240** caracteres (numéricos zero à esquerda, alfanuméricos
  espaço à direita, sem acento/quebra, caixa alta).
- Valores em **centavos**; datas **DDMMAAAA**; CPF/CNPJ só dígitos (com validação de DV).
- **Máx. 50 operações por arquivo** → múltiplos `.rem`, cada um com **NSA próprio**
  (incrementa +1 por arquivo).
- Um **lote por forma de lançamento** (o Header de Lote tem forma única).
- Formas suportadas: **45 PIX**, **01 crédito em conta**, **05 poupança**,
  **41/43 TED**. Boleto/QR/tributos ficam preparados mas **não são gerados** sem módulo.
- Segmento B PIX: chave em Informação 12 (128–226, máx. 99). Chave maior → **inválida**
  (não trunca). Tipo de chave inferido (telefone 01, e-mail 02, CPF/CNPJ 03, aleatória 04,
  dados bancários 05) — código de 2 dígitos no campo alfa de 3 posições (posições 15–17
  do Segmento B), conferido contra remessa real aceita pelo BTG.
- Inválidos **não entram** no `.rem`. Opção "bloquear se houver inválidos" impede a
  geração inteira. Pendentes (ex.: duplicidade) ficam de fora e são listados.
- **Auditoria JSON + CSV** (original × normalizado, totais, arquivos) e **ZIP** com tudo.
- Persistência (só no fluxo `CnabBtgGeracaoService.GerarAsync`, hoje sem tela chamando-o):
  `CnabBatch` + `CnabGeneratedFile` + `CnabBatchPayment` (marca `StatusCnab = CNAB_GERADO`,
  **nunca "Pago"**) + `CnabSequence` (NSA por empresa). O fluxo inline das telas de Folha
  não persiste lote/histórico.

> No texto/gerador: *"Arquivo gerado conforme regras estruturais CNAB. A validação
> final deve ser feita no ambiente BTG."*

## Configuração (`appsettings.json`)

```json
"CnabBtg": {
  "StoragePath": "App_Data/CnabBtg",
  "MaxOperationsPerFile": 50,
  "UseAutomaticNsa": true,
  "Empresas": [ { "Codigo": "EDUNORTE", ... }, { "Codigo": "FADUC", ... } ]
}
```

Se a seção `Empresas` estiver ausente, usa os valores fixos de `EmpresaPagadora.Padrao`
(EDUNORTE e FADUC). Divergências entre dados fixos e documentação devem ser ajustadas
aqui.

## Banco de dados

As 4 tabelas (`CnabBatches`, `CnabGeneratedFiles`, `CnabBatchPayments`,
`CnabSequences`) são criadas pela migration **`AddCnabBtgGerenciamento`** (inclui os
campos de conferência do lote). Para aplicar:

```bash
dotnet ef database update
```

## Cadastro de ContaBancaria

O cadastro tem **dígito da conta**, **tipo de conta** (Corrente/Poupança) e **tipo de
chave PIX** (Telefone/E-mail/CPF-CNPJ/Aleatória, ou "inferir"). A partir deles a forma
é derivada: **PIX→45**, **poupança→05**, **corrente no banco 208→01**, **corrente em
outro banco→41 (TED)**. Se o dígito não for informado, é separado de `Conta`
(ex.: `534630-2`); se o tipo de chave for "inferir", é deduzido pela própria chave.

## Testes

Projeto `MinhaAplicacaoBlazor.Tests` (xUnit) cobre: linha de 240, numérico/alfa,
valor em centavos, validação CPF/CNPJ, separação conta-dígito, quebra a 50 por arquivo,
NSA por arquivo, inválidos fora do arquivo, bloqueio por inválidos, PIX/TED e estrutura
de registros. Rodar: `dotnet test`.

## Próximos passos possíveis

- Segmentos J/O/N para boleto, tributos e PIX-QRCode (hoje não gerados sem módulo).
- Importação/reconciliação de retorno para este subsistema novo (o subsistema legado
  já tem importação de retorno própria).
