# Title

## Status

Accepted

## Context

How should the persistence of the domain object be achieved?

## Decision

Persistence is not part of the `DotNetCMS.Domain` project, since that should be free from any external dependencies.
`DotNetCMS.Domain` will only provide the `IPageRepository` interface definition, which acts as an adapter to the
outside world. `DotNetCMS.Persistence.EntityFrameworkCore` will integrate EntityFrameworkCore based on the
`IPageRepository` interface.

Tests for that (and possible other peristence implementations) are implemented in the `DotNetCMS.Persistence.Test` test
project.

## Consequences

- Since persistence is not included in `DotNetCMS.Domain` something other than EntityFrameworkCore could be used.
- Tests for all possible persistence implementations can be included in the `DotNetCMS.Persistence.Test` by inheriting
from the base test class and only implement the construction of the `PageRepository`.
- The `Repository` classes feel like a very thin wrapper around the `DbContext`, but it is still worth it, because
otherwise the `DotNetCMS.Domain` would be coupled to EntityFrameworkCore, which is not desirable.
