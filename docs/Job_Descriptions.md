# Chrono-Core — Open Roles: Job Descriptions
**Prepared by:** Senior Game Dev Architect  
**For:** Senior Project Manager → HR / Recruiting  
**Date:** 2026-02-22  
**Hiring Priority:** 🔴 Immediate — Phase 1 R&D is blocked without these roles

---

## Role 1: Lead Gameplay Programmer

### About the Role
We're building *Chrono-Core* — a modern retro Metroidvania action-platformer in Unity with a flagship time-rewind mechanic. As Lead Gameplay Programmer, you'll own our most technically complex system from day one: the **Chrono Recall (time-rewind)** architecture, as well as the core player controller. This is a hands-on lead role — you'll write production code, set technical standards, and guide junior programmers as the team scales.

### What You'll Own
- **Chrono Recall System:** Design and implement the `IRewindable` interface, Memento-pattern state snapshotting, and Command-pattern input decoupling.
- **Player Controller:** Fluid, responsive movement — running, variable jumping, dashing, wall-sliding — with deterministic physics (FixedUpdate-based logic, visual interpolation in Update).
- **Technical Standards:** Define zero-allocation coding practices, object pooling patterns, and maintain 60+ FPS across all target platforms.
- **Cross-team Collaboration:** Work directly with the Architect on architecture decisions and with the Level Designer on gameplay-feel iteration.

### Required Experience
- 4+ years of professional game development experience
- 2+ shipped titles (at least one 2D game preferred)
- Expert-level Unity C# — especially physics, custom character controllers, and finite state machines
- Strong understanding of memory management and GC avoidance in Unity
- Experience with complex game systems (e.g., save states, rewind, physics simulations)

### Nice to Have
- Experience with Unity Addressables or streaming systems
- Portfolio featuring a time-manipulation or physics-puzzle mechanic
- Console development experience (Switch, PS5, or Xbox)

### Reporting To
Senior Game Dev Architect

---

## Role 2: Level Designer

### About the Role
You'll be the spatial storyteller of *Chrono-Core*, crafting the hand-designed rooms and biome layouts that make our Metroidvania world feel alive, fair, and endlessly replayable. This is a designer-with-a-dev-mindset role — you'll work inside Unity, respect our strict chunk-memory budgets, and collaborate closely with the programmer and art teams to bring each of our five biomes to life.

### What You'll Own
- **Room / Chunk Design:** Hand-craft all rooms within the predefined chunk coordinate grid (no procedural layouts — everything is authorial and intentional).
- **Metroidvania Flow:** Map the global lock-and-key progression — ensuring no soft-locks, that each ability gate is clearly telegraphed, and that exploration always rewards curiosity.
- **Enemy Placement Blueprints:** Specify enemy type, spawn zone, and patrol paths (within defined procedural spawn pools) for each room.
- **Memory Budget Adherence:** Work within per-room asset budgets defined by the Engineering team to support seamless background streaming.
- **Iteration:** Participate in playtests and own the iteration cycle on room feel, pacing, and difficulty curve.

### Required Experience
- 3+ years of professional level design experience
- At least one shipped Metroidvania, platformer, or exploration-driven game
- Proficiency in Unity (or willingness to ramp up quickly — this role works directly in the engine)
- Strong grasp of flow, pacing, and spatial readability in 2D environments
- Ability to document designs clearly (block diagrams, GDD annotations, enemy placement sheets)

### Nice to Have
- Experience with Tiled, LDtk, or custom tilemap editors
- Familiarity with game feel concepts (coyote time, jump buffering, etc.)
- Passion for classic Metroidvania titles (Super Metroid, Hollow Knight, Ori)

### Reporting To
Senior Game Dev Architect (with close collaboration with the Product Owner on narrative and thematic direction)

---

*Both roles are full-time. Remote-friendly. Contractor engagement considered for Level Designer if timeline demands it.*
