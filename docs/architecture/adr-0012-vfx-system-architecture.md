# ADR-0012: VFX System Architecture

## Status
Proposed

## Date
2026-04-11

## Last Verified
2026-04-11

## Decision Makers
Technical Director, VFX Artist, Technical Artist

## Summary

VFX System requires stylized blood bursts, motion blur for speed feel, muzzle flashes, trail effects, and dynamic lighting. Decision: Use Unity VFX Graph for complex effects with Particle System for simple one-shots, object pooling for performance.

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Rendering / VFX |
| **Knowledge Risk** | HIGH — VFX Graph is new post-LLM-cutoff, major overhaul from legacy Shuriken |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | VFX Graph 1.x, Visual Effect Asset, Particle system updates |
| **Verification Required** | Test VFX performance at 144 FPS, validate particle counts, measure GPU impact |

### Key Post-Cutoff Changes

- **VFX Graph**: Node-based VFX authoring replacing Shuriken particle system
- **Visual Effect Assets**: New asset type containing complete VFX graphs and dependencies
- **GPU Particles**: Compute shader-based particle systems for massive counts
- **Event-Driven VFX**: VFX graphs respond to game events through exposed parameters

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Camera System (ADR-0008) for screen-space effects, Combat System (ADR-0004) for kill/impact triggers |
| **Enables** | Combat System (weapon effects), Movement System (parkour trails), Combo System (combo bursts) |
| **Blocks** | None |
| **Ordering Note** | VFX System can be implemented in parallel with Combat System |

---

## Context

### Problem Statement

The game requires visual feedback for combat (blood, muzzle flash, kill effects), movement (motion blur, trails), and combo milestones (combo bursts). VFX must support the "Momentum Ink" visual identity (high contrast, electric energy feel). Unity 6.3 LTS offers VFX Graph (new) and legacy Shuriken particles.

### Current State

No VFX system exists. Design requires decision between VFX Graph and legacy particles for visual effects.

### Constraints

- **Technical**: Must maintain 60 FPS with complex VFX (10+ simultaneous effects)
- **Aesthetic**: VFX must match "Momentum Ink" visual identity (complementary colors, kinetic energy)
- **Performance**: GPU frame budget is 4ms for VFX rendering
- **Content**: Stylized blood (not realistic pools), motion blur for speed feel

### Requirements

- **Must provide** stylized blood bursts on enemy kills
- **Must include** muzzle flash for all weapons
- **Must support** motion blur during high-speed movement
- **Must create** trail effects for parkour actions (dash, wall-run)
- **Must render** combo milestone effects at multiplier thresholds
- **Must use** object pooling for frequently spawned effects

---

## Decision

**Use Unity VFX Graph for complex effects with Particle System Pooling for simple one-shots.**

### Architecture Diagram

```
┌──────────────────────────┐
│   VFX System           │
│   (VFX Graph +         │
│    Particle Pool)         │
└──┬──────────────────────┘
   │
   ├──────────────────┬──────────────────┬──────────────────┐
   ↓                  ↓                  ↓                  ↓
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│ Combat   │     │ Movement │     │ Combo     │     │ Screen    │
│ VFX      │     │ VFX      │     │ VFX       │     │ VFX       │
│ (blood,   │     │ (blur,   │     │ (bursts)  │     │ (camera   │
│  muzzle)  │     │ trails)   │     │           │     │  effects)  │
└────┬────┘     └────┬────┘     └─────┬────┘     └─────┬────┘
     │                │                  │                  │
     └────────────────┴──────────────────┴──────────────────┘
                      ↓
              ┌─────────────────┐
              │   VFX Pool     │
              │ (reusable       │
              │    effects)      │
              └─────────────────┘
```

### Key Interfaces

```csharp
// VFX System exposes these interfaces to other systems
public interface IVFXProvider
{
    // Combat System
    void SpawnBloodEffect(Vector3 position, Vector3 normal, Color bloodColor);
    void SpawnMuzzleFlash(Vector3 position, Quaternion rotation, WeaponType weapon);
    void SpawnKillEffect(Vector3 position, EnemyType enemyType);
    void SpawnImpactEffect(Vector3 position, SurfaceType surface);

    // Movement System
    void SetMotionBlurIntensity(float intensity); // 0 to 1
    void SpawnTrailEffect(Vector3 position, TrailType type);
    void SpawnDashEffect(Vector3 position, Vector3 direction);

    // Combo System
    void SpawnComboBurst(int comboCount, Vector3 position);
    void SpawnMilestoneEffect(int milestone, Vector3 position); // 10x, 25x, 50x

    // Screen Effects
    void SpawnScreenShake(float intensity, float duration);
    void SpawnDamageFlash(Color color, float duration);

    // Pool Management
    void PreWarmPool(int effectCount);
}

public enum WeaponType
{
    AssaultRifle,
    SMG,
    Shotgun,
    SniperRifle,
    LMG
}

public enum EnemyType
{
    BasicGuard,
    ArmoredGuard,
    TargetHuman
}

public enum SurfaceType
{
    Wood,
    Metal,
    Concrete,
    Glass
}

public enum TrailType
{
    Dash,
    WallRun,
    Climb
}
```

### Implementation Guidelines

1. **VFX Graph Authoring**: Use VFX Graph for complex effects (blood bursts, combo milestones, muzzle flash)
2. **Particle Pooling**: Object pooling for simple one-shot effects (impacts, small trails)
3. **Motion Blur**: Use Unity built-in Motion Blur component; adjust intensity based on player velocity
4. **Era-Themed VFX**: Color-coded effects per era (red/cyan core with era accent overlay)
5. **GPU Particles**: Use GPU particles for blood spray (hundreds of particles)
6. **Muzzle Flash**: VFX Graph with randomized scale/rotation for weapon feel variation
7. **Combo Effects**: Burst VFX at player position when hitting milestones (10x, 25x, 50x)
8. **Screen Effects**: Post-process stack for camera shake and damage flash

---

## Alternatives Considered

### Alternative 1: Legacy Shuriken Particles Only

- **Description**: Use only legacy Unity particle system for all VFX
- **Pros**: Familiar API, extensive community knowledge, no new asset types
- **Cons**: Limited visual complexity, no node-based authoring, harder to iterate
- **Estimated Effort**: Higher (3 weeks vs 2 weeks for VFX Graph)
- **Rejection Reason**: VFX Graph provides superior visual quality and iteration speed

### Alternative 2: Custom Shader VFX

- **Description**: Write custom shaders for all visual effects
- **Pros**: Full control, potentially higher performance, no VFX Graph overhead
- **Cons**: Extremely high development time, requires shader expertise, hard to iterate
- **Estimated Effort**: Highest (5-6 weeks)
- **Rejection Reason**: VFX Graph provides similar quality with much less effort

### Alternative 3: No VFX Pooling

- **Description**: Instantiate VFX graph assets for each effect without pooling
- **Pros**: Simpler implementation, no pool management code
- **Cons**: GC spikes from VFX asset instantiations, potential frame drops
- **Estimated Effort**: Lower (1 week vs 2 weeks)
- **Rejection Reason**: Performance requirements (60 FPS) necessitate pooling for frequent effects

---

## Consequences

### Positive

- **Visual Quality**: VFX Graph enables high-fidelity effects (blood spray, dynamic muzzle flash)
- **Iteration Speed**: Node-based authoring allows rapid VFX iteration without code changes
- **Performance**: Object pooling eliminates GC spikes from VFX spawning
- **Era Theming**: VFX Graph parameters support era color variations without separate assets
- **Momentum Identity**: Motion blur and trail effects enhance "everything in motion" visual language

### Negative

- **Learning Curve**: VFX Graph is new and differs significantly from Shuriken
- **Asset Size**: VFX Graph assets include embedded data; may increase project size
- **Debugging**: Node-based graphs can be harder to debug than code-based VFX

### Neutral

- **File Structure**: Adds VFX Graph assets (.vfx), particle prefab pool script, VFX manager

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|-----------|
| VFX Graph performance overhead | MEDIUM | MEDIUM | Profile VFX with 20+ simultaneous effects; simplify graphs if exceeding GPU budget |
| Motion blur causes motion sickness | LOW | HIGH | Add motion sickness toggle in settings; cap maximum intensity |
| Blood effect violates content guidelines | LOW | LOW | Use stylized, non-realistic blood effects (cartoonish bursts) |
| VFX pool size insufficient | MEDIUM | MEDIUM | Implement dynamic pool expansion with warning logging |

---

## Performance Implications

| Metric | Before | Expected After | Budget |
|--------|--------|---------------|--------|
| CPU (frame time) | 0ms | 0.3-1.5ms | 2.0ms |
| GPU (frame time) | 0ms | 1.0-4.0ms | 6.0ms |
| Memory | 0MB | ~30MB (VFX assets + pool) | 50MB |
| Load Time | 0s | ~0.2s (VFX prewarm) | 2.0s |
| Network | N/A | N/A | N/A |

---

## Migration Plan

**From scratch**: N/A — new project implementation

---

## Validation Criteria

- [ ] Blood bursts spawn correctly on enemy kills (stylized, not realistic)
- [ ] Muzzle flash plays on every weapon fire
- [ ] Motion blur intensity scales with player velocity
- [ ] Trail effects spawn during dash, wall-run, climb actions
- [ ] Combo burst effects spawn at 10x, 25x, 50x milestones
- [ ] Screen shake provides satisfying feedback on impacts
- [ ] VFX frame time stays under 4ms GPU budget with 20+ simultaneous effects
- [ ] Era color themes apply correctly to VFX
- [ ] Object pooling prevents GC spikes during combat

---

## GDD Requirements Addressed

| GDD Document | System | Requirement | How This ADR Satisfies It |
|-------------|--------|-------------|--------------------------|
| game-concept.md | VFX System | Stylized blood bursts, motion blur, muzzle flashes | VFX Graph blood effects and motion blur post-process provide required visual feedback |
| game-concept.md | VFX System | Trail effects, dynamic lighting for motion feel | Trail VFX and dynamic lighting in VFX Graph enhance movement feel |
| combat-system.md | VFX System | Particle burst on kill, muzzle flash on fire | IVFXProvider SpawnKillEffect and SpawnMuzzleFlash methods provide combat VFX |
| combo-system.md | VFX System | Combo milestone effects | Combo burst and milestone VFX provide visual feedback for progression |

---

## Related

- ADR-0003: Parkour Movement Implementation (triggers motion blur and trails)
- ADR-0004: Combat System Architecture (triggers muzzle flash and kill effects)
- ADR-0007: Combo System Architecture (triggers combo milestone effects)
- ADR-0008: Camera System Implementation (consumes screen-shake effects)
