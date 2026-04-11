# Platform Layer

Contains Unity Engine integration and OS-level systems:

## Architecture

Platform layer provides the Unity Engine API abstraction and OS integration points.

## Dependencies

- **Required By**: None (lowest level)
- **Depends On**: None

## Coding Standards

Follow project-wide coding standards:
- PascalCase for classes, methods, public fields/properties
- _camelCase for private fields
- No magic numbers — use constants from ScriptableObjects
- All public APIs have XML comments
- Dependency injection preferred over singletons

## Testing

Platform systems testable against Unity API changes.

---

**Last Updated**: 2026-04-11
