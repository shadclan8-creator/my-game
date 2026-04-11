# Art Bible: Time's Baddest Cat

*Created: 2026-04-07*
*Status: Complete*
*Last revised: 2026-04-07*

---

## Section 1: Visual Identity Statement

### One-Line Visual Rule

Everything must feel in motion — visual language itself should suggest speed.

### Supporting Visual Principles

**1. Momentum-First Composition** — No static frames. Every composition element (buildings, props, lighting, UI) should imply directional flow. Diagonals, motion lines, and "pointing" geometry guide's player's eye through level like water.

*Design test*: When a visual decision is ambiguous, Feline Flow says we choose the option that enhances forward momentum. If two options are equal in impact, choose the one with more directional energy.

**2. Kinetic Edge Treatment** — Every edge has a velocity. Whether hard or soft, edges should feel like they're in mid-motion. Blur, glow, motion trails, or texture movement suggest speed rather than rigid stillness.

*Design test*: When debating edge hardness, Feline Flow says favor treatment that reads as "fast" over treatment that reads as "stable."

**3. Silhouette Priority** — Readability at speed is paramount. The cat, enemies, weapons, and traversal surfaces must be instantly recognizable at thumbnail scale and at full pace. Strong silhouettes over fine detail.

*Design test*: When prioritizing between silhouette fidelity and texture detail, Villainous Satisfaction says prioritize silhouette clarity first — player must identify threats instantly while moving fast.

**4. Era-Contrast Tension** — Each time period uses complementary color pairs that create visual pop and communicate era mood. 1950s = pastel pink/navy, 1980s = neon magenta/electric blue, 1920s = sepia/charcoal, Future = bright green/deep purple.

*Design test*: Nostalgic Era Heart says to era palette must be recognizable and distinct. When balancing era identity vs. gameplay readability, era contrast wins.

---

## Section 2: Color Palette

*Last revised: 2026-04-07*

### Primary Palette (Core Semantic Colors)

| Color | Semantic Role | Era Role |
|-------|---------------|-----------|
| **Electric Red (#FF2B2E)** | Danger / Enemies / Threat | Base enemy red — consistent across all eras |
| **Cyan Flow (#00F5FF)** | Traversal / Wall-run / Climb | Highlights paths of motion — cyan = GO |
| **Gold Target (#FFD700)** | Objectives / The Cat / Goals | The villain's targets and collectible items |
| **White Accent (#FFFFFF)** | Weapon Highlights / UI Active | Draw attention to kill impacts and active UI states |

### Semantic Color Usage

- **Red = Kill zone**: Enemies are always red-coded. When red appears on screen, player knows bullets or enemy attacks are incoming. Red never means "friendly" in this world.
- **Cyan = Move zone**: Any climbable surface, wall-run path, or jump-boost area is tinted cyan. This creates instant visual language for parkour without icons.
- **Gold = Reward zone**: Target humans, weapon pickups, and combo milestones use gold. It signals "predator success."
- **White = Action zone**: The cat's active weapons, muzzle flashes, and active UI elements use white to stand out.

### Per-Era Palettes

Each era uses semantic colors as base, then applies era-specific tint:

| Era | Primary Pair | Accent Pair | Mood |
|-----|-------------|------------|------|
| **1950s** | Pastel Pink (#FFB6C1) / Navy Blue (#1A365D) | Cream (#FFFDD0) / Teal (#20B2AA) | Warm, cozy but under threat |
| **1980s** | Neon Magenta (#FF00FF) / Electric Blue (#007BFF) | Yellow (#FFEB3B) / Orange (#FF7F00) | Electric, chaotic, night-life |
| **1920s** | Sepia (#F4A460) / Charcoal (#2D2D2D) | Gold (#D4AF37) / Maroon (#800020) | Noir-ish, gritty, prohibition |
| **Future** | Bright Green (#39FF14) / Deep Purple (#4B0082) | Cyan (#00FFFF) / Silver (#C0C0C0) | Alien, cold, high-tech |

### Era Color Temperature Rules

- 1950s → 1980s: Warmer colors, higher saturation (night-life energy)
- 1980s → 1920s: Desaturated, earthier, lower contrast (noir tone)
- 1920s → Future: Full saturation, cold temperature, synthetic feel

### UI Palette

UI uses semantic colors for clarity, but applies era accent colors for decorative elements:
- HUD is neutral (dark gray #1A1A1A) with semantic color accents
- Era-specific UI elements use era accent pair
- Never use red for player health (green #00FF00) — red always means enemy/danger in this game

### Colorblind Safety

| Semantic | Primary | Backup Cue |
|----------|--------|------------|
| Danger (Red) | Red | Icon shape + animation + sound design |
| Traversal (Cyan) | Cyan | Texture pattern on traversal surfaces + icon indicator |
| Objectives (Gold) | Gold | Sparkle/glow particle effect + audio cue |

---

## Section 3: Shape Language

*Last revised: 2026-04-07*

### Character Silhouette Philosophy

The cat protagonist must be instantly recognizable by distinctive profile while moving fast at speed:

| Element | Shape Rationale |
|---------|----------------|
| **Ears** | Large, angular, pointed forward | Forward-pointing ears act as directional arrow — they "pull" the eye toward movement direction. |
| **Tail** | Long, trail-tapered | Tail shows motion path visually — it "drags" behind movement, creating trail effect. |
| **Body** | Lean, athletic, low-profile | Athletic frame reads as capable and fast. No bulk that suggests slowness. |
| **Limbs** | Strong, clawed but simplified | Arms/legs are functional shapes — weapons and claws read as tools of predation. |

*Design test*: Silhouette Principle says when debating between anatomical accuracy and stylized clarity, choose silhouette that reads best at thumbnail scale and full combat pace. Realistic detail that creates visual noise is rejected in favor of clean, iconic forms.

### Distinguishing Character Archetypes

| Enemy Type | Silhouette Trait | Secondary Cue |
|-------------|------------------|--------------|
| **Basic Guard** | Blocky, rectangular | Red accent, human posture |
| **Armored Guard** | Larger, bulky with distinct head shape | Silver armor plates, weapon silhouette |
| **Target Human** | Hunched, slightly larger than guards | Gold halo or weapon glow |
| **Era Boss** | Unique per era, exaggerated profile | Era-specific silhouette + color |

### Environment Geometry

Angular, directionaling architecture that "guides" player movement through level:

| Element | Shape Philosophy | Feline Flow Connection |
|---------|----------------|---------------------|
| **Buildings** | Sharp angles, diagonals | Diagonals point toward optimal paths — "go this way" |
| **Props** | No static boxes | Every prop leans, angles, or curves in direction of traversal |
| **Paths/Roads** | Sweeping curves | Curves encourage flow-state movement — no jagged edges to slow down player |
| **Climbable Surfaces** | Tinted cyan, slightly protruding | Physical extension from geometry cues "grab here" |
| **Cover** | Angled, not orthogonal | Angled cover creates natural flow from cover → slide → fire → move |

*Design test*: Momentum-First Composition says when geometry could be rectangular vs. directional, choose directional angles that imply flow. Static orthogonal boxes are avoided.

### UI Shape Grammar

UI elements echo world aesthetic with motion-forward design:

| Element | Shape Treatment | Motion Emphasis |
|---------|----------------|---------------|
| **HUD Elements** | Rounded corners, angled edges | Corners echo movement speed — no 90° static boxes |
| **Progress Bars** | Angled bars, motion trails | Bars "lean" into movement direction |
| **Icons** | Stylized, directional | Icons imply action orientation (forward-pointing arrows) |
| **Text** | Bold, slight italic tilt | Italic suggests forward momentum |

*Design test*: UI should feel "part of game world," not an overlay. When UI shapes conflict with geometry, favor motion-forward angles.

---

## Section 4: Lighting & Atmosphere

*Last revised: 2026-04-07*

### Per-Era Lighting & Mood Targets

| Era | Primary Mood | Time of Day | Color Temp | Contrast | Atmospheric Descriptors |
|-----|-------------|------------|-----------|-------------------------|
| **1950s** | Warm, cozy under threat | Mid-morning golden | Warm, low contrast | Retro nostalgia, sitcom atmosphere, but sharp danger moments |
| **1980s** | Electric, chaotic high | Night, neon glow | High saturation, cool | Nightlife energy, arcade chaos, vibrant |
| **1920s** | Noir, gritty tension | Night / Late afternoon | Low saturation, warm | Jazz club mystery, prohibition shadows, danger in dark corners |
| **Future** | Alien, cold precision | Sterile white / hologram | Full saturation, cold | Uncanny sterility, high-tech menace, cold efficiency |

### Game State Lighting Targets

| Game State | Lighting Goal | Focus | VFX Emphasis |
|-------------|--------------|-------|---------------|
| **Exploration** | Ambient reveals paths | Cyan surfaces glow softly | Dust motes in light beams |
| **Combat** | High contrast, read threats | Muzzle flash illumination | Blood/hit sparks are bright white |
| **Victory** | Golden, celebratory | Target halo pulses | Screen shake + motion blur intensity |
| **Menus / Briefing** | Dramatic, sets tone | Era-themed backdrops | Subtle motion in background elements |

### Atmospheric Descriptors (Per Era)

- **1950s**: Cozy threat, checkerboard patterns, jukebox sunshine, cigarette smoke curling, warm amber glow from windows
- **1980s**: Electric hum, flickering neon, wet pavement reflections, steam vents, bass-driven atmosphere
- **1920s**: Dust motes in light beams, cigarette haze, jazz ambiance, pool table reflections, heavy velvet shadows
- **Future**: Sterile hum, hologram flicker, cold fluorescent glow, air recycler hum, polished chrome reflections

### Energy Level Mapping

Frenetic (Exploration) → High (Combat) → Measured (Victory) → Contemplative (Menus)

*Design test*: When lighting affects readability of traversal surfaces (cyan), Feline Flow says traversal surfaces must remain visible in all lighting conditions. Never use mood lighting that obscures "GO" path.

---

## Section 5: Character Art Direction

*Last revised: 2026-04-07*

### The Cat

The villain protagonist is athletic, lethal, and constantly in motion. Design must support fast gameplay while selling power fantasy.

| Element | Design Rationale |
|---------|----------------|
| **Silhouette** | Lean, angular, pointed | Low-profile, athletic frame suggests speed and competence. |
| **Proportions** | 2:1 head-to-body | Slightly exaggerated proportions — large head/ears emphasize silhouette, lean body reinforces agility. |
| **Posture** | Active, predatory | Cat is crouched/ready, not relaxed. Ears forward, tail raised or trailing. |
| **Expression** | Focused, intense | Narrowed eyes communicate hunting intensity. No cartoon expressions — villain is deadly serious. |
| **Weapons** | Distinct silhouettes | Each weapon type has recognizable profile even at speed. AR = angular blocky, Shotgun = wide silhouette, etc. |

### Enemy Archetypes

| Enemy Type | Silhouette Cue | Era Variations |
|------|------------------|--------------|
| **Basic Guard** | Blocky, rectangular | 1950s = Police uniform shape | 1980s = Arcade sprite shape | 1920s = Gangster Fedora shape | Future = Drone shape |
| **Armored Guard** | Bulky with distinct head | Era-specific armor plates that change color/hue | Silver armor plates, gold halo effects |
| **Target Human** | Hunched, slightly larger than guards | Gold halo or weapon glow |
| **Era Boss** | Unique per era, exaggerated profile | Era-specific silhouette + color |

*Design test*: Villainous Satisfaction says to cat must read as dominant in every encounter. Enemy silhouettes must clearly read as "threat" while cat reads as "predator."

---

## Section 6: Environment & Level Art

*Last revised: 2026-04-07*

### Era Architecture Styles

| Era | Architectural Dominant | Texture Style | Density | Props |
|-----|---------------------|------------------|---------|
| **1950s** | Suburban homes | Checkerboard fabrics, wallpaper | Medium | Cars, jukeboxes, picket fences |
| **1980s** | Neon-lit commercial | Tile, asphalt, glass | High | Arcade machines, neon signs, trash cans |
| **1920s** | Brick warehouses, speakeasies | Red brick, velvet | High | Wine bottles, crates, jazz posters |
| **Future** | Sleek facilities | Smooth metal, glass | Medium | Holograms, server racks |

### Prop Density Rules

- **Exploration areas**: Sparse — clear sightlines for parkour, cyan surfaces visible
- **Combat arenas**: Medium — enough cover for engagement, clear lines of sight for shooting
- **Objective rooms**: Dense — focused visual clutter around target and traversal paths

### Environmental Storytelling

Visual details that tell era story without text:

- **1950s**: Newspapers on tables, family photos on walls, unfinished homework on desk, warm amber glow from windows
- **1980s**: Graffiti tags, arcade cabinet burn marks, damaged street fixtures, rave flyers
- **1920s**: Dust motes in light beams, cigarette haze, jazz ambiance, pool table reflections, heavy velvet shadows
- **Future**: Holographic advertisements, data streams, abandoned research notes, maintenance access logs

*Design test*: Nostalgic Era Heart says to environment must feel authentic to the time period. Storytelling should reinforce era identity without explicit text exposition.

---

## Section 7: UI/HUD Visual Direction

*Last revised: 2026-04-07*

### HUD Design

| Element | Shape | Color | Motion | Notes |
|---------|-------|--------|--------|--------|
| **Health Bar** | Vertical, angled top | Green (#00FF00, not red!) | Angled inward on damage |
| **Combo Meter** | Horizontal, motion-trailing | Cyan/White gradient | Trail effect on build, shrinks on loss |
| **Weapon Display** | Bottom center, angled | White accent | Recoil animation visible, current weapon icon |
| **Objective Marker** | Top right, diamond | Gold | Pulse glow when target nearby |
| **Rank Display** | Bottom left | Era accent white | Flourish text on change |

### Menu UI

| Screen | Shape Language | Accent Palette | Notes |
|-------|----------------|----------------|--------|
| **Main Menu** | Angled, motion-forward | Era primary + neutral | Era-specific background art with cat silhouette motion |
| **Level Select** | Era cards, slides | Era-specific palette | Each level card shows era in its colors |
| **Pause Menu** | Dark overlay, blur | #1A1A1A + blur | Cinematic feel, motion blur emphasizes everything in motion |

*Design test*: UI Shape Grammar says menus echo world aesthetic with motion-forward angles. No 90° static boxes, corners lean into movement direction.

---

## Section 8: Asset Standards

*Last revised: 2026-04-07*

### File Formats & Naming

- **Characters**: `[Era]_[CharacterName].fbx` — e.g., `1950s_Guard.fbx`, `Future_Boss.fbx`
- **Weapons**: `Weapon_[Name].fbx` — e.g., `Weapon_AR.fbx`, `Weapon_Shotgun.fbx`
- **Animations**: `[Character]_[Action].anim` — e.g., `Cat_Run.anim`, `Cat_Dash.anim`
- **Textures**: `Texture_[Era]_[MaterialName].png` — e.g., `Texture_1950s_Wall.png`, `Texture_Future_Metal.png`
- **Materials**: `Mat_[Character/Prop]_[Era]` — e.g., `Mat_Cat_1950s`, `Mat_DinerChairs`
- **UI**: `UI_[ScreenName].uxml` — e.g., `UI_MainMenu.uxml`, `UI_HUD.uss`

### Resolution Tiers

- **Characters**: 4K diffuse, 2K normal, 1K LOD1, 512 emissive
- **Weapons**: 2K normal, 1K LOD, 512 emissive
- **Environments**: 2K diffuse, 1K normal per era, 512 emissive for props
- **UI Elements**: 512 x 512 max (use atlases where possible)

### LOD Levels & Distances

| Object Type | LOD0 (Full) | LOD1 (Medium) | LOD2 (Low) | Cull Distance |
|-------------|---------------|----------------|-----------|
| **Player** | 20m | 30m | 50m | 60m |
| **Enemies** | 15m | 25m | 40m | 80m |
| **Weapons** | Always visible | 10m | 20m | 30m | Cull |
- **Props** | 0-10m | 10-30m | 30-60m | Cull |

*Design test*: Asset Standards must respect Unity Performance Budgets from technical preferences: Target 60 FPS minimum, Frame Budget 16.6ms, Memory Ceiling 4GB minimum.

---

## Section 9: Reference Direction

*Last revised: 2026-04-07*

### Primary References

| Source | What to Draw | What to Avoid |
|---------|---------------|-----------------|
| **Overwatch** | Readable silhouettes, strong color blocking | Realistic guns + stylized characters | Don't copy hero shooter |
| **Metal Slug** | Run-and-gun core, detailed props | Don't take pixel art direction |
| **Cuphead** | Bite-sized levels, distinct themed areas | Don't take 1930s color |
| **Call of Duty** | Authentic weapon feel | Military aesthetic | Weapon audio/recoil | Not absolute realism |
| **Hotline Miami** | Fast-paced, kinetic UI | Synthwave aesthetic | Screen shake, motion blur | Don't take top-down perspective |
| **Back to the Future** | Era nostalgia | Iconic 1980s, Time travel structure | Don't take entire game scope |

### Era-Specific References

| Era | Primary Inspirations | Avoid |
|-----|-------------------|------|
| **1950s** | The Truman Show (1950s sitcom) | Pleasantville (TV 1950s-1957) | Avoid hyper-realistic domestic violence |
| **1980s** | Blade Runner (1982), Terminator (1984) | Neon-noir cityscapes | Avoid retro game pixelation |
| **1920s** | LA Confidential (1997), Untouchables (1987) | Prohibition aesthetic | Avoid cartoon-era exaggeration |
| **Future** | Cyberpunk 2077, The Matrix | Alien sterility | Avoid generic sci-fi |

### Style Anchors & Anti-Patterns

- **Motion Blur**: All high-speed actions use motion blur (clarity sacrifice for speed feel)
- **Trail Effects**: Cat tail has permanent subtle motion trail, weapons leave short trails
- **Particle Blood**: Stylized bursts (not realistic pools) — fits exaggerated violence tone
- **Dynamic Lighting**: Flashlight on weapon fire, muzzle flashes illuminate dark corners
- **Era-Specific Fog**: Each era has distinct fog density/color — heavy smog for 1920s, volumetric holograms for Future

---

## Art Director Sign-Off

> **Art Director Review (AD-ART-BIBLE)**: APPROVED — 2026-04-07
