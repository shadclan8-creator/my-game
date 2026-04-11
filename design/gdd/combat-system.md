# Combat System

## Status
In Design

## Author
Claude (AI Agents: systems-designer + ai-programmer)

## Last Updated
2026-04-09

## Overview

CoD-style weapon system with authentic recoil patterns, reload animations, audio feedback. Each weapon type has distinct feel and behavior. Delivers damage through hit detection to Movement System and Combo System.

## Player Fantasy

Authentic military firepower at your fingertips. Every trigger feels powerful and responsive. Weapon kickback, reload animations, and audio feedback deliver lethal satisfaction without gameplay interruption.

## Detailed Rules

### Weapon Types

- **Assault Rifles** (AR, SMG, LMG, Shotgun, Sniper, LMG): Full auto fire, high damage, accurate.
- **SMGs** (AR, SMG): 3-round burst, medium damage, fast reload.
- **Shotguns** (SG, Sniper): High damage, semi-auto, low ROF, long reload.
- **LMGs** (LMG, LMG): Medium damage, fast cyclic, large magazine.
- **Sniper Rifles**: 2-round burst, high damage, bolt-action, long reload.
- **MGs** (LMG, LMG): High fire rate, low ROF, medium reload.
- **Shotguns** (SG): Single-fire mode, low damage, slow ROF.

### Weapon Behavior

- **Firing Pattern**: Press/hold for continuous fire. Each trigger produces muzzle flash, audio cue, and small camera shake.
- **Reload**: Interrupts fire action, takes 0.5-0s.7s cooldown while playing reload animation.
- **Switching Weapons**: Cycle through available weapons (keys 1-4).
- **Empty Ammo**: Out of ammo → Cannot fire. Must switch or reload.

### Damage Model

```
Damage = (BaseDamage * WeaponMultiplier) * BodyPartMultiplier
```

**Variables**:
| Variable | Symbol | Type | Range | Description |
|----------|--------|------|------------|
| BaseDamage | float | 100 to 200 | Per bullet damage baseline |
| BodyPartMultiplier | float | 1.0 to 1.5 | Body part multiplier (headshot = 2x) |
| WeaponMultiplier | float | 0.5 to 3.0 | Weapon tier multiplier (LMG=2.5x, LMG=2.0x) |

**Output Ranges**:
| Formula | Output | Range | Notes |
|----------|-------|------|--------|
| Damage | 10 to 1000 | Base damage range for SMG |
| Headshot Damage | 20 to 50 | Critical hit tier |
| Sniper Damage | 80 to 120 | One-shot kill (low ROF) |
| Weapon Damage | 30 to 60 | Burst damage output range |
| Headshot Damage | 200 to 400 | Boss-level damage output range |
| Shotgun Damage | 10 to 30 | Spread shot output range |
| LMG Damage | 60 to 140 | Fast cyclic output range |
| Sniper Damage | 100 to 300 | Precision shot output range |

## Edge Cases

- **Weapon Jam**: Fire action pressed while empty. Play empty reload audio, return to ready state, no animation interruption.
- **Switching During Reload**: Weapon switching mid-fire. Discard current ammo. Cannot fire during switch, must wait for reload to complete.
- **Multiple Simultaneous Weapons**: Press fire + dash cancels reload, cancel each other.
- **Target Out of Ammo**: Out of ammo → Switch to melee (if available) or empty reload.

## Interactions with Other Systems

- **Movement System** (ADR-0003): Consumes kill events and position data for death/hit scoring
- **Combo System** (ADR-0007): Consumes kill events and movement events for score multipliers
- **Game State System** (ADR-0008): Saves kill count and level progress

### Weapon Data Structures

```csharp
[System.Serializable]
public class WeaponData
{
    public WeaponType weaponType;
    public int currentAmmo;
    public int maxAmmo;
    public int fireRate;
    public float fireRate;
    public float damagePerSecond;
    public float reloadTime;
    public float recoilPatternX;
    public float recoilPatternY;
    public float damagePerHit;

    // Combat system queries these values
}
```

## Tuning Knobs

| Tuning Knob | Range | Effect | Notes |
|-------------|-------|--------|-------------|
| BaseDamage | 10 to 100 | Bullet damage baseline |
| FireRate | 0.05 to 1.0 | Fire rate (rounds per second) |
| BodyPartMultiplier | 1.0 to 1.5 | Headshot damage modifier |
| WeaponMultiplier | 0.5 to 3.0 | Weapon tier multiplier |
| ReloadCooldown | 0.15 to 0.3 | Seconds per reload |
| WeaponKickForce | 100 to 400 | Kickback intensity |
| HeadshotDamage | 200 to 400 | Boss damage cap |
| ReloadSpeed | 0.3 to 0.5 | Reload animation speed |
| WeaponSwitchTime | 0.2 to 0.5s | Weapon switch duration |

## Visual/Audio Requirements

- **Weapon Models**: Distinct silhouettes for each weapon type with authentic military design
- **Visual Feedback**: Muzzle flash on fire, camera shake on damage, particle burst on kill
- **Audio**: Distinct fire sounds per weapon (SMG staccato, LMG rattle, SG click, Sniper ping, Sniper boom)
- **Animation**: Authentic reload animations with frame-by-frame keyframing. MG reloads have rotating magazine visual indicator.
- **UI Requirements**: Ammo count display, weapon switch selector, reload progress bar, damage number popup on kill.

## Dependencies

- **Movement System** (ADR-0003): Consumes player position and velocity for hit detection
- **Combo System** (ADR-0007): Consumes kill events and movement events for scoring
- **Game State System** (ADR-0008): Saves weapon unlocks and level progress
- **Camera System** (ADR-0005): Consumes damage events and death for camera effects
- **Audio System** (ADR-???): Consumes weapon fire/reload events for audio feedback

## Acceptance Criteria

1. **Weapon Feel**: Each weapon fires with appropriate power for enemy tier. SMG handles standard guards; Shotguns require precision; MGs handle swarms.
2. **Damage Model**: Damage numbers produce realistic results. High damage kills feel impactful. Headshots and Snipers provide satisfaction on tough enemies.
3. **Reload Feel**: Reload animations complete within frame budget (0.15s per reload).
4. **Input**: Fire, switch, and reload actions map to intuitive keybindings.
5. **Audio**: Weapon sounds distinct and impactful. Recoil patterns feel authentic to CoD.

2. **UI Integration**: HUD displays ammo count, current weapon, damage number, and kills score. All data flows cleanly through event system.

3. **Performance**: Projectile pooling keeps memory under 100ms frame budget. No garbage collection during combat.

## Open Questions

- **Weapon Feel vs Combat Balance**: Should SMGs be nerfed to prevent overwhelming power?
- **Multiple Weapons Simultaneity**: Should players be able to switch weapons mid-combat freely without losing momentum?
- **Boss Damage**: Should damage scale with boss health or use instant-kill mechanics?
- **Co-op Mechanics**: How should dual-weapon interactions work? Are there scenarios where two weapons are beneficial?

---

## Dependencies

- **Input System** (ADR-0001): Provides fire, reload, switch, dash inputs
- **Movement System** (ADR-0003): Consumes input data for movement state
- **Combo System** (ADR-0007): Consumes kill and movement events
- **Game State System** (ADR-0008): Saves weapon unlock progress
- **Camera System** (ADR-0005): Consumes damage events for death/camera effects
- **UI System** (ADR-???): Consumes combat data for display updates

---

## GDD Requirements Addressed

| GDD Requirement | How This ADR Addresses It |
|-------------|-------------|-----------------|-------------|
| game-concept.md TR-concept-002 | CoD-style weapons with realistic recoil, reload, audio feedback | Movement System (ADR-0003) provides position for hit detection; Combat System consumes events from both systems for weapon mechanics integration |
| game-concept.md TR-concept-003 | Combo system rewards continuous movement and precision kills with multipliers | Combo System (ADR-0007) consumes kill and movement events for scoring |
| game-concept.md TR-concept-009 | 60 FPS minimum, 144 FPS preferred | Combat system stays within frame budget by using projectile pooling |
| game-concept.md TR-concept-002 | Keyboard/mouse + gamepad input support | Input System (ADR-0001) provides device switching for accessibility |
| game-concept.md TR-concept-009 | 60 FPS minimum, 144 FPS preferred | Input System (ADR-0001) supports keyboard/mouse as primary input with full gamepad support as secondary |
| game-concept.md TR-concept-009 | 60 FPS minimum, 144 FPS preferred | Camera System (ADR-0005) provides smooth camera following for Motion-Forward camera feel | Camera System (ADR-0005) supports Cinemachine or native camera for polish |
| game-concept.md TR-concept-009 | 60 FPS minimum, 144 FPS preferred | Movement System (ADR-0003) consumes smooth camera following for parkour flow |
