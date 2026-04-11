# ADR-0009: Enemy AI Pathfinding

## Status
Proposed

## Date
2026-04-11

## Last Verified
2026-04-11

## Decision Makers
Technical Director, AI Programmer

## Summary
Enemy AI requires patrol, engage, and target behaviors across era-specific levels. Decision: Use Unity NavMesh with AI States pattern for enemy behaviors and pathfinding.

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Navigation / AI |
| **Knowledge Risk** | LOW — NavMesh API stable since Unity 5, minor updates in 6.3 |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | NavMeshAgent 3.x enhancements (Post-6.0), AI Navigation updates |
| **Verification Required** | Test enemy AI performance with 20+ active agents, validate era-specific behaviors |

### Key Post-Cutoff Changes

- **NavMesh Surface Types**: Expanded surface type support for era-specific traversal
- **AI Navigation Package**: Separated from core engine, requires explicit package import
- **NavMesh Link Improvements**: Better off-mesh link handling for complex level geometry

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Physics System (ADR-0002) for collision, Movement System (ADR-0003) for position tracking |
| **Enables** | Combat System (enemy targets), Target Capture System (boss behaviors), Combo System (kill triggers) |
| **Blocks** | None |
| **Ordering Note** | Enemy AI can be implemented in parallel with Combat System |

---

## Context

### Problem Statement

The game requires multiple enemy types (basic guard, armored guard, target human) with distinct behaviors across 4 era-specific levels. Enemies must patrol, engage combat, and react to player presence with era-specific intelligence levels (e.g., 1950s guards less aware than Future drones).

### Current State

No AI system exists. Design requires enemy behaviors with pathfinding, cover usage, and era-specific variations.

### Constraints

- **Technical**: Must support 20+ concurrent enemies without exceeding frame budget
- **Gameplay**: Enemy AI must provide satisfying combat challenge without feeling unfair
- **Performance**: AI pathfinding must not cause frame drops during parkour combat
- **Era Variations**: Same AI framework must support different enemy behaviors per era

### Requirements

- **Must support** multiple enemy types with distinct behaviors (patrol, engage, flee)
- **Must use** NavMesh for pathfinding on complex era geometry
- **Must react** to player position and visibility (raycast-based detection)
- **Must support** era-specific AI parameters (alertness, accuracy, patrol patterns)
- **Must provide** target human with unique capture sequence behavior

---

## Decision

**Use Unity NavMesh with AI State Machine pattern** for enemy behaviors and pathfinding.

### Architecture Diagram

```
┌──────────────────────────┐
│   Enemy AI System       │
│   (NavMesh + State     │
│    Machine)             │
└──┬──────────────────────┘
   │
   ├──────────────────┬──────────────────┐
   ↓                  ↓                  ↓
┌─────────┐     ┌─────────┐     ┌──────────┐
│ Guard   │     │ Armored │     │ Target   │
│ AI State│     │ Guard   │     │ Human    │
│ Machine │     │ AI State │     │ AI State  │
└────┬────┘     └────┬────┘     └─────┬────┘
     │                │                  │
     └────────────────┴──────────────────┘
                      ↓
              ┌─────────────────┐
              │ Combat System  │
              │ (targets)      │
              └─────────────────┘
```

### Key Interfaces

```csharp
// Enemy AI exposes these interfaces to other systems
public interface IEnemyAI
{
    // Combat System
    void TakeDamage(float damage, Vector3 hitPosition);
    bool CanSeePlayer();
    void SetAlertLevel(AlertLevel level);

    // Movement System
    Vector3 GetPosition();
    bool IsMoving();

    // State System
    EnemyState GetCurrentState();
    void SetPatrolRoute(Transform[] waypoints);
    void EngageTarget(Transform target);
}

public enum EnemyState
{
    Idle,
    Patrolling,
    Alerted,
    Engaging,
    Fleeing,
    Captured,
    Dead
}

public enum AlertLevel
{
    Unaware,
    Suspicious,
    Alerted
}
```

### Implementation Guidelines

1. **NavMesh Baking**: Bake separate NavMesh per era level with era-specific surface costs (e.g., Future era has faster traversal surfaces)
2. **State Machine**: Use Unity Animator or custom state machine for enemy behaviors (Patrol → Alert → Engage → Dead)
3. **Vision System**: Cone-based raycast detection for player awareness with era-specific detection radii
4. **Cover AI**: NavMesh queries for nearby cover points; enemies move to cover when low health
5. **Target Human AI**: Special state machine with capture sequence triggers, not standard combat AI
6. **Era Parameters**: ScriptableObjects for era-specific AI configurations (alert ranges, accuracy, patrol speed)

---

## Alternatives Considered

### Alternative 1: Behavior Tree AI

- **Description**: Use Unity's Behavior Tree system or external BT asset for enemy logic
- **Pros**: More flexible AI, easier to add new enemy types, visual editor for behavior design
- **Cons**: Higher complexity, learning curve, performance overhead for complex trees
- **Estimated Effort**: Higher (2-3 weeks vs 1-2 weeks for state machine)
- **Rejection Reason**: Enemy types are limited (3 main types); state machine provides sufficient complexity with lower effort

### Alternative 2: Custom Pathfinding

- **Description**: Implement A* or Dijkstra pathfinding from scratch
- **Pros**: Full control over pathfinding algorithm, can optimize for specific game needs
- **Cons**: Significant development time, must handle complex geometry from scratch, no built-in off-mesh links
- **Estimated Effort**: Highest (4-5 weeks)
- **Rejection Reason**: NavMesh provides production-ready pathfinding with era-specific surface support

### Alternative 3: Grid-Based AI

- **Description**: Use grid-based pathfinding and AI movement
- **Pros**: Simple to implement, predictable performance
- **Cons**: Limited accuracy on complex level geometry, requires manual grid setup per level
- **Estimated Effort**: Medium (2-3 weeks)
- **Rejection Reason**: 3D parkour levels require smooth pathfinding; grid system creates artificial constraints

---

## Consequences

### Positive

- **Production Ready**: NavMesh is battle-tested across many Unity games
- **Era Flexibility**: NavMesh surface types support era-specific traversal costs
- **Performance**: NavMesh is optimized for concurrent pathfinding queries
- **Extensibility**: State machine pattern allows easy addition of new enemy types
- **Editor Tools**: NavMesh visualization and baking tools streamline level design

### Negative

- **NavMesh Baking**: Level changes require rebaking NavMesh (can be slow for large levels)
- **State Machine Complexity**: Managing state transitions for 3+ enemy types requires careful design
- **Cover Finding**: NavMesh doesn't provide built-in cover query; requires custom implementation

### Neutral

- **File Structure**: Adds EnemyAIController.cs, EnemyState.cs, EraAIConfig ScriptableObjects

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|-----------|
| NavMesh baking time exceeds budget | MEDIUM | MEDIUM | Bake NavMesh asynchronously during level load; show loading screen |
| State machine logic becomes complex | HIGH | MEDIUM | Use Animator State Machine for visual transitions; keep logic in dedicated AI controller |
| Enemy AI performance drops at 20+ agents | MEDIUM | HIGH | Implement AI LOD (simplify behaviors for distant enemies); profile early |
| Era-specific AI parameters cause imbalance | MEDIUM | MEDIUM | Centralize AI configs in ScriptableObjects; balance through iterative testing |

---

## Performance Implications

| Metric | Before | Expected After | Budget |
|--------|--------|---------------|--------|
| CPU (frame time) | 0ms | 0.5-2.0ms | 4.0ms |
| Memory | 0MB | ~5MB (NavMesh data + AI controllers) | 50MB |
| Load Time | 0s | ~0.3s per level (NavMesh bake cache) | 2.0s |
| Network | N/A | N/A | N/A |

---

## Migration Plan

**From scratch**: N/A — new project implementation

---

## Validation Criteria

- [ ] Basic guards patrol assigned routes correctly
- [ ] Enemy vision cones detect player within configured range
- [ ] State transitions work: Idle → Alerted → Engaged → Dead
- [ ] 20+ concurrent enemies run at 60 FPS minimum
- [ ] Era-specific AI configs apply correctly (e.g., Future enemies have higher alertness)
- [ ] Target human executes unique capture sequence, not standard combat
- [ ] NavMesh rebaking on level changes completes within 5 seconds
- [ ] Enemy AI never gets stuck on level geometry

---

## GDD Requirements Addressed

| GDD Document | System | Requirement | How This ADR Satisfies It |
|-------------|--------|-------------|--------------------------|
| game-concept.md | Enemy AI | 3 enemy types with distinct behaviors | State machine pattern supports separate behaviors for guard, armored guard, target human |
| game-concept.md | Enemy AI | Era-specific enemy intelligence | ScriptableObject configs enable era-specific AI parameters |
| combat-system.md | Enemy AI | Enemy health and hit detection | IEnemyAI interface provides TakeDamage method for combat system |
| combo-system.md | Enemy AI | Kill events for combo scoring | Enemy death state triggers combo system events |

---

## Related

- ADR-0002: Physics Collision Strategy (provides collision for enemy interactions)
- ADR-0003: Parkour Movement Implementation (enemy AI tracks player position)
- ADR-0004: Combat System Architecture (consumes enemy AI targets for combat)
