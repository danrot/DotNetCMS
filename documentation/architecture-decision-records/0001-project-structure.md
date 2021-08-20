# Project Structure

## Status

Accepted

## Context

Which projects should exist within the solution? This question can also be split in two sub questions:

1. Should tests be located in the same project as the production code?
2. Should all the production code be located in a single project or should it be split in some way?

## Decision

The project structure within this solution is guided by the following two decisions:

1. Tests will be located in a separated project, [as done in the official Microsoft
documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test).
2. The rest of the solution will be [split by layers](https://softwareengineering.stackexchange.com/a/373533/172973)
according to the yet to be defined layer or onion architecture.

## Consequences

The consequences of the first decision are:

- Tests will have to add `using` statements and references to the other project, which is a bit of an additional
overhead.
- The [assemblies containing the production code will be smaller](https://stackoverflow.com/a/347177/1292378), because
they do not contain the tests, which are unnecessary in a production environment.


The consequences of the second decision are:

- There will be a `DotNetCMS.Domain` project, which is currently holding all domain logic.
- The `DotNetCMS.Domain` project might be split in more projects in the future, e.g. to allow extending a very basic
implementation to be extended with assemblies resp. packages. But at the moment it is hard to decide and going with a
single project will speed up development.
- Multiple projects [provide a good boundary for code isolation](https://stackoverflow.com/a/2658726/1292378).
- Multiple projects [might cause classes to be clustered, because developers might not understand how they are
split](https://stackoverflow.com/a/2658726/1292378).
