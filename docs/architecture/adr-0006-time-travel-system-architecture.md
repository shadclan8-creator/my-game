# ADR-0006: Time Travel System Architecture

## Status
Proposed

## Date
2026-04-09

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Feature (Progression) |
| **Knowledge Risk** | LOW — PlayerPrefs/GameState are stable |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | None |
| **Verification Required** | Test era unlocking persistence, validate save/load performance |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | Asset Management System (ADR-0005), Game State System (ADR-0008) |
| **Enables** | Level System (era scene loading), UI System (era selection display), Combo System (score reset on era switch) |
| **Blocks** | None |
| **Ordering Note** | Feature layer — unlocks content availability |

---

## Context

### Problem Statement

Implement era progression system that unlocks new time periods (1950s → 1980s → 1920s → Future) and manages available era state for level selection and asset loading.

### Constraints

- **4 Distinct Eras**: Each era must be unlockable and provide unique content
- **Progression Persistence**: Player's era progress must survive game sessions
- **Asset Dependency**: Eras cannot be loaded without Asset Management System in place
- **UI Integration**: Era selection screens must display locked/unlocked states

### Requirements

- **Must support** linear or non-linear era unlocking
- **Must integrate** with Asset Management System for era asset loading
- **Must provide** era metadata to Level System and UI System
- **Must persist** era completion status across sessions

---

## Decision

**Linear Era Unlocking with Save Data Persistence**

### Architecture Diagram

```
┌────────────────────────┐
│   Game State         │
│   System             │
└───┬───────────────┘
      │
      ↓
┌────┴────────────────┐
│  Time Travel         │
│  System              │
└───┬───────────────┘
      │       │
      ↓       ↓
┌────┴───┐ ┌────┴───┐
│ Asset     │ │   Level   │
│ Management│ │   │ System │
│  System   │ │   └─────────┘
│  └────┬──┘
│        ↓
│   ┌────┴────────────────┐
│   │ UI System           │
│   └────────────────────┘
│           │
└───────────┘
```

### Key Interfaces

```csharp
// Time Travel System interfaces
public interface ITimeTravelProvider
{
    bool IsEraUnlocked(Era era);
    void UnlockEra(Era era);
    List<Era> GetUnlockedEras();
    Era GetCurrentEra();
}

// Asset Management System provides era assets
public interface IAssetProvider
{
    AsyncOperation LoadEraAssets(Era era);
    AsyncOperation UnloadEraAssets(Era era);
}

// Game State System provides persistence
public interface IGameStateProvider
{
    void SaveEraProgress(EraProgressionData data);
    EraProgressionData LoadEraProgress();
}

// Era enum
public enum Era
{
    None,
    Era1950s,
    Era1980s,
    Era1920s,
    Future
}

// Era progression data
[System.Serializable]
public class EraProgressionData
{
    public Dictionary<Era, bool> eraUnlockStatus;
    public int currentEra;
}
```

---

## Alternatives Considered

### Alternative 1: PlayerPrefs Only

- **Description**: Use Unity's PlayerPrefs for era unlocking data
- **Pros**: Built-in, simple API, requires no custom implementation
- **Cons**: Limited to 1MB storage, no structured data support, difficult to manage complex progression
- **Rejection Reason**: Game needs multiple era states, weapon unlocks, level completion tracking per era — exceeds PlayerPrefs capacity

### Alternative 2: JSON File Saves

- **Description**: Save era progression to JSON files on disk
- **Pros**: No storage limit, structured data, easy to debug and version
- **Cons**: Manual file I/O, potential save corruption, platform-specific file paths
- **Rejection Reason**: Adds file management complexity; Asset Management System needs to load from disk files

### Alternative 3: Binary Serialization

- **Description**: Use Unity's BinaryFormatter for save data
- **Pros**: Fast, efficient file I/O, type-safe
- **Cons**: BinaryFormat deprecated, Unity 6.3 LTS recommends JSON or ScriptableObjects
- **Rejection Reason**: Future migration path unclear

---

## Consequences

### Positive

- **Structured Progression**: JSON saves provide flexible era tracking with no storage limits
- **Scalability**: System can handle additional eras or complex progression requirements
- **Debuggability**: JSON saves are human-readable for debugging save issues
- **Asset Integration**: Clean interface with Asset Management System enables async era loading

### Negative

- **Implementation Time**: Custom JSON save/load system requires development time
- **Save Corruption**: Manual file management requires robust error handling
- **Testing**: Need to validate save/load behavior across game sessions

### Risks

- **Data Loss**: Players lose progress if save files become corrupted
- **Migration**: Save format changes require migration logic for existing saves
- **Loading Performance**: Large JSON files on disk can cause startup delay

**Mitigation**

- Implement automatic backup of save files before overwriting
- Add save file versioning for migration support
- Show save/load times and implement corruption detection
- Load era assets asynchronously during menu screen to hide loading time

---

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|-------------|-------------|--------------------------|
| game-concept.md (TR-concept-005) | Time Travel unlockable eras progression | JSON save/load with EraProgressionData provides era unlock persistence without storage limits |

---

## Performance Implications

- **Memory**: Era progression data estimated at <1KB per save file; negligible
- **Load Time**: Asset loading handled by Asset Management System; Time Travel System just manages unlock state
- **Network**: Not applicable

---

## Migration Plan

N/A — new project using JSON save format from start

---

## Validation Criteria

- **Persistence test**: Verify era progress survives game restart, application close
- **Era switching test**: Confirm assets load smoothly within 1-2 seconds on all hardware tiers
- **Save corruption test**: Test behavior with corrupted save file (graceful handling or error message)
- **Maximum eras test**: Verify system supports 8 eras if future DLC adds more

---

## Related Decisions

- Asset Management Strategy (ADR-0005) — provides asset loading interfaces
- Game State System Implementation (ADR-0008) — provides save/load interfaces
- Level System Implementation (ADR-0010) — consumes era state for scene loading