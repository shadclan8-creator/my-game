# ADR-0002: Physics System Collision Strategy

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Physics |
| **Knowledge Risk** | LOW — Unity Physics stable across versions |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | None |
| **Verification Required** | Test collision performance on complex geometry, validate raycast overhead |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | None (Foundation layer system) |
| **Enables** | Movement System (collision detection), Combat System (hit detection), Enemy AI System (NavMesh) |
| **Blocks** | None |
| **Ordering Note** | Foundation system — enables core systems |

---

## Context

### Problem Statement

The game requires performant collision detection for parkour movement (wall-running, climbing any surface, quick dashes) on complex era environments while maintaining 60 FPS target.

### Constraints

- **Performance**: Frame budget of 16.6ms at 60 FPS limits collision overhead
- **Complex Geometry**: Era-specific environments (1950s suburban, 1980s neon, 1920s noir, Future sci-fi) have varying collision complexity
- **Parkour Requirements**: Wall-running, climbing, and dashing require precise surface detection and attachment

### Requirements

- **Must support** fast raycasting for parkour surface detection
- **Must support** dynamic attachment to surfaces while moving at high speed
- **Must maintain** 60 FPS minimum on lower-end systems
- **Must integrate** with NavMesh for enemy AI without conflict

---

## Decision

**Use Hybrid Collision System: Unity Physics for general collision + simplified collision hulls for parkour raycasting**

### Architecture Diagram

```
┌────────────────────┐
│   Physics System  │
│                 │
├────┬────────────┤
│    │            │
│    ↓            ↓
│ ┌────┴────┐  ┌──────┴───┐
│ │Movement  │ │ Enemy AI   │
│ │System    │ │ (NavMesh)  │
│ └────┬────┘ └─────────────┘
│      │
│      ↓
│   ┌────┴────┐
│   │ Combat    │
│   │ System    │
│   └────────────┘
```

### Key Interfaces

```csharp
// Physics System exposes collision interface
public interface IPhysicsProvider
{
    RaycastHit Raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask);
    bool SphereCast(Vector3 center, float radius, LayerMask layerMask);
    void SetCollisionMode(CollisionMode mode);
    CollisionMode GetCollisionMode();
}

// Simplified hull for raycasting (cheaper than mesh)
public struct CollisionHull
{
    public Vector3 center;
    public float radius;
    public LayerMask layerMask;
}
```

---

## Alternatives Considered

### Alternative 1: Full Mesh Collision

- **Description**: Use Unity's MeshCollider for all surfaces and character controllers
- **Pros**: Most accurate collision, built-in character controller, handles slopes and steps automatically
- **Cons**: Expensive per frame on complex geometry, doesn't support dynamic wall-running well, poor performance at scale
- **Rejection Reason**: Parkour movement requires fast surface detection with dynamic attachment; full mesh collision is too expensive for 60 FPS target

### Alternative 2: Custom Physics Engine

- **Description**: Implement custom collision detection from scratch
- **Pros**: Complete control, optimize specifically for parkour needs
- **Cons**: Massive development time, collision systems are notoriously difficult to get right, physics bugs are game-breaking
- **Rejection Reason**: Unity Physics provides sufficient functionality for this game type

---

## Consequences

### Positive

- **Performance**: Hybrid approach keeps frame overhead minimal while supporting parkour needs
- **Simplicity**: Leverages Unity's optimized Physics engine for general collision
- **Scalability**: Can handle varying geometry complexity without major performance impact

### Negative

- **Complexity**: Two collision systems must be maintained (Physics + raycast hulls)
- **Integration**: Requires careful coordination between general collision and parkour-specific raycasting

### Risks

- **Raycast Overhead**: Too many raycasts per frame can still impact CPU
- **NavMesh Conflict**: Parkour collision hulls may interfere with enemy NavMesh generation
- **Testing**: Need to validate that hybrid approach actually provides performance benefit

**Mitigation**:
- Profile raycast count during parkour gameplay; target under 50 raycasts per frame
- Design parkour collision hulls to use LayerMask effectively and reduce checks
- Prototype wall-running as standalone to measure collision performance before full integration

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-002) | Parkour movement: wall-run, climb any surface, quick dashes | Hybrid collision provides fast surface detection via raycasts while Unity Physics handles general collision |

---

## Performance Implications

- **CPU**: Raycast overhead estimated at 0.1-0.5ms per cast (depending on geometry complexity). At 50 casts/frame maximum = ~5ms budget — manageable.
- **Memory**: Minimal — only hull data structures stored
- **Load Time**: Negligible — Unity Physics initializes on game boot
- **Network**: Not applicable

---

## Migration Plan

N/A — new project using correct engine version from start

---

## Validation Criteria

- **Collision accuracy test**: Verify wall-running attachment detection is reliable across era geometries
- **Frame budget test**: Profile parkour gameplay with collision stats active; verify 60 FPS maintained
- **Integration test**: Validate NavMesh generation works alongside parkour collision hulls
- **Edge case test**: Test corner cases: thin walls, multiple surfaces, high-speed impacts

---

## Related Decisions

- Movement System Implementation (ADR-0003) — will consume collision interfaces
- Combat System Architecture (ADR-0004) — will use hit detection interfaces
