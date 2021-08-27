# Title

## Status

Accepted

## Context

Who calls `SaveChanges` from the `DbContext` class and therefore commits the transaction?

## Decision

The call to `SaveChanges` will be implemented in a globally registered ActionFilter.

## Consequences

- With a CommandBus it might be possible to apply the transaction only for certain commands, but that probably results
in a more complicated registration mechanism
- There is a (probably neglegible) overhead in always calling `SaveChanges`, even if no changes have been made
- Changes will always be saved before sending the response, but there is hardly a case where this is not desired
