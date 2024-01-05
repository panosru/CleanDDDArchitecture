# This Makefile is silent by default, use `make -s` to override
.SILENT:

# Phony targets to avoid file name conflict
.PHONY: help run RestApi WebApp Worker sonar \
		account-db-update account-db-drop account-migrations-add account-migrations-update account-migrations-apply \
		account-migrations-rollback account-migrations-list account-migrations-remove account-migrations-remove-all \
		todo-db-update todo-db-drop todo-migrations-add todo-migrations-update todo-migrations-apply \
		todo-migrations-rollback todo-migrations-list todo-migrations-remove todo-migrations-remove-all

# Include environment variables from .env file
include .env
export $(shell sed 's/=.*//' .env)

# Set default environment
ENV ?= Development

# Export DOTNET_ENVIRONMENT and ASPNETCORE_ENVIRONMENT
export DOTNET_ENVIRONMENT := $(ENV)
export ASPNETCORE_ENVIRONMENT := $(ENV)

# Display help information about this Makefile.
help:
	@echo "Available targets:"
	@echo "  run				: Run a host project like so: make run app=RestApi"
	@echo "  RestApi			: Run RestApi Host"
	@echo "  WebApp			: Run WebApp Host"
	@echo "  Worker			: Run Worker"
	@echo "  sonar				: Run SonarQube analysis"
	@echo "  db-reset			: Full database reset"
	@echo "  db-up				: Upping database"
	@echo "  account-db-update		: Update Account database"
	@echo "  account-db-drop		: Drop Account database"
	@echo "  account-migrations-add	: Add Account database migration"
	@echo "  account-migrations-update	: Update Account database migration"
	@echo "  account-migrations-apply	: Apply Account database migration"
	@echo "  account-migrations-rollback	: Rollback Account database migration"
	@echo "  account-migrations-list	: List Account database migrations"
	@echo "  account-migrations-remove	: Remove Account database migration"
	@echo "  account-migrations-remove-all	: Remove all Account database migrations"
	@echo "  todo-db-update		: Update Todo database"
	@echo "  todo-db-drop			: Drop Todo database"
	@echo "  todo-migrations-add		: Add Todo database migration"
	@echo "  todo-migrations-update	: Update Todo database migration"
	@echo "  todo-migrations-apply		: Apply Todo database migration"
	@echo "  todo-migrations-rollback	: Rollback Todo database migration"
	@echo "  todo-migrations-list		: List Todo database migrations"
	@echo "  todo-migrations-remove	: Remove Todo database migration"
	@echo "  todo-migrations-remove-all	: Remove all Todo database migrations"


	

# Run a host project
run:
	@echo "Running $(app) host"
	dotnet run --project Hosts/$(app)/Presentation --environment $(ENV)

# Run RestApi Host
RestApi:
	$(MAKE) run app=RestApi

# Run WebApp Host
WebApp:
	$(MAKE) run app=WebApp

# Run Worker Project
Worker:
	@echo "Running Worker"
	dotnet run --project Hosts/Worker --environment $(ENV)

# Target for running SonarQube analysis
sonar:
	# Echo a message
	echo "Running SonarQube analysis"
	
	# Begin SonarQube analysis
	# If this command fails, the make command will stop
	dotnet sonarscanner begin /k:"${SONAR_PROJECT_NAME}" /d:sonar.host.url="${SONAR_HOST_URL}"  /d:sonar.login="${SONAR_LOGIN}" || exit 1
	
	# Build the project
	# If this command fails, the make command will stop
	dotnet build || exit 1
	
	# End SonarQube analysis
	# If this command fails, the make command will stop
	dotnet sonarscanner end /d:sonar.login="${SONAR_LOGIN}" || exit 1

# Full database reset
db-reset:
	echo "Resetting database"
	$(MAKE) account-db-drop
	$(MAKE) account-migrations-remove-all
	$(MAKE) todo-db-drop
	$(MAKE) todo-migrations-remove-all

db-up:
	echo "Upping database"
	$(MAKE) account-db-update
	$(MAKE) account-migrations-add
	$(MAKE) account-migrations-apply
	$(MAKE) todo-db-update
	$(MAKE) todo-migrations-add
	$(MAKE) todo-migrations-apply

# Target for updating Account database
account-db-update:
	echo "Updating Account database"
	dotnet ef database update --project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for dropping Account database
account-db-drop:
	echo "Dropping Account database"
	yes | dotnet ef database drop --project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for adding Account database migration
account-migrations-add:
	echo "Adding Account database migration"
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef migrations add $(DB_MIGRATION_NAME) --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --output-dir Persistence/Migrations --context AccountDbContextWrite || exit 1

# Target for updating Account database migration
account-migrations-update:
	echo "Updating Account database migration"
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef migrations script --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for applying Account database migration
account-migrations-apply:
	echo "Applying Account database migration"
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for rolling back Account database migration
account-migrations-rollback:
	echo "Rolling back Account database migration"
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for listing Account database migrations
account-migrations-list:
	echo "Listing Account database migrations"
	dotnet ef migrations list --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for removing Account database migration
account-migrations-remove:
	echo "Removing Account database migration"
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef migrations remove --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1

# Target for removing all Account database migrations
account-migrations-remove-all:
	echo "Removing all Account database migrations"
	dotnet ef migrations remove --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Presentation --context AccountDbContextWrite || exit 1
	rm -rf Domains/Account/Infrastructure/Persistence/Migrations/*.cs || exit 1

# Target for updating Todo database
todo-db-update:
	echo "Updating Todo database"
	dotnet ef database update --project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for dropping Todo database
todo-db-drop:
	echo "Dropping Todo database"
	yes | dotnet ef database drop --project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for adding Todo database migration
todo-migrations-add:
	echo "Adding Todo database migration"
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef migrations add $(DB_MIGRATION_NAME) --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --output-dir Persistence/Migrations --context TodoDbContextWrite || exit 1

# Target for updating Todo database migration
todo-migrations-update:
	echo "Updating Todo database migration"
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef migrations script --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for applying Todo database migration
todo-migrations-apply:
	echo "Applying Todo database migration"
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for rolling back Todo database migration
todo-migrations-rollback:
	echo "Rolling back Todo database migration"
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for listing Todo database migrations
todo-migrations-list:
	echo "Listing Todo database migrations"
	dotnet ef migrations list --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for removing Todo database migration
todo-migrations-remove:
	echo "Removing Todo database migration"
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef migrations remove --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1

# Target for removing all Todo database migrations
todo-migrations-remove-all:
	echo "Removing all Todo database migrations"
	dotnet ef migrations remove --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Presentation --context TodoDbContextWrite || exit 1
	rm -rf Domains/Todo/Infrastructure/Persistence/Migrations/*.cs || exit 1
