# ADR 002: Kafka for Domain Event Messaging

**Status:** Accepted
**Date:** 2024-01-01

## Context

In microservices mode, domains need to communicate asynchronously via integration events (e.g., AccountRegistered → send welcome email, TodoCreated → notify webhooks). We needed a messaging infrastructure that is durable, scalable, and supports consumer groups for independent scaling.

## Decision

Use **Apache Kafka** (via `Confluent.Kafka`) for inter-service integration events.

## Consequences

**Positive:**
- Log-based, durable message storage — consumers can replay events
- Consumer groups enable independent scaling of each domain
- Partitioning provides ordered delivery per entity (e.g., per user)
- De-facto standard in event-driven microservices

**Negative:**
- Operational complexity: requires Zookeeper (or KRaft), monitoring, topic management
- Heavier local development setup (mitigated by Docker Compose `eventing` profile and Aspire)
- Not appropriate for request/reply patterns

## Alternatives Considered

- **RabbitMQ** — simpler operationally, but no log-based retention; once a message is consumed it's gone
- **Azure Service Bus** — cloud-vendor lock-in; not appropriate for local-first development
- **In-process MediatR notifications** — works only within a single process (monolith mode)
