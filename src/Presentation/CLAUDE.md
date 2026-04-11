# Presentation Layer

Contains user-facing presentation systems:

- **UI System** — HUD, menus, pause, settings, era-accents
- **Audio System** — Era-specific audio, weapon SFX, ambient music
- **VFX System** — Blood, muzzle flash, motion blur, combo effects

## Architecture

Presentation systems consume events from lower layers and display feedback to player.

## Dependencies

- **Required By**: None (top-level presentation)
- **Depends On**: Combat, Combo, Movement, Game State systems

## Coding Standards

Follow project-wide coding standards:
- PascalCase for classes, methods, public fields/properties
- _camelCase for private fields
- No magic numbers — use constants from ScriptableObjects
- All public APIs have XML comments
- Dependency injection preferred over singletons

## Testing

- All presentation systems must be unit-testable
- UI components testable without runtime
- Test coverage target: 80% minimum

---

**Last Updated**: 2026-04-11
