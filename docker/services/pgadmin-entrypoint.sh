#!/bin/sh
set -eu

PGADMIN_SERVER_JSON_FILE="${PGADMIN_SERVER_JSON_FILE:-/pgadmin4/servers.json}"
PGADMIN_PGPASS_FILE="${PGADMIN_PGPASS_FILE:-/tmp/pgpassfile}"

/bin/echo "postgres:5432:*:postgres:${POSTGRES_PASSWORD:-change-me}" > "${PGADMIN_PGPASS_FILE}"
chmod 600 "${PGADMIN_PGPASS_FILE}"

/entrypoint.sh &
pgadmin_pid=$!

cleanup() {
    kill "${pgadmin_pid}" 2>/dev/null || true
}

trap cleanup INT TERM

for _ in $(seq 1 30); do
    if [ -f /var/lib/pgadmin/pgadmin4.db ]; then
        break
    fi

    sleep 1
done

if [ -f "${PGADMIN_SERVER_JSON_FILE}" ]; then
    if [ "${PGADMIN_CONFIG_SERVER_MODE:-True}" = "False" ]; then
        /venv/bin/python3 /pgadmin4/setup.py load-servers "${PGADMIN_SERVER_JSON_FILE}" --replace
    else
        /venv/bin/python3 /pgadmin4/setup.py load-servers "${PGADMIN_SERVER_JSON_FILE}" --user "${PGADMIN_DEFAULT_EMAIL}" --replace
    fi
fi

wait "${pgadmin_pid}"
