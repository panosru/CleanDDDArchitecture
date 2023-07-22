.SILENT:
.PHONY: sonar account-db-update account-db-drop account-migrations-add account-migrations-update account-migrations-apply account-migrations-rollback account-migrations-list account-migrations-remove account-migrations-remove-all todo-db-update todo-db-drop todo-migrations-add todo-migrations-update todo-migrations-apply todo-migrations-rollback todo-migrations-list todo-migrations-remove todo-migrations-remove-all

include .env
export $(shell sed 's/=.*//' .env)

sonar:
	echo "Running sonarqube analysis"
	dotnet sonarscanner begin /k:"${SONAR_PROJECT_NAME}" /d:sonar.host.url="${SONAR_HOST_URL}"  /d:sonar.login="${SONAR_LOGIN}"
	dotnet build
	dotnet sonarscanner end /d:sonar.login="${SONAR_LOGIN}"

account-db-update:
	echo "Updating Account database"
	dotnet ef database update --project Hosts/RestApi/Application --context AccountDbContextWrite	

account-db-drop:
	echo "Dropping Account database"
	dotnet ef database drop --project Hosts/RestApi/Application --context AccountDbContextWrite

account-migrations-add:
	echo "Adding Account database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateAccountDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef migrations add $(DB_MIGRATION_NAME) --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context AccountDbContextWrite

account-migrations-update:
	echo "Updating Account database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateAccountDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef migrations script --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context AccountDbContextWrite

account-migrations-apply:
	echo "Applying Account database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateAccountDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context AccountDbContextWrite

account-migrations-rollback:
	echo "Rolling back Account database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateAccountDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context AccountDbContextWrite

account-migrations-list:
	echo "Listing Account database migrations"
	dotnet ef migrations list --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --context AccountDbContextWrite

account-migrations-remove:
	echo "Removing Account database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateAccountDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateAccountDomainTables)
	dotnet ef migrations remove --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --context AccountDbContextWrite

account-migrations-remove-all:
	echo "Removing all Account database migrations"
	dotnet ef migrations remove --project Domains/Account/Infrastructure --startup-project Hosts/RestApi/Application --context AccountDbContextWrite
	rm -rf Domains/Account/Infrastructure/Persistence/Migrations/*.cs

todo-db-update:
	echo "Updating Todo database"
	dotnet ef database update --project Hosts/RestApi/Application --context TodoDbContextWrite

todo-db-drop:
	echo "Dropping Todo database"
	dotnet ef database drop --project Hosts/RestApi/Application --context TodoDbContextWrite

todo-migrations-add:
	echo "Adding Todo database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateTodoDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef migrations add $(DB_MIGRATION_NAME) --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context TodoDbContextWrite

todo-migrations-update:
	echo "Updating Todo database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateTodoDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef migrations script --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context TodoDbContextWrite

todo-migrations-apply:
	echo "Applying Todo database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateTodoDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context TodoDbContextWrite

todo-migrations-rollback:
	echo "Rolling back Todo database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateTodoDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef database update $(DB_MIGRATION_NAME) --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --output-dir Persistence/Migrations --context TodoDbContextWrite

todo-migrations-list:
	echo "Listing Todo database migrations"
	dotnet ef migrations list --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --context TodoDbContextWrite

todo-migrations-remove:
	echo "Removing Todo database migration"
	# If DB_MIGRATION_NAME is not set, then set it CreateTodoDomainTables
	$(eval DB_MIGRATION_NAME ?= CreateTodoDomainTables)
	dotnet ef migrations remove --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --context TodoDbContextWrite

todo-migrations-remove-all:
	echo "Removing all Todo database migrations"
	dotnet ef migrations remove --project Domains/Todo/Infrastructure --startup-project Hosts/RestApi/Application --context TodoDbContextWrite
	rm -rf Domains/Todo/Infrastructure/Persistence/Migrations/*.cs
