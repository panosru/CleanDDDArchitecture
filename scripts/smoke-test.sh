#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
COMPOSE_FILE="${ROOT_DIR}/docker/compose.yaml"
ENV_FILE="${ROOT_DIR}/docker/.env.local"
if [[ ! -f "${ENV_FILE}" ]]; then
  ENV_FILE="${ROOT_DIR}/docker/.env.example"
fi

function compose_down() {
  docker compose --env-file "${ENV_FILE}" -f "${COMPOSE_FILE}" down --remove-orphans >/dev/null 2>&1 || true
}

function wait_for_url() {
  local url="$1"
  local attempts="${2:-40}"
  for ((i = 1; i <= attempts; i++)); do
    if curl -fsS "${url}" >/dev/null 2>&1; then
      return 0
    fi
    sleep 3
  done
  echo "Timed out waiting for ${url}" >&2
  return 1
}

trap compose_down EXIT

docker compose --env-file "${ENV_FILE}" -f "${COMPOSE_FILE}" --profile core --profile eventing --profile monolith up -d --build
wait_for_url "http://localhost:${REST_API_PORT:-8081}/health"
wait_for_url "http://localhost:${WEB_APP_PORT:-8082}"
compose_down

docker compose --env-file "${ENV_FILE}" -f "${COMPOSE_FILE}" --profile core --profile eventing --profile microservices up -d --build
wait_for_url "http://localhost:${ACCOUNT_SERVICE_PORT:-5201}/health"
wait_for_url "http://localhost:${TODO_SERVICE_PORT:-5202}/health"
wait_for_url "http://localhost:${WEATHER_SERVICE_PORT:-5203}/health"
wait_for_url "http://localhost:${GATEWAY_PORT:-8080}/health"
