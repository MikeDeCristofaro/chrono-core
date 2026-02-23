# Level Design Document v1: Chrono-Core
**Version:** 1.0
**Author:** Lead Level Designer
**Status:** Draft — Phase 1 Scope Defined
**Engine:** Unity 6 LTS | **Platform:** PC (Steam)

---

## 1. Level Design Pillars

1. **Fluidity First** — Every room must feel satisfying to traverse with base movement alone. Rewind and advanced mechanics are layered on top, never required to simply get through.
2. **Rewind as a Solution, Not a Crutch** — Puzzles must be solvable with skill alone. Rewind provides an alternate, cinematic path and rewards clever players without punishing those who don't use it.
3. **Visual Storytelling** — Backgrounds convey the decay of the Old World through environmental detail. Players should read the history of a biome just by moving through it.
4. **16-Bit Logic** — All collision operates on a clear pixel grid. Enemy telegraphs, traps, and hazards are readable within 0.5 seconds. No invisible walls.
5. **Respect the Player's Time** — Checkpoints are generous. Death sends the player back to the last room entry. Rewind is the in-combat escape valve.

---

## 2. World Map Overview

Chrono-Core features 5 worlds connected by side-scrolling Space SHMUP "Interstellar Corridor" stages. Players unlock worlds sequentially. World 1 is the starting point.

```
                    [WORLD 5: The Chrono-Nexus]  ← Final World / True Ending
                             ^
                    (SHMUP: Temporal Rift)
                             |
                    [WORLD 4: Sunken Reliquary]
                             ^
                    (SHMUP: Abyssal Trench)
                             |
     [WORLD 2: Rust-Haven] ←── [WORLD 1: Neon-Overgrowth] ──→ [WORLD 3: Crystal Spires]
     (Unlocked after W1)         (Starting World)              (Unlocked after W1)
```

SHMUP corridors between worlds are mandatory auto-scrolling stages. Ship weapon upgrades earned in SHMUP stages carry over and unlock abilities in on-foot stages (per GDD synergy design).

---

## 3. Biome List

| # | World Name | Biome Theme | Unique Gameplay Hook |
|---|-----------|-------------|---------------------|
| 1 | **Neon-Overgrowth** | Overgrown futuristic ruins — bioluminescent flora entangled with rusted old-world tech | Rewind-restore: broken platforms reassemble; locked logic gates hold open while player rewound-ghost holds the switch |
| 2 | **Rust-Haven** | Sprawling junkyard of ancient spacecraft hulls | Magnetized surfaces and polarity-switching gravity fields; player can cling to ceilings |
| 3 | **Crystal Spires** | High-altitude floating islands with low gravity | Low-gravity jumps with fragile crystal platforms that shatter on second contact — rewind restores them |
| 4 | **Sunken Reliquary** | Submerged research facility — flooded corridors | Water physics with light-refraction puzzles; air pocket management |
| 5 | **The Chrono-Nexus** | Distorted reality where timelines bleed together | Environmental rewind: sections of the level itself move in reverse; player must sync their own rewind to match |

---

## 4. World 1 — Neon-Overgrowth: Detailed Design

### Overview
- **Theme:** Lush neon-green and purple bioluminescent overgrowth consuming decayed sci-fi architecture. Think crashed colony ship slowly being reclaimed by alien jungle.
- **Tone:** Mysterious and dangerous, but not oppressive. The opening world.
- **Palette:** Deep teals and purples for backgrounds, bright cyan and lime for foreground flora, amber/orange for rusted metal and sparking machinery.
- **Music:** Chiptune jungle ambience with building synth arpeggio — transitions to urgent drum track in combat.

### Room Count: 8 rooms (linear with 1 optional side room)

### Room-by-Room Breakdown

---

#### Room 01 — Crash Site *(Tutorial)*
**Type:** Tutorial / Intro
**Description:** The Space Knight's escape pod has just crash-landed in a clearing. Smoke, scattered debris, alien flowers growing through cracks.
**Challenge:** Basic movement tutorial. Walk, jump, dash prompts appear.
**Enemies:** None.
**Rewind use:** None designed in — organic discovery zone.
**Notes:** Exits east. Sets visual and tonal baseline.

---

#### Room 02 — The Luminescent Gate
**Type:** Combat intro
**Description:** A massive stone archway covered in glowing moss. First hint of alien creatures.
**Challenge:** First combat encounter. 3x Scavenger Droids (small, melee, highly telegraphed). Gap jump required to exit east.
**Enemies:** Scavenger Droid (melee, telegraphed wind-up, 2-hit kill).
**Rewind use:** First opportunity — player can rewind if overwhelmed.
**Notes:** Gate opens after all enemies cleared. Teaches "clear room to progress" rule.

---

#### Room 03 — Hydro-Plant Alpha
**Type:** Hazard traversal
**Description:** A flooded lower corridor with sparking electrical wires hanging from the ceiling. Platforms are spaced over electrified water.
**Challenge:** Timed dash through falling water curtains that complete a circuit and electrify the floor. Rhythm-based traversal.
**Enemies:** 2x Pulse-Stinger (flying, slow ranged projectile). Flanking pressure while navigating hazards.
**Rewind use:** Safe zone to rewind before hitting the water hazard.
**Notes:** First environmental hazard. Teaches that the environment kills too.

---

#### Room 04 — The Rewind Well *(First Rewind Puzzle)*
**Type:** Rewind mechanic introduction
**Description:** A tall vertical shaft. A large stone platform slowly descends into a pit of acid. Stone blocks jut from the walls higher up.
**Challenge:** The central puzzle — ride the platform down to collect a Key, then rewind to restore the platform to its high position, allowing the player to reach the exit ledge.
**Enemies:** 2x Wall-Turrets (fixed position, projectile spam, destroyable).
**Rewind use:** Core mechanic moment. The level is designed to make the player fail once and discover rewind as the natural solution.
**Notes:** This is the "aha!" room. Must feel satisfying and not confusing. Telegraphed by an in-world journal entry near the room entrance describing the "time crystal" locals once used.

---

#### Room 05 — Botanical Gardens *(Hub)*
**Type:** Hub / Junction
**Description:** A wide, open atrium — the most beautiful room in World 1. Giant alien flowers, shafts of light, a bubbling fountain of bioluminescent liquid in the center.
**Challenge:** Combat gauntlet — 4x Overgrown Drones (flying, patrol routes). Must clear to unlock paths.
**Exits:** West (back), East (Room 07 — boss approach), North (Room 06 — optional side room).
**Enemies:** Overgrown Drone (flying, melee sting, patrol route).
**Rewind use:** Open space — good for rewinding botched aerial duels.
**Notes:** Visual and audio highlight of World 1. Should feel earned.

---

#### Room 06 — Security Overlook *(Optional Side Room)*
**Type:** Challenge / Collectible
**Description:** A high-tech surveillance room overlooking the crash site. Laser grids scan in rhythmic patterns. The room feels out of time — too clean for the overgrowth.
**Challenge:** Navigate 3 overlapping laser grids without being hit. Each grid has a different scan speed. Dash + rewind required to pass the tightest window.
**Enemies:** None (pure traversal).
**Reward:** Chrono-Cell Upgrade — adds a 4th segment to the Chrono Energy bar (extends max rewind to 12 seconds).
**Rewind use:** Mandatory. The third laser grid's timing window is designed to be nearly impossible without rewind. Teaches "rewind as precision tool."
**Notes:** Fully optional. Skilled players can attempt it; introduces the idea that exploration rewards Rewind enhancements.

---

#### Room 07 — Descent to the Core
**Type:** Gauntlet / Ramp-up
**Description:** A long downward slope descending into the planet's infrastructure. Crumbling bridges, steam vents, high enemy density.
**Challenge:** Slope forces momentum. Environmental hazards (steam vents, crumbling floor tiles) combined with heavy enemy pressure. First encounter with Elite Scavenger Droid (tanky, shield, ranged attack).
**Enemies:** 3x Scavenger Droid, 1x Elite Scavenger Droid (shield, 5-hit kill, ranged slam).
**Rewind use:** Strongly encouraged as shield timing is punishing.
**Notes:** Difficulty spike. Intended to feel intense. Music transitions to full combat theme here.

---

#### Room 08 — The Ancient Guardian's Chamber *(World 1 Boss)*
**Type:** Boss Arena
**Description:** A vast circular arena half-consumed by vines. In the center: THE THRESHER-UNIT — a rogue terraforming mech the size of a building, its chassis cracked open and blooming with alien flora.
**Boss:** **The Thresher-Unit**
**Boss Design Philosophy:** Teaches the player that rewind is not just for survival — it's a strategic combat tool.
**Boss Phases:**
- **Phase 1 (100%-50% HP):** Sweeping claw attacks with long telegraphs. Ranged seed-bomb volley (destroyable).  Arm can be staggered by melee combos.
- **Phase 2 (50%-0% HP):** Adds "Overgrowth Explosion" — a fuse-delayed area bomb that covers 60% of the arena. The ONLY safe response: hear the fuse, **rewind before detonation**, and reposition. Player is expected to die to this once to learn the mechanic.
**Reward:** World 1 complete. SHMUP Corridor to World 2 and World 3 unlocked.
**Notes:** [Irreversible] — boss death does not rewind. Post-boss cutscene is [Irreversible].

---

## 5. Rewind Integration Notes — World 1

| Room | Rewind Role |
|------|-------------|
| Room 03 | Escape valve on rhythm hazards |
| Room 04 | **Core puzzle mechanic** — platform restoration |
| Room 06 | **Required tool** — laser grid timing |
| Room 07 | Combat recovery against Elite Scavenger |
| Room 08 | **Boss mechanic** — dodge the Overgrowth Explosion |

All rewind puzzle moments must have an escape-hatch: a skilled player who doesn't rewind should still be able to complete the room with exceptional timing, but rewind makes it feel natural and cinematic.

---

## 6. Phase 1 — Grey-Box Prototype Scope

The following rooms are in scope for Sprint 1-2 grey-box prototype:

| Room | In Scope | Purpose |
|------|:--------:|---------|
| Room 01 — Crash Site | YES | Movement validation |
| Room 02 — Luminescent Gate | YES | Combat + jump validation |
| Room 03 — Hydro-Plant Alpha | NO | Phase 2 |
| Room 04 — Rewind Well | YES | Rewind mechanic validation |
| Room 05 — Botanical Gardens | NO | Phase 2 |
| Room 06 — Security Overlook | NO | Phase 2 |
| Room 07 — Descent to the Core | NO | Phase 2 |
| Room 08 — Boss Arena | YES | Boss + complex state rewind validation |

Grey-box rooms use placeholder collision geometry only. No art assets until Phase 2 vertical slice.
