# ADR-0001: Input System Architecture

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Input |
| **Knowledge Risk** | HIGH — new Input System package post-LLM-cutoff |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | InputSystem, InputAction, Unity Input Package (Post-6.0) |
| **Verification Required** | Test input latency, validate gamepad support, test keyboard/mouse mapping |

### Key Post-Cutoff Changes

- **Legacy Input Manager**: Deprecated; new Input System package is default
- **InputAction**: New callback-based API replacing old polling methods
- **Device Profiles**: Support for multiple input devices with automatic switching
- **Enhanced Gamepad Support**: Rumble, motion controls, extended controller capabilities

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | None (Foundation layer system) |
| **Enables** | Movement System (player velocity control), Combat System (fire/reload triggers), UI System (menu navigation) |
| **Blocks** | None |
| **Ordering Note** | This must be implemented before Movement and Combat systems can use input |

---

## Context

### Problem Statement

The game concept requires Keyboard/Mouse input as primary method with Gamepad support as full secondary option. Unity 6.3 LTS defaults to the new Input System package, which has a significantly different API from the Legacy Input Manager that the LLM's training data covers.

### Constraints

- **Technical**: Must use Unity Input System package for engine compatibility
- **Timeline**: Foundation layer system — must be implemented early
- **Performance**: Input sampling must be frame-efficient to support 60 FPS target
- **Cross-platform**: Input must work identically for both keyboard/mouse and gamepad

### Requirements

- **Must support** both Keyboard/Mouse and Gamepad input seamlessly
- **Must provide** low-latency input for parkour responsiveness
- **Must support** rapid input sequences for combat (fire + movement simultaneously)
- **Must integrate** with UI System for menu navigation

---

## Decision

**Use Unity Input System Package** with InputAction-based architecture for all player input.

### Architecture Diagram

```
┌──────────────────────────┐
│   Input System          │
│  (Unity Input Package)    │
│                         │
└──┬──────────────────────┘
   │                      │
   ↓                      ↓
┌────┴────┐        ┌─────┴──┐
│ Movement  │        │ Combat   │
│ System   │        │ System   │
└──────────┘        └──────────┘
```

### Key Interfaces

```csharp
// Input System exposes these interfaces to other systems
public interface IInputProvider
{
    // Movement System
    Vector2 GetMovementAxis();
    bool IsDashRequested();
    bool IsClimbRequested();
    void SetMovementMode(MovementMode mode);

    // Combat System
    bool IsFireRequested();
    bool IsReloadRequested();
    bool IsAimRequested();
    Vector2 GetAimDirection();

    // UI System
    void TogglePause();
    void NavigateUI(Vector2 direction);
    bool IsConfirmPressed();
}
```

---

## Alternatives Considered

### Alternative 1: Legacy Input Manager

- **Description**: Use the old Input Manager system that Unity has used for years
- **Pros**: Well-documented, extensive community resources, LLM has extensive training data
- **Cons**: Deprecated in Unity 6.0+, not optimized for new engine features, poor gamepad support out of box
- **Rejection Reason**: Engine is pinned to 6.3 LTS; using deprecated API creates future migration debt

### Alternative 2: Custom Input System

- **Description**: Build a custom input handling system from scratch
- **Pros**: Full control over implementation, can optimize for specific game needs
- **Cons**: Significant development time, must implement device detection, remapping, profile saving from scratch
- **Rejection Reason**: Unity Input System package provides all needed functionality with engine optimizations

---

## Consequences

### Positive

- **Engine Alignment**: Using official Unity package ensures compatibility with 6.3 LTS features
- **Built-in Features**: Input System provides device profiles, rebinding, gamepad rumble without custom implementation
- **Community Resources**: Extensive documentation and examples for Unity Input System package
- **Maintenance**: Engine updates will include Input System improvements automatically

### Negative

- **Learning Curve**: New Input System API is different from legacy patterns; requires time to learn
- **Parkour Requirements**: High-frequency input sampling needed for responsive wall-running and dash mechanics must be verified against performance budgets
- **Custom Bindings**: While Input System supports rebinding, custom UI must be implemented to expose this feature to players

### Risks

- **API Stability**: Input System package is relatively new; post-cutoff bugs may exist in early 6.3 LTS versions
- **Parkour Responsiveness**: Fast parkour requires input sampling every frame; must ensure Input System callbacks don't add overhead that affects 60 FPS target
- **Gamepad Support**: Must verify full gamepad rumble support for weapon recoil feedback

**Mitigation**:
- Prototype parkour movement early to validate input responsiveness with Input System
- Implement input polling at fixed timestep if callback overhead is a concern
- Document fallback bindings for controllers without rumble support

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-010) | Keyboard/mouse + gamepad input support | Use Unity Input System package which supports both device types seamlessly with device profiles |
| game-concept.md (TR-concept-002) | Parkour movement: quick dashes, wall-run, climb | Input System provides low-latency InputAction callbacks for rapid movement commands |

---

## Performance Implications

- **CPU**: Minimal overhead expected — Input System is designed for frame-efficient input sampling
- **Memory**: Low footprint — Input System is lightweight compared to custom implementation
- **Load Time**: Negligible — Input System initializes on game boot
- **Network**: Not applicable (single-player game)

---

## Migration Plan

**From scratch**: N/A — starting fresh project with correct engine version

---

## Validation Criteria

- **Input latency test**: Measure input-to-render latency with parkour prototype
- **Gamepad compatibility**: Test on Xbox, PlayStation, and Switch-style controllers
- **Keyboard/mouse mapping**: Verify all keys are bindable and intuitive for parkour + combat
- **Frame budget**: Validate input processing stays under 1ms @ 60 FPS

---

## Related Decisions

- Combat System Architecture (ADR-0002) — will consume Input System interfaces
- Movement System Implementation (ADR-0003) — will consume Input System interfaces
