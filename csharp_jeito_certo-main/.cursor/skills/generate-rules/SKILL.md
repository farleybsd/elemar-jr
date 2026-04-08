---
name: generate-rules
description: Gera regras compatíveis com Cursor AI para o repositório (globais, por contexto e governança técnica). Analisa a estrutura do repo, ADRs e padrões para produzir arquivos .cursor/rules/*.mdc. Use quando o usuário pedir para gerar regras do Cursor, criar rulebook, regras de arquitetura por contexto, regras de coding ou governança técnica.
---

# Gerador de Rules para Cursor

Gera um conjunto completo de arquivos de regras (`.mdc`) para desenvolvimento assistido por IA no Cursor: princípios globais, regras por contexto e governança técnica.

## Quando usar

- Gerar ou atualizar regras do projeto para o Cursor
- Criar rulebook, regras de arquitetura ou de coding por contexto
- Padronizar regras após análise de ADRs e estrutura do repositório

## Relação com outras skills

- **create-rule**: Use o formato de arquivo `.mdc` (frontmatter, globs, alwaysApply) definido na skill create-rule. Esta skill foca no **conteúdo** e no **processo de descoberta** para gerar vários arquivos de uma vez.
- **create-adr**: ADRs existentes em `docs/architecture-decision-records` são entrada para as regras; exceções e mudanças de regra devem exigir ADR.

---

## Fluxo de trabalho

### 1. Análise e entrada

Antes de gerar qualquer arquivo:

1. **Analisar regras existentes** em `.cursor/rules/` para complementar, não duplicar.
2. **Analisar a estrutura do repositório** (raiz e um nível abaixo) e classificar:
   - Mono-repo, monólito modular, serviço isolado, múltiplos contextos delimitados, etc.
3. **Se o padrão de organização não for claro**, parar e devolver **perguntas** para o usuário, em vez de assumir.

Para cada projeto/contexto do repositório, reunir (detalhes em [reference.md](reference.md)):

- Organização do repo (mono/multi, pastas, módulos)
- ADRs existentes em `docs/architecture-decision-records`
- Contextos e propósito de cada um
- Grafo de dependências entre contextos
- Projetos, responsabilidades e dependências
- Padrões arquiteturais por contexto (DDD, Ports & Adapters, Vertical Slice, etc.)
- Padrões de codificação, testes, lint, formatação
- Stack global e por contexto (linguagens, runtimes, frameworks)
- Libs aprovadas e proibidas
- Política de comunicação entre contextos e sistemas externos (resiliência, HttpClient, etc.)
- Política de testes e cobertura mínima
- Tratamento de erros (exception, result pattern, etc.)
- Multi-tenancy (se houver): regras e exemplos por contexto
- Modelagem de domínio (entidades, value objects, factories)
- Injeção de dependência
- Regras específicas da linguagem (implicit usings, etc.)
- Padrão de logging
- CI e exceções (com prazos)
- Data Access Patterns e migrations por contexto
- Documentação de APIs (Swagger)
- Conteúdo de `/shared/` (se existir)

**Se faltar informação crítica, perguntar ao usuário.**

### 2. Arquivos a gerar

| Arquivo | Conteúdo |
|---------|----------|
| `.cursor/rules/00-rulebook.mdc` | Princípios globais + precedência |
| `.cursor/rules/shared.mdc` | Só se existir `/shared` — política de reutilização |
| `.cursor/rules/contexts/<ctx>-architecture.mdc` | Arquitetura por contexto |
| `.cursor/rules/contexts/<ctx>-coding.mdc` | Código, testes, CI por contexto |

Criar a pasta `.cursor/rules/contexts/` quando houver múltiplos contextos.

### 3. Estrutura obrigatória de cada `.mdc`

Cada arquivo deve seguir este esqueleto (frontmatter conforme create-rule):

```markdown
---
description: <frase curta>
globs: <paths>
alwaysApply: <true|false>
---

# Objetivo
<clareza do problema>

- **SEMPRE** faça <x> porque <benefício>
- **SEMPRE** prefira <y> quando <condição>
- **NUNCA** use <x> pois <risco>
- **NUNCA** use <y> exceto <caso de exceção>

# Exemplos
\`\`\`ts
// ✅ Bom: <explicação>
\`\`\`
\`\`\`ts
// ❌ Ruim: <explicação>
\`\`\`

# Checklist
- [ ] Respeita boundaries?
- [ ] Respeita Dos e Donts?
- [ ] ADR se mudou regra?

# Exceções
Processo + prazo + aprovação + ADR obrigatória.
```

### 4. Regras globais mínimas (00-rulebook.mdc)

Incluir pelo menos:

- Contextos, objetivos e locais no repositório
- Mapa de dependência entre contextos
- Tecnologias comuns a todos os contextos
- Regras globais aplicáveis a todos os contextos
- Práticas de segurança (secrets, user secrets, etc.)

### 5. Apresentar e aguardar confirmação

- **Não sobrescrever** arquivos existentes até o usuário confirmar.
- Mostrar o conteúdo proposto (ou diff) e perguntar explicitamente antes de escrever em disco.

---

## Resultado esperado

- Arquivos `.mdc` completos e padronizados
- Nome do arquivo indicado no topo de cada bloco gerado
- Nenhum texto solto fora dos arquivos; toda saída pertinente dentro dos blocos de regra

---

## Referência completa

Para o prompt completo de geração, lista detalhada de entrada, exemplos e checklist de qualidade: [reference.md](reference.md).
