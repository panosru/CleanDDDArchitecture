# This Makefile is used to manage Docker resources for the project.

# .SILENT suppresses command echo. The commands will not be printed to the terminal before they are executed.
.SILENT:

# .PHONY is used to declare phony targets which do not represent a file.
.PHONY: help core monolith microservices microservices-eventing eventing observability admin smoke-test clean reset hard-clean

# Environment variables for Docker compose files and env file.
ENV_FILE ?= $(if $(wildcard .env.local),.env.local,.env.example)
COMPOSE_FILE ?= compose.yaml

# Docker compose command with env file option.
DOCKER_COMPOSE=docker compose --env-file ${ENV_FILE}

# Display help information about this Makefile.
help:
	@echo "Available targets:"
	@echo "  core         		: Start shared infrastructure"
	@echo "  monolith     		: Start the monolith demo stack"
	@echo "  microservices		: Start the gateway + service demo stack"
	@echo "  microservices-eventing	: Start microservices with Kafka + EventStore"
	@echo "  eventing     		: Start eventing dependencies"
	@echo "  observability		: Start Grafana and Loki"
	@echo "  admin       		: Start admin tooling"
	@echo "  pull-dotnet 		: Pull dotnet images"
	@echo "  smoke-test   		: Run compose smoke checks"
	@echo "  clean       		: Clean Docker resources"
	@echo "  reset       		: Reset Docker volumes"
	@echo "  hard-clean  		: Hard clean Docker resources"
	@echo ""
	@echo "Local overrides:"
	@echo "  Copy .env.local.example to .env.local to override host ports on machines"
	@echo "  that already run Postgres, Redis, or other conflicting services."

core:
	echo "Starting core infrastructure..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile core up -d --build --remove-orphans

admin:
	echo "Starting admin tooling..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile core --profile admin --profile eventing up -d --remove-orphans

# Pull dotnet images from Microsoft container registry.
pull-dotnet:
	echo "Pulling dotnet..."
	docker pull mcr.microsoft.com/dotnet/aspnet:10.0-alpine
	docker pull mcr.microsoft.com/dotnet/sdk:10.0-alpine

monolith: pull-dotnet
	echo "Starting monolith demo..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile core --profile eventing --profile monolith up -d --build --remove-orphans

microservices: pull-dotnet
	echo "Starting microservices demo..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile core --profile microservices up -d --build --remove-orphans

microservices-eventing: pull-dotnet
	echo "Starting microservices demo with eventing..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile core --profile eventing --profile microservices up -d --build --remove-orphans

eventing:
	echo "Starting eventing dependencies..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile eventing up -d --remove-orphans

observability:
	echo "Starting observability stack..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} --profile observability up -d --remove-orphans

smoke-test:
	echo "Running smoke tests..."
	../scripts/smoke-test.sh

# Clean Docker resources by bringing down services and removing orphan containers.
clean:
	echo "Cleaning..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} down --remove-orphans

# Reset Docker volumes by cleaning resources and pruning volumes.
reset: clean
	echo "Removing volumes..."
	docker volume ls -qf name=cleandddarchitecture | awk '{print $$1}' | xargs -r docker volume rm -f

# Perform a hard clean by resetting resources, pruning networks, and pruning all unused Docker objects.
hard-clean: reset
	echo "Hard cleaning..."
	docker network prune -f
	docker system prune -f
