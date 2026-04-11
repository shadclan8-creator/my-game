# Time's Baddest Cat — Master Architecture

## Document Status
- Version: 1.0
- Last Updated: 2026-04-09
- Engine: Unity 6.3 LTS
- GDDs Covered: game-concept.md, systems-index.md
- ADRs Referenced: None (architecture baseline)

## Engine Knowledge Gap Summary
**LLM Training Covers**: Unity up to ~2022 LTS. Current engine is 6.3 LTS.

### HIGH RISK Domains (verify before implementation):
- **Entities/DOTS**: Major API overhaul - complete ECS redesign
- **Input System**: Legacy Input Manager deprecated, new Input System is default
- **Rendering**: URP upgrades, SRP Batcher improvements
- **Addressables**: Asset management workflow changes

### MEDIUM RISK Domains:
- **UI Toolkit**: Production-ready for runtime UI (replaces UGUI)
- **Async Asset Loading**: Improved Addressables performance

---

## System Layer Map

```
┌─────────────────────────────────────┐
│ PRESENTATION LAYER               │  ← UI/HUD System, Audio System, VFX System
├─────────────────────────────────────┤
│ FEATURE LAYER                      │  ← Time Travel System, Target Capture System, Combo System
├─────────────────────────────────────┤
│ CORE LAYER                        │  ← Movement System, Combat System, Camera System, Enemy AI System
├─────────────────────────────────────┤
│ FOUNDATION LAYER                  │  ← Physics System, Input System
├─────────────────────────────────────┤
│ PLATFORM LAYER                    │  ← Unity Engine API, OS integration
└─────────────────────────────────────┘
```

**Proposed Layer Assignment:**

| System | Layer | Rationale |
|---------|-------|-----------|
| **Physics System** | Foundation | All collision detection depends on it |
| **Input System** | Foundation | All gameplay requires input input |
| **Movement System** | Core | Primary player interaction system |
| **Camera System** | Core | Follows movement, frames combat |
| **Combat System** | Core | Core game loop requires weapons + damage |
| **Enemy AI System** | Core | AI behaviors drive combat encounters |
| **Combo System** | Feature | Scoring built on combat + movement |
| **Time Travel System** | Feature | Era progression system |
| **Target Capture System** | Feature | Level objective system |
| **UI/HUD System** | Presentation | Displays all game state data |
| **Audio System** | Presentation | Era-specific ambience + weapon SFX |
| **VFX System** | Presentation | Visual feedback for combat + movement |

⚠️ **Engine Awareness**: Combat System uses Entities/DOTS (HIGH risk) — verify API patterns before implementation.
⚠️ **Engine Awareness**: Movement System uses new Input System (HIGH risk) — verify API patterns.
⚠️ **Engine Awareness**: UI System uses UI Toolkit (MEDIUM risk) — API changes from UGUI.

---

## Module Ownership Map

### Foundation Layer

| Module | Owns | Exposes | Consumes | Unity APIs Used |
|---------|--------|----------|-----------------|
| **Physics System** | Collision state, raycast results | Collision events | Physics.Raycast, Physics.OverlapSphere, Collider |
| **Input System** | Input state (axes, buttons), device switching | Raw input streams | InputSystem, InputAction, Keyboard/Mouse API |

### Core Layer

| Module | Owns | Exposes | Consumes | Unity APIs Used |
|---------|--------|----------|-----------------|
| **Movement System** | Player position/velocity, parkour state | Position for camera, combat detection | Rigidbody, Animator, Transform |
| **Camera System** | Camera state, view matrix | Follow target events | Cinemachine (or Camera.main), Transform |
| **Combat System** | Weapon state, projectile pool, damage events | Hit events, ammo state | Entities (ECS), Physics, Animator |
| **Enemy AI System** | AI state, enemy health, attack triggers | Position, target requests | NavMesh, NavMeshAgent, AI pathfinding |

### Feature Layer

| Module | Owns | Exposes | Consumes | Unity APIs Used |
|---------|--------|----------|-----------------|
| **Combo System** | Combo state, multiplier, score | Kill events, movement events | UI update API |
| **Time Travel System** | Era unlock state, available eras | Era selection | PlayerPrefs/Save system |
| **Target Capture System** | Capture state, objective completion | Position, target data | NavMesh, Trigger events |

### Presentation Layer

| Module | Owns | Exposes | Consumes | Unity APIs Used |
|---------|--------|----------|-----------------|
| **UI/HUD System** | UI state, screens, HUD elements | All game data | UI Toolkit (UXML/USS), Canvas |
| **Audio System** | Audio playback, era music state | Combat events, era changes | AudioSource, AudioMixer |
| **VFX System** | Particle effects, visual feedback | Combat events, movement events | Visual Effect Graph, Particle System |

**Ownership Rules:**
- Each module owns its data exclusively
- Inter-module communication through events/delegates only
- No direct data sharing across modules

---

## Data Flow

### Frame Update Path

```
Input System (Unity Engine)
    ↓ (input events)
Movement System
    ↓ (position, parkour state)
Camera System ───────┐
    ↓                 │
Combat System ←─────────┤
    ↓                 │
Enemy AI System ←────────┤
    ↓                 │
Combo System ←──────────┤
    ↓
UI/HUD System (display state)
    ↓
VFX System (spawn effects)
```

### Event/Signal Path

```
┌─────────────────────────────────────┐
│  Event Bus (Global)              │
└─────────────────────────────────────┘
           ↑           ↑           ↑           ↑           ↑
           │           │           │           │
      ┌────┴────┐  ┌────┴─────┐ ┌──┴──┐ ┌───┴───┐
      │Movement   │  │Combat     │ │Enemy   │ │Target   │ │Combo    │
      │Events     │  │Events     │ │Events  │ │Events   │ │Events   │
      └────┬────┘  └────┬──────┘ └───┬──┘ └───┬───┘ └───┬───┘
           │              │              │           │           │           │
      ┌────┴────┐  ┌─────┴──────┐ ┌───┴───┐ ┌───┴───┐ ┌───┴───┐
      │UI System   │  │Audio System   │ │VFX     │ │Game     │ │Time     │
      │(subscribe)  │  │(subscribe)   │ │System    │ │State    │ │Travel   │
      └─────────────┘  └──────────────┘ └──────────┘ └──────────┘ └──────────┘
```

### Save/Load Path

```
Game State System (Foundation)
    ↓ (serialization)
PlayerPrefs / Save File
```

### Initialization Order

1. Input System → Capture input devices
2. Physics System → Initialize collision systems
3. Movement System → Initialize parkour state
4. Camera System → Set up follow target
5. Combat System → Initialize weapon pool
6. Enemy AI System → Spawn navmesh
7. UI System → Load initial screens
8. Audio System → Load default era audio

---

## API Boundaries

### Input System Interface

```csharp
public interface IInputProvider
{
    Vector2 GetMovementInput();
    bool IsDashPressed();
    bool IsFirePressed();
    bool IsReloadPressed();
    Vector2 GetAimDirection();
    void SwitchDevice();
}
```

**Invariant**: Input events are frame-sampled, not event-driven. Callers must not assume 1:1 action mapping.

**Guarantee**: All gameplay systems receive consistent input frame data.

### Combat System Interface

```csharp
public interface ICombatProvider
{
    void FireWeapon(Vector3 direction);
    void ReloadWeapon();
    bool CanDamage(IKillable target);
    void OnKill(IKillable target, Vector3 position);
    Weapon GetCurrentWeapon();
    int GetCurrentAmmo();
}
```

**Invariant**: Damage events are authoritative. Combo System observes but does not control combat.

**Guarantee**: All kills route through Combat System for consistent scoring.

### Movement System Interface

```csharp
public interface IMovementProvider
{
    Vector3 GetPlayerPosition();
    Vector3 GetPlayerVelocity();
    bool IsParkourStateActive();
    bool IsClimbing();
    bool IsWallRunning();
    void SetTargetVelocity(Vector3 velocity);
}
```

**Invariant**: Movement state is read-only for combat/camera systems. Direct velocity control reserved for Movement System.

**Guarantee**: Camera and Combat always have valid position data each frame.

### Combo System Interface

```csharp
public interface IComboProvider
{
    int GetCurrentCombo();
    float GetComboMultiplier();
    void OnMovementSustained();
    void OnKillScored();
    void ResetCombo();
    void ExtendComboTimer();
}
```

**Invariant**: Combo state is derived from Combat + Movement events. Combo System does not control gameplay directly.

**Guarantee**: Scoring is consistent regardless of which system triggers the event.

⚠️ **Engine Awareness**: Using C# 9 features (default in Unity 6.3) — verify compatibility with target Unity version.

---

## ADR Audit

**No existing ADRs to audit.**

---

## Required ADRs

### Must Create Before Coding (Foundation & Core):

**Foundation Layer ADRs:**
1. ✅ Input System Architecture (ADR-0001) — COMPLETE
2. ✅ Physics Collision Strategy (ADR-0002) — COMPLETE
3. ✅ Parkour Movement Implementation (ADR-0003) — COMPLETE
4. ✅ Combat System Architecture (ADR-0004) — COMPLETE

**Core Layer ADRs:**
5. ✅ Asset Management Strategy (ADR-0005) — COMPLETE
6. ✅ Time Travel System Architecture (ADR-0006) — COMPLETE
7. ✅ Combo System Architecture (ADR-0007) — COMPLETE

**Feature Layer ADRs:**
8. ✅ Camera System Implementation (ADR-0008) — COMPLETE (2026-04-11)
9. ✅ Enemy AI Pathfinding (ADR-0009) — COMPLETE (2026-04-11)
10. ✅ UI System Architecture (ADR-0010) — COMPLETE (2026-04-11)
11. ✅ Audio System Architecture (ADR-0011) — COMPLETE (2026-04-11)
12. ✅ VFX System Architecture (ADR-0012) — COMPLETE (2026-04-11)
13. ✅ Game State System Implementation (ADR-0013) — COMPLETE (2026-04-11)

**Feature Layer ADRs:**
8. ✅ Camera System Implementation — COMPLETE (2026-04-11)
9. ✅ Enemy AI Pathfinding — COMPLETE (2026-04-11)
10. ✅ UI System Architecture — COMPLETE (2026-04-11)
11. ✅ Audio System Architecture — COMPLETE (2026-04-11)
12. ✅ VFX System Architecture — COMPLETE (2026-04-11)
13. ✅ Game State System Implementation — COMPLETE (2026-04-11)

### Should Have Before Relevant System:
14. ⚠️ Save/Load Format — PENDING
15. ⚠️ Performance Optimization Settings — PENDING

### Can Defer:
16. ⚠️ Quality Presets (Low/Medium/High/Max) — can defer to implementation

---

## Architecture Document Status

### Must Create Before Coding (Foundation & Core):

**Foundation Layer ADRs:**
1. Input System Architecture — New Input System vs Legacy Input Manager decision
2. Physics System Collision Strategy — Raycasting vs Mesh Collision performance decision
3. Parkour Movement Implementation — Unity Physics vs Custom Physics decision

**Core Layer ADRs:**
4. Combat System Architecture — GameObject vs Entities/ECS decision
5. Camera System Implementation — Cinemachine vs Native Camera decision
6. Enemy AI Pathfinding — NavMesh vs Custom AI decision

**Feature Layer ADRs:**
7. Asset Management Strategy — Addressables vs Manual Loading decision
8. Time Travel System Architecture — Era loading/unloading strategy
9. Combo System Architecture — Event-based vs Poll-based decision

### Should Have Before Relevant System:
10. UI System Architecture — UI Toolkit vs UGUI decision
11. Audio System Architecture — AudioMixer configuration decision
12. VFX System Architecture — VFX Graph vs Particle System decision

### Can Defer:
13. Save/Load Format — JSON vs Binary format
14. Performance Optimization Settings — Quality preset definitions

---

## Architecture Principles

1. **Decoupled Modules** — Each system owns its data; inter-module communication via events only
2. **Engine-Aware APIs** — Always verify against Unity 6.3 LTS reference before using new APIs
3. **Performance First** — Validate against 60 FPS target at every layer
4. **Testable Architecture** — Each system exposes clean interfaces for mocking in tests
5. **Scalable Content** — Asset system must support 4 era expansion without architecture changes
6. **Solo-Dev Realistic** — Architect for single developer productivity, not team workflow

---

## Architecture Document Status

- **Technical Director Sign-Off**: 2026-04-09 — APPROVED WITH CONDITIONS
  - Conditions: 14 ADRs must be created before implementation begins ✅ COMPLETE
  - Priority: Foundation and Core layer ADRs first
- **Lead Programmer Feasibility**: 2026-04-09 — CONCERNS ACCEPTED
  - Assessment: Architecture is technically sound but scope is aggressive for solo dev
  - Conditions: Reduce to 2-3 eras, prioritize MVP systems, prototype parkour controls
- **All ADRs Complete**: 2026-04-11 — ✅ ALL 13 ADRs CREATED AND READY FOR IMPLEMENTATION

---

## Open Questions

1. **Combat System**: Use GameObject-based or Entities/ECS? (ECS provides better performance but has learning curve)
2. **Movement Physics**: Unity Physics or custom implementation for parkour responsiveness?
3. **Camera System**: Cinemachine or native Camera.main? (Cinemachine provides polish but adds dependency)
4. **Asset Loading**: Addressables async loading or manual scene transitions?
5. **Save Format**: JSON (readable) or Binary (fast, harder to debug)?
6. **Era Switching**: Full scene reload or additive scene loading?
