# Template de mensagem de commit

Padrão: **Conventional Commits** (uma linha de assunto obrigatória, corpo opcional).

## Estrutura

```
<tipo>(<escopo>): <descrição em imperativo>

[corpo opcional - o que e por quê]

[rodapé opcional - Breaking Changes, refs]
```

- **tipo:** obrigatório; indica o tipo da mudança.
- **escopo:** opcional; módulo/área afetada (ex.: `api`, `auth`, `ui`).
- **descrição:** obrigatória; verbo no imperativo, sem ponto final, ~50–72 caracteres.

## Tipos

| Tipo     | Uso |
|----------|-----|
| `feat`   | Nova funcionalidade |
| `fix`    | Correção de bug |
| `docs`   | Só documentação |
| `style`  | Formatação, espaços, sem mudança de lógica |
| `refactor` | Refatoração (nem feat nem fix) |
| `test`   | Adição ou ajuste de testes |
| `chore`  | Build, CI, deps, tarefas gerais |

## Exemplos

**Uma linha:**

```
feat(enrollment): adiciona endpoint de listagem de matrículas
fix(payment): corrige cálculo de juros em atraso
docs: atualiza README com instruções de deploy
chore: atualiza pacotes NuGet do projeto Api
```

**Com corpo:**

```
feat(auth): adiciona login com JWT

- Endpoint POST /auth/login
- Validação de credenciais
- Retorno de access token e refresh token
```

**Breaking change (rodapé):**

```
refactor(api): remove suporte a API v1

BREAKING CHANGE: clientes devem migrar para /v2
```

## Checklist para a mensagem

- [ ] Tipo correto para a mudança
- [ ] Escopo preenchido quando fizer sentido
- [ ] Descrição no imperativo (“adiciona” e não “adicionado”)
- [ ] Sem ponto final na linha de assunto
- [ ] Corpo usado só quando precisar explicar o quê/porquê
