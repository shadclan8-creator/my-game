# ADR-0003: Parkour Movement Implementation

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Physics + Core (Movement) |
| **Knowledge Risk** | LOW — Unity Rigidbody and Animator stable |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | Rigidbody, Animator, Transform (stable APIs) |
| **Verification Required** | Test movement responsiveness at 60 FPS, validate wall-running attachment on complex geometries |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Input System (ADR-0001), Physics System (ADR-0002) |
| **Enables** | Camera System (player position), Combat System (aiming target) |
| **Blocks** | None |
| **Ordering Note** | Core system — depends on foundation systems |

---

## Context

### Problem Statement

Implement feline predator movement that is always in motion, supporting wall-running, climbing any surface, and quick dashes while maintaining 60 FPS target.

### Constraints

- **Feline Flow pillar**: Movement must be fluid, never stop, maintain momentum
- **Input latency**: Parkour requires low-latency input for responsiveness
- **Complex geometry**: Movement must work on 4 distinct era environments with varying collision surfaces
- **Performance**: Frame budget of 16.6ms at 60 FPS leaves limited time for movement calculations

### Requirements

- **Must support** wall-running on any surface type
- **Must support** climbing vertical and horizontal surfaces
- **Must support** quick dashes in any direction with cooldown
- **Must maintain** momentum between movement actions (no stopping between actions)
- **Must provide** smooth camera follow for fast player motion

---

## Decision

**Use Unity Rigidbody with Custom Movement Logic for Parkour-Specific Behaviors**

### Architecture Diagram

```
┌─────────────────────┐
│  Input System       │
│                     │
└───┬──────────────┘
      │
      ↓
┌────┴────┐
│  Movement  │
│  System    │
│            │
├────┬───────┤
│    │       │
│    ↓       ↓
│ ┌────┴──┐ ┌──────┴──┐
│ │ Physics  │ │ Camera    │
│ │ System   │ │ System    │
│ └────┬────┘ └───────────┘
│      │
│      ↓
│   ┌────┴───┐
│   │ Combat  │
│   │ System    │
│   └───────────┘
```

### Key Interfaces

```csharp
// Movement System exposes player state and parkour actions
public interface IMovementProvider
{
    Vector3 GetPlayerPosition();
    Vector3 GetPlayerVelocity();
    bool IsParkourStateActive();
    bool IsWallRunning();
    bool IsClimbing();
    bool IsDashing();
    void SetParkourState(ParkourState state);
    void SetWallRunningSurface(Vector3 surfaceNormal, Vector3 surfacePoint);
}

// Parkour states managed by Movement System
public enum ParkourState
{
    Idle,
    Running,
    WallRunning,
    Climbing,
    Dashing,
    Falling
}

// Input system triggers parkour actions
public delegate void OnWallRunRequested();
public delegate void OnClimbRequested();
public delegate void OnDashRequested();
```

---

## Alternatives Considered

### Alternative 1: CharacterController

- **Description**: Use Unity's CharacterController component
- **Pros**: Built-in collision detection, handles slopes and steps automatically
- **Cons**: Designed for humanoid movement; doesn't support wall-running or complex parkour naturally
- **Rejection Reason**: Feline Flow pillar requires fluid, momentum-based parkour; CharacterController is too restrictive

### Alternative 2: Full Custom Physics Engine

- **Description**: Implement custom physics with custom character controller
- **Pros**: Complete control, can optimize specifically for feline movement
- **Cons**: Massive development time, collision systems are difficult to implement correctly
- **Rejection Reason**: Unity Rigidbody provides sufficient physics for this game type

---

## Consequences

### Positive

- **Fluidity**: Custom movement logic enables smooth parkour transitions and momentum preservation
- **Performance**: Rigidbody is Unity-optimized; calculations remain within frame budget
- **Flexibility**: Can implement era-specific movement modifiers without architecture changes

### Negative

- **Debugging Complexity**: Custom movement logic can introduce subtle bugs (getting stuck, falling through surfaces)
- **Physics Coupling**: Movement logic tightly coupled to Rigidbody; changing one affects the other

### Risks

- **Surface Detection Bugs**: Wall-running attachment may fail on certain geometry types or edge cases
- **Momentum Loss**: Transitioning between parkour states may interrupt player momentum unexpectedly
- **Testing**: Requires extensive playtesting across all 4 era environments

**Mitigation**:
- Implement surface detection validation before allowing attachment
- Add generous fallback surfaces (if specific surface fails, attach to nearby surface)
- Profile movement extensively on complex geometries before finalizing

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-002) | Parkour movement: wall-run, climb any surface, quick dashes | Custom movement logic with Rigidbody enables feline fluid movement while Unity Physics handles collision |
| game-concept.md (TR-concept-009) | 60 FPS minimum, 144 FPS preferred | Rigidbody and optimized movement logic designed for frame-budget compliance |

---

## Performance Implications

- **CPU**: Movement logic estimated at 1-2ms per frame including Rigidbody updates; acceptable within 16.6ms budget
- **Memory**: Minimal — only state structs and cached surfaces stored
- **Load Time**: Negligible — Rigidbody and Animator components initialize on scene load
- **Network**: Not applicable

---

## Migration Plan

N/A — new project using correct engine version

---

## Validation Criteria

- **Fluidity test**: Verify parkour actions chain smoothly without perceptible stopping
- **Momentum test**: Validate velocity is maintained across all parkour state transitions
- **Frame budget test**: Profile movement system with full scene; verify 60 FPS maintained
- **Surface detection test**: Test wall-running on all 4 era surface types

---

## Related Decisions

- Input System Architecture (ADR-0001) — provides input triggers for parkour actions
- Physics Collision Strategy (ADR-0002) — provides collision detection for surfaces
- Camera System Implementation (ADR-0005) — will consume player position and velocity
