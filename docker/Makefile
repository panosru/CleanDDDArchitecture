# This Makefile is used to manage Docker resources for the project.

# .SILENT suppresses command echo. The commands will not be printed to the terminal before they are executed.
.SILENT:

# .PHONY is used to declare phony targets which do not represent a file.
.PHONY: help infrastructure admin pull-dotnet api web worker clean reset hard-clean

# Environment variables for Docker compose files and env file.
ENV_FILE ?= .env.dev
COMPOSE_FILE ?= compose.yaml
COMPOSE_LOCAL_FILE ?= compose.local.yaml
COMPOSE_ADMIN_FILE ?= compose.admin.yaml
COMPOSE_STAGING_FILE ?= compose.staging.yaml

# Docker compose command with env file option.
DOCKER_COMPOSE=docker-compose --env-file ${ENV_FILE}

# Display help information about this Makefile.
help:
	@echo "Available targets:"
	@echo "  infrastructure	: Create infrastructure"
	@echo "  admin       		: Create admin"
	@echo "  pull-dotnet 		: Pull dotnet images"
	@echo "  api         		: Create API service"
	@echo "  web         		: Create web service"
	@echo "  worker     	 	: Create worker service"
	@echo "  clean       		: Clean Docker resources"
	@echo "  reset       		: Reset Docker volumes"
	@echo "  hard-clean  		: Hard clean Docker resources"

# Create infrastructure using Docker compose.
infrastructure:
	echo "Creating infrastructure..."
	${DOCKER_COMPOSE} -f ${COMPOSE_FILE} -f ${COMPOSE_LOCAL_FILE} up --force-recreate --build --remove-orphans

# Create admin using Docker compose.
admin:
	echo "Creating admin..."
	${DOCKER_COMPOSE} -f ${COMPOSE_ADMIN_FILE} up --force-recreate

# Pull dotnet images from Microsoft container registry.
pull-dotnet:
	echo "Pulling dotnet..."
	docker pull mcr.microsoft.com/dotnet/aspnet:8.0-alpine
	docker pull mcr.microsoft.com/dotnet/sdk:8.0-alpine

# Create API service using Docker compose after pulling dotnet images.
api: pull-dotnet
	echo "Creating api..."
	${DOCKER_COMPOSE} -f ${COMPOSE_STAGING_FILE} up rest-api --force-recreate --build

# Create web service using Docker compose after pulling dotnet images.
web: pull-dotnet
	echo "Creating web..."
	${DOCKER_COMPOSE} -f ${COMPOSE_STAGING_FILE} up web-app --force-recreate --build

# Create worker server using Docker compose after pulling dotnet images.
worker: pull-dotnet
	echo "Creating worker..."
	${DOCKER_COMPOSE} -f ${COMPOSE_STAGING_FILE} up worker --force-recreate --build

# Clean Docker resources by bringing down services and removing orphan containers.
clean:
	echo "Cleaning..."
	docker-compose down --remove-orphans

# Reset Docker volumes by cleaning resources and pruning volumes.
reset: clean
	echo "Removing volumes..."
	docker volume ls -qf name=CleanDDDArchitecture- | awk '{print $1}' | xargs -r docker volume rm -f

# Perform a hard clean by resetting resources, pruning networks, and pruning all unused Docker objects.
hard-clean: reset
	echo "Hard cleaning..."
	docker network prune -f
	docker system prune -f
