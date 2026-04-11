# Time's Baddest Cat — Assets Directory

This directory contains all game assets for Unity 6.3 LTS project.

## Directory Structure

```
Assets/
├── Art/                 # Visual assets (models, textures, materials)
├── Audio/               # Sound files, music, SFX
├── VFX/                 # VFX Graph assets, particle prefabs
├── UI/                  # UI Toolkit documents (UXML, USS), sprite assets
├── Materials/            # Era-specific materials and shaders
├── Prefabs/             # Reusable GameObject prefabs
├── Scripts/              # C# scripts (mirrored from src/ structure)
├── Addressables/         # Addressable asset groups for async loading
└── Scenes/              # Unity scene files (.unity)
```

## Era Structure

Each era has its own assets to enable modular loading:

```
Assets/Art/
├── 1950s/              # Pink/Navy era assets
│   ├── Characters/
│   ├── Environment/
│   ├── Weapons/
│   └── UI/
├── 1980s/              # Neon Magenta/Electric Blue era assets
├── 1920s/              # Sepia/Charcoal era assets
└── Future/              # Bright Green/Deep Purple era assets
```

## Asset Naming Conventions

- **Prefabs**: PascalCase, descriptive (e.g., `BasicGuard.prefab`, `Weapon_AR.prefab`)
- **Textures**: `[Era]_[Type]_[Description]` (e.g., `1950s_Diner_Wall_Wood`)
- **Materials**: `[Era]_[Surface]_[Color]` (e.g., `1950s_Wall_Pink`)
- **Audio**: `[Era]_[Type]_[Variant]` (e.g., `1950s_Music_Jazz_Main`)

## Memory Budgets

- Total asset memory: 50MB maximum (4 eras × ~12MB per era)
- Single era: 12MB when active (others unloaded)
- Streaming: Use Addressables for async loading to minimize load time

---

**Last Updated**: 2026-04-11
