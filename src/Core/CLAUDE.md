# Core Layer

Contains primary gameplay systems:

- **Movement System** — Parkour movement, wall-running, dashing, climbing
- **Camera System** — Camera following, shake, FOV dynamics, ADS
- **Combat System** — Weapons, damage, projectile pooling
- **Enemy AI System** — NavMesh pathfinding, AI state machine, enemy behaviors

## Architecture

Core systems depend on Foundation layer and expose interfaces to Feature/Presentation layers.

## Dependencies

- **Required By**: Time Travel, Combo, VFX, UI, Audio systems
- **Depends On**: Physics, Input Foundation systems

## Coding Standards

Follow project-wide coding standards:
- PascalCase for classes, methods, public fields/properties
- _camelCase for private fields
- No magic numbers — use constants from ScriptableObjects
- All public APIs have XML comments
- Dependency injection preferred over singletons

## Testing

- All core systems must be unit-testable
- Mockable interfaces for dependent systems
- Test coverage target: 80% minimum

---

**Last Updated**: 2026-04-11
