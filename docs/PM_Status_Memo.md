# Chrono-Core — QA Status Memo to Senior PM
**Date:** 2026-02-22  
**From:** Senior QA  
**To:** Senior Project Manager  
**Re:** Pre-Phase 1 QA Readiness & Open Blockers

---

Hey — wanted to loop you in proactively before Phase 1 kicks off. I've reviewed the GDD v2 and TDD and filed a full QA review (`docs/QA_Review.md`), but here's the short version for your planning purposes.

## Where We Stand

The architecture is solid. The Architect has done good foundational work. However, **QA cannot write a single test case or commit to a test timeline** until we get answers to several open design questions. These need to be resolved in a planning meeting before Phase 1 begins — not mid-sprint.

## What's Blocking QA

### 🔴 Hard Blockers (need answers before Phase 1 start)

| # | Question | Owner |
|---|----------|-------|
| 1 | What is the exact rewind duration and resource cost? | PO |
| 2 | What happens when a player rewinds across a room boundary? | Architect |
| 3 | Does rewind affect enemies, projectiles, and destructibles? | PO |
| 4 | Which Unity automated test framework are we using? | Architect |

### 🟡 Important (need answers before Phase 2)

| # | Question | Owner |
|---|----------|-------|
| 5 | When are dev kits procured for Switch, PS5, Xbox? | Architect/Producer |
| 6 | Has the team reviewed per-platform TRC/LotCheck requirements? | Architect |

## Schedule Risk You Need to Know About

The current plan puts console certification prep in **Phase 4 (Months 11-12)**. This is a timeline risk I'd flag immediately — Sony, Nintendo, and Microsoft cert cycles often take 2-4 months of back-and-forth. If we start cert prep that late and fail even one submission round, we're looking at a slip. I'd recommend getting a producer or platform contact assigned by Phase 2 at the latest.

## What I Need from You

1. **Schedule a pre-Phase 1 planning sync** with PO + Architect to resolve the four blockers above. 30–45 minutes should do it.
2. **Add a QA Kickoff task to Phase 1** — I need ~1 week to set up the test framework and write initial smoke tests so regressions don't get missed during the rapid iteration of early prototyping.
3. **Flag the console cert timeline risk** to stakeholders. We may want to phase the platform targets (launch PC first, console follow-up) rather than attempt simultaneous cert.

Happy to join any planning call or provide more detail on any of this. Full risk breakdown is in `docs/QA_Review.md`.

— Senior QA
