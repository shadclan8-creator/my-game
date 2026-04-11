# ADR-0005: Asset Management Strategy

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Platform + Addressables |
| **Knowledge Risk** | MEDIUM — Addressables workflow changes in 6.0 LTS |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md`, Unity Addressables documentation |
| **Post-Cutoff APIs Used** | Addressables API (post-6.0) |
| **Verification Required** | Test asset loading performance, validate era switching smoothness |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | None (Platform layer system) |
| **Enables** | Time Travel System (era unlocking), Level System (scene loading), Game State System (save data) |
| **Blocks** | None |
| **Ordering Note** | Platform layer system — must be implemented before content can be loaded |

---

## Context

### Problem Statement

The game has 4 distinct eras (1950s, 1980s, 1920s, Future) with unique assets for 12 levels. Memory budget is 4GB minimum, 8GB recommended. Solo development requires efficient asset management to avoid memory explosion and enable era switching without frame drops.

### Constraints

- **Memory Budget**: 4GB minimum spec constrains total asset memory
- **4 Eras**: Each era has unique environment, enemy, weapon, and audio assets
- **Era Switching**: Players may switch between eras during gameplay
- **12 Levels**: Content volume requires organized asset cataloging
- **Performance**: Frame budget of 16.6ms at 60 FPS; asset loading must not cause stutter

### Requirements

- **Must support** era-specific asset loading and unloading
- **Must minimize** memory footprint by streaming assets rather than loading everything upfront
- **Must provide** fast era switching (< 1 second)
- **Must catalog** all assets for dependency tracking

---

## Decision

**Use Unity Addressables for Async Asset Loading with Era-Based Cataloging**

### Architecture Diagram

```
┌─────────────────────┐
│ Game State         │
│ System             │
│ (save/load)        │
└───┬──────────────┘
      │
      ↓
┌────┴────┐
┌────┴────┐ ┌─────┴───┐
│ Asset       │ │ Time       │ │ Level       │
│ Management  │ │ Travel     │ │ System     │
│ System      │ │ System     │ │ (scenes)    │
└────┬───────┘ └─────────────┘ └─────────────┘
      │
      ↓
┌────┴────────────┐
│     Era Assets     │
│  (4 catalogs)     │
└───────────────────┘
```

### Key Interfaces

```csharp
// Asset Management System interfaces
public interface IAssetProvider
{
    AsyncOperation LoadEraAssets(Era era);
    AsyncOperation UnloadEraAssets(Era era);
    GameObject GetAsset(string assetPath);
    bool IsAssetLoaded(string assetPath);
}

// Era enum for asset organization
public enum Era
{
    None,
    Era1950s,
    Era1980s,
    Era1920s,
    Future
}

// Asset catalog structure
[System.Serializable]
public class EraAssetCatalog
{
    public Era era;
    public List<string> weaponAssets;
    public List<string> enemyAssets;
    public List<string> environmentAssets;
    public List<string> audioAssets;
}
```

---

## Alternatives Considered

### Alternative 1: Load All Assets on Startup

- **Description**: Preload all 4 era asset catalogs during game initialization
- **Pros**: Instant asset availability, no loading during gameplay
- **Cons**: Violates 4GB memory budget (all 4 eras loaded at once)
- **Rejection Reason**: Minimum spec is 4GB; loading all assets would require ~3.5GB+ memory just for environment assets

### Alternative 2: Manual Scene Loading

- **Description**: Load and unload scenes synchronously when switching eras
- **Pros**: Simple implementation, full control over loading process
- **Cons**: Frame drops during era switches (2-3 second stutter), poor user experience
- **Rejection Reason**: Feline Flow pillar requires constant motion; synchronous loading breaks flow

### Alternative 3: Resources.Load

- **Description**: Use Unity's Resources.Load for asset access
- **Pros**: Simple API, built into Unity
- **Cons**: Assets stored in Resources folder increase build size, cannot be unloaded individually, poor memory management
- **Rejection Reason**: Need dynamic unloading for era switching; Resources API is not designed for this use case

---

## Consequences

### Positive

- **Memory Efficiency**: Addressables provides async loading and unloading, fitting assets within budget
- **Smooth Era Switching**: Async loading allows background preparation of next era while current era runs
- **Scalability**: Addressables system scales with content; adding new eras doesn't require architecture changes
- **Cataloging**: Built-in content catalogs provide dependency tracking without custom implementation

### Negative

- **Learning Curve**: Addressables has different workflow than direct asset loading; requires training
- **Build Complexity**: Must configure Addressable groups and labels correctly for proper loading
- **Debugging**: Asset loading issues can be difficult to diagnose; Unity's address system adds complexity

### Risks

- **Configuration Errors**: Incorrect Addressables configuration causes assets to fail loading silently
- **Dependency Hell**: Asset catalogs must be kept in sync with actual asset files; mismatches cause runtime errors
- **Memory Fragmentation**: Improperly configured asset loading/unloading can cause memory fragmentation over long play sessions

**Mitigation**:
- Implement asset validation system to verify all Addressables references are valid before build
- Create tooling to generate Addressables groups from asset catalog automatically
- Profile memory usage with Unity Profiler during era transitions
- Document Addressables configuration clearly for team

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-009) | 4GB minimum, 8GB recommended memory budget | Addressables async loading/unloading keeps memory within 4GB budget |
| game-concept.md (TR-concept-001) | 4 distinct eras with unique content | Era-based asset catalogs via Addressables enable organized era-specific asset management |
| game-concept.md (TR-concept-009) | 60 FPS minimum | Async asset loading prevents frame drops during era switching |

---

## Performance Implications

- **Memory**: Target ~1.5-1.8GB active era assets in memory at any time; fits 4GB minimum with headroom for OS
- **Load Time**: Era transitions take 1-2 seconds for async asset load; acceptable
- **CPU**: Addressables background loading adds minimal overhead (~0.2ms)
- **Network**: Not applicable

---

## Migration Plan

N/A — new project using correct engine version

---

## Validation Criteria

- **Memory test**: Verify game stays under 4GB memory with one era fully loaded
- **Era switch test**: Validate era switching completes in <2 seconds without frame drops
- **Asset validation test**: Confirm all Addressables references resolve correctly
- **Long session test**: Profile memory usage over 30+ minute gameplay to detect fragmentation

---

## Related Decisions

- Time Travel System Architecture (ADR-0006) — will use Asset Management System for era unlocking
- Level System Implementation (ADR-0010) — will consume asset loading interfaces
- Game State System Implementation (ADR-0008) — will save era unlock progress
