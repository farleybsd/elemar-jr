# Referência — Fluxo completo e template ADR

Use este documento quando precisar do fluxo detalhado. O template de base para escrita de ADRs está em [TEMPLATE.md](TEMPLATE.md) na pasta da skill.

---

## Fluxo de trabalho completo (prompt base)

Atue como Arquiteto de Software e Engenheiro de Documentação. Objetivo: gerenciar o ciclo de vida dos ADRs no repositório.

**Configuração:** Pasta de ADRs fixa: `docs/architecture-decision-records` na raiz do repositório. Verificar/criar a pasta antes de ler ou escrever. Idioma PT-BR; modo ativo (análise top-down).

### Passo 1: Arqueologia e análise de gaps

**1.0. Pasta de ADRs**  
Verificar se `docs/architecture-decision-records` existe na raiz do repo. Se não existir, criar a pasta (e `docs/` se necessário). Todas as ADRs são lidas e criadas neste caminho.

**1.1. Leitura inicial**  
Ler todos os `.md` em `docs/architecture-decision-records` para saber o que já está decidido.

**1.2. Análise macro (C4 – Containers)**  
Antes de ler código, analisar a **estrutura de diretórios** (raiz e subníveis imediatos).

- **Topologia:** Monorepo? Monólito modular? Microserviços em um repo? Front e back separados?
- **Containers:** Pastas que representam aplicações/serviços (ex.: `client/`, `server/`, `apps/`, `legacy/`).
- **Contextos:** Divisão por domínio (ex.: `modules/sales`, `modules/inventory`) ou por camadas (ex.: `layers/`).
- **Coexistência:** Evidências de migração (ex.: `v1/`, `v2/`, `legacy/`, `modern/`).

Registrar observações estruturais como potenciais ADRs de organização.

**1.3. Stack e padrões (componentes)**  
Aprofundar em arquivos de configuração e código:

- Stack principal.
- Padrões (Clean Arch, Hexagonal, MVC).
- Libs que definem comportamento (ORMs, frameworks de UI).

**1.4. Testes**  
Analisar projetos de testes (unidade e integração):

- Padrões de escrita.
- Stack (bibliotecas).
- Integração (containers, ambientes).
- Mocks.
- Assertivas e bibliotecas.
- Nomenclatura e organização dos arquivos de teste.

**1.5. Gaps**  
Comparar estrutura (1.2) e stack (1.3) com o que está documentado (1.1). Listar decisões implícitas.

### Passo 2: Proposta (pausa obrigatória)

Apresentar lista numerada de ADRs sugeridos, agrupados:

**Grupo A: Estrutura e organização (containers)**  
- [ID sugerido] – [Título] (ex.: Estratégia de monorepo, Separação front/back).  
  *Evidência:* "Com base nas pastas X e Y..."

**Grupo B: Engenharia e padrões**  
- [ID sugerido] – [Título].  
  *Evidência:* "Com base no uso da lib X e padrão Y..."

Determinar o próximo número sequencial. **Parar e aguardar confirmação.**

### Passo 3: Criação (após aprovação)

Para cada item aprovado:

1. Garantir que `docs/architecture-decision-records` existe na raiz; se não, criar a pasta.
2. Ler o template do projeto (se existir `TEMPLATE.md` na pasta de ADRs) ou o da skill: [TEMPLATE.md](TEMPLATE.md).
3. Escrever: em **Contexto**, indicar onde no repo a decisão é visível (caminhos); em **Implementação Técnica**, usar árvore (`tree`) para ilustrar (conforme seção do template).
4. Criar o arquivo em `docs/architecture-decision-records/ADR-XXX-slug.md` com data atual.

### Passo 4: Verificação final

Verificar se ADRs de estrutura (Grupo A) sustentam as de engenharia (Grupo B). Ex.: a estrutura de pastas suporta a arquitetura hexagonal descrita?

---

## Template de base

O template mestre com todas as seções (Contexto, Decisão, Comparação das Abordagens, Prós e Contras, Padrões Estabelecidos, Implementação Técnica, Benefícios, Riscos e Limitações, Próximos Passos, Relacionado e metadados) está em **[TEMPLATE.md](TEMPLATE.md)** na pasta da skill. Use-o ao criar ADRs quando o projeto não tiver `TEMPLATE.md` na pasta de ADRs.

---

## Comando inicial sugerido

"Estou pronto. Inicie o Passo 1 (com foco no item 1.2 – análise macro) e apresente as propostas."
