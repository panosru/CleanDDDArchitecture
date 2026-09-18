# ADR 006: Keep the dependency tree free of commercial licences

**Status:** Accepted
**Date:** 2026-09-18

## Context

This repository is a template: people copy it into their own products, often commercial and closed-source. Whatever licence a dependency carries, every one of those products inherits it.

Several libraries that .NET templates traditionally ship with have changed licence:

- **MediatR** 13 and later require a commercial licence key (Lucky Penny Software). 12.x is Apache-2.0.
- **AutoMapper** 15 and later are dual-licensed RPL-1.5 / commercial. RPL-1.5 is reciprocal: using it without the commercial licence obliges you to publish your source.
- **FluentAssertions** 8 is licensed for non-commercial use only.

Separately, the build suppressed NuGet vulnerability warnings (NU1901–NU1904), so advisories in transitive packages went unnoticed.

## Decision

- **MediatR is pinned to 12.5.0**, the last Apache-2.0 release (it arrives through Aviant). The version must not float to 13+.
- **No object mapper.** DTOs and view models map themselves: a static `From(entity)` for in-memory mapping, and an `Expression<Func<TEntity, TDto>> Projection` where EF Core should translate the mapping into SQL (`.Select(TodoListDto.Projection)`). If mapping grows large, use a source generator such as Mapperly (Apache-2.0), not a runtime mapper.
- **AwesomeAssertions** (Apache-2.0, a continuation of the FluentAssertions API) replaces FluentAssertions.
- **Vulnerable packages fail the build.** NU1902–NU1904 (moderate, high, critical) are errors and `NuGetAuditMode` is `all`, so transitive packages are audited. A vulnerable transitive package is fixed by pinning the patched version in `Directory.Packages.props` with a comment naming the advisory.

## Consequences

**Positive:**
- Copying the template never obliges anyone to buy a licence or publish their code.
- Mappings are visible, debuggable, and checked by the compiler. Moving to explicit mapping exposed a real bug: `TodoItemDto.Done` was never populated, because AutoMapper's name matching could not pair it with `IsCompleted`.
- New advisories break CI instead of shipping silently.

**Negative:**
- MediatR stays on 12.x and will not receive new features. If that becomes a problem, the alternative is a source-generated mediator (e.g. `Mediator` by martinothamar, MIT), not an upgrade.
- Explicit mapping is more code than a mapping profile for large DTOs.
- A new advisory can break the build for reasons unrelated to the change being made.

## Alternatives Considered

- **Buy licences / use community tiers.** Workable for a single company, but a template would pass the obligation to every user, most of whom would not notice.
- **Keep AutoMapper 14 (last MIT release).** Frozen, and it has its own high-severity advisory (GHSA-rvv3-g6hj-g44x).
