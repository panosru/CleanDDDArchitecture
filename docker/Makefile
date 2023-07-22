.SILENT:
.PHONY: infra admin pull-dotnet api web clean reset hard-clean

ENV_FILE ?= .env.dev
COMPOSE_FILE ?= compose.yaml
COMPOSE_LOCAL_FILE ?= compose.local.yaml
COMPOSE_ADMIN_FILE ?= compose.admin.yaml
COMPOSE_STAGING_FILE ?= compose.staging.yaml

infra:
	echo "Creating infrastructure..."
	docker-compose --env-file ${ENV_FILE} -f ${COMPOSE_FILE} -f ${COMPOSE_LOCAL_FILE} up --force-recreate --build --remove-orphans

admin:
	echo "Creating admin..."
	docker-compose --env-file ${ENV_FILE} -f ${COMPOSE_ADMIN_FILE} up --force-recreate

pull-dotnet:
	echo "Pulling dotnet..."
	docker pull mcr.microsoft.com/dotnet/aspnet:7.0-alpine
	docker pull mcr.microsoft.com/dotnet/sdk:7.0-alpine

api: pull-dotnet
	echo "Creating api..."
	docker-compose --env-file ${ENV_FILE} -f ${COMPOSE_STAGING_FILE} up rest-api --force-recreate --build

web: pull-dotnet
	echo "Creating web..."
	docker-compose --env-file ${ENV_FILE} -f ${COMPOSE_STAGING_FILE} up web-app --force-recreate --build

clean:
	echo "Cleaning..."
	docker-compose down --remove-orphans

reset: clean
	echo "Resetting..."
	docker volume prune -f

hard-clean: reset
	echo "Hard cleaning..."
	docker network prune -f
	docker system prune -f
