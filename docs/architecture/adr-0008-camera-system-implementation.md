# ADR-0008: Camera System Implementation

## Status
Proposed

## Date
2026-04-11

## Last Verified
2026-04-11

## Decision Makers
Technical Director, Lead Programmer

## Summary
Camera System must follow fast-paced parkour movement while maintaining readability. Decision: Use Cinemachine for production polish with custom motion-forward feel through camera shake and dynamic FOV adjustments.

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Rendering / Camera |
| **Knowledge Risk** | MEDIUM — Cinemachine API changes post-LLM-cutoff, but core concepts stable |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | Cinemachine 3.x API (Post-6.0), Virtual Camera extensions |
| **Verification Required** | Test camera shake performance, validate FOV adjustments at 144 FPS |

### Key Post-Cutoff Changes

- **Cinemachine 3.x**: Complete API overhaul from 2.x, new component architecture
- **Impulse Listener**: New impulse-based camera shake system replacing legacy shake methods
- **FOV Kicks**: Built-in FOV manipulation for weapon recoil and movement feedback
- **Noise Handling**: Improved perlin noise-based camera movement for dynamic feel

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Movement System (ADR-0003) for position tracking, Input System (ADR-0001) for aim control |
| **Enables** | Combat System (combat framing), UI System (screen-space effects), VFX System (camera-based effects) |
| **Blocks** | None |
| **Ordering Note** | Camera must be implemented before Combat System integration for weapon-camera coupling |

---

## Context

### Problem Statement

The game requires a camera that follows fast parkour movement (wall-running, dashing, climbing) while maintaining combat readability. Camera must feel "in motion" even when player stops briefly (Feline Flow pillar). Unity 6.3 LTS offers native Camera.main and Cinemachine 3.x packages.

### Current State

No camera system exists. Design phase requires decision on camera architecture before implementation.

### Constraints

- **Technical**: Must maintain 60 FPS minimum, 144 FPS preferred
- **Gameplay**: Camera must never make player lose orientation during fast movement
- **Aesthetic**: Motion-forward visual language requires camera shake and FOV dynamics
- **Platform**: Must support both keyboard/mouse and gamepad aiming

### Requirements

- **Must follow** player position smoothly during wall-running and dashing
- **Must provide** aim-down-sights (ADS) for precision shooting
- **Must include** camera shake on weapon fire and impacts
- **Must support** dynamic FOV based on movement velocity
- **Must maintain** readable combat framing (enemies always visible)

---

## Decision

**Use Cinemachine 3.x** with custom ImpulseListener for camera shake and Virtual Camera for player tracking.

### Architecture Diagram

```
┌──────────────────────────┐
│   Camera System          │
│   (Cinemachine 3.x)     │
└──┬──────────────────────┘
   │                      │
   ↓                      ↓
┌────┴────┐        ┌─────┴──┐
│ Movement  │        │ Combat   │
│ System   │        │ System   │
│ (pos)    │        │ (aim)    │
└──────────┘        └──────────┘
```

### Key Interfaces

```csharp
// Camera System exposes these interfaces to other systems
public interface ICameraProvider
{
    // Movement System
    void SetTarget(Transform target);
    void SetLookAtTarget(Transform lookTarget);
    void AddCameraImpulse(Vector3 impulse);
    void ShakeCamera(float intensity, float duration);

    // Combat System
    void EnableAimMode(bool enabled);
    void ApplyRecoilFOV(float amount);
    void OnWeaponFired(float recoilStrength);

    // VFX/System
    Camera MainCamera { get; }
    Vector3 Forward { get; }
    Vector3 Right { get; }
}
```

### Implementation Guidelines

1. **Virtual Camera Setup**: Use Cinemachine Virtual Camera targeting player with Look At component
2. **Camera Shake**: Implement Cinemachine Impulse Listener with custom impulse sources for weapon fire and impacts
3. **FOV Dynamics**: Modify Virtual Camera FOV based on player velocity (expand when fast, contract when still)
4. **ADS Mode**: Switch camera transform when aiming-down-sights for precision
5. **Motion Blur**: Use Cinemachine Motion Blur component for speed feel (tunable in quality settings)

---

## Alternatives Considered

### Alternative 1: Native Camera.main

- **Description**: Use Unity's native Camera.main with manual follow logic in Update()
- **Pros**: Zero dependencies, full control over every frame, no Cinemachine learning curve
- **Cons**: Must implement camera smoothing, shake, FOV dynamics from scratch; reinventing well-solved problems
- **Estimated Effort**: Higher (2-3 weeks vs 1 week with Cinemachine)
- **Rejection Reason**: Cinemachine provides production-ready camera polish; solo dev needs time efficiency

### Alternative 2: Custom Camera System

- **Description**: Build custom camera system with state machine for different camera modes
- **Pros**: Exact control for game-specific needs (parkour camera angles)
- **Cons**: Significant development time, must solve smoothing, shake, collisions from scratch
- **Estimated Effort**: Highest (3-4 weeks)
- **Rejection Reason**: Cinemachine supports custom camera behaviors through extensions without full rewrite

---

## Consequences

### Positive

- **Production Polish**: Cinemachine provides industry-standard camera smoothing and shake
- **Motion-Forward Feel**: Built-in camera shake and FOV dynamics support Feline Flow pillar
- **Extensibility**: Cinemachine extensions allow game-specific camera behaviors without rewrite
- **Rapid Iteration**: Visual editor for camera tuning reduces iteration time
- **Proven Solution**: Extensive community resources and examples for Cinemachine 3.x

### Negative

- **Learning Curve**: Cinemachine 3.x API differs from 2.x; requires time to learn
- **Additional Dependency**: Cinemachine package adds ~10MB to project size
- **Performance Overhead**: Cinemachine adds minor CPU overhead (estimated 0.5-1ms per frame)

### Neutral

- **File Structure**: Adds CameraController.cs and associated Cinemachine prefabs to project

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|-----------|
| Cinemachine overhead exceeds budget | MEDIUM | MEDIUM | Profile camera performance during parkour; if >1ms, fallback to selective features |
| Camera shake causes motion sickness | MEDIUM | HIGH | Add motion sickness toggle in settings; limit shake intensity to safe range |
| FOV changes cause visual discomfort | LOW | MEDIUM | Make FOV changes gradual (lerp) and allow user disabling in settings |

---

## Performance Implications

| Metric | Before | Expected After | Budget |
|--------|--------|---------------|--------|
| CPU (frame time) | 0ms | 0.5-1.0ms | 2.0ms |
| Memory | 0MB | ~10MB (Cinemachine package) | 50MB |
| Load Time | 0s | ~0.5s | 2.0s |
| Network | N/A | N/A | N/A |

---

## Migration Plan

**From scratch**: N/A — new project implementation

---

## Validation Criteria

- [ ] Camera follows wall-running player smoothly without losing target
- [ ] Camera shake provides satisfying feedback on weapon fire
- [ ] FOV expansion on fast movement enhances speed feel without disorientation
- [ ] ADS mode provides adequate precision for headshots
- [ ] Camera frame time stays under 1ms at 144 FPS during parkour combat
- [ ] Camera never clips through level geometry
- [ ] Gamepad aiming provides equivalent precision to mouse/keyboard

---

## GDD Requirements Addressed

| GDD Document | System | Requirement | How This ADR Satisfies It |
|-------------|--------|-------------|--------------------------|
| game-concept.md | Camera | Motion-forward camera feel | Cinemachine ImpulseListener and FOV dynamics provide constant motion feel |
| game-concept.md | Camera | Combat readability | Cinemachine Virtual Camera Look At ensures enemies remain in frame |
| physics-system.md | Camera | Surface detection visibility | Camera positioning provides clear view of traversable surfaces (cyan-coded) |
| combat-system.md | Camera | Weapon recoil feedback | Cinemachine Impulse on weapon fire provides recoil shake |

---

## Related

- ADR-0001: Input System Architecture (provides aim control)
- ADR-0003: Parkour Movement Implementation (provides player position for camera to follow)
- ADR-0004: Combat System Architecture (consumes camera events for aim and recoil)
