# Title

## Status

Accepted

## Context

How should serialization happen and should DTOs be used?

## Decision

For now the domain objects are directly serialized instead of using DTOs.

## Consequences

- DTOs do not have to be created, therefore there is less overhead and coupling. DTOs would have to be adjusted most of
the times the domain object changes.
- If domain objects change the serialization also changes, which might affect external stakeholders. It might be
possible to mitigate that by configuring the serializer using virtual properties, aliases etc., if such features are
provided by the serializer.
