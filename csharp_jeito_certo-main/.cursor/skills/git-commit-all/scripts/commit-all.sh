#!/usr/bin/env bash
# commit-all.sh — Adiciona todas as alterações (add, edit, delete) e commita com mensagem padronizada.
# Uso: ./commit-all.sh "tipo(escopo): descrição"
# Deve ser executado na raiz do repositório git.

set -e

if [ -z "$1" ]; then
  echo "Uso: $0 \"<mensagem de commit>\"" >&2
  echo "Exemplo: $0 \"feat(api): adiciona endpoint de matrículas\"" >&2
  exit 1
fi

MSG="$1"
ROOT_GIT="$(git rev-parse --show-toplevel 2>/dev/null)" || true

if [ -z "$ROOT_GIT" ]; then
  echo "Erro: não é um repositório git." >&2
  exit 1
fi

cd "$ROOT_GIT"
git add -A

if git diff --staged --quiet 2>/dev/null; then
  echo "Nenhuma alteração para commitar (working tree limpo)."
  exit 0
fi

echo "Alterações que serão commitadas:"
git status --short
echo ""
git commit -m "$MSG"
echo "Commit realizado com sucesso."
