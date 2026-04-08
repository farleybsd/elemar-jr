---
name: test-open-telemetry
description: Garante que o compose do workshop Coreografia - with observability está em execução, abre o Aspire Dashboard com login por token e executa a funcionalidade AddNewEnrollment via test-add-enrollment.sh com dados aleatórios válidos. Use quando o usuário pedir para validar o ambiente compose, subir serviços, abrir o Aspire ou testar a inscrição no projeto Coreografia - with observability.
---

# Coreografia Compose – validação E2E

Skill para o projeto **Coreografia - with observability** (compose com GymErp, Postgres, Kafka, Aspire Dashboard). Garante serviços no ar, abre o Aspire com token e dispara uma nova inscrição com dados aleatórios.

## Escopo

- Base: `Modulo 02/Workshop 03/Coreografia - with observability/`
- Compose: `docker-compose.yml` na mesma pasta
- Script de teste: `GymErp/Domain/Subscriptions/Features/AddNewEnrollment/test-add-enrollment.sh`
- API: `http://localhost:5035`
- Aspire UI: `http://localhost:18888`

## 0. Detectar containers já em execução (e decidir rebuild do app)

A partir da pasta do workshop: `Modulo 02/Workshop 03/Coreografia - with observability/`, o agente deve:

1. Verificar se `docker compose` já está com serviços rodando (especialmente `gymerp`):

```bash
cd "Modulo 02/Workshop 03/Coreografia - with observability"
docker compose ps
```

2. Se houver indícios de containers em execução (por exemplo, `gymerp`/`postgres`/`kafka`/`aspire-dashboard` como `running`), perguntar ao usuário:

**"Os containers do workshop já parecem estar em execução. Deseja reconstruir (re-build) o container da aplicação `gymerp`? (Sim/Não)"**

3. Se o usuário responder **Sim**:

- Parar tudo:

```bash
docker compose down
```

- Subir novamente fazendo build do app (`gymerp`) e garantindo que dependências iniciem:

```bash
docker compose up -d --build gymerp
```

4. Se o usuário responder **Não**:

- Manter a execução atual e apenas garantir que os serviços estão no ar:

```bash
docker compose up -d
```

## 1. Garantir que os serviços estão executando (pós-rebuild/opcional)

A partir da raiz do repositório:

```bash
cd "Modulo 02/Workshop 03/Coreografia - with observability"
docker compose up -d --build
docker compose ps
```

Todos os serviços (postgres, zookeeper, kafka, kafka-init, aspire-dashboard, gymerp) devem estar `running` (ou `exited` apenas para kafka-init). Se `gymerp` estiver unhealthy ou reiniciando, verificar logs: `docker compose logs gymerp --tail=100`.

Opcional – aguardar a API responder antes de seguir:

```bash
curl -sS -o /dev/null -w "%{http_code}" http://localhost:5035/healthz
# Esperar 200
```

## 2. Abrir o Aspire Dashboard (login com token) - opcional

Após garantir o estado dos containers no passo anterior, o agente deve perguntar:

**"Deseja abrir o Aspire Dashboard no browser já logado com o token? (Sim/Não)"**

Se responder **Sim**:

O Aspire exige token na URL. O token aparece nos logs do container ao subir.

**Obter o token:**

```bash
cd "Modulo 02/Workshop 03/Coreografia - with observability"
docker compose logs aspire-dashboard 2>/dev/null | grep -oE 'login\?t=[a-f0-9]+' | head -1
```

Saída típica: `login?t=4e560688528c7e2c35bd2d714a249011` (o valor muda a cada subida).

**URL completa:** `http://localhost:18888/login?t=<TOKEN>`

O agente deve abrir essa URL no browser (navegação MCP ou instruir o usuário). Se usar MCP browser: navegar para a URL com o token; isso já autentica e evita a tela de login manual.

Se responder **Não**: pular o passo de abertura do browser e seguir para o `test-add-enrollment.sh`.

## 3. Executar AddNewEnrollment com dados aleatórios

O script `test-add-enrollment.sh` aceita variáveis de ambiente. O domínio exige: **nome com pelo menos 10 caracteres**, **CPF válido (11 dígitos, não repetidos, dígitos verificadores corretos)**, **e-mail válido**, **telefone 10–11 dígitos**, **data nascimento ISO**, **gênero (ex.: M, F)**, **endereço**.

**Gerar dados válidos e chamar o script:**

- **CPF válido de teste (exemplo):** `529.982.247-25` (apenas dígitos: `52998224725`).
- **Nome:** mínimo 10 caracteres (ex.: "Maria Silva Santos", "João Pedro Oliveira").
- **E-mail:** formato válido (ex.: `maria.silva@example.com`).
- **Telefone:** 10 ou 11 dígitos (ex.: `11987654321`).
- **Data:** ISO 8601 (ex.: `1995-03-20`).
- **Gênero:** `M` ou `F`.
- **Endereço:** qualquer string não vazia.

**Exemplo de execução com dados "aleatórios" (variáveis explícitas):**

A partir da raiz do repositório:

```bash
cd "Modulo 02/Workshop 03/Coreografia - with observability/GymErp/Domain/Subscriptions/Features/AddNewEnrollment"
BASE_URL=http://localhost:5035 \
NAME="Maria Silva Santos" \
EMAIL="maria.silva@example.com" \
PHONE="11987654321" \
DOCUMENT="52998224725" \
BIRTH_DATE="1995-03-20" \
GENDER="F" \
ADDRESS="Rua das Flores, 100" \
./test-add-enrollment.sh
```

Para variar em múltiplas execuções, alterar NAME, EMAIL, PHONE, DOCUMENT (usar outro CPF válido), BIRTH_DATE, ADDRESS. Pode usar outros CPFs de teste válidos (gerados por algoritmo ou listas conhecidas) para evitar duplicidade de documento.

**Script em linha única (alternativa):**

```bash
cd "Modulo 02/Workshop 03/Coreografia - with observability"
BASE_URL=http://localhost:5035 NAME="João Pedro Oliveira" EMAIL="joao.oliveira@example.com" PHONE="21999887766" DOCUMENT="52998224725" BIRTH_DATE="1990-07-15" GENDER="M" ADDRESS="Av. Brasil, 500" ./GymErp/Domain/Subscriptions/Features/AddNewEnrollment/test-add-enrollment.sh
```

A resposta esperada é HTTP 200 e um GUID no corpo (id da matrícula criada).

## Ordem sugerida do fluxo completo

1. Subir/verificar o compose e `docker compose ps`.
2. Se containers já estiverem em execução: perguntar e, se necessário, fazer rebuild do `gymerp` (com `docker compose down` + `up -d --build gymerp`).
3. (Opcional) Verificar saúde da API: `curl -sS http://localhost:5035/healthz`.
4. Depois do estado final: perguntar se deve abrir o Aspire no browser já logado com token.
5. Executar `test-add-enrollment.sh` com variáveis de ambiente preenchidas com dados válidos (aleatórios ou de exemplo).
6. Confirmar resposta 200 e GUID; no Aspire Dashboard podem aparecer novos traces/metrics da requisição.

## Referência rápida – validações do domínio (Enrollment)

- **CPF:** 11 dígitos; não pode ser repetido (111.111.111-11); dígitos verificadores corretos.
- **Nome:** não vazio, trim length >= 10.
- **E-mail:** formato válido (MailAddress).
- **Telefone:** 10 ou 11 dígitos (apenas números).

Se o script retornar 400, revisar esses campos nos dados passados.
