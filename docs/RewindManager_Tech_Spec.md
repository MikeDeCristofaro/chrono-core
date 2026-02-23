# RewindManager Technical Implementation Spec
**Version:** 1.0
**Author:** Lead Gameplay Programmer
**Status:** Ready for Implementation
**Engine:** Unity 6 LTS | **Platform:** PC (Steam)

---

## 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                        RewindManager                        │
│  - Circular buffer of RewindFrame[540]                      │
│  - Maintains list of IRewindable registrants                │
│  - Owns RewindCheckpoint (room boundary safe point)         │
│  - Drives CaptureFrame() and TickRewind() each FixedUpdate  │
└──────────────┬───────────────────────────┬──────────────────┘
               │ Captures / Restores       │ Checkpoint writes
    ┌──────────▼──────────┐    ┌───────────▼──────────────┐
    │   IRewindable        │    │  RoomTransitionController│
    │   (interface)        │    │  Calls OnRoomEntry()     │
    └──────┬───────────────┘    └──────────────────────────┘
           │ Implemented by
   ┌───────┴──────────────────────────────────────┐
   │  PlayerController  EnemyBase  Projectile     │
   │  DestructibleObject  ...                     │
   └──────────────────────────────────────────────┘

    RewindFrame
    └── Dictionary<string, RewindSnapshot>
        (one snapshot per registered IRewindable, keyed by GetRewindableId())

    RewindSnapshot (struct)
    └── position, velocity, health, animStateHash,
        dashTimer, iFrameTimer, isActive, customData
```

---

## 2. RewindSnapshot Struct

```csharp
/// <summary>
/// Immutable value-type snapshot of a single entity's state at one FixedUpdate tick.
/// Kept as a struct to avoid GC allocations in the circular buffer.
/// </summary>
[System.Serializable]
public struct RewindSnapshot
{
    // Transform
    public Vector2 Position;
    public Vector2 Velocity;

    // Vitals
    public int Health;
    public bool IsActive;          // False if entity was not yet spawned or already destroyed

    // Animation
    public int AnimatorStateHash;  // Animator.GetCurrentAnimatorStateInfo(0).fullPathHash
    public float AnimatorNormTime; // Normalized time within current state

    // Player-specific (ignored by non-player entities)
    public float DashCooldownTimer;
    public float IFrameTimer;
    public float ChronoEnergyAmount;

    // Extensibility — entity-specific extra data (AI state index, etc.)
    public int CustomIntA;         // e.g. AI state machine index
    public float CustomFloatA;     // e.g. enemy patrol timer
    public bool CustomBoolA;       // e.g. shield active
}
```

**Rules:**
- Must remain a `struct` (not class) to avoid heap allocations per frame.
- All fields are value types. No object references.
- `CustomInt/Float/BoolA` used for entity-specific overflow. Document usage in the implementing class.

---

## 3. IRewindable Interface

```csharp
/// <summary>
/// Implement on any MonoBehaviour that participates in the rewind system.
/// Register with RewindManager in Awake(). Deregister in OnDestroy().
/// </summary>
public interface IRewindable
{
    /// <summary>
    /// Called at the START of FixedUpdate by RewindManager — before physics resolution.
    /// Must push the entity's current state as a RewindSnapshot into the provided frame slot.
    /// </summary>
    RewindSnapshot CaptureState();

    /// <summary>
    /// Called during active rewind playback each FixedUpdate.
    /// Must restore entity state from the provided RewindSnapshot.
    /// Position tolerance: ±0.05 Unity units.
    /// </summary>
    void RestoreState(RewindSnapshot snapshot);

    /// <summary>
    /// Returns a stable, unique string ID. Must be consistent across scene loads.
    /// Recommended format: "{EntityType}_{GUID}" set in the Unity Inspector.
    /// </summary>
    string GetRewindableId();
}
```

---

## 4. RewindManager Class Spec

### Public API

```csharp
public class RewindManager : MonoBehaviour
{
    // ── State ────────────────────────────────────────────
    public bool IsRewinding { get; private set; }
    public float BufferedSeconds { get; private set; }

    // ── Events ───────────────────────────────────────────
    public event Action OnRewindStart;   // AudioRewindController subscribes
    public event Action OnRewindStop;

    // ── Registration ─────────────────────────────────────
    public void Register(IRewindable entity);    // Called by entities in Awake()
    public void Deregister(IRewindable entity);  // Called in OnDestroy()

    // ── Room Boundary ────────────────────────────────────
    public void OnRoomEntry(Vector2 entryPosition); // Called by RoomTransitionController

    // ── Input-Driven (called by InputHandler) ────────────
    public void StartRewind();
    public void StopRewind();
}
```

### Private Data Structures

```csharp
// Circular buffer — fixed-size array, no heap allocations after init
private RewindFrame[] _buffer = new RewindFrame[BUFFER_CAPACITY]; // 540
private int _head = 0;           // Points to next write slot
private int _count = 0;          // How many valid frames are stored

// Registered entities
private List<IRewindable> _rewindables = new List<IRewindable>();

// Room boundary checkpoint
private RewindCheckpoint _currentCheckpoint;

private const int BUFFER_CAPACITY = 540;
private const int ROOM_BOUNDARY_FRAME_INDEX = -1; // Sentinel stored in buffer as marker
```

### Key Method Pseudocode

#### CaptureFrame() — called every FixedUpdate when NOT rewinding
```
function CaptureFrame():
    frame = new RewindFrame()
    for each entity in _rewindables:
        snapshot = entity.CaptureState()
        frame.snapshots[entity.GetRewindableId()] = snapshot

    _buffer[_head] = frame
    _head = (_head + 1) % BUFFER_CAPACITY
    _count = min(_count + 1, BUFFER_CAPACITY)
    BufferedSeconds = _count / 60.0f
```

#### TickRewind() — called every FixedUpdate when IS rewinding
```
function TickRewind():
    if _count <= 0:
        StopRewind()
        return

    // Check room boundary
    currentFrame = _buffer[(_head - 1 + BUFFER_CAPACITY) % BUFFER_CAPACITY]
    if currentFrame.IsCheckpointFrame:
        StopRewind()
        SnapPlayerToCheckpointPosition()
        return

    // Pop frame
    _head = (_head - 1 + BUFFER_CAPACITY) % BUFFER_CAPACITY
    _count--
    BufferedSeconds = _count / 60.0f

    frame = _buffer[_head]
    for each entity in _rewindables:
        if frame.snapshots.ContainsKey(entity.GetRewindableId()):
            entity.RestoreState(frame.snapshots[entity.GetRewindableId()])
        else:
            // Entity didn't exist at this frame — deactivate it
            entity.gameObject.SetActive(false)
```

#### StartRewind()
```
function StartRewind():
    if ChronoEnergySystem.CurrentEnergy <= 0: return
    if _count < 6: return  // Buffer too small (< 0.1s) — silent fail

    IsRewinding = true
    Physics2D.simulationMode = SimulationMode2D.Script  // Freeze physics
    OnRewindStart?.Invoke()
```

#### StopRewind()
```
function StopRewind():
    IsRewinding = false
    Physics2D.simulationMode = SimulationMode2D.FixedUpdate  // Resume physics
    ChronoEnergySystem.StartRegenCountdown()
    OnRewindStop?.Invoke()
```

#### OnRoomEntry(Vector2 entryPosition)
```
function OnRoomEntry(entryPosition):
    // Write a checkpoint marker at the current buffer head
    checkpointFrame = new RewindFrame()
    checkpointFrame.IsCheckpointFrame = true
    checkpointFrame.CheckpointPosition = entryPosition

    _buffer[_head] = checkpointFrame
    _head = (_head + 1) % BUFFER_CAPACITY
    _count = min(_count + 1, BUFFER_CAPACITY)
```

---

## 5. Circular Buffer Implementation Notes

The buffer is a fixed-size array of `RewindFrame[540]`. It is pre-allocated in `Awake()` and never resized — zero GC allocations during gameplay.

```
Buffer visualization (capacity = 10 for illustration):

  Index:  [0][1][2][3][4][5][6][7][8][9]
  Data:   [ ][ ][F][F][F][F][F][F][ ][ ]
                 ↑                 ↑
               tail              head

  - head = next write position
  - When head reaches capacity, it wraps to 0 (modulo)
  - When _count == BUFFER_CAPACITY, writing overwrites the oldest frame (tail advances)
  - Reading during rewind: walk backward from (head-1) toward (head - count)
```

**RewindFrame struct:**
```csharp
public struct RewindFrame
{
    public Dictionary<string, RewindSnapshot> Snapshots;
    public bool IsCheckpointFrame;
    public Vector2 CheckpointPosition;
}
```

> **Note:** `Dictionary` inside a struct means the buffer itself has object references. For Phase 1 this is acceptable. Phase 2 optimization target: replace with a fixed-key array using entity index instead of string key to eliminate dictionary overhead.

---

## 6. Entity Registration Pattern

All `IRewindable` entities self-register in `Awake()`:

```csharp
public class PlayerController : MonoBehaviour, IRewindable
{
    private void Awake()
    {
        RewindManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        RewindManager.Instance.Deregister(this);
    }

    public string GetRewindableId() => "Player_Singleton";

    public RewindSnapshot CaptureState()
    {
        return new RewindSnapshot
        {
            Position       = _rb.position,
            Velocity       = _rb.velocity,
            Health         = _health,
            IsActive       = gameObject.activeSelf,
            AnimatorStateHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash,
            AnimatorNormTime  = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime,
            DashCooldownTimer = _dashCooldownTimer,
            IFrameTimer       = _iFrameTimer,
            ChronoEnergyAmount = ChronoEnergySystem.CurrentEnergy
        };
    }

    public void RestoreState(RewindSnapshot s)
    {
        _rb.position  = s.Position;
        _rb.velocity  = s.Velocity;
        _health       = s.Health;
        _dashCooldownTimer = s.DashCooldownTimer;
        _iFrameTimer       = s.IFrameTimer;
        _animator.Play(s.AnimatorStateHash, 0, s.AnimatorNormTime);
        ChronoEnergySystem.SetEnergy(s.ChronoEnergyAmount);
    }
}
```

**For dynamically spawned entities (enemies, projectiles):**
- Register in `Awake()` — entity is registered even before it's visible
- `IsActive = false` in `CaptureState()` if the entity hasn't spawned yet in the timeline
- `RewindManager` respects `IsActive = false` by calling `SetActive(false)` instead of `RestoreState()`

---

## 7. [Irreversible] Attribute Usage

```csharp
/// <summary>
/// Tag events that must not be rolled back by rewind.
/// Applied to event-raising methods in EventManager.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method)]
public class IrreversibleAttribute : System.Attribute { }

// Example usage:
public class EventManager : MonoBehaviour
{
    [Irreversible]
    public void RaiseBossDefeated(string bossId)
    {
        _defeatedBosses.Add(bossId);
        // This method is tagged — RewindManager never captures or rolls back _defeatedBosses
    }
}
```

**How RewindManager respects this:**
- `_defeatedBosses`, `_unlockedRooms`, `_triggeredCutscenes` live in `EventManager`, not in any `IRewindable`.
- `RewindManager` has no reference to `EventManager`. The isolation is the protection — these states are never in the circular buffer.

---

## 8. Known Risks and Open Questions

| # | Risk / Question | Severity | Owner |
|---|----------------|----------|-------|
| 1 | Dictionary inside RewindFrame causes GC pressure at 60fps | Medium | Programmer — Phase 2 optimization |
| 2 | Re-instantiating enemies on rewind past their spawn: object pooling required to avoid instantation cost | High | Architect — entity pool system needed before enemy work |
| 3 | Animator state restoration may cause 1-frame visual pop on blend tree transitions | Low | QA to flag during Phase 2 playtesting |
| 4 | How does RewindManager handle entities spawned by other entities (e.g., enemy drops a projectile mid-rewind)? | Medium | Open — needs Architect decision before enemy implementation |

---

## 9. Implementation Order

Build in this order for fastest testable prototype:

1. **`RewindSnapshot` struct** — no dependencies, pure data
2. **`IRewindable` interface** — no dependencies
3. **`RewindManager` skeleton** — registration, empty buffer, `CaptureFrame()` only
4. **`PlayerController` implements `IRewindable`** — can now test TC-06, TC-07
5. **`ChronoEnergySystem`** — standalone, drives energy and triggers StartRewind / StopRewind
6. **`StartRewind()` / `StopRewind()` / `TickRewind()`** — full rewind playback
7. **`RewindCheckpoint` + `OnRoomEntry()`** — room boundary support (TC-08)
8. **`IrreversibleAttribute` + `EventManager`** — irreversible event isolation (TC-09)
9. **`AudioRewindController`** — subscribes to events, drives pitch shift and SFX reverse
10. **Enemy + Projectile `IRewindable` implementations** — enemy rewind support
