# Control Manifest

**Version**: 1.0
**Manifest Date**: 2026-04-11
**Architecture Document**: docs/architecture/architecture.md
**ADR Coverage**: 13/13 (100%)

---

## Purpose

This document extracts all architectural constraints, required patterns, and forbidden patterns from all Accepted ADRs. Story authors reference this manifest when implementing features — no need to read individual ADRs.

---

## Usage

When authoring a story file (`production/epics/[epic-slug]/story-NNN-[slug].md`):
1. Check the system layer for this story
2. Read the Required Patterns for that layer/system
3. Read the Forbidden Patterns for that layer/system
4. Apply constraints during implementation

---

## Foundation Layer

### Input System (ADR-0001)

#### Required Patterns

- MUST use Unity Input System Package
- MUST support both Keyboard/Mouse and Gamepad seamlessly
- MUST provide low-latency input for parkour responsiveness
- MUST support rapid input sequences for combat (fire + movement simultaneously)
- MUST integrate with UI System for menu navigation

#### Forbidden Patterns

- MUST NOT use Legacy Input Manager (deprecated in Unity 6.0+)
- MUST NOT assume 1:1 action mapping (InputAction is frame-sampled, not event-driven)
- MUST NOT implement custom input handling from scratch

#### Constraints

- Input sampling must be frame-efficient to support 60 FPS target
- High-frequency input sampling needed for responsive wall-running and dash mechanics
- Input polling at fixed timestep if callback overhead is a concern
- Document fallback bindings for controllers without rumble support

---

### Physics System (ADR-0002)

#### Required Patterns

- MUST use hybrid collision strategy: Raycasting for detection, Unity Physics for projectile movement
- MUST detect traversable surfaces with LayerMask
- MUST use object pooling for projectiles (never instantiate during gameplay)
- MUST use continuous collision detection (SphereCollider with continuous checks) for projectiles

#### Forbidden Patterns

- MUST NOT use single frame raycast alone for projectiles (use continuous collision)
- MUST NOT instantiate projectiles during gameplay
- MUST NOT allow climbing on moving objects (elevators, platforms)

#### Constraints

- Projectile pooling required to maintain frame budget
- Maximum raycast distance enforced at 20m for edge of world safety
- Ground detection threshold: 0.5 to 1.5m minimum distance
- Physics calculations must stay under 0.5ms frame budget during parkour with 20+ active projectiles

---

### Movement System (ADR-0003)

#### Required Patterns

- MUST use Unity Physics Rigidbody for player movement
- MUST implement smooth damping for responsive parkour feel
- MUST support wall-running, climbing any surface, quick directional dashes, sliding
- MUST attach to surfaces via fixed joint or configurable spring during wall-run

#### Forbidden Patterns

- MUST NOT implement custom physics from scratch for parkour responsiveness (use Unity Physics)
- MUST NOT use static standing while aiming (movement is always in progress)
- MUST NOT allow zero wall-run vector (ignore upward wall-run input)
- MUST NOT allow climbing on moving platforms (disallow, provide error message)

#### Constraints

- Reduced friction on traversable surfaces during wall-running
- Reduced gravity during sliding and climbing
- Maximum wall-run attachment distance: 5.0m
- Dash cooldown: 0.2s minimum between dashes
- Weapon switch cooldown: 0.3s minimum

---

### Camera System (ADR-0008)

#### Required Patterns

- MUST use Cinemachine 3.x with custom ImpulseListener
- MUST provide motion-forward camera feel (shake, FOV dynamics)
- MUST follow player position smoothly during wall-running and dashing
- MUST support aim-down-sights (ADS) for precision shooting

#### Forbidden Patterns

- MUST NOT use native Camera.main for complex behaviors (Cinemachine required)
- MUST NOT allow camera to lose player orientation during fast movement
- MUST NOT implement custom camera smoothing from scratch

#### Constraints

- Use Cinemachine ImpulseListener for camera shake on weapon fire and impacts
- FOV must expand based on player velocity (speed feel) and contract when still
- ADS mode switches camera transform for precision
- Cinemachine Motion Blur component for speed feel (tunable in quality settings)
- Camera frame time must stay under 1ms at 144 FPS

---

### Game State System (ADR-0013)

#### Required Patterns

- MUST use JSON serialization for game state
- MUST use PlayerPrefs for settings (quick access)
- MUST use file-based saves for game progress (save slot system)
- MUST support quick save (checkpoint) and full save (menu-based)

#### Forbidden Patterns

- MUST NOT use binary serialization (less human-readable, harder to debug)
- MUST NOT use ScriptableObject-based saves (not designed for runtime)
- MUST NOT use database-based saves (overkill for simple data)

#### Constraints

- Save file naming: `save_slot_0.json`, `save_slot_1.json`, etc.
- Use `Application.persistentDataPath` for cross-platform save location
- 5 save slots minimum
- Auto-save on level completion and era unlock
- Version checking in save files for future compatibility
- Backup creation before overwriting

---

## Core Layer

### Combat System (ADR-0004)

#### Required Patterns

- MUST use hybrid GameObject + DOTS for projectiles
- MUST implement CoD-style weapon feel (recoil, reload, audio feedback)
- MUST support instant-kill detection for headshots
- MUST use pooled projectiles (never instantiate during gameplay)

#### Forbidden Patterns

- MUST NOT use GameObject-only projectiles (too slow for many shots)
- MUST NOT implement weapon switching mid-fire without cooldown
- MUST NOT allow firing during reload

#### Constraints

- Projectile pooling keeps memory under 100ms frame budget
- Damage = (BaseDamage * WeaponMultiplier) * BodyPartMultiplier
- Reload cooldown: 0.15 to 0.3s
- Weapon switch time: 0.2 to 0.5s
- Use DOTS Entities for projectiles only (player, weapons remain GameObject)

---

### Enemy AI System (ADR-0009)

#### Required Patterns

- MUST use Unity NavMesh with AI State Machine pattern
- MUST support multiple enemy types with distinct behaviors (patrol, engage, flee)
- MUST use cone-based raycast detection for player awareness
- MUST support era-specific AI parameters (alertness, accuracy, patrol patterns)

#### Forbidden Patterns

- MUST NOT use Behavior Tree (higher complexity, unnecessary for 3 enemy types)
- MUST NOT implement custom pathfinding (A* or Dijkstra) from scratch
- MUST NOT use grid-based AI (creates artificial constraints on 3D levels)

#### Constraints

- ScriptableObjects for era-specific AI configurations
- NavMesh surface types for era-specific traversal costs
- Support 20+ concurrent enemies without exceeding frame budget
- Cover finding via NavMesh queries
- Target human AI has unique capture sequence, not standard combat

---

### Combo System (ADR-0007)

#### Required Patterns

- MUST use event-based scoring (observes Combat + Movement events)
- MUST reward continuous movement and precision kills with multipliers
- MUST extend combo timer on kills, reset when momentum breaks
- MUST break combo when player stops moving or dies

#### Forbidden Patterns

- MUST NOT use polling-based combo calculation (event-driven required)
- MUST NOT allow combo to continue after death (reset on death)
- MUST NOT allow negative combo (player stops moving = reset to 1.0x)

#### Constraints

- Combo timer: 0 to 30 seconds
- Decay rate: 0.1 to 0.5 per second
- Combo milestones: burst VFX at 10x, 25x, 50x multipliers
- Combo system does NOT control gameplay directly (observes events)

---

### Time Travel System (ADR-0006)

#### Required Patterns

- MUST use Addressables for async era loading
- MUST support linear era progression (1950s → 1980s → 1920s → Future)
- MUST support era-specific enemies, weapons, and environmental interactions
- MUST preserve player state when switching eras

#### Forbidden Patterns

- MUST NOT use manual scene transitions (Addressables required)
- MUST NOT keep all era assets loaded simultaneously (memory bloat)
- MUST NOT allow era unlock without completing previous era

#### Constraints

- Eras: 4 total (1950s, 1980s, 1920s, Future)
- 3 levels per era minimum for progression
- Era loading: asynchronous with loading screen
- Unload previous era when switching

---

## Feature Layer

### Target Capture System (ADR-0005)

#### Required Patterns

- MUST implement unique capture sequence per target human
- MUST be distinct from standard combat
- MUST integrate with Camera System for cinematic capture moments
- MUST support objective tracking via HUD

#### Forbidden Patterns

- MUST NOT treat target capture as standard kill
- MUST NOT allow capture without meeting specific conditions

#### Constraints

- Each level has one unique target human
- Capture sequence unique per era
- Camera involvement for cinematic moments

---

## Presentation Layer

### UI System (ADR-0010)

#### Required Patterns

- MUST use Unity UI Toolkit (UXML/USS)
- MUST support both keyboard/mouse and gamepad navigation
- MUST provide full gamepad d-pad navigation (no hover-only interactions)
- MUST support era-specific visual accents without compromising readability

#### Forbidden Patterns

- MUST NOT use UGUI (deprecated default in Unity 6.0+)
- MUST NOT implement mouse-only interactions (gamepad must be fully accessible)
- MUST NOT use hybrid UI system (mixed systems create maintenance overhead)

#### Constraints

- Era-accents: WCAG 2.1 AA compliant color contrast ratios
- USS variables for era-specific colors
- Focus system for gamepad users
- Stack-based menu navigation with push/pop
- HUD frame time must stay under 0.5ms at 144 FPS

---

### Audio System (ADR-0011)

#### Required Patterns

- MUST use Unity AudioMixer with Addressables for era audio banks
- MUST support era-specific ambient audio (jazz, arcade, ragtime, synth)
- MUST provide CoD-authentic weapon SFX with recoil feedback
- MUST support 3-5 SFX variations per weapon for authenticity

#### Forbidden Patterns

- MUST NOT load all era audio at game start (memory bloat)
- MUST NOT create new AudioSource for every sound (use object pooling)

#### Constraints

- Addressables for era audio banks (4 eras)
- AudioMixer snapshots for ducking (music lowers during combat)
- Object pooling for one-shot sounds (impacts, footsteps)
- Memory budget: 50MB target for audio
- Era audio load time: 0.3-0.5s

---

### VFX System (ADR-0012)

#### Required Patterns

- MUST use Unity VFX Graph for complex effects with Particle System pooling
- MUST provide stylized blood bursts (not realistic pools)
- MUST support motion blur for speed feel
- MUST include muzzle flashes, trail effects, and combo milestone effects

#### Forbidden Patterns

- MUST NOT use legacy Shuriken particles only (VFX Graph required for complex effects)
- MUST NOT implement custom shader VFX from scratch (too complex for solo dev)
- MUST NOT instantiate VFX without pooling for frequently spawned effects

#### Constraints

- VFX Graph for complex effects (blood bursts, combo milestones, muzzle flash)
- Particle pooling for simple one-shots (impacts, small trails)
- Motion blur: Unity built-in Motion Blur component
- GPU frame budget: 4ms maximum for VFX rendering
- Object pooling prevents GC spikes during combat

---

## Cross-Layer Patterns

### Event Communication

- ALL inter-module communication MUST use events/delegates only
- NO direct data sharing across modules
- Each module owns its data exclusively

### Performance Requirements

- 60 FPS minimum, 144 FPS preferred
- Frame budget: 16.6ms @ 60 FPS, 6.9ms @ 144 FPS
- Memory ceiling: 4 GB minimum, 8 GB recommended

### Engine Awareness

- Unity 6.3 LTS — verify API usage against engine reference
- High-risk domains (Entities/DOTS, Input System, UI Toolkit) require verification

---

## Version History

| Version | Date | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-04-11 | Initial manifest created from all 13 Accepted ADRs |

---

**Next Steps**: After manifest approval, proceed to `/create-epics` for sprint planning.
