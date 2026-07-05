# Subsistema CNAB BTG (Pagamentos — FEBRABAN 240)

Gera arquivos de remessa de pagamento no padrão **FEBRABAN 240** para o **Banco BTG
Pactual (208)**, a partir dos pagamentos das folhas fechadas da aplicação. Núcleo
puro (sem EF), independente da origem dos dados: recebe uma lista de `PaymentInput`.

## Fluxo do usuário

1. Menu **Relatórios Financeiros** → tipo **"Pagamentos Bancários"** → **Gerar Relatório**.
2. No bloco do relatório, ao lado de **Exportar Excel**, clicar em **Gerar CNAB**.
3. No popup: selecionar os pagamentos e preencher empresa pagadora, data de
   pagamento, NSA (automático ou inicial), nome do arquivo, tipo principal,
   ambiente (Teste/Produção), bloquear-se-inválidos, separar-lotes-por-forma e
   convênio (opcional).
4. **Gerar CNAB** → mostra classificação (VÁLIDO / CORRIGIDO / PENDENTE / INVÁLIDO),
   arquivos gerados e botão **Baixar ZIP** (contém os `.rem` + auditoria JSON/CSV).

## Estrutura de código (`/CnabBtg`)

| Pasta | Conteúdo |
|---|---|
| `Generation/` | `Cnab240LineBuilder` (linha de 240), `CnabText`, `FormaLancamento`, `EmpresaPagadora`, `CnabGenerationOptions/Result`, `CnabRecordBuilders` (Header Arquivo/Lote, Segmento A/B, Trailer Lote/Arquivo), `CnabBtgPaymentGenerator` |
| `Payments/` | `PaymentInput`, `NormalizedPayment`, `PaymentNormalizer`, `PaymentValidator` |
| `Audit/` | `CnabAuditReport/Row`, `CnabAuditWriter` (JSON/CSV), `CnabZipPacker` |
| `Data/` | `CnabBatch`, `CnabGeneratedFile`, `CnabBatchPayment`, `CnabSequence` |
| (raiz) | `CnabBtgGeracaoService` (liga folhas fechadas → gerador → persistência), DTOs |

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
- Segmento B PIX: chave em Informação 10 (33–67, máx. 35). Chave maior → **inválida**
  (não trunca). Tipo de chave inferido (telefone 001, e-mail 002, CPF/CNPJ 003, aleatória 004).
- Inválidos **não entram** no `.rem`. Opção "bloquear se houver inválidos" impede a
  geração inteira. Pendentes (ex.: duplicidade) ficam de fora e são listados.
- **Auditoria JSON + CSV** (original × normalizado, totais, arquivos) e **ZIP** com tudo.
- Persistência: `CnabBatch` + `CnabGeneratedFile` + `CnabBatchPayment` (marca
  `StatusCnab = CNAB_GERADO`, **nunca "Pago"**) + `CnabSequence` (NSA por empresa).
- Pagamento já incluído em um CNAB ativo **não** pode ser reincluído.

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

As tabelas são criadas pela migration **`AddCnabBtg`**. Para aplicar:

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
