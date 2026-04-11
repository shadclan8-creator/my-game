# Review Log: game-concept.md

## Review — 2026-04-09 — Verdict: NEEDS REVISION
Scope signal: XL (cross-cutting concerns, requires multiple ADRs)
Specialists: game-designer, systems-designer, performance-analyst, art-director, level-designer
Blocking items: 14 | Recommended: 8

---

### Specialist Findings Summary

**[game-designer]**
- Feline Flow contradicts CoD-style tactical combat (cover/positioning vs constant motion)
- MDA framework misalignment - Sensation/Fantasy not delivered by 30-second loop
- Discovery vs Motion contradiction unsolved - players will miss content or break flow
- Only 3 levels per era insufficient for "authentic era" experience
- Villain protagonist lacks narrative depth to prevent player alienation
- Target Capture mechanic unclear - no distinction from kill everyone

**[systems-designer]**
- CRITICAL: Level System has 7 dependents - 2-week delay cascades to 6+ months
- CATASTROPHIC: Combat System has 9 dependents - stalls entire game loop
- CRITICAL: No Asset Management System defined for 4 eras × massive content
- BLOCKING: MVP cannot test core hypothesis (no weapons, enemies, combos)
- Time Travel vs Level architecture undefined - no file structure/asset loading strategy
- CRITICAL: Solo dev scope unrealistic - 15-19 months minimum for stated content
- Undocumented circular dependencies (Movement↔Combat, Combo↔Combat, Camera↔Movement)
- Missing: Progression System, Environment System (referenced but undefined)

**[performance-analyst]**
- CRITICAL: 12 unique era environments cannot reliably hit 60 FPS on 4GB RAM
- Parkour collision on complex geometry is CPU-intensive
- CRITICAL: Motion blur will cripple lower-end systems (12-24% of frame budget)
- MISSING: Occlusion culling not defined - draw call explosion inevitable
- CRITICAL: Memory budget crisis - ~3.5GB minimum leaves no headroom
- MISSING: Profiling strategy, quality presets, target GPU spec, CPU threading

**[art-director]**
- CRITICAL: "70% asset sharing" is dangerously optimistic - expect 40-60% unique
- CRITICAL UX ISSUE: Cyan traversal surfaces create visual chaos/decision paralysis
- CRITICAL: Stylized weapons will feel "toy-like" vs CoD authenticity expectation
- CRITICAL USABILITY ISSUE: "Momentum Ink" motion everywhere causes cognitive overload
- CRITICAL ACCESSIBILITY: Colorblind backup insufficient (no shape coding)
- PROJECT KILLER: Film-quality references impossible for solo dev

**[level-designer]**
- CRITICAL: Cyan surfaces create visual pollution in dense urban environments
- CRITICAL: Buildings "point toward optimal paths" undermines exploration
- 3 levels per era cannot deliver distinct era experiences
- CRITICAL: "Never stop moving" contradicts combat in cover elements
- CRITICAL: 12 unique target capture sequences = massive scope for solo dev
- Color code conflicts (cyan=climbable AND objectives)
- Keyboard/mouse parkour control scheme unvalidated

---

### Verdict: NEEDS REVISION

The concept has **fundamental contradictions** between core pillars that must be resolved:
1. Feline Flow (always in motion) vs CoD Combat (tactical positioning/cover)
2. Villainous Satisfaction (power fantasy) vs skill-based weapon mechanics
3. Discovery content vs speed-driven gameplay

**Critical architectural gaps**:
- No Asset Management System for 4-era content
- MVP systems insufficient to test core hypothesis (missing Combat, Enemy AI, Combo, UI)
- Bottleneck risks: Level (7 dependents) and Combat (9 dependents)

**Performance risks**:
- Memory budget unrealistic for 4GB minimum (~3.5GB minimum needed)
- Missing occlusion culling, quality presets, profiling strategy
- Motion blur and 12 unique era environments threaten 60 FPS target

**Art direction conflicts**:
- Cyan traversal creates visual noise
- Stylized weapons undermine CoD authenticity
- Motion identity causes cognitive overload

**Scope issues**:
- 12 levels for solo dev unrealistic (15-19 months minimum)
- 12 unique target capture sequences = 12 mini-games

---

### Required Before Implementation

1. **Reconcile Pillar Conflicts** - Decide between pure momentum combat OR accept that flow is ideal, not mandate. Cover should not punish combos.

2. **Define Asset Management System** - Create GDD for era switching, asset loading/unloading, memory management.

3. **Expand MVP Systems** - Add Combat, Enemy AI, Combo, and UI to MVP to make core loop testable.

4. **Scope Reduction** - Reduce to 2-3 eras maximum, 6-8 levels total for solo dev reality.

5. **Resolve Visual Language Issues** - Cyan traversal: use context-sensitive highlighting instead of constant glow. Motion identity: 80% static, 20% motion.

6. **Create Critical ADRs** - Asset Management Strategy, Time Travel Integration, Content Scope Reduction.

7. **Add Performance Documentation** - Define quality presets, occlusion culling, profiling strategy.

8. **Fix Colorblind Accessibility** - Implement shape coding, not just texture/sound backups.

9. **Clarify Target Capture** - Define mechanical distinction between targets and standard enemies.

10. **Validate Control Scheme** - Prototype keyboard/mouse parkour before committing.

11. **Reduce Target Capture Scope** - 6-8 unique sequences maximum, not 12.

12. **Set Realistic Art Expectations** - "inspired by" not "referencing" for solo dev quality.

13. **Add Missing Systems to Index** - Progression System, Environment System, Asset Management System.

14. **Document Circular Dependencies** - Movement↔Combat, Combo↔Combat, Camera↔Movement in design docs.

---

### Recommended Revisions

1. Update MVP definition to include Combat, Enemy AI, Combo, UI systems
2. Reduce target capture sequences to 6-8 maximum
3. Change "cyan surfaces" to context-sensitive highlighting
4. Add performance budget section with quality presets
5. Add occlusion culling to asset standards
6. Create Asset Management System in systems-index
7. Add shape-based colorblind accessibility
8. Document circular dependencies in all affected GDDs
9. Reduce era count to 2-3 for initial release
10. Commit to 80% static UI, 20% motion
11. Define keyboard/mouse parkour feasibility assessment
12. Add CPU threading strategy to technical preferences
13. Create worst-case performance test scene requirement
14. Define "minimum viable aesthetic" vs ideal in art bible

---

### Senior Verdict

**[creative-director] Not invoked - solo mode execution based on specialist synthesis**

The concept requires **fundamental restructuring** before implementation. The core pillars are at odds, systems architecture has critical bottlenecks and gaps, performance is unachievable on stated minimum specs, and scope is unrealistic for solo development within the 12-18 month timeline.

Priority: **Resolve pillar conflicts → Create Asset Management System → Expand MVP → Reduce scope → Document performance strategy.**

---

*Next review will assess whether these revisions were applied.*
