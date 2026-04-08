#!/usr/bin/env bash
#
# Testa o endpoint POST /api/enrollments (AddNewEnrollment).
#
# Uso:
#   ./test-add-enrollment.sh
#   NAME="Maria" EMAIL="maria@test.com" ./test-add-enrollment.sh
#   BASE_URL=http://localhost:5035 BIRTH_DATE="1995-01-20" ./test-add-enrollment.sh
#
# Variáveis de ambiente (opcionais):
#   BASE_URL     - Base da API (default: http://localhost:5035)
#   NAME         - Nome
#   EMAIL        - E-mail
#   PHONE        - Telefone
#   DOCUMENT     - Documento (CPF)
#   BIRTH_DATE   - Data de nascimento (ISO 8601, ex: 1990-05-15)
#   GENDER       - Gênero (ex: M, F)
#   ADDRESS      - Endereço
#

set -e

BASE_URL="${BASE_URL:-http://localhost:5035}"
NAME="${NAME:-João Silva}"
EMAIL="${EMAIL:-joao@example.com}"
PHONE="${PHONE:-11999990000}"
DOCUMENT="${DOCUMENT:-12345678900}"
BIRTH_DATE="${BIRTH_DATE:-1990-05-15}"
GENDER="${GENDER:-M}"
ADDRESS="${ADDRESS:-Rua Exemplo, 100}"

URL="${BASE_URL}/api/enrollments"

if command -v jq >/dev/null 2>&1; then
  BODY=$(jq -n \
    --arg name "$NAME" \
    --arg email "$EMAIL" \
    --arg phone "$PHONE" \
    --arg document "$DOCUMENT" \
    --arg birthDate "$BIRTH_DATE" \
    --arg gender "$GENDER" \
    --arg address "$ADDRESS" \
    '{ name: $name, email: $email, phone: $phone, document: $document, birthDate: $birthDate, gender: $gender, address: $address }')
else
  # Fallback sem jq: monta JSON escapando aspas duplas nos valores
  escape_json() { printf '%s' "$1" | sed 's/\\/\\\\/g; s/"/\\"/g'; }
  BODY=$(printf '{"name":"%s","email":"%s","phone":"%s","document":"%s","birthDate":"%s","gender":"%s","address":"%s"}' \
    "$(escape_json "$NAME")" \
    "$(escape_json "$EMAIL")" \
    "$(escape_json "$PHONE")" \
    "$(escape_json "$DOCUMENT")" \
    "$(escape_json "$BIRTH_DATE")" \
    "$(escape_json "$GENDER")" \
    "$(escape_json "$ADDRESS")")
fi

echo "POST $URL"
echo "---"
RESPONSE=$(curl -sS -w "\n%{http_code}" -X POST "$URL" \
  -H "Content-Type: application/json" \
  -d "$BODY")
# sed '$d' remove a última linha (compatível com macOS e Linux); head -n -1 só no GNU
HTTP_BODY=$(echo "$RESPONSE" | sed '$d')
HTTP_CODE=$(echo "$RESPONSE" | tail -n 1)
echo "HTTP $HTTP_CODE"
echo "$HTTP_BODY"
