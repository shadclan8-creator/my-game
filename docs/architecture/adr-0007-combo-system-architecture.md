# ADR-0007: Combo System Architecture

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Feature (Scoring) |
| **Knowledge Risk** | LOW — event-based patterns stable |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | None |
| **Verification Required** | Test combo scaling performance at high multiplier values, validate timer accuracy |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Combat System (kill events), Movement System (movement events) |
| **Enables** | UI System (combo display), VFX System (combo milestones), Game State System (score persistence) |
| **Blocks** | None |
| **Ordering Note** | Feature layer — scoring depends on combat + movement systems |

---

## Context

### Problem Statement

Implement combo scoring system that rewards continuous movement and precision kills with multipliers while maintaining 60 FPS target during fast-paced combat.

### Constraints

- **Movement-Based Combos**: Combos must build on movement state from Movement System
- **Kill Triggers**: Combat System must provide kill events for combo building
- **Timer Decay**: Combo timer decays during idle periods
- **Multiplier Scaling**: Score must scale with combo multiplier; must avoid float overflow
- **UI Performance**: Combo display must update efficiently without frame drop

### Requirements

- **Must provide** real-time combo feedback
- **Must support** combo breaker behaviors (taking damage, stopping movement)
- **Must integrate** with Combat System and Movement System cleanly
- **Must display** combo in UI without performance impact

---

## Decision

**Event-Based Combo with Fixed Timestep Updates for Performance**

### Architecture Diagram

```
┌────────────────────────┐
│   Movement   │
│   System      │
└───┬───────────────┘
      │
      ↓
┌────┴────────────────┐
│   Combat   │
│   System    │
└───┬───────────────┘
      │
      ↓
      ┌────┴────────────────┐
      │                    │
      │   Combo   ┌──────────┴──┐
      │   System   │   UI    │
      │   │        │   System │
      │   └────┬────┘   └──────────┘
      │        │
      ┌────┴────┐
      │   VFX      │
      │   System   │
      │   │        │
      └────┬────┘
```

### Key Interfaces

```csharp
// Combo System interfaces
public interface IComboProvider
{
    int GetCurrentCombo();
    float GetComboMultiplier();
    void OnKillScored(Vector3 hitPosition);
    void OnMovementSustained();
    void ResetCombo();
    void ExtendComboTimer();
}

// Movement System provides combo triggers
public interface IMovementComboTrigger
{
    bool IsInMotion();
    bool IsWallRunning();
    bool IsClimbing();
}

// Combat System provides kill events
public interface ICombatKillTrigger
{
    void RegisterKillListener(Action<Vector3> onKill);
}
```

---

## Alternatives Considered

### Alternative 1: Per-Frame Calculation

- **Description**: Update combo state every frame based on movement and kills
- **Pros**: Immediate feedback, simple implementation
- **Cons**: Unnecessary CPU overhead; combo timer decay requires per-frame calculation but doesn't need 60 FPS precision
- **Rejection Reason**: Combo changes don't need sub-frame resolution

### Alternative 2: Fixed Timestep Calculation

- **Description**: Update combo state at fixed timestep (e.g., every 100ms)
- **Pros**: Reduces CPU overhead significantly, more performant
- **Cons**: Slight input latency (up to 100ms) for timer updates
- **Rejection Reason**: Combo multiplier changes are rare; 100ms latency acceptable

### Alternative 3: Event-Based Scoring

- **Description**: Combo System observes kills and movement events, calculates score separately
- **Pros**: Decouples scoring from frame loop, Combat System controls when score is calculated
- **Cons**: Adds complexity, requires event management system
- **Rejection Reason**: Over-architecture for simple combo system

---

## Consequences

### Positive

- **Performance**: Fixed timestep reduces CPU from per-frame updates
- **Scalability**: Event-based architecture supports complex scoring without frame impact
- **Decoupling**: Combo System doesn't tightly couple to frame loop

### Negative

- **Latency**: 100ms timestep may feel slightly less responsive
- **Complexity**: Event-based system adds more code than simple counter
- **Debugging**: Event flow can be harder to trace than direct updates

### Risks

- **Timer Precision**: Fixed timestep may cause timer decay to be slightly inconsistent
- **Event Misses**: Events might be lost if any system doesn't fire correctly
- **Float Overflow**: High multipliers could cause score overflow if not clamped

**Mitigation**

- Use uint for score to eliminate overflow risk at high multipliers
- Validate combo multiplier clamped to reasonable maximum (e.g., 10.0x)
- Add event logging for debugging missing combo increments
- Consider adaptive timestep (50ms) if 100ms feels unresponsive

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-004) | Combo system rewards continuous movement and precision kills with multipliers | Fixed timestep updates reduce CPU overhead; event-based architecture decouples scoring from frame loop while maintaining responsiveness |
| game-concept.md (TR-concept-009) | 60 FPS minimum, 144 FPS preferred | Fixed timestep calculation reduces per-frame overhead to meet frame budget |

---

## Performance Implications

- **CPU**: Combo updates at 10Hz (100ms timestep) ~0.1ms per frame; negligible
- **Memory**: Combo state estimated at <1KB; negligible
- **Load Time**: Instant — combo system initializes on game boot
- **Network**: Not applicable

---

## Migration Plan

N/A — new project

---

## Validation Criteria

- **Combo accuracy test**: Verify combo increments correctly with timer decay at various movement states
- **Performance test**: Profile combo system with 1000+ rapid kills; verify 60 FPS maintained
- **Scalability test**: Test at maximum combo multiplier (10x); verify no performance degradation
- **Event reliability test**: Simulate missed events; verify combo doesn't stall

---

## Related Decisions

- Combat System Architecture (ADR-0004) — provides kill events
- Movement System Implementation (ADR-0003) — provides movement state for combo building
