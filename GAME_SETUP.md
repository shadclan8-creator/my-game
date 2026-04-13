# Time's Baddest Cat - Game Setup Guide

## Quick Start

### 1. Open the Project in Unity
1. Open Unity Hub
2. Click "Add" and select the project folder: `C:\Users\wedad\Desktop\my-game`
3. Open the project (Unity 6.3 LTS required)

### 2. Create a Playable Scene

#### Option A: Automatic Setup (Recommended)
1. In Unity, go to `GameObject > Create Empty`
2. Name it "SceneSetup"
3. Add the `SceneSetup` component to it (from `Assets/Scripts/Gameplay/`)
4. In the Inspector, click "Setup Scene"
5. The scene will be automatically populated with:
   - Player character (orange cat)
   - Main camera with camera system
   - 1950s Diner level with booths, counter, and enemies
   - HUD displaying health, ammo, combo, and score
   - Game manager

#### Option B: Manual Setup
1. Create a GameObject named "Player" and add the `PlayerSetup` component
2. Create a GameObject named "LevelRoot" and add the `LevelGenerator` component
3. Create a GameObject named "HUD" and add the `HUDController` component
4. Create a GameObject named "GameManager" and add the `GameManager` component

### 3. Configure Layers
Make sure these layers exist in Unity (Project Settings > Tags and Layers):
- Player (Layer 6)
- Ground (Layer 8)
- Enemy (Layer 10)
- Traversable (Layer 9) - for wall-running surfaces
- Environment (Layer 11)

### 4. Play the Game
- Press Play in Unity
- Use **WASD** to move
- Use **Mouse** to look around
- Press **Space** to jump
- Press **Shift** to dash
- Press **Left Mouse Button** to fire
- Press **R** to reload
- Press **Escape** to pause

## Game Systems Implemented

### Core Systems
- **InputSystem** - Handles keyboard/mouse and gamepad input
- **PhysicsSystem** - Collision detection and surface detection
- **MovementSystem** - Parkour movement (wall-run, dash, climb)
- **CombatSystem** - Weapon firing and damage
- **CameraSystem** - Follow camera with FOV dynamics
- **ComboSystem** - Scoring and combo multipliers
- **EnemyAIProvider** - Manages all enemies
- **GameManager** - Game state and scoring

### UI/HUD
- Health display with bar
- Ammo counter
- Combo and multiplier display
- Score display
- Crosshair

### Level
- Procedurally generated 1950s Diner
- Color-coded traversable surfaces (cyan)
- Enemy spawn points
- Target location

## Controls

| Action | Keyboard/Mouse |
|--------|----------------|
| Move | WASD |
| Look | Mouse |
| Jump | Space |
| Dash | Shift |
| Fire | Left Mouse |
| Reload | R |
| Pause | Escape |

## Next Steps for Development

1. **Create actual weapon prefabs** - Add 3D models for weapons
2. **Implement weapon switch system** - Allow switching between weapons
3. **Add enemy behaviors** - Implement chase, attack, and patrol states
4. **Create target capture sequence** - Unique sequences for each level's target
5. **Add sound effects** - Weapon sounds, cat vocalizations, ambient audio
6. **Create additional eras** - 1980s, 1920s, Future
7. **Implement leaderboards** - Score tracking and online leaderboards
8. **Add polish** - Particles, animations, screen effects

## File Structure

```
Assets/
├── Scripts/
│   ├── Foundation/
│   │   ├── InputSystem.cs
│   │   ├── PhysicsSystem.cs
│   │   ├── GameEventBase.cs
│   │   └── GameInterfaces.cs
│   ├── Core/
│   │   ├── MovementSystem.cs
│   │   ├── CombatSystem.cs
│   │   ├── CameraSystem.cs
│   │   ├── ComboSystem.cs
│   │   ├── EnemyAI.cs
│   │   └── EnemyAIProvider.cs
│   ├── Gameplay/
│   │   ├── PlayerController.cs
│   │   ├── PlayerSetup.cs
│   │   ├── GameManager.cs
│   │   ├── SceneSetup.cs
│   │   ├── LevelGenerator.cs
│   │   └── HUDController.cs
│   └── Tests/
│       └── Helpers/
│           └── GameAssert.cs
```

## Known Issues

- Cinemachine package is not included - using custom camera system instead
- TMP (TextMeshPro) is required for HUD - install via Package Manager
- Some input actions need to be configured in the new Input System
- Enemy AI is basic - needs more sophisticated behaviors

## Requirements

- Unity 6.3 LTS
- TextMeshPro (install via Package Manager)
- New Input System package (install via Package Manager)

## Installing Required Packages

1. Open Window > Package Manager
2. Select "Unity Registry"
3. Search for and install:
   - TextMeshPro
   - Input System

## License

MIT License - See LICENSE file for details.
