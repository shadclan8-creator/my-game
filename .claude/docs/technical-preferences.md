# Technical Preferences

## Engine & Language

- **Engine**: Unity 2022 LTS
- **Language**: C#
- **Rendering**: Universal Render Pipeline (URP)
- **Physics**: Unity Physics

## Input & Platform

<!-- Written by /setup-engine. Read by /ux-design, /ux-review, /test-setup, /team-ui, and /dev-story -->
<!-- to scope interaction specs, test helpers, and implementation to the correct input methods. -->

- **Target Platforms**: PC (Steam / Epic)
- **Input Methods**: Keyboard/Mouse, Gamepad
- **Primary Input**: Keyboard/Mouse
- **Gamepad Support**: Full (recommended for broad appeal)
- **Touch Support**: None
- **Platform Notes**: Steamworks integration for achievements/leaderboards; Epic Games Store release as secondary. All UI must support d-pad navigation for gamepad users. No hover-only interactions.

## Naming Conventions

- **Classes**: PascalCase (e.g., `PlayerController`)
- **Public fields/properties**: PascalCase (e.g., `MoveSpeed`)
- **Private fields**: _camelCase (e.g., `_moveSpeed`)
- **Methods**: PascalCase (e.g., `TakeDamage()`)
- **Files**: PascalCase matching class (e.g., `PlayerController.cs`)
- **Scenes**: PascalCase (e.g., `MainMenuScene.unity`, `Level01_Diner.unity`)
- **Constants**: PascalCase (e.g., `MaxHealth`, `DefaultMoveSpeed`)
- **GameObjects**: PascalCase in scene, camelCase in code references
- **Events/Delegates**: PascalCase + `EventHandler` suffix (e.g., `HealthChangedEventHandler`)

## Performance Budgets

- **Target Framerate**: 60 FPS minimum, 144 FPS preferred on capable hardware
- **Frame Budget**: 16.6ms @ 60 FPS, 6.9ms @ 144 FPS
- **Draw Calls**: Engine-optimized with URP batching; target under 1000 per frame
- **Memory Ceiling**: 4 GB minimum, 8 GB recommended

## Testing

- **Framework**: NUnit (Unity's built-in test framework)
- **Minimum Coverage**: 80% for core gameplay systems
- **Required Tests**: Movement system, weapon behavior, combo scoring, collision systems, input handling

## Forbidden Patterns

<!-- Add patterns that should never appear in this project's codebase -->
- [None configured yet — add as architectural decisions are made]

## Allowed Libraries / Addons

<!-- Add approved third-party dependencies here -->
- [None configured yet — add as dependencies are approved]

## Architecture Decisions Log

<!-- Quick reference linking to full ADRs in docs/architecture/ -->
- [No ADRs yet — use /architecture-decision to create one]

## Engine Specialists

<!-- Written by /setup-engine. Read by /code-review, /architecture-decision, /architecture-review, and team skills -->
<!-- to know which specialist to spawn for engine-specific validation. -->

- **Primary**: unity-specialist
- **Language/Code Specialist**: unity-specialist (C# review — primary covers it)
- **Shader Specialist**: unity-shader-specialist (Shader Graph, HLSL, URP/HDRP materials)
- **UI Specialist**: unity-ui-specialist (UI Toolkit UXML/USS, UGUI Canvas, runtime UI)
- **Additional Specialists**: unity-dots-specialist (ECS, Jobs system, Burst compiler), unity-addressables-specialist (asset loading, memory management, content catalogs)
- **Routing Notes**: Invoke primary for architecture and general C# code review. Invoke DOTS specialist for any ECS/Jobs/Burst code. Invoke shader specialist for rendering and visual effects. Invoke UI specialist for all interface implementation. Invoke Addressables specialist for asset management systems.

### File Extension Routing

| File Extension / Type | Specialist to Spawn |
|-----------------------|---------------------|
| Game code (.cs files) | unity-specialist |
| Shader / material files (.shader, .shadergraph, .mat) | unity-shader-specialist |
| UI / screen files (.uxml, .uss, Canvas prefabs) | unity-ui-specialist |
| Scene / prefab / level files (.unity, .prefab) | unity-specialist |
| Native extension / plugin files (.dll, native plugins) | unity-specialist |
| General architecture review | unity-specialist |
