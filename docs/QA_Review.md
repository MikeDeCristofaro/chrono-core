# Chrono-Core — Senior QA Review
**Date:** 2026-02-22  
**Reviewer:** Senior QA  
**Documents Reviewed:**  
- PO Plan: [Original Requirements (Core-Vanguard)](file:///c:/Users/miked/.gemini/antigravity/brain/6b789561-b631-497c-84d0-423162366938/implementation_plan.md)  
- Architect Plan: [GDD v2](file:///c:/Users/miked/.gemini/antigravity/playground/astral-apogee/docs/Game_Design_Document_v2.md)  
- Architect Plan: [Technical Design Document](file:///c:/Users/miked/.gemini/antigravity/playground/astral-apogee/docs/Technical_Design_Document.md)

---

## Executive Summary

The Chrono-Core design is ambitious and technically coherent at a high level. The Architect did excellent foundational work defining the `IRewindable` architecture and the chunk-streaming strategy. However, as Senior QA I've identified **five high-risk areas** that, left unaddressed, could cause significant test failures, re-work, or certification failure late in the project. I'm documenting these now so the PO and Architect can refine the plan before we get deep into production.

---

## 🔴 Risk 1: The Time-Rewind Mechanic is Untestable as Currently Spec'd

This is my biggest concern. The TDD describes a sophisticated state-snapshotting system (`IRewindable`, Memento Pattern, Command Pattern, circular buffer), but **there is no definition of what "correct rewind" looks like from a player-observable standpoint.**

### Specific challenges:
- **No acceptance criteria exist.** How many seconds can the player rewind? Does rewinding cost a resource? Does it affect enemies? Does it revert damage taken? We cannot write a test case without this.
- **Physics determinism is assumed but not guaranteed.** The TDD mandates `FixedUpdate()` for logic, but Unity's physics engine has known non-determinism issues (floating-point accumulation, Rigidbody interpolation modes). A rewind to "5 seconds ago" may land the player in a slightly wrong position and we need a tolerance spec.
- **Edge cases are undefined.** What happens when a player rewinding crosses a room boundary? The TDD flags this as something to test in Phase 2 but offers no proposed solution. This could be a hard blocker.
- **Audio is not mentioned.** Does music/SFX rewind? Does it just stop? A jarring audio experience is a day-one review bomb risk.

> [!CAUTION]
> I will block a QA sign-off on the Phase 1 Rewind Prototype without a written acceptance criteria doc. The Architect needs to define pass/fail behavior for the 10+ edge cases before a single test can be written.

### Questions for PO & Architect:
1. What is the exact duration and resource cost of the rewind? (PO decision)
2. What is the expected behavior when rewinding through a room transition? (Architect decision — the TDD notes this as an open problem)
3. Does rewind affect enemy state, projectiles, and destructibles? (PO/GDD decision)

---

## 🔴 Risk 2: Multi-Platform Certification Is Underestimated in the Timeline

The GDD v2 lists PC (Steam), Switch, PS5, and Xbox Series X/S as target platforms. Console certification (Sony Lotcheck, Nintendo LOT, Microsoft MXDC) is a lengthy, iterative process that typically requires **2-4 separate submission cycles**, taking 4–8 weeks each. The current schedule allocates only **Months 11-12 (Phase 4)** for "Polish & QA," with console certification prep listed as a single line item.

### Specific challenges:
- **Technical Requirements Checklists (TRCs) must be coded against from Day 1.** Sony, Nintendo, and Microsoft each have extensive platform requirements (save system behavior, controller disconnection handling, suspend/resume behavior, parental controls, accessibility) that are not retrofittable in 2 months.
- **Switch performance is a wildcard.** The TDD acknowledges Switch as "low-end hardware" and URP as the mitigation. However, zero-allocation gameplay loops and Addressable streaming need to be validated on *actual Switch hardware* by at least Phase 3, not Phase 4.
- **DualSense haptics add integration overhead.** The input spec mentions DualSense haptics — this requires a separate testing track and Sony cert validation.

> [!WARNING]
> Console certification prep needs to begin in Phase 3, not Phase 4. I recommend the Architect define a Platform Compatibility Matrix by Phase 2 and set up dedicated build targets for each console before production begins.

### Questions for Architect:
4. Is there a plan to obtain dev kits for Switch, PS5, and Xbox early enough for Phase 3 performance testing?
5. Has the team reviewed TRC/LotCheck requirements yet? There are often 200+ platform-specific test cases we'd need to track.

---

## 🟡 Risk 3: World Streaming x Rewind Interaction Is a Compound Risk

The TDD correctly identifies the interaction between the `RewindManager` and chunk-loading as something to test in Phase 2. This is good — but the plan doesn't define *how* we'd validate this.

### Specific challenges:
- **Dangling pointers to unloaded chunks.** If a player rewinds into a just-unloaded room, the `RewindManager` holds stale references. The TDD acknowledges this but offers no resolution. This needs a design decision, not just a test.
- **Memory budget validation is manual.** "Strict memory profiling per room" is mentioned but there's no automated gate. This will regress silently unless we build a CI/CD memory budget check per room.
- **Chunk seam bugs are notoriously hard to reproduce.** Room boundary transitions will likely cause edge cases (enemies straddling boundaries, projectiles crossing seams mid-rewind) that are nearly impossible to catch without a dedicated chunk-boundary stress test suite.

> [!IMPORTANT]
> I'd like the Architect to propose a resolution for the dangling pointer problem before Phase 2 begins. The two main solutions (freeze rewind near boundaries vs. cache states for recently unloaded chunks) need to be picked so we know what to test against.

---

## 🟡 Risk 4: Accessibility Testing Is Understaffed and Under-Specced

Both the PO plan and GDD v2 list accessibility features (remappable controls, colorblind modes, adjustable game speed). This is great. However, accessibility is currently treated as a Phase 4 checkbox rather than a design pillar.

### Specific challenges:
- **"Adjustable game speed" interacts with the rewind mechanic.** If a player has reduced game speed for accessibility reasons, does the rewind buffer record at the reduced speed or wall-clock time? This is not specified.
- **Colorblind modes need design assets early.** You can't validate colorblind palettes in a 2-week Phase 4 pass. Art assets need to be delivered in at least one alternative colorblind-safe palette from the start of Phase 3.
- **No accessibility testing methodology is defined.** We should commit to testing against WCAG 2.1 guidelines or a game-specific standard (e.g., Game Accessibility Guidelines). We need a human playtester with the relevant disability, not just a simulated check.

### Questions for PO:
6. What is the priority of accessibility features relative to launch? Is it a hard launch requirement or post-launch DLC/patch?
7. Is there budget for external accessibility consulting or playtesting?

---

## 🟡 Risk 5: CI/CD and Automation Plans Are Thin

The TDD mentions a CI/CD pipeline for automated performance benchmarking. This is exactly the right instinct, but the plan is vague.

### Specific challenges:
- **"Automated performance benchmarks" are undefined.** What frame-time threshold triggers a build failure? (The TDD says 60 FPS / 16.6ms target, but this needs to be codified as a CI gate, not a human review.)
- **No automated functional test framework is named.** Unity has several options (Unity Test Framework, Playmode Tests, GameDriver, etc.). The team needs to pick one and onboard QA to it in Phase 1, or we'll be writing manual test scripts in Phase 4.
- **No smoke test suite is planned for Phase 1.** Grey-box prototype should have a basic suite of automated checks (player can move, player can jump, player can attack) so regressions are caught during the rapid iteration of early phases.

> [!NOTE]
> I strongly recommend we add a week to Phase 1 for QA to set up the test framework and write the initial smoke test suite. This is cheap insurance that pays off throughout the entire project.

### Questions for Architect:
8. What automated testing framework do you want to use with Unity? (Unity Playmode Tests? GameDriver? Something custom?)
9. Can we add a per-room memory budget gate to the CI/CD pipeline from Phase 2 onward?

---

## Summary of Open Questions for the Next Planning Meeting

| # | Question | Owner | Urgency |
|---|----------|-------|---------|
| 1 | Rewind duration and resource cost? | PO | 🔴 Pre-Phase 1 |
| 2 | Behavior when rewinding across a room boundary? | Architect | 🔴 Pre-Phase 1 |
| 3 | Does rewind affect enemies, projectiles, destructibles? | PO | 🔴 Pre-Phase 1 |
| 4 | Dev kit procurement plan for consoles? | Architect | 🔴 Pre-Phase 2 |
| 5 | TRC/LotCheck review completed? | Architect | 🟡 Pre-Phase 2 |
| 6 | Accessibility as hard launch requirement? | PO | 🟡 Pre-Phase 3 |
| 7 | Budget for accessibility playtesting? | PO | 🟡 Pre-Phase 3 |
| 8 | Unity automated test framework decision? | Architect | 🔴 Pre-Phase 1 |
| 9 | Per-room memory budget CI gate? | Architect | 🟡 Pre-Phase 2 |

---

## Recommended Additions to the Plan

1. **Add a "QA Kickoff" milestone to Phase 1.** QA needs to set up the test framework, write the smoke suite, and document the rewind acceptance criteria before production begins.
2. **Add a "Console Parity" milestone to Phase 3.** Performance validation on all four platforms must happen *before* Phase 4 polish, not during it.
3. **Create a Rewind Acceptance Criteria document.** The Architect should own this; QA will sign off on it.
4. **Create a Platform Compatibility Matrix.** One row per platform, one column per TRC category. This lives in `docs/` and QA tracks it through Phase 4.
