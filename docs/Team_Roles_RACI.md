# Chrono-Core: Team Roles & RACI Chart

**Date:** 2026-02-22  
**Status:** Draft — Roles partially filled, unassigned roles flagged

---

## Defined Functional Roles

| Role | Responsibilities | Assigned |
|---|---|---|
| **Product Owner (PO)** | Game vision, feature prioritization, GDD approval | 🟢 Filled |
| **Senior Project Manager (PM)** | Sprint planning, milestone tracking, team coordination, JIRA management | 🟢 Filled |
| **Senior Game Dev Architect** | Engine architecture, TDD, technical standards, code reviews | 🟢 Filled |
| **Lead Gameplay Programmer** | Player controller, combat system, Chrono Recall system | 🔴 Unassigned |
| **Gameplay Programmer** | Enemy AI, interactive objects, IRewindable implementations | 🔴 Unassigned |
| **Systems Programmer** | World Streaming (Addressables), UI/UX systems, Input system | 🔴 Unassigned |
| **DevOps / Build Engineer** | CI/CD pipeline, Unity Cloud Builds, performance benchmarking | 🔴 Unassigned |
| **Lead Artist (2D Pixel Art)** | Art style guide, character sprites, tilesets | 🔴 Unassigned |
| **Environment Artist** | Biome tilework, backgrounds, parallax layers | 🔴 Unassigned |
| **VFX Artist** | Particle systems (Unity URP), hit effects, Chrono Recall VFX | 🔴 Unassigned |
| **Composer** | Full game soundtrack (chiptune + orchestral hybrid) | 🔴 Unassigned |
| **Audio Designer (SFX)** | Sound effects, FMOD integration, adaptive audio | 🔴 Unassigned |
| **Level Designer** | Room/chunk layout, biome flow, enemy placement blueprints | 🔴 Unassigned |
| **Narrative Designer / Writer** | Dialogue, lore, audio logs, localization management | 🔴 Unassigned |
| **QA Lead** | Test planning, bug tracking, console certification coordination | 🔴 Unassigned |

---

## RACI Matrix — Key Deliverables

> **R** = Responsible (does the work) | **A** = Accountable (final approval) | **C** = Consulted | **I** = Informed

| Deliverable | PO | PM | Architect | Lead Programmer | Lead Artist | QA Lead |
|---|---|---|---|---|---|---|
| Game Design Document | **A** | I | C | C | C | I |
| Technical Design Document | C | I | **A/R** | R | I | I |
| Art Style Guide | **A** | I | I | I | **R** | I |
| Rewind R&D Prototype | I | I | **A** | **R** | I | C |
| Vertical Slice Build | **A** | C | C | **R** | R | **R** |
| Level Design (each biome) | C | I | I | I | C | C |
| CI/CD Pipeline Setup | I | I | **A** | C | I | C |
| Boss Fight Implementation | C | I | C | **R** | C | **R** |
| Console Certification Builds | I | **A** | C | R | I | **R** |
| Localization | C | **A** | I | I | I | I |

---

## Staffing Gaps & Hiring Priority

| Priority | Role | Rationale |
|---|---|---|
| 🔴 Immediate | **Lead Gameplay Programmer** | Phase 1 R&D cannot begin without this role. The Rewind prototype is blocked. |
| 🔴 Immediate | **Level Designer** | GDD complete — level layouts can begin in parallel with R&D. |
| 🟡 Before Phase 2 | **Gameplay Programmer** | Enemy AI needed for Vertical Slice. |
| 🟡 Before Phase 2 | **Lead Artist (2D Pixel Art)** | Art style must be finalized before Vertical Slice assets are created. |
| 🟡 Before Phase 2 | **Systems Programmer** | World Streaming must be built and tested in Phase 2. |
| 🟢 Before Phase 3 | All remaining roles | Audio, VFX, Narrative, QA — needed before full Production begins. |
