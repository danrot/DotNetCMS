# Title

## Status

Accepted

## Context

How should the application layer resp. the application services be structured?

## Decision

The application layer consists of application services grouped by aggregate (and maybe entities) that contain methods
accepting a `Command` object. These command objects are implemented as records and can be created using [model bindings
in ASP.NET](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-5.0).

The application services will throw exceptions like `PageNotFoundException` if something does not exist, because the
repository only works with already existing objects resp. can handle the nullable return value from the `GetById`
method themselves. This is done because exception management has some performance impact, but in the application layers
update and delete functionality it would be hard to differ between success and error otherwise (e.g. the `DeleteAsync`
function from the application services returns nothing, and even if it returned a boolean it would not be possible to
show the cause of the error this way).

## Consequences

- Commands can be easily generated using ASP.NET model binding
- These commands could be handled by a command bus, but using application services is more explicit (e.g. they have a
proper return value)
- The grouping of methods in application services might get tricky at some point, but having a single class for each
command seems like overkill
- Exceptions like `PageNotFoundException` are not available in the domain layer, but they should be used sparingly
anyway
