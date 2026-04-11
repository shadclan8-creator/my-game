# ADR-0004: Combat System Architecture

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Core (Combat) + Entities/DOTS |
| **Knowledge Risk** | HIGH — Entities/DOTS major API overhaul post-cutoff |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md`, Unity 6.3 DOTS docs |
| **Post-Cutoff APIs Used** | Entities (ECS), Jobs System, Burst Compiler |
| **Verification Required** | Test combat performance with DOTS, validate weapon pooling under high fire rate |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Input System (ADR-0001), Movement System (ADR-0003), Combo System (ADR-0007) |
| **Enables** | Enemy AI System (damage targets), VFX System (kill effects), UI System (ammo/weapon display), Game State System (weapon unlocks) |
| **Blocks** | None |
| **Ordering Note** | Core system — depends on foundation systems |

---

## Context

### Problem Statement

Implement combat system supporting CoD-style weapon feel (recoil, reload animations, audio feedback) with weapon pooling, damage detection, and kill events while maintaining 60 FPS target.

### Constraints

- **CoD Gun Authenticity**: Weapons must feel like military firearms with realistic recoil and reload
- **Performance**: Frame budget of 16.6ms at 60 FPS limits combat calculations
- **Weapons**: 8 weapons across 4 eras = potentially 32 weapon variations
- **Pooling**: Must use object pooling for projectiles to maintain performance
- **Integration**: Must work with Combo System for scoring, Movement System for hit detection

### Requirements

- **Must support** weapon swapping and selection
- **Must provide** CoD-style recoil patterns per weapon type
- **Must implement** reload animations with interruption support
- **Must use** object pooling for projectiles
- **Must trigger** kill events for Combo System integration

---

## Decision

**Use Entities/DOTS for Weapon System with GameObject Pool for Projectiles**

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
│    ↓       ↓
│ ┌────┴──┐ ┌──────┴──┐
│ │Combat   │ │ Combo    │
│ │System   │ │ System    │
│ └────┬────┘ └───────────┘
│      │
│      ↓     ↓
│ ┌────┴──┐ ┌──────┴──┐
│ │ Enemy  │ │ VFX      │
│ │ AI      │ │ System    │
│ └──────────┘ └───────────┘
```

### Key Interfaces

```csharp
// Combat System interfaces for weapon and damage
public interface ICombatProvider
{
    void FireWeapon(Vector3 direction);
    void ReloadWeapon();
    bool CanDamage(IKillable target);
    void OnKill(IKillable target, Vector3 hitPosition);
    Weapon GetCurrentWeapon();
    int GetCurrentAmmo();
    void SwitchWeapon(int weaponId);
}

// Killable interface for damage targets
public interface IKillable
{
    void TakeDamage(int damage, Vector3 hitPoint);
    bool IsDead();
    Vector3 GetPosition();
}

// Weapon data structure (managed by DOTS)
public struct WeaponData
{
    public int weaponId;
    public int currentAmmo;
    public int maxAmmo;
    public float reloadTime;
    public float fireRate;
    public Entity entityRef;
}
```

---

## Alternatives Considered

### Alternative 1: Pure GameObject-Based Combat

- **Description**: Use traditional GameObjects with MonoBehaviour for all weapons
- **Pros**: Familiar Unity pattern, extensive learning resources available
- **Cons**: Poor performance with many weapons, cannot easily benefit from DOTS optimizations
- **Rejection Reason**: Entities/DOTS is production-ready in Unity 6.3 LTS and provides significant performance benefits for projectile-heavy systems

### Alternative 2: Hybrid Approach (GameObject weapons + DOTS projectiles)

- **Description**: Keep weapon logic as GameObjects but use DOTS for projectiles only
- **Pros**: Leverages existing Unity patterns for weapons while gaining DOTS performance for projectiles
- **Cons**: Complexity of managing two different entity systems (GO vs DOTS)
- **Rejection Reason**: Inconsistent architecture; DOTS learning curve applies to projectiles but not weapons

---

## Consequences

### Positive

- **Projectile Performance**: DOTS ECS efficiently handles thousands of active projectiles
- **Learning Curve**: GameObject-based weapons use familiar patterns; team can ramp up to full DOTS later if needed
- **Weapon Feel**: MonoBehaviour weapons support complex animation and audio systems easily

### Negative

- **Architectural Split**: Two systems (GO weapons + DOTS projectiles) require different patterns and debugging approaches
- **DOTS Learning Curve**: Entities/DOTS is major API change; new patterns unfamiliar to team
- **Integration Complexity**: Synchronizing GO-based weapons with DOTS projectile pools adds complexity

### Risks

- **DOTS API Stability**: As new API (introduced 6.0), may have bugs or missing features compared to GameObject approach
- **Performance Regression**: If weapon logic becomes complex, may outgrow GameObject performance benefits
- **Testing**: Requires testing both GO components and DOTS systems thoroughly

**Mitigation**:
- Prototype core combat with both approaches; measure performance difference before committing
- Start with simpler weapons; use DOTS only if performance is insufficient
- Document DOTS patterns thoroughly for team knowledge sharing

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-003) | CoD-style weapons with recoil, reload, audio | MonoBehaviour weapons support complex animations; DOTS projectiles handle projectile performance |
| game-concept.md (TR-concept-009) | 60 FPS minimum | DOTS projectile pooling reduces CPU/GPU overhead; frame budget compliant |
| game-concept.md (TR-concept-002) | Parkour movement: wall-run, climb, quick dashes | Movement System hit detection integrates with Combat System for aiming at moving targets |

---

## Performance Implications

- **CPU**: Weapon logic estimated at 1-2ms per frame; DOTS projectile updates at 0.05ms per projectile; total acceptable
- **Memory**: DOTS entity storage is memory-efficient; projectile pool reduces allocations
- **Load Time**: Minimal — weapons are small; initialize on equip
- **Network**: Not applicable

---

## Migration Plan

N/A — new project using correct engine version

---

## Validation Criteria

- **Fire rate test**: Validate weapons maintain CoD-style fire rates (AR: 600 RPM, Shotgun: 200 RPM) without performance degradation
- **Recoil test**: Verify recoil patterns feel authentic while maintaining playability
- **Projectile pool test**: Confirm no allocations during sustained combat
- **Frame budget test**: Profile combat with 100+ active projectiles; verify 60 FPS maintained

---

## Related Decisions

- Input System Architecture (ADR-0001) — provides fire/reload triggers
- Movement System Implementation (ADR-0003) — provides player position for hit detection
- Combo System Architecture (ADR-0007) — consumes kill events for scoring
