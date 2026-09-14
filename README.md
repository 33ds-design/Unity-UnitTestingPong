> **Attribution**: This project is a fork of [Mina Pêcheux's Unity-UnitTestingPong](https://github.com/MinaPecheux/Unity-UnitTestingPong) (January 2022).
> Original game code and initial test suite by Mina Pêcheux.
> Test suite expansion, CI/CD pipeline, and coverage integration by 33ds-design (2026).
> Licensed under the [MIT License](LICENSE).

# Unity Pong — Unit Testing & CI/CD

![Unity Tests](https://github.com/33ds-design/Unity-UnitTestingPong/actions/workflows/unity-test.yml/badge.svg)
[![codecov](https://codecov.io/gh/33ds-design/Unity-UnitTestingPong/branch/master/graph/badge.svg)](https://codecov.io/gh/33ds-design/Unity-UnitTestingPong)

A Pong game built in Unity, extended with **121 automated tests** (EditMode + PlayMode), **GitHub Actions CI**, and **Codecov coverage reporting**.

Developed with Tuanjie Engine 1.16.13, CI runs on standard Unity 2022.3 LTS Docker images.

---

## Table of Contents

- [Project Structure](#project-structure)
- [Game Features](#game-features)
- [Test Suite](#test-suite)
- [CI/CD Pipeline](#cicd-pipeline)
- [Local Development](#local-development)
- [License](#license)

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── BallManager.cs        # Ball physics: launch, speed increment, wall/paddle collision
│   ├── GameHandler.cs        # State machine: WaitingToStart → Playing → GameOver
│   ├── GameManager.cs        # MonoBehaviour bridge: UI, scoring, lifecycle delegation
│   ├── PaddleManager.cs      # Paddle movement, boundary clamping, AI tracking logic
│   └── ScoreTrigger.cs       # Collision-based scoring via trigger zones
├── Tests/
│   ├── EditorTests/          # 87 EditMode unit tests (NUnit, no runtime needed)
│   │   ├── BallManagerTests.cs
│   │   ├── GameHandlerTests.cs
│   │   ├── GameManagerTests.cs
│   │   ├── PaddleManagerTests.cs
│   │   └── ScoreTriggerTests.cs
│   └── PlayTests/            # 34 PlayMode integration tests (UnityTest coroutines)
│       ├── BallPhysicsPlayTests.cs
│       ├── GameStatePlayTests.cs
│       ├── PaddleMovementPlayTests.cs
│       └── ScoreTriggerPlayTests.cs
├── Tests/Editor/Tests.asmdef  # EditMode assembly definition
└── Tests/PlayTests/PlayTests.asmdef  # PlayMode assembly definition
.github/
└── workflows/
    └── unity-test.yml        # CI: EditMode + PlayMode + Coverage jobs
scripts/
├── patch-for-ci.sh           # Patches Tuanjie project for standard Unity CI
└── run-coverage.ps1          # Local coverage runner (Windows)
docs/
└── CI_SETUP.md               # CI configuration guide
```

## Game Features

| Feature | Description |
|---------|-------------|
| **State Machine** | `WaitingToStart → Playing → GameOver` with restart support |
| **Ball Physics** | Launch with random angle, speed increment on paddle hit, max speed cap |
| **AI Paddle** | Reaction-delayed ball tracking with configurable delay threshold |
| **Scoring** | Trigger-zone based scoring, first to `maxScore` (default 5) wins |
| **Boundary Clamping** | Paddles clamped to ±4 units on Y axis |

## Test Suite

### EditMode Unit Tests (87 tests)

No Unity runtime required — pure logic verification via NUnit.

| Test File | Count | Coverage |
|-----------|-------|----------|
| `BallManagerTests.cs` | 14 | Launch, speed increment, max speed, direction queries |
| `GameHandlerTests.cs` | 18 | State transitions, scoring, game over, restart, total score |
| `GameManagerTests.cs` | 17 | MonoBehaviour bridge: Start, ScorePoint, RestartGame, state queries |
| `PaddleManagerTests.cs` | 19 | MoveUp/Down, bounds, speed config, AI tracking |
| `ScoreTriggerTests.cs` | 19 | Left/right trigger scoring, game-not-playing guard, accumulation |

### PlayMode Integration Tests (34 tests)

Uses `UnityTest` coroutines with real Unity runtime — physics, `Rigidbody2D`, `Time.deltaTime`.

| Test File | Count | Coverage |
|-----------|-------|----------|
| `BallPhysicsPlayTests.cs` | 7 | Rigidbody velocity, wall bounce, paddle hit speed-up, directional movement |
| `GameStatePlayTests.cs` | 13 | Start → Playing → GameOver lifecycle, scoring, restart, ball velocity after launch |
| `PaddleMovementPlayTests.cs` | 10 | Move up/down, bounds, speed config, AI追球, no-target guard |
| `ScoreTriggerPlayTests.cs` | 4 | Trigger scoring via reflection, game-state guard, accumulation |

### Testing Techniques Used

- **Reflection**: Access private methods (`UpdateAI`, `OnTriggerEnter2D`) and fields (`_aiTimer`, `_ball`) to bypass Unity lifecycle constraints in EditMode
- **WaitForFixedUpdate**: Ensure physics simulation steps complete before assertions in PlayMode
- **Static reset**: `PaddleManager.ResetSpeed()` / `PaddleManager.SetSpeed()` for test isolation
- **Singleton injection**: `GameManager.instance = _gameManager` for ScoreTrigger integration tests

## CI/CD Pipeline

### GitHub Actions Workflow (`.github/workflows/unity-test.yml`)

Three parallel jobs run on every push and pull request:

| Job | Runner | Mode | Artifacts |
|-----|--------|------|-----------|
| **EditMode Tests** | `ubuntu-latest` | EditMode | XML test results |
| **PlayMode Tests** | `ubuntu-latest` | PlayMode | XML test results |
| **Code Coverage** | `ubuntu-latest` | EditMode + Coverage | Cobertura XML → Codecov |

### Unity License Activation

Uses **serial-based activation** with three GitHub Secrets:

| Secret | Purpose |
|--------|---------|
| `UNITY_SERIAL` | Unity serial number (format: `XX-XXXX-XXXX-XXXX-XXXX-XXXX`) |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password |

### Tuanjie Engine Compatibility

CI runs on standard Unity 2022.3.48f1 Docker images (`unityci/editor:ubuntu-2022.3.48f1-linux-il2cpp-3`). The `scripts/patch-for-ci.sh` script automatically:

1. Converts `ProjectVersion.txt` from `2022.3.61t14` → `2022.3.48f1`
2. Removes Tuanjie-specific packages from `manifest.json`
3. Removes MCP plugin package (CI doesn't need editor connection)
4. Adds `com.unity.testtools.codecoverage` package for coverage reporting

### Code Coverage

Coverage data flows: Unity OpenCover XML → ReportGenerator → Cobertura XML → Codecov.

Assembly filters: `+Assembly-CSharp,+Pong` (only game code, not test code).

## Local Development

### Prerequisites

- [Tuanjie Engine 1.16.13](https://unity.cn/tuanjie) (or Unity 2022.3 LTS)
- Git

### Running Tests Locally

#### In Tuanjie/Unity Editor

1. Open the project in Tuanjie Editor
2. `Window` → `General` → `Test Runner`
3. For EditMode tests: select **EditMode** tab → **Run All**
4. For PlayMode tests: select **PlayMode** tab → **Run All**

#### Coverage Report (Windows)

```powershell
# EditMode coverage only
.\scripts\run-coverage.ps1 -Mode EditMode

# Both EditMode and PlayMode
.\scripts\run-coverage.ps1 -Mode All
```

Output: `test-results/` (raw XML) and `coverage-report/` (HTML report).

### Running Tests via CLI (Standard Unity)

```bash
# Requires Unity 2022.3 LTS installed locally
Unity.exe -runTests -batchmode -projectPath . -testPlatform editmode -testResults test-results/editmode.xml
Unity.exe -runTests -batchmode -projectPath . -testPlatform playmode -testResults test-results/playmode.xml
```

## License

This project is licensed under the [MIT License](LICENSE).

### Attribution

- **Original game code & initial tests**: Mina Pêcheux (January 2022) — [Source repo](https://github.com/MinaPecheux/Unity-UnitTestingPong)
- **Test expansion, CI/CD, coverage**: 33ds-design (2026)
- **Original article**: [Unit testing automation in Unity](https://blog.codemagic.io/unit-testing-automation-unity/) on Codemagic blog
