# Foundation Layer

Contains foundational systems that all gameplay depends on:

- **Physics System** — Collision detection, projectile physics, environmental collision
- **Input System** — Keyboard/mouse and gamepad input, device switching

## Architecture

Foundation systems have no dependencies. They initialize first and expose interfaces to Core layer systems.

## Dependencies

- **Required By**: Movement, Camera, Combat, Combo, Enemy AI, UI, VFX systems
- **Depends On**: None

## Coding Standards

Follow project-wide coding standards:
- PascalCase for classes, methods, public fields/properties
- _camelCase for private fields
- No magic numbers — use constants from ScriptableObjects
- All public APIs have XML comments
- Dependency injection preferred over singletons

## Testing

- All foundation systems must be unit-testable
- Mockable interfaces for dependent systems
- Test coverage target: 80% minimum

---

**Last Updated**: 2026-04-11
