# QA Smoke Test Plan: Chrono-Core
**Version:** 1.0
**Author:** Senior QA Engineer
**Phase:** 1 — Grey-Box Prototype
**Framework:** Unity Test Framework (UTF) — PlayMode Tests
**CI/CD:** GameCI + GitHub Actions

---

## 1. Test Scope

### In Scope (Phase 1)
- Player movement: walk, jump, dash
- Dash cooldown and invincibility frames (i-frames)
- Chrono Energy system: depletion and regeneration
- RewindManager: buffer fill rate, state capture, playback
- Rewind position restoration accuracy
- Rewind hard-stop at room boundary
- Irreversible event preservation across rewind
- Rewind interaction with dash state

### Out of Scope (Phase 1)
- Enemy AI behavior
- Projectile interactions
- Destructible objects
- Boss encounters
- Audio rewind behavior (Unit tests pending AudioRewindController implementation)
- UI rendering and HUD accuracy
- Save/load system
- SHMUP stages

---

## 2. Test Environment

| Property | Value |
|----------|-------|
| Engine | Unity 6 LTS |
| Render Pipeline | Universal Render Pipeline (URP) 2D |
| Test Framework | Unity Test Framework (UTF) 1.4+ |
| Test Mode | PlayMode (requires game runtime — physics, FixedUpdate) |
| Target Platform | PC (Standalone) |
| Run locally | `Window > General > Test Runner > PlayMode > Run All` |
| Run in CI | Triggered on every PR to `develop` via GameCI + GitHub Actions |
| Test scene | `Assets/Tests/PlayMode/Scenes/SmokeTestScene.unity` — minimal scene, player + floor, no enemies |

### CI Gate Rule
All smoke tests must pass before a PR can be merged to `develop`. A single failing test blocks the merge. The Architect is automatically tagged in the failing PR.

---

## 3. Smoke Test Cases

---

### TC-01: Player Can Walk Left and Right

**Given** the player is standing on a flat surface with no obstacles
**When** left or right movement input is held for 1 second
**Then** the player's X position changes by more than 0 units in the corresponding direction

```
Test ID: TC-01
Category: Movement
Severity: P0 — Blocker
Expected: PASS
```

---

### TC-02: Player Can Jump

**Given** the player is grounded
**When** the Jump input is triggered
**Then** the player's Y position increases by at least 1 Unity unit within 0.2 seconds

```
Test ID: TC-02
Category: Movement
Severity: P0 — Blocker
Expected: PASS
```

---

### TC-03: Player Can Dash

**Given** the player is grounded or airborne
**When** the Dash input is triggered
**Then** the player's velocity exceeds normal run speed by at least 2x for the duration of the dash

```
Test ID: TC-03
Category: Movement
Severity: P0 — Blocker
Expected: PASS
```

---

### TC-04: Dash Cooldown Enforced

**Given** the player just completed a dash
**When** the Dash input is triggered again within 0.8 seconds
**Then** no second dash occurs (player velocity does not spike)

```
Test ID: TC-04
Category: Movement
Severity: P1 — Critical
Expected: PASS
```

---

### TC-05: Dash Grants I-Frames for 0.2 Seconds

**Given** the player initiates a dash
**When** a damage source overlaps the player hitbox during the first 0.2 seconds of dash
**Then** no damage is applied to the player

**And When** the same damage source overlaps after 0.21 seconds
**Then** damage is applied normally

```
Test ID: TC-05
Category: Movement / Combat
Severity: P1 — Critical
Expected: PASS
```

---

### TC-06: Rewind Buffer Fills at 60fps

**Given** the game has been running in PlayMode for 5 seconds with no rewind activity
**When** the RewindManager buffer is inspected
**Then** it contains exactly 300 frames (5s × 60fps), within a tolerance of ±3 frames

```
Test ID: TC-06
Category: Rewind — Buffer
Severity: P0 — Blocker
Expected: PASS
```

---

### TC-07: Rewind Restores Player Position Within Tolerance

**Given** the player walks east for 3 seconds, recording their position at t=0
**When** the player activates rewind for 3 seconds
**Then** the player's final position is within ±0.05 Unity units of the t=0 recorded position

```
Test ID: TC-07
Category: Rewind — Accuracy
Severity: P0 — Blocker
Expected: PASS
```

---

### TC-08: Rewind Hard-Stops at Room Boundary Checkpoint

**Given** a RewindCheckpoint has been written at timestamp T for position P
**And** the player has been active for 10 seconds after the checkpoint
**When** the player rewinds for more than 10 seconds
**Then** rewind stops at timestamp T and the player is snapped to position P

```
Test ID: TC-08
Category: Rewind — Room Boundary
Severity: P0 — Blocker
Expected: FAIL (RoomTransitionController not yet implemented)
Status: Known failure — added to CI exclusion list until RoomManager is built
```

---

### TC-09: Irreversible Events Not Rolled Back

**Given** an [Irreversible] event flag (e.g., BossDefeated) is set at timestamp T
**When** the player rewinds to a timestamp before T
**Then** the [Irreversible] flag remains set (BossDefeated = true)

```
Test ID: TC-09
Category: Rewind — Irreversible Events
Severity: P0 — Blocker
Expected: FAIL (IrreversibleEventManager not yet implemented)
Status: Known failure — added to CI exclusion list until EventManager is built
```

---

### TC-10: Chrono Energy Depletes During Rewind

**Given** the player has a full Chrono Energy bar
**When** the player holds Rewind for 3 seconds
**Then** exactly 1 of 3 energy segments is depleted

```
Test ID: TC-10
Category: Rewind — Chrono Energy
Severity: P1 — Critical
Expected: PASS
```

---

### TC-11: Chrono Energy Regenerates After Delay

**Given** the player used 1 segment of Chrono Energy
**When** 8 seconds elapse with no rewind input
**Then** energy regeneration begins (segment fill starts)
**And When** a further 4 seconds elapse
**Then** the segment is fully restored

```
Test ID: TC-11
Category: Rewind — Chrono Energy
Severity: P1 — Critical
Expected: PASS
```

---

### TC-12: Rewind During Dash Cancels Dash

**Given** the player has initiated a dash
**When** the Rewind input is triggered during the dash
**Then** the dash animation and velocity spike are immediately cancelled
**And** the player's state is restored from the rewind buffer including i-frame timer

```
Test ID: TC-12
Category: Rewind — Interaction
Severity: P2 — High
Expected: PASS
```

---

## 4. Pass/Fail Criteria

### A smoke run PASSES when:
- All tests not in the Known Failures list have status PASS
- The Unity build compiles with zero errors
- Zero unhandled exceptions appear in the Unity Console during test execution
- Total runtime of the smoke suite is under 3 minutes

### A smoke run FAILS when:
- Any non-known-failure test fails
- The build fails to compile
- A NullReferenceException or unhandled exception fires during any test
- Suite runtime exceeds 3 minutes (indicates runaway coroutine or infinite loop)

---

## 5. Known Failing Tests (Blocked)

These tests are written and committed but are expected to fail until the specified system is implemented. They are excluded from the CI merge gate until unblocked.

| Test ID | Blocked By | Owner |
|---------|-----------|-------|
| TC-08 | RoomTransitionController not implemented | Architect |
| TC-09 | IrreversibleEventManager not implemented | Architect |

When these are implemented, the QA Engineer removes them from the exclusion list and they join the standard merge gate.

---

## 6. Regression Policy

### On a test failure in CI:
1. GitHub Actions posts a failing PR check with the test name and failure message.
2. The PR author is automatically requested for review fix.
3. The Architect is tagged on any P0 failure.
4. **The PR cannot be merged until all non-excluded tests pass.**

### On a known flaky test (intermittent failure):
1. QA Engineer must reproduce locally 3 times.
2. If confirmed flaky, the test is temporarily added to the exclusion list with a ticket number.
3. Fix must be merged within the same sprint.

### Regression discovered outside CI (manual testing, exploratory):
1. New bug is logged with reproduction steps and severity.
2. P0/P1 bugs block the next sprint demo unless explicitly accepted by the Lead.
3. A regression test covering the bug is added before the fix is merged.
