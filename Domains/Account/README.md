# Account Domain

The Account domain handles user identity, authentication, and authorisation using **Event Sourcing** and **ASP.NET Core Identity**.

## Patterns Used

- **Event Sourcing** — all account state changes are recorded as events (registered, email confirmed, password changed, etc.)
- **CQRS** — commands (register, login, reset password) and queries (get profile) are separated
- **Identity** — ASP.NET Core Identity for password hashing, roles, and claims
- **JWT** — the AccountService is the sole issuer of JWT access and refresh tokens consumed by other services

## Key Use Cases

| Use Case | Type | Description |
|---|---|---|
| Register | Command | Create a new user account |
| Login | Command | Authenticate and receive JWT |
| Refresh Token | Command | Exchange a refresh token for a new access token |
| Confirm Email | Command | Verify email via confirmation link |
| Reset Password | Command | Initiate and complete password reset flow |
| Get Profile | Query | Retrieve current user's profile |

## Structure

```
Domains/Account/
├── Core/               # AccountAggregate, Domain Events, IAccountRepository
├── Application/        # Commands, Queries, Handlers, DTOs, Validators
├── Infrastructure/     # EF Core DbContext, Repository implementation, Migrations
├── CrossCutting/       # AddAccountDomain(), AutoMapper profiles, health checks
└── Hosts/
    └── RestApi/
        └── Presentation/  # API controllers (v1.0)
```

## Running Tests

```bash
# Unit tests
dotnet test Domains/Account/Tests/Unit/Unit.csproj

# Integration tests (requires Docker)
dotnet test Domains/Account/Tests/Integration/Integration.csproj

# Behaviour / acceptance tests
dotnet test Domains/Account/Tests/Behaviour/Behaviour.csproj
dotnet test Hosts/RestApi/Tests/Behaviour/Behaviour.csproj
```

Integration tests use Testcontainers to spin up a real PostgreSQL instance automatically.

## Database Migrations

```bash
# Add a new migration
make account-migrations-add name=MyMigration

# Apply pending migrations
make account-migrations-apply

# List existing migrations
make account-migrations-list
```
