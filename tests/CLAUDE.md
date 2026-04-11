# Tests Directory

This directory contains all test suites for Time's Baddest Cat.

## Directory Structure

```
tests/
├── Editor/               # Edit-time tests (run in Unity Editor)
│   └── Unit/          # Unit tests for individual systems
├── PlayMode/             # Play-in-Editor tests (run in simulated game)
├── Integration/          # Integration tests for multi-system interactions
└── Helpers/              # Test helper utilities and factories
```

## Test Categories

| Category | Purpose | Location |
|----------|----------|----------|
| **Unit Tests** | Test individual system logic in isolation | `tests/Editor/Unit/` |
| **Integration Tests** | Test interactions between multiple systems | `tests/Integration/` |
| **Play Tests** | Test game as player experiences it | `tests/PlayMode/` |

## Testing Standards

Following `.claude/docs/coding-standards.md`:

- **Naming**: `[system]_[feature]_test.cs` for files, `test_[scenario]_[expected]` for functions
- **Determinism**: Tests must produce the same result every run — no random seeds
- **Isolation**: Each test sets up and tears down its own state
- **No Hardcoded Data**: Test fixtures use constant files or factory functions
- **Independence**: Unit tests do not call external APIs, databases, or file I/O

## Coverage Targets

| System | Minimum Coverage | Notes |
|---------|----------------|--------|
| Input System | 80% | Core system, essential for all gameplay |
| Movement System | 80% | Core system, parkour complexity |
| Physics System | 80% | Foundation system, collision critical |
| Combat System | 80% | Core system, weapon mechanics |
| Camera System | 70% | Core system, polish-focused |
| Enemy AI System | 70% | Core system, behavior complexity |
| Combo System | 80% | Core system, scoring logic |
| UI System | 60% | Presentation system, screen-space |
| Audio System | 60% | Presentation system, asset-heavy |
| VFX System | 50% | Presentation system, visual focus |

## Running Tests

**Unity Editor Tests**: Run via `Test Runner` window or `Unity Test Framework`

**Play Mode Tests**: Run via `[Runtime]` test attribute

---

**Last Updated**: 2026-04-11
