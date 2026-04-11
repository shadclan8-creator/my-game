# Game Concept: Time's Baddest Cat

*Created: 2026-04-07*
*Status: Draft*

---

## Elevator Pitch

> A fast-paced run-and-gun where you're a feline supervillain rampaging through iconic time periods, hunting humans with a growing arsenal of Call of Duty-style weapons.

> Test: Can someone who has never heard of this game understand what they'd be doing in 10 seconds? Yes — cat villain, time travel, shoot humans, nostalgic eras.

---

## Core Identity

| Aspect | Detail |
| ---- | ---- |
| **Genre** | Run-and-Gun / Action-Platformer |
| **Platform** | PC (Steam / Epic) |
| **Target Audience** | Achievers & Explorers (skill mastery + era discovery) |
| **Player Count** | Single-player |
| **Session Length** | 30-60 minutes (self-contained levels with replay value) |
| **Monetization** | Premium (single-purchase) |
| **Estimated Scope** | Medium (12-18 months, solo) |
| **Comparable Titles** | Hotline Miami, Metal Slug, Cuphead |

---

## Core Fantasy

Domination through firepower — you're the apex predator in every era. You play as a time-traveling cat villain who hunts humans across nostalgic time periods, growing increasingly powerful with each capture. The fantasy is absolute power expressed through lethal precision and unstoppable momentum — humans are prey, and you are the ultimate predator.

This fantasy delivers something players can't experience elsewhere: the thrill of being an overpowered antagonist whose movement flows like liquid and whose weapons devastate with military authenticity. The juxtaposition of absurd premise (cat villain) with deadly-serious weapon feel creates the emotional hook.

---

## Unique Hook

Like Hotline Miami's tactical chaos AND ALSO a time-traveling premise where each era gives you new tools and enemies.

The hook works because:
- **Explained in one sentence**: Fast run-and-gun with time-traveling eras
- **Genuinely novel**: Villainous cat protagonist + authentic CoD weapons + era-specific mechanics
- **Connected to core fantasy**: Each era = new hunting ground, new prey, new ways to express dominance
- **Affects gameplay**: Eras aren't just visual skins — each has unique enemy behaviors, era-specific weapons, era-authentic environments

---

## Player Experience Analysis (MDA Framework)

### Target Aesthetics (What player FEELS)

| Aesthetic | Priority | How We Deliver It |
| ---- | ---- | ---- |
| **Sensation** (sensory pleasure) | 1 | Recoiling weapons, motion blur, controller rumble on impact, visceral audio feedback |
| **Fantasy** (make-believe, role-playing) | 2 | Villainous cat identity, era immersion, power fantasy through arsenal |
| **Narrative** (drama, story arc) | 3 | Villain's grand plan revealed through briefings, target human mini-stories |
| **Challenge** (obstacle course, mastery) | 1 | Skill-based movement, combo scoring, rank-based level completion |
| **Fellowship** (social connection) | N/A | Single-player only (anti-pillar) |
| **Discovery** (exploration, secrets) | 2 | Era-specific secrets, alternate traversal paths, unlockable weapons |
| **Expression** (self-expression, creativity) | 3 | Weapon selection, approach choices (loud vs. stealthy entry), route selection |
| **Submission** (relaxation, comfort zone) | N/A | Game is about challenge, not relaxation |

### Key Dynamics (Emergent player behaviors)

- Players will chain parkour movement with weapon fire to maintain momentum and combo multipliers
- Players will replay levels to optimize routes and achieve higher ranks
- Players will experiment with weapon-era combinations (e.g., using a future laser rifle in a 1950s diner)
- Players will discover optimal "hunting patterns" for each enemy type and era

### Core Mechanics (Systems we build)

1. **Parkour Movement System** — Wall-running, climbing any surface, quick directional dashes. The cat flows through levels like water, never stopping. Movement is always in progress — no static standing while aiming.
2. **Authentic Weapon System** — CoD-inspired weapons with realistic reload animations, recoil patterns, audio feedback, and distinct feel per weapon type (AR, SMG, Shotgun, Sniper Rifle, LMG).
3. **Time Travel Progression** — Unlockable eras (1950s, 1980s, 1920s, Future) with era-specific enemies, weapons, and environmental interactions.
4. **Combo Scoring System** — Rewards continuous movement and precision kills with multipliers. Breaking momentum breaks combo.
5. **Target Capture Mechanic** — Each level has a target human with unique capture sequence. Levels aren't just "kill everyone" — the hunt matters.

---

## Player Motivation Profile

### Primary Psychological Needs Served

| Need | How This Game Satisfies It | Strength |
| ---- | ---- | ---- |
| **Autonomy** (freedom, meaningful choice) | Multiple traversal paths through each level; weapon selection before each level; approach decisions (loud vs. stealthy entry) | Supporting |
| **Competence** (mastery, skill growth) | Skill-based movement system with high skill ceiling; combo scoring; rank-based level completion; unlockable weapons for mastery | Core |
| **Relatedness** (connection, belonging) | Cat villain monologues reveal personality; target humans have mini-stories; but primarily solo experience | Minimal |

### Player Type Appeal (Bartle Taxonomy)

- [x] **Achievers** (goal completion, collection, progression) — How: Rank each level, unlock all weapons, achieve perfect combo runs, complete all eras
- [x] **Explorers** (discovery, understanding systems, finding secrets) — How: Alternate traversal paths in each level, era-specific secrets, unlockable era weapons, discovering enemy behavior patterns
- [ ] **Socializers** (relationships, cooperation, community) — Not applicable (single-player, anti-pillar)
- [x] **Killers/Competitors** (domination, PvP, leaderboards) — How: Score leaderboards per level, rank-based completion, mastery over enemy patterns

### Flow State Design

- **Onboarding curve**: First level (1950s diner) teaches movement mechanics gradually — wall-run, dash, climb, shoot, capture target. Tutorial is built into gameplay, not separate mode.
- **Difficulty scaling**: Each era introduces new enemy types and behaviors. Later eras have more complex environments and harder target capture sequences. Ranks (D to S) provide difficulty scaling through precision requirements.
- **Feedback clarity**: Visual combo counter, rank indicators at level end, unlock notifications, weapon acquisition feedback. Players always know what they're improving toward.
- **Recovery from failure**: Fast restart (sub-5 seconds), failure is educational (shows combo where it broke), no punishment beyond lost time. Players can restart immediately without penalty.

---

## Core Loop

### Moment-to-Moment (30 seconds)

**Scan → Dash → Engage → Transition → Repeat**

1. **Scan**: Spot enemies and the path forward. Cyan surfaces indicate climbable/wall-runable.
2. **Dash**: Quick directional movement to optimal position. Momentum bar (combo meter) builds with movement.
3. **Engage**: Wall-run for angle, fire CoD weapon with recoil satisfaction. Each kill extends combo timer.
4. **Transition**: Slide or climb to new position before combo timer expires. Never stop moving.
5. **Repeat**: Maintain momentum throughout level.

The 30-second loop is intrinsically satisfying because:
- Audio: Purring engine of gunfire, cat vocalizations on kills, whoosh of movement
- Visual: Motion blur during dashes, dust clouds from landings, satisfying impact animations
- Tactile: Controller rumble on shots, distinct sounds for different surfaces
- Flow: Combos reward continuous movement — stopping breaks the chain

### Short-Term (5-15 minutes)

**The Hunt Cycle per level:**

1. **Briefing** — Target human identified, intel on their defenses/weaknesses (30 seconds)
2. **Infiltration** — Navigate through nostalgic environment, absorb the era atmosphere (2-3 minutes)
3. **Encounter** — Fight through guards to reach target, using parkour and weapons (2-4 minutes)
4. **The Catch** — Unique sequence to capture/neutralize the target (30-60 seconds)
5. **Escape/Extraction** — Optional bonus phase — get out before reinforcements, continue combo (1-2 minutes)

"One more run" psychology comes from: combo optimization, rank improvement (D → C → B → A → S), and weapon unlock goals.

### Session-Level (30-120 minutes)

**A complete session:**

- **Warmup**: Replay a favorite level to re-establish flow (10-15 minutes)
- **Progression**: Tackle 2-3 new levels in order (20-30 minutes)
- **Grind**: Replay a level to unlock a specific weapon or improve score (15-20 minutes)
- **Closure**: End on a high note, see accumulated progress (era unlocks, weapon count) (5 minutes)

Natural stopping points: After each level completion, after unlocking a new weapon, between eras.

### Long-Term Progression

**Player growth over days/weeks:**

- **Arsenal**: Unlock CoD weapons across eras — M4A1 (1950s), AK-47 (1980s), Tommy Gun (1920s), Laser Rifle (Future), plus era-specific specials.
- **Eras**: Unlock new time periods — 1950s → 1980s → 1920s → Future. Each era is visually and mechanically distinct.
- **Mastery**: Develop skill with mobility and weapon combos. Achieve S-rank on all levels, unlock completion achievements.
- **The Plan**: Reveal the cat villain's grand scheme through level briefings. The story unfolds as you progress.

**When is the game "done"?** After catching all 12 target humans and completing the final era — revealing the cat's true goal and the reason for the human hunt.

### Retention Hooks

- **Curiosity**: Unanswered questions about the cat's true motive; unexplored eras; locked weapons; hidden level paths
- **Investment**: Progress through eras; accumulated weapon arsenal; rank achievements; S-rank unlocks
- **Social**: Leaderboards per level (community-run); community discoveries for optimal routes
- **Mastery**: Skills to improve (parkour precision, weapon mastery); challenges to overcome (S-rank all levels); rankings to climb (leaderboards)

---

## Game Pillars

Design pillars are non-negotiable principles that guide EVERY decision. When two design choices conflict, pillars break the tie.

### Pillar 1: Feline Flow

Movement and combat are always in motion. The cat never stops; the player's camera never sits still. Momentum is king.

*Design test*: If we're debating between X and Y, this pillar says we choose the option that keeps movement continuous.

### Pillar 2: Villainous Satisfaction

The player *is* the powerful cat villain. Combat should feel empowering, tools should feel lethal, humans should feel like prey you outsmart and outgun.

*Design test*: If we're debating between X and Y, this pillar says we choose the option that makes the player feel dominant.

### Pillar 3: Nostalgic Era Heart

Each time period is authentically realized — music, architecture, props, enemy behavior. The era isn't just a skin; it's the soul of the level.

*Design test*: If we're debating between X and Y, this pillar says we choose the option that feels true to the time period.

### Pillar 4: CoD Gun Authenticity

Weapons feel, sound, and behave like their Call of Duty inspirations. Reload animations, recoil patterns, audio feedback — everything sells the military fantasy.

*Design test*: If we're debating between X and Y, this pillar says we choose the option that honors the weapon fantasy.

### Anti-Pillars (What This Game Is NOT)

Anti-pillars are equally important — they prevent scope creep and keep vision focused. Every "no" protects a "yes."

- **NOT stealth as a primary mechanic**: We will not have stealth as a primary mechanic because it would compromise Feline Flow — stopping to hide kills momentum.
- **NOT complex weapon customization**: We will not have complex weapon customization/perks because it would compromise Villainous Satisfaction — the cat's power should feel inherent, not earned through grinding.
- **NOT co-op or multiplayer**: We will not have co-op or multiplayer because it would compromise Feline Flow — the cat's parkour movement doesn't work with multiple players.

---

## Visual Identity Anchor

**Selected Direction: Momentum Ink**

### One-Line Visual Rule

Everything must feel in motion — the visual language itself should suggest speed.

### Supporting Visual Principles

1. **Mood & Atmosphere**: Electric energy, kinetic chaos, blurred edges. The world feels like it's being caught mid-action.

2. **Shape Language**: Sharp angles and sweeping curves that imply directionality. No static boxes — every building prop leans or flows in the direction of traversal. Buildings "point" toward optimal paths. The cat's silhouette emphasizes ears and tail that trail in movement.

3. **Color Philosophy**: High-contrast complementary pairs (orange/teal, red/cyan). Colors indicate threat and traversal:
   - Red = danger / enemies
   - Cyan = climbable surfaces / wall-run paths
   - Yellow/gold = objectives / targets
   - Era-specific palettes shift but maintain complementary tension:
     - 1950s: Pastel pink / navy blue
     - 1980s: Neon magenta / electric blue
     - 1920s: Sepia / charcoal
     - Future: Bright green / deep purple

4. **Visual Style**: Stylized realism. Not photorealistic (too content-heavy), but recognizable military weapons and era-specific environments. Think Overwatch meets Metal Slug: clean, readable, with strong silhouettes.

### Design Tests

- **Movement test**: Can a player glance at a scene and immediately know the traversal path? If not, shape language or color coding has failed.
- **Threat test**: Are enemies immediately readable against the environment? If red-on-red, color philosophy has failed.
- **Motion test**: Does even a paused screenshot feel like it's mid-action? If not, the "everything in motion" rule has failed.

---

## Inspiration and References

| Reference | What We Take From It | What We Do Differently | Why It Matters |
| ---- | ---- | ---- | ---- |
| **Hotline Miami** | Combo scoring system, rank-based completion, fast-paced action, "one more run" psychology | Time-travel era progression (not single location), villainous cat protagonist (not human), parkour movement (not top-down) | Validates combo-based replay motivation |
| **Metal Slug** | Run-and-gun core, era-specific enemy types, satisfying weapon feel, boss encounters | 3D parkour movement (not 2D side-scrolling), continuous momentum focus (no defensive play), weapon authenticity over arcade exaggeration | Validates run-and-gun genre appeal |
| **Cuphead** | Bite-sized self-contained levels, distinct themed areas, boss-target structure | Modern weapon feel (not retro), unlockable arsenal progression, time-travel era theme | Validates level structure and replay motivation |
| **Call of Duty** | Authentic weapon feel, reload animations, recoil patterns, audio feedback | Absurd villainous cat premise, parkour movement system, time-travel eras | Validates military weapon fantasy |

**Non-game inspirations:**
- **Back to the Future films**: Nostalgic time period structure (1955, 1985, 2015), era-specific vehicles and environments
- **Looney Tunes**: Cat-and-mouse chase dynamics, exaggerated physics during action (humor tone)
- **John Wick films**: One-person-army fantasy, flow-state combat, movement-through-fire choreography

---

## Target Player Profile

| Attribute | Detail |
| ---- | ---- |
| **Age range** | 18-35 |
| **Gaming experience** | Mid-core to Hardcore — appreciates skill mastery, has played CoD-style shooters |
| **Time availability** | 30-60 minute sessions on weeknights, longer on weekends |
| **Platform preference** | PC — comfortable with keyboard/mouse controls, appreciates Steam leaderboards |
| **Current games they play** | CoD, Valorant, Hotline Miami, Hades, Cuphead, roguelikes |
| **What they're looking for** | Skill expression through movement, satisfying combat feel, replayability through mastery, unique premise |
| **What would turn them away** | Poor weapon feel, clunky movement, repetitive level design, forced story over action |

---

## Technical Considerations

| Consideration | Assessment |
| ---- | ---- |
| **Recommended Engine** | Unity — developer has experience; excellent 3D physics and animation system for parkour; strong Asset Store for CoD-style weapons; solid PC export pipeline |
| **Key Technical Challenges** | Parkour collision on complex era environments (getting stuck is flow-killer); weapon feel tuning (making CoD-style weapons feel satisfying); performance on lower-end PCs with 12 unique levels |
| **Art Style** | 3D stylized realism — readable silhouettes, complementary color coding, motion-emphasized visual language |
| **Art Pipeline Complexity** | Medium — requires 3D modeling for cat protagonist, human enemies, era environments, weapons; but stylized approach reduces complexity from photorealism |
| **Audio Needs** | Moderate — satisfying weapon SFX (CoD-authentic), era-specific music, cat vocalizations, environmental audio for each time period |
| **Networking** | None — single-player only |
| **Content Volume** | 12 levels organized into 4 eras (3 per era), 6-8 weapons, 15+ enemy types, 3-5 hours core gameplay, 8-12+ hours for completionists |
| **Procedural Systems** | Minimal — level layouts are hand-crafted for intentional parkour flow; procedural elements limited to enemy spawns and pickups |

---

## Risks and Open Questions

### Design Risks

- **Core loop may not sustain sessions > 30 minutes**: If the "rampage" gameplay becomes repetitive before the full 5-minute hunt cycle, players may burn out early. *Mitigation*: Vary enemy behaviors per era, introduce era-specific mechanics, ensure each target capture sequence is unique.
- **12 levels becoming repetitive**: If era variations aren't strong enough, players may feel like playing the same level with different skins. *Mitigation*: Distinct era mechanics (e.g., 1920s has darkness/spotlights, Future has verticality), era-specific weapons, unique target capture sequences.
- **Balancing "always in motion" with discovery**: If the game never lets the player stop, they may miss discovery content. *Mitigation*: Discovery is integrated INTO movement (cyan-marked paths, optional traversal routes that reward momentum, not exploration that stops flow).

### Technical Risks

- **Parkour collision bugs**: Getting stuck or falling through level geometry kills Feline Flow. *Mitigation*: Extensive playtesting on collision surfaces, fallback recovery systems (cat auto-detaches from surfaces), generous collision margins.
- **Weapon feel not matching CoD standards**: CoD players have high expectations for weapon feedback. *Mitigation*: Early prototyping focused purely on weapon feel, iterate heavily on recoil patterns and audio, use Asset Store references as baseline.
- **Performance with 12 unique level assets**: Medium art pipeline complexity + 12 unique environments may strain performance. *Mitigation*: Shared asset library per era (3 levels share 70% of assets), LOD systems, performance profiling per era.

### Market Risks

- **Villain protagonist may alienate some players**: Playing *as* the villain is subversive but may not appeal to everyone. *Mitigation*: Emphasize the absurdity (cat villain), make targets deserving in context, lean into the humor.
- **"Cat with guns" may feel gimmicky**: Without excellent core gameplay, the premise may feel like a joke that wears thin. *Mitigation*: Ensure core loop stands on its own as satisfying action — cat villain is flavor, not the whole meal.
- **Competing with established run-and-gun games**: Metal Slug remains the genre gold standard. *Mitigation*: Differentiate through 3D parkour movement, time-travel progression, combo scoring system — we're not trying to be Metal Slug.

### Scope Risks

- **12 levels may exceed "weeks" timeline**: User wants "weeks" timeline but concept has 12 levels. *Mitigation*: Scope tiers defined (MVP = 1 level, Alpha = 4 levels, Beta = 8 levels, Full = 12 levels). Start with MVP and expand based on capacity.
- **Art pipeline for 4 distinct eras**: Creating 4 visually distinct eras is content-heavy for solo dev. *Mitigation*: Asset sharing within eras (70% shared per era), stylized approach reduces detail requirements, focus on era-specific lighting and color over unique architecture.

### Open Questions

- **What is the cat's true motive?** The villain's grand plan is revealed through briefings, but the core question needs clear answer by game end. *Prototype*: Narrative prototype with 3-4 level briefings to test story pacing.
- **Can parkour feel good on keyboard/mouse without complex controller schemes?** 3D parkour is typically controller-optimized. *Prototype*: MVP with keyboard/mouse controls, test playability before committing to platform.
- **What's the right balance between authentic CoD weapons and arcade fun?** Too realistic = frustrating, too arcade = lacks satisfaction. *Prototype*: Weapon-only prototype testing recoil patterns, audio feedback, kill impact.

---

## MVP Definition

The absolute minimum version that validates the core hypothesis. The MVP answers ONE question: "Is the core loop fun?"

**Core hypothesis**: Players find the parkour-movement + weapon-fire combo system engaging for 30+ minute sessions and want to replay levels for better ranks.

**Required for MVP**:

1. **1 complete level** (1950s diner era) — Full vertical slice showing all core systems
2. **2 weapons** (AR, Shotgun) — Demonstrates weapon variety and authenticity
3. **Full parkour movement system** (wall-run, dash, climb, slide) — Tests Feline Flow pillar
4. **3 enemy types** (basic guard, armored guard, target human) — Shows combat variety
5. **Target capture sequence** — Unique sequence for catching the level's target
6. **Basic combo scoring system** — Rewards continuous movement, breaks on stop
7. **Rank system** (D to S) — Shows replay motivation
8. **Color-coded traversal surfaces** (cyan = climbable/wall-run) — Tests Visual Identity Anchor

**Explicitly NOT in MVP** (defer to later):

- Additional eras (1980s, 1920s, Future)
- Additional weapons (SMG, Sniper Rifle, LMG, era specials)
- Leaderboards (community features)
- Extended narrative beyond level briefing
- Unlock system
- Soundtrack beyond basic SFX
- Full art polish (placeholder assets acceptable)

### Scope Tiers (if budget/time shrinks)

| Tier | Content | Features | Timeline |
| ---- | ---- | ---- | ---- |
| **MVP** | 1 level (1950s diner), 2 weapons, 3 enemy types | Core movement, 2 weapons, basic combo, rank system, target capture | 3-6 weeks |
| **Vertical Slice** | 1 full era (3 levels: diner, house, drive-in), 4 weapons, 6 enemy types | All core features, weapon unlock, era-specific mechanics | 6-9 months |
| **Alpha** | 2 eras (6 levels), 6 weapons, 10 enemy types | All core systems, era progression, full combo system, leaderboard infrastructure | 9-12 months |
| **Beta** | 3 eras (9 levels), 8 weapons, 15 enemy types | All features except final era, story complete through 3 eras | 12-15 months |
| **Full Vision** | 4 eras (12 levels), 8 weapons, 15+ enemy types | All features, all eras, complete story, full polish | 15-18 months |

---

## Next Steps

- [ ] Run `/setup-engine` to configure Unity engine and populate version-aware reference docs
- [ ] Run `/art-bible` to create the visual identity specification — do this BEFORE writing GDDs. The art bible gates asset production and shapes technical architecture decisions (rendering, VFX, UI systems).
- [ ] Use `/design-review design/gdd/game-concept.md` to validate concept completeness before going downstream
- [ ] Discuss vision with the `creative-director` agent for pillar refinement (director review pending from brainstorm session)
- [ ] Decompose the concept into individual systems with `/map-systems` — maps dependencies, assigns priorities, and creates the systems index
- [ ] Author per-system GDDs with `/design-system` — guided, section-by-section GDD writing for each system identified in step 4
- [ ] Plan the technical architecture with `/create-architecture` — produces the master architecture blueprint and Required ADR list
- [ ] Record key architectural decisions with `/architecture-decision (×N)` — write one ADR per decision in the Required ADR list from `/create-architecture`
- [ ] Validate readiness to advance with `/gate-check` — phase gate before committing to production
- [ ] Prototype the riskiest system with `/prototype [core-mechanic]` — validate the core loop before full implementation
- [ ] Run `/playtest-report` after the prototype to validate the core hypothesis
- [ ] If validated, plan the first sprint with `/sprint-plan new`

---

## Summary

| Aspect | Detail |
| ---- | ---- |
| **Elevator Pitch** | Fast-paced run-and-gun where you're a feline supervillain rampaging through iconic time periods, hunting humans with Call of Duty-style weapons |
| **Pillars** | Feline Flow, Villainous Satisfaction, Nostalgic Era Heart, CoD Gun Authenticity |
| **Primary Player Type** | Achievers (mastery, ranking, completion) + Explorers (era discovery, secrets) |
| **Engine** | Unity (user preference) |
| **Platform** | PC (Steam / Epic) |
| **Biggest Risk** | 12 levels + 4 distinct eras may exceed "weeks" timeline for solo dev |
| **File Path** | `design/gdd/game-concept.md` |
