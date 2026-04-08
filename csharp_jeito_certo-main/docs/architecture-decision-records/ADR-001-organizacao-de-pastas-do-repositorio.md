# ADR-001: Organização de pastas do repositório

## Contexto

O repositório **C# Jeito Certo** é um monorepo de **material didático** de um **treinamento sobre C#** e .NET: exemplos e workshops pensados para apoiar as aulas, com foco em boas práticas, arquitetura e comunicação entre serviços. A estrutura de pastas existe, em primeiro lugar, para **organizar o conteúdo do curso** — ou seja, para que módulos, workshops e variantes de exemplo sigam uma ordem e um lugar previsíveis, facilitando o uso em sala (ou em estudo autônomo).

Como o objetivo é **didático**, e não a entrega de um produto único em produção, **aceitamos de forma explícita**:

- **Duplicação de código** entre pastas/samples, quando isso mantém cada exemplo autocontido e fácil de abrir e rodar isoladamente.
- **Eventuais “más práticas”** ou código deliberadamente simplificado/imperfeito em alguns exemplos, quando isso serve para contrastar com a versão “certa” em outra pasta (ex.: controller-based vs vertical slice) ou para não sobrecarregar o aluno com complexidade naquele momento do curso.

A estrutura foi desenhada para:

- **Separar o conteúdo por módulo e por workshop** — módulos são as unidades didáticas do curso; workshops são os blocos práticos dentro de cada módulo. Essa separação é a espinha dorsal da organização do material.
- Abrigar múltiplas **variantes** de um mesmo tema (várias arquiteturas, padrões de integração, etc.) em pastas distintas, cada uma com sua própria solução (.sln) e projetos.
- Manter documentação de decisões de arquitetura em um local único e previsível na raiz (`docs/architecture-decision-records/`).
- Facilitar que alunos e instrutores localizem rapidamente o exemplo correto (por módulo → workshop → pasta da variante).

Alternativas consideradas: um único nível de pastas com todos os projetos; organização apenas por tipo de arquitetura; múltiplos repositórios (um por módulo). A opção escolhida foi **um único repositório (monorepo) organizado por Módulo → Workshop → pasta da variante/sample**.

## Decisão

Adotar uma **organização hierárquica em três níveis** na raiz do repositório, com **módulos e workshops como eixo da organização do conteúdo**:

1. **Módulo** — primeira divisão do material do treinamento. Pastas `Modulo 01`, `Modulo 02`, `Modulo 03` correspondem às unidades principais do curso; a pasta `Masterclass` reúne **aulas adicionais** (ver adendo abaixo). Cada módulo agrupa um ou mais workshops.
2. **Workshop** — dentro de cada módulo, uma pasta por workshop (ex.: `Workshop 01`, `Workshop 03`). Os workshops são os blocos práticos em que o conteúdo é entregue; a organização por workshop permite seguir a ordem das aulas e localizar o exemplo certo para cada etapa do treinamento.
3. **Variante ou sample** — dentro de cada workshop, uma pasta por “versão” do exemplo: uma arquitetura (Layered, Clean Architecture, Vertical Slice), um padrão de integração (Orquestração Síncrona, Coreografia com Kafka/RabbitMQ, Outbox) ou um sample numerado (sample01, sample02, …). Cada uma dessas pastas contém uma ou mais soluções (.sln) e projetos (.csproj) autocontidos.

Na raiz, manter ainda:

- **docs/** — documentação do repositório; as ADRs ficam em `docs/architecture-decision-records/`.
- **README.md** — visão geral, objetivo e tabelas descrevendo a estrutura por módulo/workshop.
- Arquivos de configuração do repositório (.gitignore, workspace, etc.).

Cada pasta de variante/sample é um **container** independente: pode ser aberta e construída sem depender de outras pastas. Não há compartilhamento de código entre samples; **a duplicação é aceita de forma consciente**, pois o repositório é material didático e a prioridade é que cada exemplo seja autocontido e alinhado à organização do conteúdo (módulo → workshop → variante).

## Padrões Estabelecidos

### 1. **Nomenclatura de pastas de módulo**

- **Módulos do curso:** `Modulo 01`, `Modulo 02`, `Modulo 03` (com espaço e número com zero à esquerda) — organizam o conteúdo das unidades principais do treinamento.
- **Masterclass:** pasta `Masterclass` para **aulas adicionais** do curso e seus exemplos (detalhes no adendo abaixo).

### 2. **Nomenclatura de workshops e variantes**

- Workshops: `Workshop NN` (ex.: `Workshop 01`, `Workshop 03`).
- Variantes por arquitetura ou padrão: nome descritivo com espaços e hífens quando fizer sentido (ex.: `Layered Architecture`, `Vertical Slice Architecture`, `Coreografia - Kafka`, `Coreografia + Outbox - Kafka`).
- Samples numerados: `sample01`, `sample02`, … (minúsculas, zero à esquerda), principalmente em `Masterclass/engenharia_projetos/`.

### 3. **Documentação de decisões**

- ADRs (Architecture Decision Records) ficam exclusivamente em **`docs/architecture-decision-records/`** na raiz do repositório, com nomenclatura `ADR-XXX-slug-do-titulo.md`.

## Implementação Técnica

### **Estrutura de Arquivos**

```
(repositório raiz)
├── README.md
├── docs/
│   └── architecture-decision-records/
│       └── ADR-001-organizacao-de-pastas-do-repositorio.md
├── Modulo 01/
│   └── Workshop 03/
│       ├── FinanceManager - ControllerBased/
│       └── FinanceManager - Vertical Slice/
├── Modulo 02/
│   └── Workshop 03/
│       ├── Orquestracao Sincrona/
│       ├── Coreografia - RabbitMq/
│       ├── Coreografia - RabbitMq - Native/
│       ├── Coreografia - Kafka/
│       ├── Coreografia - Kafka - Native/
│       └── Coreografia + Outbox - Kafka/
├── Modulo 03/
│   └── Workshop 01/
│       ├── Layered Architecture/
│       ├── Clean Architecture/
│       ├── Ports And Adapters/
│       └── Vertical Slice Architecture/
└── Masterclass/
    ├── engenharia_projetos/
    │   ├── sample01/
    │   ├── sample02/
    │   ├── sample03/
    │   ├── sample04/
    │   └── sample05/
    └── memory_profiling/
```

Cada pasta folha (ex.: `Clean Architecture`, `sample05`) contém uma ou mais soluções (.sln) e seus projetos; a estrutura interna pode variar conforme o estilo arquitetural do exemplo.

## Benefícios

### ✅ **Navegação previsível**
- Alunos e instrutores encontram o exemplo pelo caminho Módulo → Workshop → variante, alinhado ao conteúdo do curso.

### ✅ **Containers independentes**
- Cada variante/sample é autocontido; evita dependências cruzadas entre pastas e simplifica build e testes por exemplo.

### ✅ **Documentação centralizada**
- Todas as decisões de arquitetura ficam em `docs/architecture-decision-records/`, facilitando descoberta e manutenção.

### ✅ **Escalabilidade**
- Novos módulos, workshops ou variantes são adicionados como novas pastas na hierarquia existente, sem alterar a convenção.

## Adendo: pasta Masterclass

A pasta **`Masterclass/`** na raiz do repositório é reservada a **aulas adicionais** do curso — conteúdos extras que não compõem o núcleo sequencial dos Módulos 01, 02 e 03. Cada subpasta dentro de `Masterclass/` representa um bloco de aula adicional e contém seus próprios exemplos, em geral organizados em samples numerados (ex.: `engenharia_projetos/sample01`, `sample02`, …) ou por tema (ex.: `memory_profiling`). Os exemplos da Masterclass seguem a mesma convenção de containers independentes: cada sample é autocontido, com sua própria solução e projetos, e pode haver duplicação de código entre samples em prol da clareza didática. A estrutura esperada é:

- **Masterclass/**  
  - *&lt;nome_da_aula_adicional&gt;/* (ex.: `engenharia_projetos`, `memory_profiling`)  
    - *&lt;variante ou sample&gt;/* (ex.: `sample01`, `sample02`, …) — cada um com suas soluções e projetos.

Assim, a organização do conteúdo continua coerente: módulos principais em `Modulo NN/`, aulas adicionais e seus exemplos em `Masterclass/`.

## Riscos e Limitações

- **Duplicação de código** entre samples é **intencional e aceita** (material didático); não deve ser eliminada às custas de quebrar a autonomia de cada pasta ou a organização módulo → workshop → variante.
- **Código didático:** em alguns exemplos podem existir simplificações ou “más práticas” deliberadas para fins de comparação ou progressão pedagógica; isso não indica que o mesmo seja desejável em projetos de produção.
- **Nomes com espaços** (ex.: `Modulo 01`, `FinanceManager - Vertical Slice`) exigem cuidado em scripts e ferramentas que não tratem caminhos com espaços.

## Próximos Passos

1. Manter o README.md atualizado com novas pastas quando forem criados módulos, workshops ou variantes.
2. Documentar decisões arquiteturais relevantes em novos ADRs em `docs/architecture-decision-records/`.

## Relacionado

- **README.md** (raiz do repositório): descreve a estrutura dos projetos por módulo e workshop.

---

**Data:** 09/03/2025  
**Decisão Tomada por:** Equipe do curso C# Jeito Certo  
**Status:** Ativa
