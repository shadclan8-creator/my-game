# ADR-0011: Audio System Architecture

## Status
Proposed

## Date
2026-04-11

## Last Verified
2026-04-11

## Decision Makers
Technical Director, Audio Director, Sound Designer

## Summary

Audio System requires era-specific ambient audio, CoD-style weapon SFX, and cat vocalizations. Decision: Use Unity Audio Mixer with Addressables for era audio banks and pooled AudioSource for performance.

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Audio |
| **Knowledge Risk** | LOW — Audio system API stable for years; minor mixer updates in 6.3 |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | AudioMixer enhancements (Post-6.0), AudioRandomizer improvements |
| **Verification Required** | Test audio loading times, validate 3D spatial audio, measure memory footprint per era |

### Key Post-Cutoff Changes

- **AudioMixer Updates**: Improved snapshot transitions, better ducking automation
- **AudioRandomizer**: Built-in randomization for SFX variations (less need for manual pooling)
- **Spatial Audio**: Enhanced HRTF support for 3D positioning

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Addressables System (ADR-0005) for era audio loading, Game State System (ADR-0013) for era tracking |
| **Enables** | Combat System (weapon SFX), Movement System (footstep sounds), UI System (menu SFX) |
| **Blocks** | None |
| **Ordering Note** | Audio System can be implemented in parallel with Combat System |

---

## Context

### Problem Statement

The game requires era-specific ambient audio (1950s jazz, 1980s arcade, 1920s ragtime, Future synth), CoD-authentic weapon sounds with recoil feedback, cat vocalizations, and environmental SFX. Must support 4 eras worth of content without memory bloat.

### Current State

No audio system exists. Design requires era-based audio management with performance-efficient playback.

### Constraints

- **Technical**: Must stay within memory budget (50MB target) while supporting 4 era audio banks
- **Performance**: Audio must not cause frame drops; spatial positioning for combat
- **Platform**: Must support both stereo speakers and headphones (5.1/7.1 optional)
- **Quality**: CoD-level weapon sound authenticity (recoil audio, reload sounds, fire variations)

### Requirements

- **Must load** era-specific audio banks dynamically (unload previous era when switching)
- **Must provide** 3D spatial audio for combat (enemy positions, gunfire)
- **Must include** weapon SFX variations (3-5 per weapon) for authenticity
- **Must support** Audio Mixer snapshots for ducking (music lowers during combat)
- **Must pool** frequently used SFX (footsteps, impacts) for performance

---

## Decision

**Use Unity AudioMixer with Addressables for era audio banks and Object Pooling for SFX.**

### Architecture Diagram

```
┌──────────────────────────┐
│   Audio System          │
│   (AudioMixer +        │
│    Addressables)         │
└──┬──────────────────────┘
   │
   ├──────────────────┬──────────────────┬──────────────────┐
   ↓                  ↓                  ↓                  ↓
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│ 1950s   │     │ 1980s   │     │ 1920s    │     │ Future   │
│ Audio    │     │ Audio    │     │ Audio     │     │ Audio     │
│ Bank    │     │ Bank     │     │ Bank      │     │ Bank      │
└────┬────┘     └────┬────┘     └─────┬────┘     └─────┬────┘
     │                │                  │                  │
     └────────────────┴──────────────────┴──────────────────┘
                      ↓
              ┌─────────────────┐
              │   SFX Pool     │
              │ (one-shot sounds) │
              └────────┬─────────┘
                      ↓
            ┌──────────────────────┐
            │ AudioSource 3D Pool  │
            │ (reusable instances)   │
            └──────────────────────┘
```

### Key Interfaces

```csharp
// Audio System exposes these interfaces to other systems
public interface IAudioProvider
{
    // Combat System
    void PlayWeaponSFX(WeaponType weapon, SFXVariant variant);
    void PlayReloadSFX(WeaponType weapon);
    void PlayImpactSFX(ImpactType impact, Vector3 position);

    // Movement System
    void PlayFootstepSFX(SurfaceType surface, Vector3 position);
    void PlayParkourSFX(ParkourAction action, Vector3 position);

    // Era Management
    void LoadEraAudio(EraType era);
    void UnloadEraAudio(EraType era);

    // Cat Character
    void PlayCatVocalization(CatMood mood);

    // Mixer Control
    void SetMixerSnapshot(AudioSnapshot snapshot); // Menu, Combat, Ambience
    void SetMusicVolume(float volume);
    void SetSFXVolume(float volume);
}

public enum WeaponType
{
    AssaultRifle,
    SMG,
    Shotgun,
    SniperRifle,
    LMG
}

public enum SFXVariant
{
    Variation1,
    Variation2,
    Variation3,
    Variation4,
    Variation5
}

public enum SurfaceType
{
    Wood,
    Metal,
    Concrete,
    Glass,
    Grass,
    Carpet
}

public enum ParkourAction
{
    WallRun,
    Dash,
    Land,
    Climb
}

public enum CatMood
{
    Hunting,
    Satisfied,
    Angry,
    Purring,
    Taunting
}

public enum ImpactType
{
    BloodHit,
    Headshot,
    Environmental,
    CoverHit
}

public enum AudioSnapshot
{
    Menu,       // Music full, SFX lowered
    Ambience,   // Era music full, SFX normal
    Combat,      // Music ducked, SFX emphasized
    Capture      // Combat music stinger for target capture
}
```

### Implementation Guidelines

1. **AudioMixer Setup**: Create master mixer with groups for Music, SFX, Voice, UI
2. **Era Audio Banks**: Use Addressables for era-specific bundles (1950sBank, 1980sBank, etc.)
3. **SFX Pooling**: Object pooling for one-shot sounds (impacts, footsteps); reuse AudioSource instances
4. **Weapon SFX Variations**: 3-5 variations per weapon; randomize on each fire for authenticity
5. **3D Spatial Audio**: All gameplay SFX use 3D AudioSource with rolloff curves
6. **Mixer Snapshots**: Create snapshots for Menu, Ambience, Combat states; transition on game state changes
7. **Cat Vocalizations**: Randomized playback based on gameplay context (kills, near misses, taunts)
8. **Era Loading**: Unload previous era bank on level transition, load new era bank asynchronously

---

## Alternatives Considered

### Alternative 1: All Audio Loaded at Start

- **Description**: Load all era audio at game start, never unload
- **Pros**: Instant audio availability, no loading transitions
- **Cons**: Memory bloat (200+MB vs 50MB target), wasted RAM for unused eras
- **Estimated Effort**: Lower (1 week vs 2 weeks)
- **Rejection Reason**: Memory budget of 50MB makes this impossible; era loading required

### Alternative 2: External Audio Engine (FMOD/Wwise)

- **Description**: Use professional audio middleware for advanced audio features
- **Pros**: Advanced features, better compression, built-in profiling
- **Cons**: License costs (hundreds of dollars), integration complexity, learning curve
- **Estimated Effort**: Higher (3-4 weeks vs 2 weeks for Unity audio)
- **Rejection Reason**: Solo dev budget; Unity AudioMixer sufficient for project scope

### Alternative 3: No Audio Pooling

- **Description**: Create new AudioSource for every sound playback
- **Pros**: Simpler implementation, no pool management
- **Cons**: Garbage collection spikes, frame drops from instantiations, memory fragmentation
- **Estimated Effort**: Lower (1 week vs 2 weeks)
- **Rejection Reason**: Performance requirements (60 FPS) require pooling for frequent sounds

---

## Consequences

### Positive

- **Memory Efficient**: Addressables era loading keeps memory under 50MB budget
- **Performance**: Object pooling eliminates GC spikes from AudioSource instantiation
- **Era Immersion**: Distinct audio banks per era create strong time period identity
- **Authentic Weapons**: SFX variations provide CoD-level weapon feel
- **Dynamic Mixing**: AudioMixer snapshots enable seamless volume balancing

### Negative

- **Loading Transitions**: Era audio banks take 0.3-0.5s to load; brief audio gap on era switch
- **Pool Management**: Object pooling adds code complexity and requires pool size tuning
- **Addressables Complexity**: Async loading adds state management complexity

### Neutral

- **File Structure**: Adds AudioMixer asset, era audio bundles, SFX pool manager script

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|-----------|
| Era audio load exceeds frame budget | MEDIUM | MEDIUM | Load era audio during level load screen; show progress bar |
| SFX pool size insufficient | LOW | MEDIUM | Implement dynamic pool expansion with warning logging |
| AudioMixer performance overhead | LOW | LOW | Profile mixer performance with 100+ concurrent sounds |
| Weapon SFX variations not authentic enough | MEDIUM | HIGH | Playtest with CoD players; iterate on sound design |

---

## Performance Implications

| Metric | Before | Expected After | Budget |
|--------|--------|---------------|--------|
| CPU (frame time) | 0ms | 0.2-1.0ms | 2.0ms |
| Memory | 0MB | ~45MB (single era) | 50MB |
| Load Time | 0s | ~0.4s per era | 2.0s |
| Network | N/A | N/A | N/A |

---

## Migration Plan

**From scratch**: N/A — new project implementation

---

## Validation Criteria

- [ ] Era audio loads and unloads correctly on level transitions
- [ ] Weapon SFX with variations plays on each fire (randomized)
- [ ] 3D spatial audio provides accurate enemy positioning
- [ ] AudioMixer snapshots transition smoothly (no audio pops)
- [ ] Combat ducking reduces music volume appropriately
- [ ] Cat vocalizations play at appropriate moments
- [ ] SFX pool maintains 20+ simultaneous sounds at 60 FPS
- [ ] Memory usage stays under 50MB with single era loaded
- [ ] Footstep SFX varies by surface type (wood, metal, concrete)

---

## GDD Requirements Addressed

| GDD Document | System | Requirement | How This ADR Satisfies It |
|-------------|--------|-------------|--------------------------|
| game-concept.md | Audio System | Era-specific ambient audio (jazz, arcade, ragtime, synth) | Addressables era audio banks provide distinct music/SFX per era |
| game-concept.md | Audio System | CoD-style weapon audio (recoil, reload, variations) | 3-5 SFX variations per weapon with 3D spatial audio provide CoD authenticity |
| game-concept.md | Audio System | Cat vocalizations and impact sounds | IAudioProvider interface includes PlayCatVocalization and PlayImpactSFX methods |
| combat-system.md | Audio System | Weapon fire/reload audio feedback | Combat System calls IAudioProvider for weapon-specific SFX playback |

---

## Related

- ADR-0004: Combat System Architecture (triggers weapon SFX)
- ADR-0003: Parkour Movement Implementation (triggers footstep and parkour SFX)
- ADR-0005: Asset Management Strategy (provides Addressables for era audio loading)
- ADR-0013: Game State System Implementation (tracks current era for audio loading)
