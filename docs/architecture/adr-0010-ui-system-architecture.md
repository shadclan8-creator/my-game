# ADR-0010: UI System Architecture

## Status
Proposed

## Date
2026-04-11

## Last Verified
2026-04-11

## Decision Makers
Technical Director, UI Programmer, UX Designer

## Summary

UI System requires menus, HUD, and pause screens with keyboard/mouse and gamepad support. Decision: Use Unity UI Toolkit (UXML/USS) for all runtime UI, era-accents for visual theming.

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | UI / Rendering |
| **Knowledge Risk** | HIGH — UI Toolkit is post-cutoff, replaces legacy UGUI |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | UI Toolkit 1.x, UXML, USS, UI Document, Visual Tree |
| **Verification Required** | Test gamepad navigation, verify accessibility (d-pad support), validate era accent rendering |

### Key Post-Cutoff Changes

- **UI Toolkit**: Runtime-ready UI system replacing UGUI as default
- **UXML (UI XML)**: Declarative UI markup instead of Canvas-based hierarchy
- **USS (UI Style Sheet)**: CSS-like styling for UI elements
- **Visual Tree**: New runtime UI hierarchy system optimized for performant updates
- **Event System**: New event-driven UI interactions replacing OnClick-style callbacks

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Input System (ADR-0001) for menu navigation, Game State System (ADR-0013) for state display |
| **Enables** | Settings System (menu display), Combo System (HUD display), Combat System (weapon info display) |
| **Blocks** | None |
| **Ordering Note** | UI System must be implemented after Input System for menu navigation mapping |

---

## Context

### Problem Statement

The game requires complete UI stack: main menu, pause menu, HUD (combo, health, weapon, objective), death screen, and settings. Must support both keyboard/mouse and gamepad input seamlessly. Unity 6.3 LTS offers UI Toolkit (new default) and legacy UGUI.

### Current State

No UI system exists. Design requires decision between UI Toolkit and UGUI before implementation.

### Constraints

- **Technical**: Must use Unity 6.3 LTS-compatible UI system
- **Platform**: Full gamepad d-pad navigation required (no hover-only interactions)
- **Performance**: UI must not impact 60 FPS gameplay target
- **Aesthetic**: Era-specific accents (1950s pink/navy, 1980s neon, 1920s sepia, Future green/purple)

### Requirements

- **Must support** keyboard/mouse and full gamepad navigation (d-pad, buttons)
- **Must display** HUD elements: health bar (green), combo counter, weapon display, objective marker, rank display
- **Must include** era-specific visual accents without compromising readability
- **Must provide** accessible navigation for gamepad users (no mouse-only interactions)
- **Must support** pause menu that stops gameplay input while allowing UI interaction

---

## Decision

**Use Unity UI Toolkit (UXML/USS)** for all runtime UI with USS for era-specific theming.

### Architecture Diagram

```
┌──────────────────────────┐
│   UI System            │
│   (UI Toolkit)         │
└──┬──────────────────────┘
   │
   ├──────────────────┬──────────────────┬──────────────────┐
   ↓                  ↓                  ↓                  ↓
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│  HUD    │     │ Menus   │     │ Pause    │     │ Death     │
│ Layer   │     │ Layer   │     │ Overlay  │     │ Screen   │
└────┬────┘     └────┬────┘     └─────┬────┘     └─────┬────┘
     │                │                  │                  │
     └────────────────┴──────────────────┴──────────────────┘
                      ↓
              ┌─────────────────┐
              │ Combat/Combo   │
              │ (data bindings) │
              └─────────────────┘
```

### Key Interfaces

```csharp
// UI System exposes these interfaces to other systems
public interface IUIProvider
{
    // HUD Updates
    void UpdateHealth(float current, float max);
    void UpdateCombo(int count, float multiplier, float timer);
    void UpdateWeaponDisplay(string weaponName, int ammo);
    void UpdateObjective(string text);

    // Screen Management
    void ShowPauseMenu();
    void HidePauseMenu();
    void ShowDeathScreen();
    void ShowMainMenu();
    void ShowSettingsMenu();

    // Era Accents
    void ApplyEraTheme(EraType era);

    // Accessibility
    void SetNavigationMode(NavigationMode mode); // Keyboard/Mouse vs Gamepad
}

public enum EraType
{
    NineteenFifties,  // Pink/Navy
    NineteenEighties,  // Neon Magenta/Electric Blue
    NineteenTwenties,   // Sepia/Charcoal
    Future               // Bright Green/Deep Purple
}

public enum NavigationMode
{
    KeyboardMouse,
    Gamepad
}
```

### Implementation Guidelines

1. **UI Document Structure**: Use UI Toolkit UI Document for each screen (MainMenu, PauseMenu, HUD)
2. **UXML Layouts**: Declarative UXML for UI structure; use USS for styling
3. **Era Theming**: USS variables for era-specific colors; apply theme via EraType enum
4. **HUD Layer**: Persistent HUD UI Document in top layer during gameplay
5. **Menu System**: Stack-based menu navigation with push/pop for depth
6. **Gamepad Navigation**: UI Toolkit built-in navigation system; ensure all elements are keyboard/gamepad accessible
7. **Data Binding**: Use UI Toolkit data binding for reactive HUD updates (health, combo)
8. **Accessibility**: Focus system for gamepad; color contrast ratios WCAG 2.1 AA compliant

---

## Alternatives Considered

### Alternative 1: UGUI (Unity's legacy UI)

- **Description**: Use legacy Canvas-based UGUI system
- **Pros**: Extensive documentation and tutorials, LLM has extensive training data, familiar to most Unity devs
- **Cons**: Deprecated default in Unity 6.0+, not optimized for new engine features, limited styling power
- **Rejection Reason**: Engine is pinned to 6.3 LTS; using deprecated system creates future migration debt

### Alternative 2: Hybrid UI System

- **Description**: Use UGUI for menus, UI Toolkit for HUD, or vice versa
- **Pros**: Leverage strengths of both systems
- **Cons**: Double complexity, mixed styling systems, inconsistent input handling
- **Estimated Effort**: Higher (3-4 weeks vs 2 weeks for single system)
- **Rejection Reason**: Mixed UI systems create maintenance overhead; single system provides consistency

### Alternative 3: Custom UI Framework

- **Description**: Build custom UI system from scratch (immediate mode GUI)
- **Pros**: Full control, zero dependencies, can optimize for specific needs
- **Cons**: Massive development time, must implement accessibility, input, animation from scratch
- **Estimated Effort**: Highest (6-8 weeks)
- **Rejection Reason**: UI Toolkit provides production-ready UI with accessibility features out of box

---

## Consequences

### Positive

- **Engine Aligned**: UI Toolkit is Unity 6.3 LTS default, future-proof
- **Declarative UI**: UXML markup separates structure from logic; easier to maintain
- **CSS-like Styling**: USS provides familiar styling syntax; era theming through variable substitution
- **Built-in Accessibility**: UI Toolkit includes focus system, screen reader support, keyboard navigation
- **Performance**: Visual Tree system optimized for runtime updates; less overhead than UGUI Canvas

### Negative

- **Learning Curve**: UI Toolkit is new and differs from UGUI; requires time to learn
- **Documentation Gaps**: Post-cutoff means fewer community resources and examples
- **Debugging**: UXML structure can be harder to debug than Unity inspector-based UGUI
- **Asset Store Compatibility**: Fewer UI Toolkit-compatible assets available

### Neutral

- **File Structure**: Adds UI documents (.uxml), style sheets (.uss), and C# UI controllers

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|-----------|
| UI Toolkit performance issues | LOW | MEDIUM | Profile HUD updates at 144 FPS; implement object pooling for dynamic UI elements |
| Era themes compromise readability | MEDIUM | HIGH | WCAG 2.1 AA color contrast validation for all era palettes |
| Gamepad navigation incomplete | MEDIUM | HIGH | Extensive playtesting with controller; focus visualizer in debug mode |
| Learning curve delays implementation | HIGH | MEDIUM | Prototype HUD first (simplest screen), expand to menus after learning UI Toolkit |

---

## Performance Implications

| Metric | Before | Expected After | Budget |
|--------|--------|---------------|--------|
| CPU (frame time) | 0ms | 0.3-1.0ms | 2.0ms |
| Memory | 0MB | ~15MB (UI assets + runtime UI) | 50MB |
| Load Time | 0s | ~0.2s (UI document parsing) | 2.0s |
| Network | N/A | N/A | N/A |

---

## Migration Plan

**From scratch**: N/A — new project implementation

---

## Validation Criteria

- [ ] HUD displays health, combo, weapon, objective, rank correctly
- [ ] Era themes apply correctly with appropriate color schemes
- [ ] Gamepad d-pad navigation works on all menus
- [ ] Pause menu stops gameplay input while allowing UI interaction
- [ ] Keyboard/mouse and gamepad can switch seamlessly (automatic detection)
- [ ] HUD frame time stays under 0.5ms at 144 FPS
- [ ] UI colors meet WCAG 2.1 AA contrast requirements
- [ ] Focus system works correctly for gamepad users
- [ ] Era accents don't compromise gameplay readability

---

## GDD Requirements Addressed

| GDD Document | System | Requirement | How This ADR Satisfies It |
|-------------|--------|-------------|--------------------------|
| game-concept.md | UI System | Full gamepad support with d-pad navigation | UI Toolkit built-in navigation system ensures all UI elements are gamepad accessible |
| game-concept.md | UI System | Era-accents without compromising readability | USS variables for era-specific colors; contrast validation ensures readability |
| ui-system.md | HUD System | Health bar (green), combo counter, weapon display, objective marker, rank display | UI Toolkit HUD layer with data binding provides reactive updates for all HUD elements |
| ui-system.md | Menu System | Main menu, pause, settings, level select | Stack-based menu system with push/pop for complete menu navigation |
| ui-system.md | UI Events | Mode switch, objective proximity, player death, device connect/disconnect | Event system through UI Toolkit callbacks handles all required UI events |

---

## Related

- ADR-0001: Input System Architecture (provides menu navigation input)
- ADR-0013: Game State System Implementation (provides state data for UI display)
- ADR-0008: Camera System Implementation (provides screen-space effects integration)
