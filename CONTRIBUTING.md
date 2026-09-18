# Contributing

Thanks for helping. Fixes, tests, documentation and new examples of DDD patterns are all welcome.

## Before you start

- **Bugs:** open an issue with the steps to reproduce, what you expected and what happened.
- **New patterns or larger changes:** open an issue first. This is a reference application, so every addition should teach something, and it should fit the architecture described in the README and the ADRs.
- **Security issues:** do not open a public issue; see [SECURITY.md](SECURITY.md).

## Working locally

```bash
git clone --recurse-submodules https://github.com/panosru/CleanDDDArchitecture.git
dotnet build CleanDDDArchitecture.sln
dotnet test CleanDDDArchitecture.sln            # end-to-end tests need Docker
dotnet run --project Hosts/AppHost              # the whole system under Aspire
```

## Rules the build enforces

- **Architecture.** `Tests/Architecture` checks the dependency rules: Core depends on no outer layer, Application on neither Infrastructure nor Hosts, a domain only on itself and Shared, and aggregates and domain events live in Core. If a rule fails, restructure the change rather than the rule.
- **Contexts talk through integration events.** A domain never references another. Put the contract in `Domains/Shared/Core/IntegrationEvents`, write it to the outbox in the same transaction as the change, and handle it with an idempotent `INotificationHandler<IntegrationEventReceived<T>>`.
- **Behaviour, not setters.** Change entities and aggregates through methods that enforce their rules; refuse with `DomainRuleException`.
- **Licences and vulnerabilities.** Only permissive dependencies (MIT, Apache-2.0, BSD); see [ADR 006](docs/adr/006-dependency-licensing.md). A package with a known moderate or worse advisory fails the build.

## Pull requests

- One concern per PR, with tests: a unit test for domain logic, a behaviour or integration test for anything crossing a boundary.
- Add an entry under **Unreleased** in [CHANGELOG.md](CHANGELOG.md).
- Record a significant design choice as an ADR in `docs/adr`.
- Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/).
- CI (build, all tests including end-to-end, CodeQL) must be green.

The framework itself lives in the `Library/Aviant` submodule. Changes to it go to [tecfinity/Aviant](https://github.com/tecfinity/Aviant) first.
