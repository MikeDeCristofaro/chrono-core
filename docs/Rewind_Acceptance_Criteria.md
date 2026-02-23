# Rewind Acceptance Criteria: Chrono-Core
**Version:** 1.0
**Status:** Approved
**Author:** Lead Architect
**Target Platform:** PC (Steam)
**Engine:** Unity 6 LTS

---

## 1. Overview

The Rewind Mechanic is the core gameplay pillar of Chrono-Core. It allows the player to roll back the game state to a previous point in time to undo mistakes, evade damage, or manipulate environment state. From the player's perspective: hold the Rewind input to reverse time up to 9 seconds. Enemies, projectiles, and destructibles roll back with the player. Permanent events do not. The mechanic is governed by the Chrono Energy resource and implemented via a frame-based circular buffer updated every `FixedUpdate` tick.

---

## 2. Chrono Energy System

| Attribute | Specification |
|-----------|--------------|
| Capacity | 3 segments |
| Seconds per segment | 3 seconds |
| Max rewind duration | 9 seconds (3 segments full) |
| Regeneration delay | 8 seconds after last rewind input released |
| Regeneration rate | 1 full segment per 4 seconds (after delay) |
| Auto-stop | Immediate when energy reaches 0 |

### Acceptance Criteria — Chrono Energy

**AC-CE-01: Segment Drain Rate**
- Given the player has a full Chrono Energy bar (3 segments)
- When the player holds the Rewind input
- Then segments drain at a rate of 1 segment per 3 seconds of active rewind

**AC-CE-02: Auto-Stop on Empty**
- Given Chrono Energy reaches 0
- When the player is actively rewinding
- Then time must resume at 1.0x scale immediately with no further rollback

**AC-CE-03: Regeneration Delay**
- Given any Chrono Energy was consumed
- When 8 seconds elapse with no rewind input held
- Then energy begins regenerating

**AC-CE-04: Regen Rate**
- Given energy regeneration has started
- When 4 seconds elapse
- Then one full segment is restored

**AC-CE-05: Regen Interrupted**
- Given energy is regenerating
- When the player activates rewind before a segment fully restores
- Then regeneration resets and the 8-second delay begins again from zero

**AC-CE-06: Room Boundary Hard Stop**
- Given the player is rewinding
- When the rewind buffer reaches the timestamp of the current room's entry point
- Then rewind stops immediately and the player is snapped to the room entry position

---

## 3. IRewindable Contract

Every entity participating in the rewind system must implement `IRewindable`.

```csharp
/// <summary>
/// Implement on any entity that participates in the rewind system.
/// CaptureState() is called by RewindManager every FixedUpdate tick.
/// RestoreState() is called during active rewind playback.
/// </summary>
public interface IRewindable
{
    /// <summary>Called at the START of FixedUpdate. Pushes current state to buffer.</summary>
    void CaptureState();

    /// <summary>Applies the given snapshot to this entity's state.</summary>
    void RestoreState(RewindSnapshot snapshot);

    /// <summary>Returns a stable unique ID for buffer tracking across scene changes.</summary>
    string GetRewindableId();
}
```

**Strict Rules:**
- `CaptureState()` must always be called at the **start** of `FixedUpdate`, before physics resolution.
- `RestoreState()` must apply position within **±0.05 Unity units** positional tolerance.
- Entities that are spawned mid-session (enemies, projectiles) must register with `RewindManager` in `Awake()`.
- Entities destroyed during rewind playback must be re-instantiated by `RewindManager` if they appear in the buffer.

---

## 4. Rewind Buffer Spec

| Property | Value |
|----------|-------|
| Buffer type | Circular array (ring buffer) |
| Frame capacity | 540 frames |
| Target framerate | 60 Hz (FixedUpdate) |
| Max coverage | 9.0 seconds |
| Eviction policy | Oldest frame overwritten when buffer is full |
| Empty buffer behavior | Rewind input has no effect; no error thrown |

### Timing Rule
Snapshot is captured at the **start** of `FixedUpdate` (before `Physics2D` resolution), not the end. This prevents the 1-frame velocity restoration pop (Bug CHR-001, resolved).

---

## 5. Entity Coverage Matrix

| Entity Type | Rewindable | Properties Captured | Notes |
|-------------|:----------:|---------------------|-------|
| PlayerController | YES | Position, velocity, health, dash state, i-frame timer | Gravity scale 3.2 |
| Enemy (all types) | YES | Position, AI state index, health, active status | De-spawned enemies re-spawn on rewind |
| Projectile | YES | Position, velocity vector, TTL, damage value | Pooled — return to pool on rewind past spawn |
| Destructible object | YES | Mesh/sprite state (intact / broken), collision layer | |
| Boss — alive/fighting | YES | Position, health, phase, attack pattern index | |
| Boss Death event | **NO** | — | [Irreversible] — prevents progression soft-lock |
| Room unlock / door | **NO** | — | [Irreversible] |
| Dialogue trigger | **NO** | — | [Irreversible] |
| Cutscene trigger | **NO** | — | [Irreversible] |
| Chrono Energy bar | YES | Segment count, regen timer | Rewinds with player |
| Particle systems | NO | — | Visual only; gameplay-critical telegraphs excepted |

---

## 6. Edge Case Definitions

### EC-01: Room Boundary
The `RewindManager` holds a `RewindCheckpoint` written by `RoomTransitionController` on every room entry. This checkpoint stores the room entry timestamp and entry position. Rewind cannot proceed past this checkpoint. The player is snapped to entry position. Chrono Energy at the room entry is also restored.

### EC-02: Audio During Rewind
- Music: pitch-shift down linearly during rewind (target: one octave down at max rewind speed). Restored to normal pitch on rewind stop.
- SFX: play in reverse at **0.5x volume** during rewind. No new SFX fire during rewind.
- Implementation: `AudioRewindController` subscribes to `RewindManager.OnRewindStart` / `OnRewindStop`.

### EC-03: Physics Drift Tolerance
Positional drift of up to **±0.05 Unity units** (~3 cm) is acceptable at 60fps due to floating-point accumulation. Tests must assert within this tolerance, not exact equality.

### EC-04: Rewind During Dash
- If the player is mid-dash when rewind starts, the dash is cancelled.
- The buffer snapshot at the rewind target timestamp contains the player's state before the dash began.
- The remaining i-frame duration at the restore point is also restored from the snapshot.

### EC-05: Rewind While Taking Damage
- Damage applied during the rewind window is undone.
- Player health is restored to the value stored in the snapshot at the rewind target timestamp.
- The hit-stun animation is cancelled immediately on rewind start.

### EC-06: Rapid Inputs Before Rewind
- All input commands are captured via the Command Pattern.
- The buffer records the resulting state, not the inputs.
- Rapid direction changes immediately before a rewind are correctly handled because snapshot timing is at FixedUpdate start (pre-physics), eliminating the EC-06 pop.

### EC-07: Empty Buffer
- If `< 6 frames` of data exist in the buffer (less than 0.1 seconds), the rewind input is silently ignored.
- No error, no visual feedback, no energy consumption.

### EC-08: Rewind Past Entity Spawn
- If rewind reaches a timestamp before an enemy or projectile was spawned, it is deactivated (returned to pool / disabled).
- If rewind stops after that timestamp again, it is re-activated with state from that frame.

---

## 7. Acceptance Criteria — Full Test Suite (Given/When/Then)

### Movement & Buffer

**AC-01: Buffer Fills at 60fps**
- Given the game has been running for 10 seconds
- When the buffer is inspected
- Then it contains exactly 540 frames (9 seconds at 60Hz), with older frames evicted

**AC-02: State Precision Within Tolerance**
- Given the player moves and then activates rewind targeting 3 seconds ago
- When `RestoreState()` is called
- Then the player's position is within ±0.05 Unity units of the recorded snapshot at that timestamp

**AC-03: Buffer Empty — No Effect**
- Given the game has been running for less than 0.1 seconds
- When the player activates rewind
- Then nothing happens and no Chrono Energy is consumed

### Entities

**AC-04: Enemy Rolls Back**
- Given an enemy moved from position A to position B over 4 seconds
- When the player rewinds 4 seconds
- Then the enemy is at position A with its health and AI state restored

**AC-05: Projectile Rolls Back**
- Given a projectile was fired 2 seconds ago
- When the player rewinds 3 seconds
- Then the projectile is returned to its pool (pre-spawn) and no longer active

**AC-06: Destructible Restores**
- Given a destructible crate was broken 5 seconds ago
- When the player rewinds 6 seconds
- Then the crate is restored to its intact mesh and collision layer

### Irreversible Events

**AC-07: Boss Death Not Reversed**
- Given the boss defeat flag is set
- When the player rewinds to a timestamp before the boss died
- Then the boss remains in its death state; the victory condition persists

**AC-08: Door Unlock Not Reversed**
- Given a room unlock door is open
- When the player rewinds past the unlock moment
- Then the door remains open

### Room Boundary

**AC-09: Hard Stop At Room Entry Point**
- Given the player entered Room 2 from Room 1 five seconds ago
- When the player rewinds more than 5 seconds
- Then rewind stops and the player is positioned at the Room 1 exit / Room 2 entry point

### Audio

**AC-10: Music Pitch Shift On Rewind Start**
- Given normal gameplay music is playing at 1.0x pitch
- When rewind starts
- Then music pitch shifts down within 1 frame; no audio stutter

**AC-11: SFX Volume During Rewind**
- Given an SFX is playing
- When rewind starts
- Then the SFX plays in reverse at 0.5x volume

---

## 8. Out of Scope

- Rewinding of purely visual / cosmetic particle systems (no gameplay information conveyed)
- Rewinding of transient UI animations or menu state
- Rewinding of save-game state or persistent progression
- Non-PC platform considerations (Steam Deck may be revisited post-launch)
- Networked / multiplayer rewind (not applicable)
