# Systems Index

*Created: 2026-04-07*
*Last Updated: 2026-04-07*
*Review Mode: Full*

---

## Overview

Total Systems: 11
- MVP Systems: 4
- Vertical Slice Systems: 6
- Alpha Systems: 9
- Full Vision Systems: 15

---

## System Definitions

### [Movement] Parkour Movement System

**Category**: Core Gameplay
**Description**: Enables the cat's signature fast, fluid movement through levels. Wall-running, climbing any surface, quick directional dashes, and sliding create unstoppable momentum.

**Core Fantasy Connection**: Serves "Feline Flow" pillar — movement is always in motion, no static standing.

**Primary GDD Reference**: `design/gdd/movement-system.md`

---

### [Combat] Authentic Weapon System

**Category**: Core Gameplay
**Description**: CoD-inspired weapons with realistic recoil patterns, reload animations, audio feedback. Each weapon type has distinct feel and behavior.

**Core Fantasy Connection**: Serves "Villainous Satisfaction" (lethal tools) and "CoD Gun Authenticity" (military feel) pillars.

**Primary GDD Reference**: `design/gdd/combat-system.md`

---

### [Scoring] Combo System

**Category**: Core Gameplay
**Description**: Rewards continuous movement and precision kills with multipliers. Breaking momentum breaks the combo. Drives replayability and mastery.

**Core Fantasy Connection**: Serves "Feline Flow" (continuous action) and "Villainous Satisfaction" (domination through skill) pillars.

**Dependencies**: Requires Movement System (to build combos), Combat System (kills trigger combos)
**Primary GDD Reference**: `design/gdd/combo-system.md`

---

### [Progression] Time Travel System

**Category**: Core Gameplay
**Description**: Unlockable eras (1950s, 1980s, 1920s, Future) with era-specific enemies, weapons, and environmental interactions.

**Core Fantasy Connection**: Serves "Nostalgic Era Heart" pillar — each era is authentically realized.

**Dependencies**: Requires Combat System (era-specific enemies), Environment System (era-specific levels)
**Primary GDD Reference**: `design/gdd/timetravel-system.md`

---

### [Objective] Target Capture System

**Category**: Core Gameplay
**Description**: Each level has a target human with unique capture sequence. Levels aren't "kill everyone" — the hunt matters.

**Core Fantasy Connection**: Serves "Villainous Satisfaction" — unique target capture per era.

**Dependencies**: Requires Combat System (guard/defeating enemy), Combo System (score for capture), Camera System (cinematic capture moments)
**Primary GDD Reference**: `design/gdd/target-capture-system.md`

---

### [Physics] Physics & Collision System

**Category**: Foundation
**Description**: Handles all physics interactions including movement collision detection, projectile physics, environmental collision, and cover/cover interactions.

**Dependencies Required By**: Movement System (parkour movement), Combat System (projectile hits)

**Primary GDD Reference**: `design/gdd/physics-system.md`

---

### [Camera] Camera System

**Category**: Core Gameplay
**Description**: Follows player's fast movement smoothly while maintaining readability. Supports parkour, combat, and objective tracking. Motion-forward camera feel.

**Dependencies Required By**: Movement System (camera follows cat), Input System (camera control)
**Primary GDD Reference**: `design/gdd/camera-system.md`

---

### [Input] Player Input System

**Category**: Foundation
**Description**: Keyboard/mouse and gamepad input handling with seamless switching. Supports fast action inputs and parkour controls.

**Dependencies Required By**: Movement System (dash, climb, slide), Combat System (weapon fire), UI System (input mapping)
**Primary GDD Reference**: `design/gdd/input-system.md`

---

### [UI] UI & HUD System

**Category**: Presentation
**Description**: Combo meter, health bar (green #00FF00 — NOT red), weapon display, objective marker, rank display. Era-accents with motion-forward design.

**Dependencies Required By**: Combat System (combat data), Combo System (combo data), Game State System (state to display)
**Primary GDD Reference**: `design/gdd/ui-system.md`

---

### [Audio] Audio & SFX System

**Category**: Presentation
**Description**: CoD-style weapon audio (recoil, reload, fire), era-specific ambient audio (jazz, arcade, etc.), cat vocalizations, impact sounds, and VFX (blood, muzzle flash, motion trails).

**Dependencies Required By**: Combat System (weapon triggers), Progression System (era unlocks), Physics System (impact SFX), Combo System (combo SFX)

**Primary GDD Reference**: `design/gdd/audio-system.md`

---

### [State] Game State & Session Management

**Category**: Core Gameplay
**Description**: Manages unlocked eras, collected weapons, player progression, level completion data, and save/load functionality.

**Dependencies Required By**: Progression System (unlock logic), Combo System (scores persist), Objective System (level completion)
**Primary GDD Reference**: `design/gdd/game-state-system.md`

---

### [AI] Enemy AI System

**Category**: Core Gameplay
**Description**: Guard AI (patrol, engage), armored guard (tougher), target human (unique behavior), era boss behaviors. Enemies are red-coded threats.

**Dependencies Required By**: Combat System (AI attacks), Physics System (pathfinding), Combo System (kill animations)

**Primary GDD Reference**: `design/gdd/enemy-ai-system.md`

---

### [Level] Level & Environment System

**Category**: Feature
**Description**: Era-specific level design, environment storytelling, traversal path design, era-themed props and decorations. 4 eras × 3 levels each = 12 total.

**Dependencies Required By**: Movement System (cyan surfaces), Environment Art Direction (era-specific assets), Progression System (era unlocks)
**Primary GDD Reference**: `design/gdd/level-environment-system.md`

---

### [VFX] Particle & Visual Effects System

**Category**: Presentation
**Description**: Stylized blood bursts (not realistic pools), motion blur for speed feel, muzzle flashes, trail effects, dynamic lighting events.

**Dependencies Required By**: Combat System (kill triggers), Camera System (camera shake), Combo System (combo bursts)

**Primary GDD Reference**: `design/gdd/vfx-system.md`

---

---

## Dependency Map

```
          +-----------------------+
          |                       |
+----------------+      +----------------+
|     [Physics]      |          |
+----------------+      |          |          |
+----------------+      |          |          |          |
| [Movement] ----+---> [Combat] +--------> [Scoring]       |
|               |          |          |          |          |
|      +-----------------------+          |          |
|      |          |          |          |          |
|      +----> [Camera]         |          |
|      |   +------> [Input]         |          |
|      |          |   +----> [UI]      +----> [VFX]          |
|      |          |   |    +----> |          |   +----> |          |
|      |          |   |         |          |   |    +----> |          |   +----> [AI]          |
|      |          |   |         |          |   |         |   |    +----> |          |   |    +----> [State]      |
|      |          |   |         |          |   |         |   |         |   |    +----> |          |   |    +----> +----> [Level]     |
|      |          |   |         |          |   |         |   |         |   |         |   |    +----> |          |   |    +---->      |     |
|      |          |   |         |          |   |         |   |         |   |         |   |    +----> |          |   |    +---->      |     |
|      |          |   |         |          |   |         |   |         |   |         |   |         +----> |          |   |    +----> |      |     |
|      +-----------------------+          |   |         |   |         |   |         |   |         |   |         |         +----> |          |   |    +----> |      |     |
|                            |         |   |         |   |         |   |         |   |         |   |         |   |    +---->      |     |
|                       |         |   |   |         |   |         |   |         |   |         |   |         |   |    +----> |      |     |
|                       |   |   |   |         |   |   |         |   |   |         |   |         |   |   |         |   |   |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |   |   |         |   |   |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |         |   |         |   |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |         |   |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |         |   |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |         |   |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |         |    +---->      |     |
|                       |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | | | | |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   | |   |   | |   |   | |   |   |   |   |   |   |   |   |   |   |   |   | |   |   |   | | | | | | | | | | | | | | | | | | | | | | | | | | | | |   |   |   | |   |   |   |   |   |   |   |   |   |   |   |   | | | | | | | | | | | | | | | |   | | |   |   |   |   |   |   |   |   |   | |   |   |   |   | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | |   | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | |   | | | | | | | |   | | | | | | | | | | | | |   | |   |   | | | | | | | | | | | | | | | | | | | | | | | | |   | | | | | | | |   | | | | | | | | | | | | | |   | | | |   | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | |   |   |   | |   |   |   |   | | | | | | | |   | | | | | |   |   | | | | | | | | | | | | | | |   |   | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | |   |   |   |   | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | | . | | . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . [system]` to design it
```

---

## Priority Assignments

### Milestone: MVP (3-6 months, solo)
| Tier | System | GDD Reference | Design Order |
|-----|--------|----------------|-------------|
| Foundation | Physics | physics-system.md | 1 |
| Foundation | Input | input-system.md | 2 |
| Core | Movement | movement-system.md | 3 |
| Core | Camera | camera-system.md | 4 |

**MVP Core Loop Tests:**
- Parkour: Wall-run, dash, climb through 1 1950s diner level
- Weapons: Fire AR and Shotgun with authentic feel
- Combo: Build and maintain combo through level
- Target: Capture target human with unique sequence
- Readability: Cyan surfaces visible at all times

---

### Milestone: Vertical Slice (6-9 months, solo)
| Tier | System | GDD Reference | Design Order |
|-----|--------|----------------|-------------|
| Foundation | Physics | physics-system.md | 1 |
| Foundation | Input | input-system.md | 2 |
| Core | Movement | movement-system.md | 1 |
| Core | Camera | camera-system.md | 2 |
| Core | Combat | combat-system.md | 3 |
| Feature | Combo | combo-system.md | 4 |
| Feature | Enemy AI | enemy-ai-system.md | 5 |

**Vertical Slice Core Loop Tests:**
- All MVP tests plus
- Multiple enemy types (basic + armored guard)
- Multiple weapons (AR, Shotgun, plus 1 era weapon)
- Combo scoring system
- Target capture across 1 full era (3 levels)

---

### Milestone: Alpha (9-12 months, solo)
| Tier | System | GDD Reference | Design Order |
|-----|--------|----------------|-------------|
| Foundation | Physics | physics-system.md | 1 |
| Foundation | Input | input-system.md | 2 |
| Core | Movement | movement-system.md | 1 |
| Core | Camera | camera-system.md | 1 |
| Core | Combat | combat-system.md | 1 |
| Core | Combo | combo-system.md | 2 |
| Feature | Enemy AI | enemy-ai-system.md | 2 |
| Feature | Time Travel | timetravel-system.md | 3 |
| Feature | Objective | target-capture-system.md | 4 |

**Alpha Core Loop Tests:**
- All Vertical Slice tests plus
- Time Travel: First 2 eras (1950s + 1980s) with unique enemies/weapons
- Progression: Unlock weapons, complete levels for ranking

---

### Milestone: Beta (12-15 months, solo)
| Tier | System | GDD Reference | Design Order |
|-----|--------|----------------|-------------|
| Foundation | Physics | physics-system.md | 1 |
| Foundation | Input | input-system.md | 1 |
| Core | Movement | movement-system.md | 1 |
| Core | Camera | camera-system.md | 1 |
| Core | Combat | combat-system.md | 1 |
| Core | Combo | combo-system.md | 1 |
| Feature | Enemy AI | enemy-ai-system.md | 1 |
| Feature | Time Travel | timetravel-system.md | 2 |
| Feature | Objective | target-capture-system.md | 3 |
| Feature | Level | level-environment-system.md | 4 |
| Feature | Audio | audio-system.md | 5 |
| Presentation | UI | ui-system.md | 6 |

**Beta Core Loop Tests:**
- All Alpha tests plus
- Audio: Era-specific ambient audio + weapon sounds
- UI: Full HUD with era accents
- Level: All 4 eras (12 levels)

---

### Milestone: Full Vision (15-18 months, solo)
| Tier | System | GDD Reference | Design Order |
|-----|--------|----------------|-------------|
| Foundation | Physics | physics-system.md | 1 |
| Foundation | Input | input-system.md | 1 |
| Core | Movement | movement-system.md | 1 |
| Core | Camera | camera-system.md | 1 |
| Core | Combat | combat-system.md | 1 |
| Core | Combo | combo-system.md | 1 |
| Feature | Enemy AI | enemy-ai-system.md | 1 |
| Feature | Time Travel | timetravel-system.md | 1 |
| Feature | Objective | target-capture-system.md | 2 |
| Feature | Level | level-environment-system.md | 3 |
| Feature | Audio | audio-system.md | 4 |
| Presentation | UI | ui-system.md | 5 |
| Presentation | VFX | vfx-system.md | 6 |
| Polish | Game State | game-state-system.md | 7 |

**Full Vision Core Loop Tests:**
- All Beta tests plus
- VFX: Particle effects, dynamic lighting
- Game State: Full progression (all eras, all weapons)
- Polish: Animation polish, performance optimization, leaderboards

---

## Implementation Order

### MVP Systems (Design Order 1-4)
1. **Physics System** — Foundation. All movement and combat needs collision.
2. **Input System** — Foundation. All gameplay needs input.
3. **Movement System** — Core MVP feature. Parkour is the core fantasy.
4. **Camera System** — Core MVP feature. Must follow fast movement.

### Vertical Slice Systems (Add to MVP, design order 5)
5. **Combat System** — Core MVP feature. Weapons + enemies + hit detection.
6. **Combo System** — Core MVP feature. Rewards continuous play.

### Alpha Systems (Add to Vertical Slice, design order 7-9)
7. **Enemy AI System** — Core MVP feature. Guards + target behaviors.
8. **Time Travel System** — First feature system. Unlock eras + era progression.

### Beta Systems (Add to Alpha, design order 10-15)
9. **Objective System** — Core MVP feature extended. Target capture across eras.
10. **Level System** — Feature MVP expansion. Era-specific levels + environmental storytelling.
11. **Audio System** — Presentation MVP. Era immersion through sound.
12. **UI System** — Presentation MVP. HUD + menus with era accents.

### Full Vision Systems (Add to Beta, design order 16-22)
13. **VFX System** — Polish. Visual juice, motion effects, dynamic lighting.
14. **Game State System** — Polish. Full progression, saves/loads.

---

## Development Notes

**Bottlenecks** (High-risk, many dependents):
- **Level System** — Depends on Movement (cyan surfaces), Combat (enemy placement), Time Travel (era content), Audio (era ambient), VFX (era effects), Game State (progression), UI (era accents). 7 systems depend on it. Design and implement carefully to avoid becoming blocked.
- **Combat System** — Depends on Physics (collisions), Movement (hit detection from fast movement), Camera (combat framing), Enemy AI (targets), Combo (kill triggers), Audio (weapon sounds), VFX (kill effects), Game State (weapon unlocks). 9 systems depend on it.
- **UI System** — Depends on Combat (data to display), Combo (combo meter), Game State (state to display), Progression (era unlocks). 4 systems depend on it.

**Circular Dependencies** (None detected)

---

## Change Log

| Date | Change | Description |
|-----|--------|-------------|
| 2026-04-07 | Created | Initial systems index for Time's Baddest Cat project |

