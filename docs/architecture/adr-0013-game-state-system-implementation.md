# ADR-0013: Game State System Implementation

## Status
Proposed

## Date
2026-04-11

## Last Verified
2026-04-11

## Decision Makers
Technical Director, Lead Programmer

## Summary

Game State System manages unlocked eras, collected weapons, level completion, and save/load functionality. Decision: Use JSON serialization with PlayerPrefs for quick saves and file-based saves for full game state.

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS |
| **Domain** | Persistence / Data |
| **Knowledge Risk** | LOW — JSON API and PlayerPrefs are stable and well-documented |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md` |
| **Post-Cutoff APIs Used** | None (JSON and PlayerPrefs pre-date LLM-cutoff) |
| **Verification Required** | Test save/load reliability, validate file size growth, verify cross-session persistence |

---

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | None (Foundation/Foundation layer system) |
| **Enables** | Time Travel System (era unlocks), Combat System (weapon state), Combo System (score persistence), UI System (state display) |
| **Blocks** | None |
| **Ordering Note** | Game State must be implemented early; other systems depend on it for persistence |

---

## Context

### Problem Statement

The game requires persistence for unlocked eras (4 time periods), collected weapons (6-8 per era), level completion status (12 levels), and combo scores. Players must be able to save progress and continue across sessions. Unity 6.3 LTS offers PlayerPrefs (quick settings) and file-based saves (game state).

### Current State

No game state system exists. Design requires save/load architecture with consideration for solo dev timeline.

### Constraints

- **Technical**: Must handle 4 eras × 3 levels = 12 level completion states plus weapon unlocks
- **Platform**: PC-only (Steam/Epic) with potential for future console ports
- **Performance**: Save operations must be fast (<0.5s) and not interrupt gameplay
- **Security**: Prevent save tampering (optional but desirable for leaderboards)

### Requirements

- **Must save** unlocked eras and their completion status
- **Must persist** collected weapons per era
- **Must store** level completion data (best rank, completion time, secrets found)
- **Must support** quick save (checkpoint) and full save (menu-based)
- **Must provide** save slot system (3-5 save slots for player choice)

---

## Decision

**Use JSON serialization for game state with PlayerPrefs for settings and file-based saves for game progress.**

### Architecture Diagram

```
┌──────────────────────────┐
│   Game State System     │
│   (JSON + Files)       │
└──┬──────────────────────┘
   │
   ├──────────────────┬──────────────────┬──────────────────┐
   ↓                  ↓                  ↓                  ↓
┌─────────┐     ┌─────────┐     ┌──────────┐     ┌──────────┐
│ Era     │     │ Weapon  │     │ Level     │     │ Settings  │
│ Progress │     │ Unlocks  │     │ Data      │     │ (Prefs)   │
└────┬────┘     └────┬────┘     └─────┬────┘     └─────┬────┘
     │                │                  │                  │
     └────────────────┴──────────────────┴──────────────────┘
                      ↓
              ┌─────────────────┐
              │   Save Files    │
              │ (slot1, slot2, │
              │    slot3)       │
              └─────────────────┘
```

### Key Interfaces

```csharp
// Game State System exposes these interfaces to other systems
public interface IGameStateProvider
{
    // Era Progression
    bool IsEraUnlocked(EraType era);
    void UnlockEra(EraType era);
    EraProgress GetEraProgress(EraType era);

    // Weapon Unlocks
    bool IsWeaponUnlocked(WeaponType weapon, EraType era);
    void UnlockWeapon(WeaponType weapon, EraType era);
    WeaponData[] GetUnlockedWeapons();

    // Level Completion
    bool IsLevelComplete(string levelId);
    LevelCompletion GetLevelCompletion(string levelId);
    void UpdateLevelCompletion(string levelId, LevelCompletion completion);

    // Save/Load
    bool SaveGame(int slot);
    bool LoadGame(int slot);
    bool QuickSave();
    bool QuickLoad();
    SaveSlotData[] GetSaveSlots();

    // Settings
    GameSettings GetSettings();
    void SaveSettings(GameSettings settings);
}

[System.Serializable]
public class EraProgress
{
    public EraType eraType;
    public bool isUnlocked;
    public List<string> completedLevels;
    public List<WeaponType> unlockedWeapons;
}

[System.Serializable]
public class LevelCompletion
{
    public string levelId;
    public bool isComplete;
    public Rank bestRank;
    public float completionTime;
    public int secretsFound;
    public int maxCombo;
}

[System.Serializable]
public class SaveSlotData
{
    public int slot;
    public DateTime timestamp;
    public EraType currentEra;
    public string currentLevel;
    public List<EraProgress> eraProgress;
    public GameSettings settings;
}

[System.Serializable]
public class GameSettings
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public bool motionBlurEnabled;
    public bool screenShakeEnabled;
    public float mouseSensitivity;
    public float controllerSensitivity;
    public QualityPreset qualityPreset;
}

public enum QualityPreset
{
    Low,
    Medium,
    High,
    Ultra
}
```

### Implementation Guidelines

1. **JSON Serialization**: Use Unity's JsonUtility for game state; serializable classes for all data
2. **Save Slots**: 5 save slots; file naming: `save_slot_0.json`, `save_slot_1.json`, etc.
3. **Persistent Data Path**: Use `Application.persistentDataPath` for cross-platform save location
4. **Settings Storage**: PlayerPrefs for quick settings access (no file I/O for options)
5. **Auto-Save**: Auto-save on level completion and era unlock; manual saves through pause menu
6. **Quick Save/Load**: Separate system for checkpoint saves (within-level only)
7. **Version Checking**: Save file version header for future compatibility
8. **Backup on Save**: Create backup of previous save before overwriting

---

## Alternatives Considered

### Alternative 1: Binary Serialization

- **Description**: Use Unity's binary formatter or custom binary format
- **Pros**: Smaller file size, faster serialization/deserialization
- **Cons**: Less human-readable, harder to debug, version compatibility issues
- **Estimated Effort**: Similar (2 weeks vs 2 weeks for JSON)
- **Rejection Reason**: JSON provides readable saves for debugging; file size not a concern (<1MB saves)

### Alternative 2: ScriptableObject-Based Saves

- **Description**: Use ScriptableObjects as save format
- **Pros**: Native Unity format, good editor integration
- **Cons**: Not designed for runtime serialization, platform-dependent format
- **Estimated Effort**: Higher (3 weeks)
- **Rejection Reason**: ScriptableObjects not suitable for cross-platform save files

### Alternative 3: Database-Based Saves

- **Description**: Use SQLite or similar database for save data
- **Pros**: Fast queries, easy versioning, complex relationships
- **Cons**: Additional dependency, overkill for simple data, platform compatibility concerns
- **Estimated Effort**: Highest (4 weeks)
- **Rejection Reason**: Save data is simple; JSON provides sufficient functionality without database complexity

---

## Consequences

### Positive

- **Human-Readable**: JSON saves can be opened and debugged with text editor
- **Platform Independent**: Persistent data path handles Windows/Mac/Linux correctly
- **Simple Implementation**: JsonUtility provides built-in serialization without third-party libraries
- **Settings Separation**: PlayerPrefs for settings means instant access without file I/O
- **Backup Safety**: File-based saves allow backup creation before overwriting

### Negative

- **File Size**: JSON is verbose; saves will be larger than binary (still <1MB)
- **No Tamper Protection**: Plain JSON can be edited; may need hashing for leaderboards
- **Manual Migration**: Format changes require version checking and migration logic

### Neutral

- **File Structure**: Adds GameStateManager.cs, save slot data classes, JSON save files in persistent data

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|-----------|
| Save file corruption | LOW | HIGH | Backup previous save before overwriting; validate JSON structure on load |
| Save file bloat over time | MEDIUM | MEDIUM | Version checking; compact on major updates; limit save count |
| Cross-platform path issues | LOW | MEDIUM | Use Application.persistentDataPath which handles platform differences |
| Save/load causes frame drops | MEDIUM | MEDIUM | Async save operations with loading screen; prevent save during combat |

---

## Performance Implications

| Metric | Before | Expected After | Budget |
|--------|--------|---------------|--------|
| CPU (frame time) | 0ms | 0.1-0.5ms | 2.0ms |
| Memory | 0MB | ~2MB (in-memory game state) | 50MB |
| Load Time | 0s | ~0.3s (JSON parse) | 2.0s |
| Disk Space | 0MB | ~5MB (5 saves + backups) | 100MB |

---

## Migration Plan

**From scratch**: N/A — new project implementation

**Future Migration**:
1. Add version field to SaveSlotData
2. On load, check version and apply migration logic
3. Keep backup of old saves until user confirms new version works

---

## Validation Criteria

- [ ] Eras unlock correctly after completing previous era levels
- [ ] Weapons persist across sessions (unlocked weapons remain unlocked)
- [ ] Level completion saves rank, time, secrets, and max combo
- [ ] Save slots show timestamp and era/level preview
- [ ] Quick save/load works within current level
- [ ] Full save/load persists all game state
- [ ] Settings persist across game restarts via PlayerPrefs
- [ ] Backup saves created before overwriting
- [ ] Save file size stays under 1MB
- [ ] JSON parses correctly on all platforms

---

## GDD Requirements Addressed

| GDD Document | System | Requirement | How This ADR Satisfies It |
|-------------|--------|-------------|--------------------------|
| game-concept.md | Game State System | Unlock eras, collect weapons, level completion | JSON save structure stores era progress, weapon unlocks, and level completion data |
| game-concept.md | Game State System | Save/load functionality | IGameStateProvider provides SaveGame, LoadGame, QuickSave, QuickLoad methods |
| combat-system.md | Game State System | Weapon unlock persistence | WeaponData and UnlockWeapon methods provide weapon state management |
| combo-system.md | Game State System | Score persistence | LevelCompletion class stores maxCombo and bestRank for score tracking |
| ui-system.md | Game State System | Settings management | GameSettings class with PlayerPrefs provides options storage |

---

## Related

- ADR-0006: Time Travel System Architecture (consumes era unlock data)
- ADR-0004: Combat System Architecture (consumes weapon unlock data)
- ADR-0007: Combo System Architecture (consumes score persistence)
- ADR-0010: UI System Architecture (consumes settings data)
