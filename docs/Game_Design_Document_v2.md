# Modern Retro Console Game - Game Design Document v2

## 1. High-Level Concept
**"Chrono-Core"**
A fast-paced, 16-bit style action-platformer that blends classic retro aesthetics (SNES/Genesis era) with modern game design sensibilities (fluid movement, forgiving checkpoints, light procedural elements, and physics-based interactions). 

*   **Genre:** Action Platformer / Metroidvania-lite
*   **Visual Style:** High-fidelity pixel art (16-bit color palette constraint, but with modern widescreen aspect ratio, dynamic lighting, and particle effects leveraging the Unity URP).
*   **Audio Style:** Chiptune music augmented with modern orchestral instrumentation and high-quality sound effects.

## 2. Requirements

### 2.1 Game Requirements (Features)
*   **Core Mechanics:** 
    *   Tight, responsive controls (running, jumping, dashing, wall-sliding).
    *   Combat system involving melee combos and a time-manipulation mechanic (rewind short bursts to solve puzzles or dodge attacks).
*   **Level Design:** Non-linear exploration with distinct biomes.
*   **Procedural Scope:** Restricting procedural generation to enemy spawns and minor environmental decor. Map layout and room design must be hand-crafted to avoid Metroidvania soft-locks.
*   **Accessibility:** Remappable controls, colorblind modes, adjustable game speed.
*   **Modern Touches:** Auto-saving, zero loading screens within zones, built-in speedrun timer.

### 2.2 Technical Requirements
*   **Engine:** Unity (Universal Render Pipeline). Required for native console portal out-of-the-box, ensuring high-fidelity dynamic lighting and particles within a 2D environment.
*   **Target Platforms:** PC (Steam), Switch, PlayStation 5, Xbox Series X/S.
*   **Performance:** 60 FPS minimum on all target platforms, aiming for 120 FPS on next-gen/PC.
*   **Resolution:** Native 1080p rendering (pixel perfect scaling) up to 4K.
*   **Architecture:** Component-based entity system for easy iteration on enemies and interactive objects. Custom `IRewindable` architecture required for Time-Rewind.
*   **Input Handling:** Support for all modern controllers (XInput, DirectInput, DualSense haptics) via Unity's New Input System.

## 3. Overall Plan & Development Phases

### Phase 1: Pre-Production & R&D (Weeks 1-6)
- [ ] Finalize Game Design Document (GDD) and Technical Design Document (TDD).
- [ ] Establish art style guide and technical constraints.
- [ ] Set up Unity project, version control (Git), and CI/CD pipelines.
- [ ] **Core Architecture & Rewind Prototype R&D:** Dedicated 2-week sprint to prove out the Time-Rewind state management and decouple visual rendering from logic updates.
- [ ] Develop a grey-box prototype focusing solely on core mechanics (movement and combat).

### Phase 2: Vertical Slice & World Streaming (Weeks 7-14)
- [ ] Implement robust player controller.
- [ ] Create one fully polished level consisting of 2-3 chunked rooms.
- [ ] **World Streaming Test:** Prove out the chunk-based loading architecture across room boundaries without dropping frames or breaking the rewind simulation.
- [ ] Integrate foundational art (character sprites, tileset) and music.
- [ ] Implement basic UI/UX menus.
- [ ] Internal playtesting and iteration on game feel.

### Phase 3: Production (Months 4-10)
- [ ] Level design execution for all remaining biomes.
- [ ] Boss fights and enemy AI design and implementation.
- [ ] Story and localized text integration.
- [ ] Audio design (SFX and full soundtrack).

### Phase 4: Polish & QA (Months 11-12)
- [ ] Bug hunting and performance optimization via automated profiling.
- [ ] Console certification preparation.
- [ ] Accessibility feature pass.
- [ ] Marketing asset creation (trailers, screenshots).
