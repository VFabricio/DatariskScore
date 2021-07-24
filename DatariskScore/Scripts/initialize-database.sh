set -x
set -eo pipefail

if ! [ -x "$(command -v psql)" ]; then
  echo >&2 "Error: `psql` is not installed."
  exit 1
fi

POSTGRES_USER=postgres
POSTGRES_PASSWORD=password
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DATABASE=datarisk

CONNECTION="postgresql://${POSTGRES_USER}:${POSTGRES_PASSWORD}@${POSTGRES_HOST}:${POSTGRES_PORT}"
CONNECTION_WITH_DB="${CONNECTION}/${POSTGRES_DATABASE}"

SCRIPT_DIR=$(dirname "${BASH_SOURCE[0]}")

psql ${CONNECTION} -a -f "${SCRIPT_DIR}/../Database/create-database.sql"
psql ${CONNECTION_WITH_DB} -a -f "${SCRIPT_DIR}/../Database/create-score-table.sql"
