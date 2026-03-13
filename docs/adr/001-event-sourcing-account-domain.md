# ADR 001: Event Sourcing for the Account Domain

**Status:** Accepted
**Date:** 2024-01-01

## Context

The Account domain manages user identity, authentication, and access control — all safety-critical operations with strict audit requirements. We needed a persistence strategy that provides full traceability of every state change.

## Decision

Use **Event Sourcing** for the Account domain. Every change to an account (registration, email confirmation, password change, role assignment) is stored as an immutable domain event. Current state is derived by replaying events.

Other domains (Todo, Weather) use conventional CRUD persistence with EF Core.

## Consequences

**Positive:**
- Complete, immutable audit trail of all identity operations
- Full temporal query capability (account state at any point in time)
- Domain events are first-class citizens, enabling integration events and projections
- Natural fit for eventual consistency across service boundaries

**Negative:**
- Higher complexity than CRUD — requires event store, projections, snapshotting strategy
- Read-side queries require read-model projections
- Schema evolution of events requires versioning discipline

## Alternatives Considered

- **CRUD with audit log table** — simpler but audit log is secondary, not primary; state and history can diverge
- **CRUD with change data capture (CDC)** — infrastructure-heavy, no domain semantics
