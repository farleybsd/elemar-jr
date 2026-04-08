---
name: create-adr
description: Gerencia o ciclo de vida dos ADRs (Architecture Decision Records) no repositório. Executa arqueologia de software, análise de gaps, proposta de decisões e escrita de ADRs em docs/architecture-decision-records. Use quando o usuário pedir para criar ADRs, documentar decisões de arquitetura, analisar decisões implícitas do projeto ou gerenciar a pasta de architecture-decision-records.
---

# ADR — Architecture Decision Records

## Configuração

**Pasta de ADRs (fixa):** `docs/architecture-decision-records` na **raiz do repositório**. Todas as ADRs devem ser criadas neste caminho.

| Parâmetro | Padrão | Descrição |
|-----------|--------|-----------|
| **Pasta de ADRs** | `docs/architecture-decision-records` | Pasta na raiz do repo onde ficam os ADRs. **Sempre** usar este caminho para ler e criar ADRs. |
| **Template** | Template da skill | Se na pasta de ADRs existir `TEMPLATE.md`, use-o; caso contrário use o da skill: [TEMPLATE.md](TEMPLATE.md). |
| **Escopo da análise** | Projeto inteiro | Analisar só a pasta de ADRs ou o repositório todo (recomendado para proposta de novos ADRs). |
| **Idioma** | Português (PT-BR) | Idioma de saída dos ADRs. |

### Verificação e criação da pasta

Antes de ler ou escrever ADRs:

1. **Verificar** se existe a pasta `docs/architecture-decision-records` na raiz do repositório.
2. **Se não existir**, criar a pasta (e `docs/`, se necessário).
3. Só então prosseguir com leitura dos ADRs existentes ou criação de novos arquivos.

Assim garante-se que as ADRs são sempre criadas em `docs/architecture-decision-records`.

---

## Fluxo de trabalho (4 passos)

Siga os passos na ordem. Detalhes completos e prompts estão em [reference.md](reference.md).

### Passo 1: Arqueologia e análise de gaps

0. **Pasta de ADRs:** Verificar se `docs/architecture-decision-records` existe na raiz do repo; se não, criar a pasta. Usar sempre este caminho.
1. **Leitura inicial:** Ler todos os `.md` em `docs/architecture-decision-records` para saber o que já está decidido.
2. **Análise macro (C4 – Containers):** Antes de ler código, analisar a **estrutura de diretórios** (raiz e um nível abaixo).
   - Topologia: monorepo, monólito modular, microserviços, front/back separados?
   - Containers: pastas que são aplicações/serviços (`client/`, `server/`, `apps/`).
   - Contextos: domínio (ex.: `modules/sales`) ou camadas (ex.: `layers/`).
   - Migração: pastas como `v1/`, `v2/`, `legacy/`, `modern/`.
3. **Stack e padrões:** Configurações (`package.json`, `*.csproj`, `go.mod`, `docker-compose.yml`) e código para identificar stack, padrões (Clean Arch, Hexagonal, MVC) e libs principais.
4. **Testes:** Projetos de teste (unitários e integração) — padrões, stack, mocks, assertivas, nomenclatura e organização dos arquivos.
5. **Gaps:** Comparar estrutura + stack com o que está documentado nos ADRs; listar decisões implícitas.

### Passo 2: Proposta de decisões (pausa obrigatória)

- Listar ADRs sugeridos, **numerados** e agrupados por categoria.
- **Grupo A — Estrutura e organização (containers):** ex.: estratégia de monorepo, separação front/back. Evidência: pastas X e Y.
- **Grupo B — Engenharia e padrões:** ex.: uso da lib X, padrão Y. Evidência: código/config.
- Usar o **próximo número sequencial** disponível na pasta de ADRs.
- **Parar e aguardar confirmação do usuário** antes de escrever arquivos.

### Passo 3: Criação e escrita (após aprovação)

Para cada ADR aprovado:

1. Ler o template do projeto (se existir `TEMPLATE.md` na pasta de ADRs) ou o da skill: [TEMPLATE.md](TEMPLATE.md).
2. Escrever o conteúdo seguindo a estrutura do template (Contexto, Decisão, seções opcionais quando fizerem sentido).
   - Em **Contexto**, indicar *onde* no repositório a decisão aparece (caminhos de pasta).
   - Em **Implementação Técnica**, usar a subseção **Estrutura de Arquivos** com árvore (`tree` ou equivalente) para ilustrar.
3. **Garantir a pasta:** Se `docs/architecture-decision-records` não existir na raiz, criá-la. Criar o arquivo em `docs/architecture-decision-records/ADR-XXX-slug.md` (slug em minúsculas, hífens) com a **data atual**.

### Passo 4: Verificação final

- Conferir se ADRs de estrutura (Grupo A) sustentam as de engenharia (Grupo B).
- Ex.: a estrutura de pastas está alinhada à arquitetura hexagonal documentada?

---

## Nomenclatura e arquivos

- Nome do arquivo: `ADR-XXX-slug-do-titulo.md` (XXX = número com zeros à esquerda, ex.: 001, 012).
- Slug: minúsculas, palavras separadas por hífen, sem acentos.

---

## Comando inicial sugerido

Se o usuário não especificar como começar, inicie pelo **Passo 1** (com foco em 1.2 – análise macro) e, ao final, apresente as **propostas do Passo 2** e pause para aprovação.

Para fluxo completo, consulte [reference.md](reference.md). O template de base está em [TEMPLATE.md](TEMPLATE.md).
