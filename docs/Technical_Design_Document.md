# Chrono-Core: Technical Design Document (TDD)

## 1. Overview
This document outlines the core architectural pillars necessary to achieve the performance, portability, and gameplay requirements of Chrono-Core. The game targets multiple consoles and PC, demanding strict performance overheads and resilient game loops.

## 2. Engine and Rendering
*   **Engine:** Unity LTS (Latest Long-Term Support version).
*   **Rendering Pipeline:** Universal Render Pipeline (URP).
    *   **Rationale:** URP provides out-of-the-box support for the 2D lighting and particle effects required for our visual style, while remaining highly performant on low-end hardware like the Nintendo Switch.
    *   **2D Setup:** The camera must be set to Orthographic and utilize the 2D Pixel Perfect Camera component to ensure assets scale cleanly to 1080p and 4K without artifacting or "shimmering".

## 3. Core Architecture: Time-Rewind
The Time-Rewind mechanic is the most technically complex feature and dictates the entire gameplay architecture.

### 3.1 `IRewindable` Interface
Every interactive entity (Player, Enemies, Moving Platforms, Projectiles) must implement the `IRewindable` interface.
*   **State Snapshotting (Memento Pattern):** Entities must capture their current state (Position, Velocity, Animation Frame, Health, State Machine enum) into a lightweight struct every *fixed frame*.
*   **Buffer Management:** The `RewindManager` will maintain a circular buffer of these states (e.g., storing the last 5 seconds of gameplay at 60 FixedUpdate ticks per second = 300 snapshots per entity).
*   **Frame Independence:** Logic updates must occur strictly within Unity's `FixedUpdate()` loop. Visual interpolation happens in `Update()`. This ensures the simulation is deterministic enough to rewind without visual jitter.

### 3.2 Input Handling (Command Pattern)
Player inputs cannot directly apply forces to the character controller.
*   Inputs must generate `Command` objects (e.g., `JumpCommand`, `AttackCommand`).
*   During a rewind, the system stops reading live input and instead reverses through the state buffer, overriding the `Command` stream to ensure the player's past actions are accurately represented or wiped as they travel backward.

## 4. World Streaming Architecture
To achieve "zero loading screens" across a Metroidvania map while maintaining 60-120 FPS, the game world cannot be loaded into memory simultaneously.

### 4.1 Chunk-Based Design
*   The world is divided into discrete "Rooms" or "Chunks."
*   Level designers must build rooms within predefined coordinate boundaries.
*   Rooms are saved as separate Prefabs or Sub-Scenes.

### 4.2 Asynchronous Loading (Addressables)
*   We will utilize the **Unity Addressables System**.
*   **Streaming Strategy:** 
    *   The active room is fully loaded.
    *   Adjacent rooms (rooms connected via doors or map transitions) are loaded asynchronously in the background.
    *   Non-adjacent rooms are unloaded from memory.
*   **Memory Budgets:** Strict memory profiling will be enforced per room to guarantee that streaming in an adjacent room does not cause a CPU spike or garbage collection stutter.
*   **Rewind Constraint:** We must test rewinding *accross* a chunk boundary in Phase 2 to ensure the `RewindManager` can hold pointers to unloaded objects safely, or if we need to cache states of recently unloaded chunks.

## 5. Performance and QA
*   **Target Frame Rate:** 60 FPS minimum (16.6ms frame time). 120 FPS on next-gen/PC (8.3ms frame time).
*   **Garbage Collection:** We must strictly adhere to zero-allocation gameplay loops. Object pooling is mandatory for projectiles, enemies, and particle effects to prevent GC spikes.
*   **CI/CD Pipeline:** A build server (e.g., Jenkins or GitHub Actions) will be configured to automatically trigger Unity Cloud Builds and run automated performance benchmark tests on target hardware.

## 6. Procedural Generation Constraints
*   **Allowed:** Spawning specific enemy variants, randomized loot drops from destructibles, and procedural noise for foliage sway or weather particles.
*   **Disallowed:** Procedural room layouts, randomized platform placement, or altering the critical path. The Metroidvania world design must remain static to guarantee solvability and tight pacing.
