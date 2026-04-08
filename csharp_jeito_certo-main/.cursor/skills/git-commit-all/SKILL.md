---
name: git-commit-all
description: Realiza commit de todas as alterações do repositório (add, edit, delete) com mensagem padronizada. Use quando o usuário pedir para commitar alterações, fazer commit de tudo, ou salvar mudanças com mensagem padronizada.
---

# Commit de todas as alterações

Realiza `git add` de todos os arquivos alterados e executa `git commit` com mensagem no formato definido no template.

## Quando usar

- Usuário pede para commitar todas as alterações
- Usuário pede commit com mensagem padronizada
- Há mudanças (novos, editados, removidos) e deseja um único commit padronizado

## Template da mensagem

Use o formato **Conventional Commits**. Template completo em [MESSAGE_TEMPLATE.md](MESSAGE_TEMPLATE.md).

**Formato resumido:**

```
<tipo>(<escopo opcional>): <descrição curta>

<corpo opcional>
```

**Tipos comuns:** `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`.

**Exemplos rápidos:**

- `feat(auth): adiciona login com JWT`
- `fix(api): corrige serialização de datas`
- `chore: atualiza dependências`

## Fluxo

1. **Verificar estado:** `git status` para listar alterações.
2. **Definir mensagem:** Com base nas alterações, montar a mensagem conforme [MESSAGE_TEMPLATE.md](MESSAGE_TEMPLATE.md).
3. **Executar commit:** Rodar o script passando a mensagem.

## Script

O script está em `scripts/commit-all.sh`. **Executar** a partir da raiz do repositório.

**Uso:**

```bash
# Mensagem em uma linha (escapar aspas no shell)
./.cursor/skills/git-commit-all/scripts/commit-all.sh "feat(api): adiciona endpoint de matrículas"

# Ou com variável
MSG="fix: corrige formatação de datas"
./.cursor/skills/git-commit-all/scripts/commit-all.sh "$MSG"
```

**Comportamento do script:**

- Faz `git add -A` (inclui add, edit e delete)
- Exibe `git status --short` antes de commitar
- Executa `git commit -m "<mensagem>"`
- Se não houver alterações, não tenta commit e informa

**Permissão:** Garantir que o script é executável: `chmod +x .cursor/skills/git-commit-all/scripts/commit-all.sh`

## Regras

- Sempre usar o template de mensagem; não commitar com mensagem vazia ou genérica.
- Uma linha de assunto com até ~72 caracteres; detalhes no corpo se necessário.
- Escrever a mensagem em português (PT-BR), salvo convenção diferente do projeto.
