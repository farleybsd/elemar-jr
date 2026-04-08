#!/usr/bin/env bash
#
# Testa o endpoint POST /api/payments/one-off (ProcessCharging).
#
# Uso:
#   ./test-process-charging.sh
#   BASE_URL=http://localhost:5035 ./test-process-charging.sh
#
# Variaveis de ambiente (opcionais):
#   BASE_URL     - Base da API (default: http://localhost:5035)
#

set -e

BASE_URL="${BASE_URL:-http://localhost:5035}"
URL="${BASE_URL}/api/payments/one-off"

echo "POST $URL"
echo "---"
RESPONSE=$(curl -sS -w "\n%{http_code}" -X POST "$URL" \
  -H "Content-Type: application/json")
# sed '$d' remove a ultima linha (compativel com macOS e Linux); head -n -1 so no GNU
HTTP_BODY=$(echo "$RESPONSE" | sed '$d')
HTTP_CODE=$(echo "$RESPONSE" | tail -n 1)
echo "HTTP $HTTP_CODE"
echo "$HTTP_BODY"

