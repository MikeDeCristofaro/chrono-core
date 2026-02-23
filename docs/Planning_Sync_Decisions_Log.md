# Chrono-Core — Cross-Team Planning Sync
**Date:** 2026-02-22  
**Facilitator:** Senior Game Dev Manager  
**Attendees:** Product Owner, Architect, Senior QA  
**Purpose:** Close all 9 open questions raised in the Senior QA Review before Phase 1 begins.

---

## ✅ Decisions Made

---

### Question 1 — Rewind: Duration and Resource Cost?
**Owner:** Product Owner  
**Decision:** ✅ RESOLVED

> The rewind mechanic will use a **dedicated resource called Chrono Energy**, displayed as a segmented bar in the HUD (3 segments). Each segment allows **3 seconds of rewind** (max 9 seconds total). Chrono Energy regenerates slowly over 8 seconds when not rewinding. The player cannot rewind further back than the current room entry point.

**Acceptance Criteria (for QA):**
- Player can rewind up to 9 seconds when full bar is available.
- Bar depletes in real-time proportional to rewind duration.
- Bar begins regeneration 8 seconds after last rewind action ends.
- Rewind cannot cross the current room entry point — hard stop with a visual/audio cue.

---

### Question 2 — Rewind: Behavior when crossing a room boundary?
**Owner:** Architect  
**Decision:** ✅ RESOLVED

> The `RewindManager` will **freeze the rewind at the room entry point** rather than attempting a cross-boundary state restore. The player will be snapped to the room entry position with a stamped state from the moment of entry. This avoids dangling references to unloaded chunks entirely and is the simpler, more testable implementation.

**Architecture note:** The `RoomTransitionController` will write a mandatory `RewindCheckpoint` to the `RewindManager` on every room entry. This checkpoint is always kept in memory and is never evicted until the room is exited again.

---

### Question 3 — Rewind: Does it affect enemies, projectiles, destructibles?
**Owner:** Product Owner  
**Decision:** ✅ RESOLVED

> **Yes — full world rewind.** Enemies, projectiles, and destructibles all roll back. This is the design intent ("undo a mistake") and a core fantasy pillar. Permanently triggered events (cutscenes, room unlocks, boss death) are **excluded** from rewind scope and flagged with `[Irreversible]` in the GDD.

**Acceptance Criteria addendum for QA:**
- All `IRewindable` entities (enemies, projectiles, breakables) correctly restore position, health, and state.
- Entities tagged `[Irreversible]` (e.g., boss death, door opened) do NOT revert.
- Test matrix required: each entity type × rewind duration (1s, 3s, 6s, 9s).

---

### Question 4 — Dev kit procurement plan for consoles?
**Owner:** Architect  
**Decision:** ✅ RESOLVED

> Dev kit procurement will begin **immediately (Phase 1, Week 1)**. Priority order: PS5 first (longest Sony approval lead time), then Nintendo Switch 2, then Xbox Series. Target: all kits received and configured by **end of Phase 2**.

| Platform | Est. Lead Time | Target Received |
|----------|---------------|-----------------|
| PS5 Dev Kit | 6–8 weeks | End of Phase 1 |
| Switch 2 Dev Kit | 8–10 weeks | Start of Phase 2 |
| Xbox Series Dev Kit | 2–3 weeks | Mid Phase 1 |

**Action:** Architect to submit procurement requests Day 1 of Phase 1.

---

### Question 5 — TRC/LotCheck requirements reviewed?
**Owner:** Architect + Senior QA  
**Decision:** ✅ RESOLVED (partially)

> A formal Platform Compatibility Matrix will be created by the Architect in Phase 1, Week 3 (after dev kit paperwork is submitted). QA will populate it with TRC categories from published Sony, Nintendo, and Microsoft documentation. The matrix will live at `docs/Platform_Compatibility_Matrix.md` and will be tracked as a living document through Phase 4.

**Action:** QA to draft the matrix skeleton by end of Phase 1. Architect to review and sign off.

---

### Question 6 — Is accessibility a hard launch requirement?
**Owner:** Product Owner  
**Decision:** ✅ RESOLVED

> Accessibility is a **Tier 1 launch requirement** — the game will not ship without it. Specifically: remappable controls and colorblind mode are hard requirements. Adjustable game speed is a "best effort" with a minimum of a 0.75x slow mode.

**Impact on art pipeline:** The Art team must deliver colorblind-safe palette variants for every character and UI element beginning in Phase 3, not Phase 4. QA will include colorblind simulation testing in Phase 3 acceptance criteria.

---

### Question 7 — Budget for accessibility playtesting?
**Owner:** Product Owner  
**Decision:** ✅ RESOLVED

> **Yes, budget is approved** for one round of external accessibility playtesting, targeted for **Phase 3 (Month 8)**. The PO will engage an external accessibility consultancy (e.g., specialisterne, AbleGamers, or similar). QA will coordinate the session and document findings.

---

### Question 8 — Unity automated test framework decision?
**Owner:** Architect  
**Decision:** ✅ RESOLVED

> **Unity Test Framework (UTF) with Playmode Tests** will be the primary framework. Rationale: it's first-party, has no additional licensing cost, supports both EditMode and PlayMode, and integrates natively with Unity's CI/CD (GameCI). GameDriver will be evaluated in Phase 2 for higher-level integration tests if UTF proves insufficient.

**Action:** Architect to set up the UTF project structure in Phase 1, Week 1. QA to onboard and write the initial smoke suite by end of Phase 1, Week 2.

---

### Question 9 — Per-room memory budget CI gate?
**Owner:** Architect  
**Decision:** ✅ RESOLVED

> **Yes — implemented starting Phase 2.** Each room scene will have a defined memory budget (target: ≤180MB per room on Switch hardware, the lowest common denominator). The CI/CD pipeline (GameCI + GitHub Actions) will run a memory profiling step on every PR that touches a scene file. Builds that exceed the budget will **fail the PR check**.

| Platform Budget Target | Limit |
|-----------------------|-------|
| Switch 2 (lowest spec) | 180 MB / room |
| PS5 / Xbox / PC | No strict gate (monitored) |

---

## ✅ Scope — Confirmed by Executive

> **PC (Steam) ONLY.** The project targets PC exclusively. Multi-platform console ports are **not in scope**. This is a final decision from the executive. The Architect's GDD v2 references to console platforms should be removed.

---

## 📋 Action Items Summary

| # | Action | Owner | Due |
|---|--------|-------|-----|
| A1 | Write full Rewind Acceptance Criteria doc | Architect | Phase 1, Week 1 |
| A2 | Set up Unity Test Framework project structure | Architect | Phase 1, Week 1 |
| A3 | Write initial smoke test suite (movement, jump, attack, rewind) | QA | Phase 1, Week 2 |
| A4 | Remove console platform references from GDD v2 — PC only | Architect | Phase 1, Week 1 |
| A5 | Schedule external accessibility consultancy for Month 8 | PO | Phase 2 |
| A6 | Deliver colorblind palette variants start of Phase 3 | PO (Art) | Phase 3, Month 4 |
| A7 | Implement per-room memory CI gate (PC budget baseline) | Architect | Phase 2, Week 1 |

---

## ✅ Phase 1 Revised Checklist (Updated)

- [ ] Finalize and publish GDD v2 — remove console references, PC only (A4)
- [ ] Create Rewind Acceptance Criteria doc (A1)
- [ ] Establish art style guide and colorblind palette requirements
- [ ] Select Unity, set up version control and GameCI pipeline (PC build target only)
- [ ] Set up Unity Test Framework (A2)
- [ ] Write initial smoke test suite (A3)
- [ ] Develop grey-box prototype: movement, combat, rewind (2-week R&D block)
