# Todo Domain

The Todo domain demonstrates the **Subdomain pattern** — a single bounded context split into two cohesive subdomains with their own independent lifecycle.

## Patterns Used

- **Subdomains** — `TodoList` and `TodoItem` are independent sub-bounded contexts
- **DDD** — Aggregates, Value Objects, Domain Events
- **CQRS** — separate commands and queries per subdomain
- **Vertical Slice** — each feature is fully contained across all layers

## Subdomains

### TodoList

Manages collections of todo items.

| Use Case | Type |
|---|---|
| Create TodoList | Command |
| Update TodoList | Command |
| Delete TodoList | Command |
| Get TodoLists | Query |

### TodoItem

Manages individual tasks within a list.

| Use Case | Type |
|---|---|
| Create TodoItem | Command |
| Update TodoItem | Command |
| Delete TodoItem | Command |
| Toggle TodoItem done | Command |
| Get TodoItems | Query |

## Structure

```
Domains/Todo/
├── Core/
├── Application/
├── Infrastructure/
├── CrossCutting/
├── Hosts/RestApi/Presentation/       # shared Todo API surface
└── SubDomains/
    ├── TodoList/
    │   ├── Core/
    │   ├── Application/
    │   ├── Infrastructure/
    │   ├── CrossCutting/
    │   └── Hosts/RestApi/Presentation/
    └── TodoItem/
        ├── Core/
        ├── Application/
        ├── Infrastructure/
        ├── CrossCutting/
        └── Hosts/RestApi/Presentation/
```

## Running Tests

```bash
# Domain unit tests
dotnet test Domains/Todo/Tests/Unit/Unit.csproj

# Integration tests (requires Docker)
dotnet test Domains/Todo/Tests/Integration/Integration.csproj

# Subdomain behaviour tests
dotnet test Domains/Todo/SubDomains/TodoList/Tests/Behaviour/Behaviour.csproj
dotnet test Domains/Todo/SubDomains/TodoItem/Tests/Behaviour/Behaviour.csproj
```

## Database Migrations

```bash
make todo-migrations-add name=MyMigration
make todo-migrations-apply
make todo-migrations-list
```
