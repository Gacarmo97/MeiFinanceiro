# MeiFinanceiro

Aplicação web para controle financeiro de um MEI (Microempreendedor Individual), com suporte a múltiplos estabelecimentos e acompanhamento real do mês a mês. Originalmente prototipado como um site estático em HTML/CSS/JS, migrado para uma solução .NET em camadas (DDD) com persistência em SQLite.

## Stack

- **.NET 9** / C#
- **Blazor Server** (`Microsoft.AspNetCore.Components`, renderização interativa no servidor)
- **Entity Framework Core** + **SQLite**
- **Bootstrap 5** (arquivos estáticos locais, sem dependência de CDN)
- **xUnit** para os testes de domínio

## Arquitetura

Solução em 4 camadas (DDD), referenciando-se em cascata `Web → Application → Infrastructure → Domain`:

```
MeiFinanceiro/
  MeiFinanceiro.sln
  src/
    MeiFinanceiro.Domain/           Entidades, enums, interfaces de repositório e as calculadoras (regras de negócio puras)
    MeiFinanceiro.Application/      Casos de uso que orquestram domínio + repositórios
    MeiFinanceiro.Infrastructure/   EF Core (SQLite), DbContext, repositórios, migrations
    MeiFinanceiro.Web/              Blazor Server + Bootstrap, páginas da aplicação
  tests/
    MeiFinanceiro.Domain.Tests/     xUnit — cada calculadora testada com valores já validados manualmente
```

A camada `Domain` não depende de nada (nem de EF Core): cada calculadora é uma classe estática com um método `Calcular(Entrada) : Resultado`, sem efeitos colaterais, o que torna as regras de negócio fáceis de testar isoladamente.

## Modelo de domínio

- **`Estabelecimento`** — cada negócio cadastrado (nome, atividade MEI que define o DAS, salário mínimo de referência). Suporta múltiplos estabelecimentos, com isolamento total de dados entre eles.
- **`FechamentoMensal`** — o fechamento real de um mês (faturamento, CMV, custos fixos, DAS calculado, lucro líquido e margem), único por estabelecimento + ano + mês.

## Funcionalidades

### Simulações (calculadoras, sem persistência)
| Tela | O que calcula |
|---|---|
| Ponto de Equilíbrio | Faturamento mínimo mensal para cobrir custos fixos + variáveis + DAS-MEI |
| Preço de Venda | Custo de insumos (com % de perda/quebra) + margem de lucro → preço de venda (método do divisor de markup) |
| Orçamento de Gastos | Faturamento esperado + lucro desejado → teto de gastos totais e de custos fixos |
| Retorno do Investimento | Investimento inicial + lucro mensal → payback (meses) e ROI anual |
| CMV Mensal | Estoque inicial + compras − estoque final → % do faturamento virado custo de insumo, com faixas de referência |
| Custo de Funcionário | Salário + benefícios → custo total mensal real (FGTS 8%, INSS patronal MEI reduzido a 3%, 13º e férias proporcionais) |
| Engenharia de Cardápio | Classifica pratos em Estrela / Cavalo de batalha / Enigma / Abacaxi (popularidade x lucratividade) |

### Dados reais (persistidos em SQLite)
| Tela | O que faz |
|---|---|
| Estabelecimentos | CRUD de estabelecimentos — cada um com sua atividade e salário mínimo de referência |
| Fechamento Mensal | Lança os valores reais do mês (mesma conta da DRE Simplificada), com histórico editável |
| Resumo | Dashboard: último mês, médias móveis de lucro líquido (3/6 meses) e histórico completo |

## Como rodar

Pré-requisito: .NET 9 SDK.

```bash
# Restaurar e compilar
dotnet build

# Rodar os testes de domínio
dotnet test tests/MeiFinanceiro.Domain.Tests

# Subir a aplicação web (aplica as migrations automaticamente na primeira execução)
dotnet run --project src/MeiFinanceiro.Web
```

A aplicação sobe em `https://localhost:5xxx` (ou na porta definida em `--urls`). O banco SQLite (`meifinanceiro.db`) é criado automaticamente dentro de `src/MeiFinanceiro.Web` na primeira execução.

### Criando uma nova migration

```bash
dotnet ef migrations add NomeDaMigration --project src/MeiFinanceiro.Infrastructure --startup-project src/MeiFinanceiro.Web
```

## Histórico

Este projeto nasceu de um protótipo em HTML/CSS/JS estático (sem persistência, com as mesmas 7 calculadoras de simulação), mantido fora deste repositório, que serviu de base para as regras de negócio replicadas aqui.
