# Feature Layer

Contains game-defining feature systems:

- **Time Travel System** — Era unlocking and switching, era-specific content
- **Target Capture System** — Level objectives, unique capture sequences
- **Combo System** — Scoring, multipliers, momentum tracking

## Architecture

Feature systems depend on Core layer and expose data to Presentation layer.

## Dependencies

- **Required By**: Game State, UI, Audio, VFX systems
- **Depends On**: Movement, Combat, Enemy AI Core systems

## Coding Standards

Follow project-wide coding standards:
- PascalCase for classes, methods, public fields/properties
- _camelCase for private fields
- No magic numbers — use constants from ScriptableObjects
- All public APIs have XML comments
- Dependency injection preferred over singletons

## Testing

- All feature systems must be unit-testable
- Mockable interfaces for dependent systems
- Test coverage target: 80% minimum

---

**Last Updated**: 2026-04-11
