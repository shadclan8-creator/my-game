# UI System

## Status
In Design

## Author
Claude (AI Agents: ux-designer + ui-programmer)

## Last Updated
2026-04-09

## Overview

Menus, HUD, and pause screens using Unity UI Toolkit. Supports keyboard/mouse and gamepad input. Displays combo count, health, weapon info, and objective markers. Era-accents in UI design but not dominant over gameplay clarity.

## Player Fantasy

The UI is your awareness in the world. Every element is clearly communicated without disrupting your flow. HUD elements, objective markers, and game state are always visible. The UI system is your eyes in Time's Baddest Cat.

## Detailed Rules

### UI Architecture

- **Unity UI Toolkit**: Use UXML (UXML) and USS (Cascading Style Sheets) for runtime UI.
- **Menu System**: Main menu, pause, settings, level select, quit. State management via menus.
- **HUD System**: Health bar, combo counter, weapon display, objective marker, rank display.
- **Pause Menu**: Pauses entire application to show cursor and maintain 60 FPS minimum target during menus.
- **Game Overhead UI**: Shows title screen with era-specific art. Static composition with gameplay HUD overlay.
- **Death/Continue UI**: Shows death filter effect on fallen enemies. Displays "You died" with level restart prompt.
- **Photo Mode**: Captures screenshots for pause menu backgrounds.

### UI Events

- **Mode Switch**: Raised when camera mode changes (Follow → Cinematic → Photo → Death → Continue). Used for state transitions.
- **Objective Proximity**: Raised when player approaches objective threshold. Used for objective display updates.
- **Player Death**: Raised when player health reaches zero. Triggers death mode.
- **Device Connected**: Fired when controller connects. Used for profile loading and haptic feedback.
- **Device Disconnected**: Fired when controller disconnects. Used for profile reversion to keyboard/mouse.

### Dead Zones

- **Hold Input Buffer**: Input actions are captured but not yet processed. Dead zones persist across input modes.
- **Input Cooldowns**: Short cooldowns after specific actions. Dash cooldown: 0.2s minimum between dashes. Weapon switch cooldown: 0.3s minimum.
- **Rapid Input Suppression**: Multiple key presses detected in short window. First valid action processed, buffer subsequent rapid presses ignored.
- **Input Dead Zone Handling**: Input is properly ignored during dead zones. All registered input actions should be ignored.
- **Gamepad Reconnect During Combat**: Controller disconnected then reconnected during combat. State preservation works correctly.
- **Keyboard Ghost Input**: Key pressed during window focus is ignored. Input resumes when window regains focus.
- **Rapid Input Succession**: Multiple key presses detected. All registered input actions processed in order.
- **Simultaneous Input**: Player presses dash and fire simultaneously. Fire input takes priority. Dash may be queued.
- **Controller Reconnect Mid-Action**: Controller disconnected mid-action, reconnected, state preserved. Works as expected.

## Dependencies

- **Movement System** (ADR-0003): Consumes player position/velocity for camera following. Requires combo events for scoring multipliers.
- **Combat System** (ADR-0004): Consumes kill events and damage events for HUD updates.
- **Camera System** (ADR-0005): Consumes player position/velocity for camera following.
- **Game State System** (ADR-0008): Saves/unlocks era progression, combo score, weapon unlocks. Consumes kill events for death state.
- **Audio System** (ADR-???): Consumes weapon fire/load events for audio feedback.

## Formulas

```
UIPosition = GetWorldPosition()
HUDOffset = CurrentPosition + (UIOffset)
ComboTimer = Clamp(ComboTimer * DecayRate - CurrentTime)
```

**Variables**
| Variable | Symbol | Type | Range | Description |
|----------|--------|------|------------|
| ComboTimer | float | 0 to 30 | Combo timer (seconds to zero) |
| DecayRate | float | 0.1 to 0.5 | Decay per second rate |
| UIOffset | Vector2 | N/A | Offset vector for camera position |

**Output Ranges**
| Formula | Output | Range | Notes |
|----------|-------|------|--------|
| ComboTimer | float | 0 to 30 | Current time to zero combo |
| DecayRate | float | 0.1 to 0.5 | Combo decay rate per second |
| UIOffset | Vector2 | Vector2 | -10 to 10 pixels offset from world position |

## Edge Cases

- **Empty Combo Timer**: Combo timer reaches zero → No combo. Timer resets. Player can start new combo immediately.
- **Negative Combo**: Player stops moving → Timer decays rapidly. Combo multiplier is reset to 1.0x immediately.
- **Combo Reset**: Player dies → Combo timer and score are reset. No penalty on death.
- **Objective Missed**: Player passes objective → Combo timer does not get extended time bonus.
- **Player Death**: Player health reaches zero → Combo timer paused. Combo system must preserve death state and show "Game Over" screen.
- **UI Hidden Objective**: Objective marker displayed but player not in proximity. No proximity event raised. Player may not know objective is nearby.
- **Multiple Objectives**: Player activates multiple objectives → All proximity events raised correctly. No conflicts detected.
- **UI Damage Pop**: Player receives damage → Screen shake and hit markers. UI displays damage number and temporary health bar.
- **UI Death Freeze**: Time freezes → "You died" filter applied. Gameplay input disabled during freeze.
- **Gamepad Vibration**: Controller rumble provides weapon recoil feedback via Input System interface.
- **Weapon Switching**: Player cycles through available weapons correctly. No input conflicts.
- **Mode Transition**: Camera mode changes smoothly (Follow → Death → Continue) used for state transitions.
- **Game Over/Resume**: Quick restart option shown correctly after death. Menu background updates correctly.
- **Photo Mode**: Screenshot captured correctly during pause menu. Static composition matches art bible.
- **Debug Mode**: Overhead view shows collision geometry, traversal paths, debug info, game state. Player position visible during debug.

## Tuning Knobs

| Tuning Knob | Range | Effect | Notes |
|-------------|-------|--------|------------|
| ComboTimer | float | 0.0 to 30.0 | Seconds to zero combo | Decay rate constant |
| DecayRate | float | 0.1 to 0.5 | Decay per second |
| UIOffset | float | -10 to 10 pixels offset from world position |
| LookDeadZoneDelay | float | 0.5 | Seconds to hold unprocessed input during dead zone |
| GamepadVibrationIntensity | float | 0.0 to 1.0 | Vibration intensity for weapon recoil feedback |
| MaxFollowSpeed | float | 25.0 | m/s maximum camera follow speed |
| RotationLead | float | 0.1.0 | Rotation lead for camera smoothing |
| MaxLookAheadTime | float | 0.1.0 | How far ahead camera looks ahead of player |
| InputDeadZoneBuffer | float | 0.5 | Seconds to hold unprocessed input before processing |
| GamepadVibrationIntensity | float | 0.0.1 | Controller reconnection delay | 0.2 | Seconds to attempt reconnection |
| RapidInputSuppression | int | 2 | How many key presses to buffer per second |
| Simultaneous Input | bool | false | All registered input actions processed in order |
| WeaponSwitchTime | float | 0.2 | Seconds between weapon switches |
| InputDeadZoneBuffer | float | 0.5 | Seconds to hold unprocessed input during dead zone |

## Acceptance Criteria

1. **Input Responsiveness**: All registered input actions process within one frame.
2. **Device Switching**: Controller connects/disconnects detected within 0.5s and profile loading completes.
3. **Keyboard Ghost Input**: Window focus ignored during window focus. Input resumes when focus regains.
4. **Rapid Input Succession**: Multiple key presses detected. All registered input actions processed in order.
5. **Simultaneous Input**: Dash and fire inputs fire input takes priority. Dash may be queued but not simultaneous.
6. **Gamepad Reconnect During Combat**: Controller disconnected then reconnected. State preservation works correctly.
7. **Mode Transitions**: Camera mode changes smoothly. (Follow → Death → Continue, Death → Photo) used for state transitions.
8. **Player Death**: Health reaches zero → Combo timer paused. State preservation works.
9. **UI Integration**: HUD displays all required data (combo count, health, weapons, objectives) correctly without performance impact.
10. **Readability**: HUD elements, objective markers, health bar, and weapon display are always readable. No colorblind accessibility issues detected.
11. **Performance**: All HUD rendering stays within 0.5ms frame budget.
12. **State Preservation**: Game state persists across level loads and player deaths. Combo timer and score saved correctly on respawn.

## Open Questions

- Should gamepad support force feedback? — Already implemented via Input System.
- Should there be more HUD elements? — Currently sufficient for MVP. Additions post-MVP (per-objectives, level score tracking, rank system).
- Should camera support death freeze? — Already implemented via Camera System.
- Should there be photo mode? — Already implemented via Camera System.

## Dependencies

- **Movement System** (ADR-0003): Consumes position/velocity for camera following.
- **Combat System** (ADR-0004): Consumes kill events and damage events for HUD updates.
- **Camera System** (ADR-0005): Consumes player position/velocity for camera following.
- **Game State System** (ADR-0008): Consumes save/unlock data, combo score, weapon unlocks. Consumes death events for death state.
- **Audio System** (ADR-???): Should exist but not defined in systems-index.md. Consider adding if audio is critical to MVP.

## Acceptance Criteria

1. **Player Death Handling**: Player health reaches zero → Death event raised. "You died" screen shown. Death mode applied correctly.
2. **State Preservation**: Game state persists across level loads and player deaths. Combo timer and score preserved on respawn.
3. **UI Visibility**: HUD elements, objective markers, health bar, weapon display always visible.
4. **Device Support**: Gamepad works correctly. Vibration feedback authentic for weapon recoil. Keyboard/mouse input seamless switching.
5. **Mode Transitions**: Camera mode changes smoothly. All transitions work correctly.
6. **Performance**: HUD rendering within 60 FPS target. No performance degradation.
7. **State Persistence**: Game state saved correctly between sessions.
8. **Gamepad Reconnect Mid-Action**: Controller disconnect/reconnect handled with state preservation.
9. **Readability**: HUD elements always readable. No colorblind accessibility issues.
10. **Game State System**: Save/load format supports 8 weapons, 15+ levels. Era unlock data persisted correctly.

---

## Open Questions

- Should UI support quick resume after death? — Already implemented via Camera System.
- Should there be multiple HUD overlays? — Currently not needed for MVP. Could add post-MVP objective markers (score, rank, death events).
- Should there be objective proximity audio cues? — Already implemented via Combo System.
- Should camera support cinematic photo mode? — Already implemented via Camera System.
- Should there be debug mode toggle? — Already implemented via Camera System.

---

## Dependencies

- **Movement System** (ADR-0003): Consumes position/velocity for camera following
- **Combat System** (ADR-0004): Consumes kill events and damage events for HUD updates
- **Camera System** (ADR-0005): Consumes player position/velocity for camera following
- **Game State System** (ADR-0008): Consumes death events for game state
- **Audio System** (ADR???): Not defined in systems-index.md

---

## Data Structures

```
[System.Serializable]
public class UIState
{
    public ComboTimer float;
    public float ComboMultiplier;
    public bool IsActive;
    public Vector2 uiOffset;
    public int health;
    public int currentHealth;
    public Vector3 worldPosition;
    public bool isObjectiveActive;
    public int deathMode;
}

// HUD position offset from world position
public Vector2 GetHUDPosition() { return worldPosition + uiOffset; }
public void SetDeathMode(DeathMode deathMode);
public void SetObjectiveActive(bool isActive);
public void OnPlayerDeath();
}
```