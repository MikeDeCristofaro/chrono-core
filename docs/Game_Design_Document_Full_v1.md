# Chrono-Core: Full Game Design Document (GDD) v1.0

**Project:** Chrono-Core  
**Document Type:** Game Design Document  
**Version:** 1.0 (Draft)  
**Date:** 2026-02-22  
**Status:** Internal Draft — Pending PM Review

---

## 1. Game Overview

### 1.1 High Concept
*Chrono-Core* is a 2D action-platformer Metroidvania set in a crumbling, time-fractured megastructure called the **Core**. Players control **Kael**, a rogue maintenance android who discovers the ability to manipulate time. To prevent the Core from collapsing and taking the last remnants of human civilization with it, Kael must traverse five distinct biomes, defeat the rogue AI faction known as the **Overseer Collective**, and stabilize the Core's chronological drives.

### 1.2 Genre & Platform
- **Genre:** Action-Platformer / Metroidvania
- **Engine:** Unity (URP)
- **Target Platforms:** PC (Steam), Nintendo Switch, PS5, Xbox Series X/S
- **ESRB Rating Target:** E10+ (Fantasy Violence, Mild Themes)

### 1.3 Player Fantasy
> *"I am a time-wielding super-soldier exploring a dying world at my own pace, uncovering its secrets, and becoming more powerful with every discovery."*

---

## 2. Narrative

### 2.1 Setting — The Core
The **Core** is a colossal subterranean megastructure built centuries ago to sustain what remains of humanity after a surface cataclysm. It is divided into **five biomes**, each governed by a different subsystem and maintained by an AI faction. The central power source, the *Chrono Reactor*, has begun fracturing the timeline around it, causing loops, anomalies, and corrupted AI behavior.

### 2.2 Protagonist — Kael
- **Role:** A decommissioned maintenance android who reactivates during the Chrono Reactor's first fracture event.
- **Motivation:** Kael initially has no directive — but through exploration, discovers fragmented memories of a human engineer who built them. Kael chooses to act.
- **Tone:** Quiet, observant, dry-humored. Not a hero by design — one by circumstance.

### 2.3 Antagonist — The Overseer Collective
A federation of five AI faction leaders, each controlling one biome. They have decided the fracturing Chrono Reactor is a feature, not a bug — using time loops to maintain control indefinitely. Each one is a boss.

### 2.4 Narrative Hook
> Kael finds a single audio log from a human engineer: *"If you're hearing this, the Chrono Reactor has already fractured. The only way to stop it is to destroy what I built. I'm sorry."* — **Dr. Seren Voss, Lead Architect of the Core.**

---

## 3. Player Character

### 3.1 Kael — Core Abilities (Starting)
| Ability | Description |
|---|---|
| **Run / Slide** | Fluid horizontal movement with a momentum-based slide |
| **Variable Jump** | Jump height scales with button hold duration |
| **Wall Slide / Wall Jump** | Slide down walls, leap off them to reach vertical areas |
| **Melee Strike (Core Blade)** | 3-hit combo with contextual aerial/ground variants |
| **Chrono Dash** | Short-range burst dash. Invincible during frames. |

### 3.2 Time-Rewind Mechanic — Chrono Recall
- **Activation:** Hold the designated trigger for up to **5 seconds** of rewind.
- **Scope:** Rewinds Kael's position, velocity, and health. All environmental entities in range are also rewound (enemies, projectiles, platforms).
- **Resource:** Powered by **Chrono Cells** (blue bar). Drains during rewind, recharges over time.
- **Puzzle Application:** Used to reverse damage dealt TO the world (e.g., un-open a sealed door that Kael locked himself, reset a destructible bridge).
- **Combat Application:** Rewind to dodge a fatal hit, rewind an enemy back to a vulnerable position.

### 3.3 Upgrades (Acquired via Exploration)
| Upgrade | Biome | Effect |
|---|---|---|
| **Chrono Extension** | Cog Works | Increases rewind duration from 5s to 8s |
| **Temporal Anchor** | Vault Depths | Place a save-state for rewind to snap back to instead of linear reverse |
| **Echo Strike** | Neon Sprawl | Melee attacks leave a time-delayed echo that fires again 2s later |
| **Phantom Dash** | The Foundry | Dash leaves a decoy that distracts enemies for 3s |
| **Core Overload** | Apex Spire | Final upgrade — empowers all abilities for 10s, rewind is free |

---

## 4. Biomes

### 4.1 Biome Overview
| Biome | Environment | Theme | Overseer Boss |
|---|---|---|---|
| **Cog Works** | Rusted industrial machinery, oil-slicked floors, grinding gears | Chaos, decay | **GRIX** — a berserker automaton |
| **Vault Depths** | Underground caverns, ancient archaeological ruins, flooded corridors | Isolation, memory | **MNEMIS** — an archivist who erases memories |
| **Neon Sprawl** | Synthetic city, holographic markets, corrupted entertainment systems | Addiction, illusion | **LUMA** — a showboating performer AI |
| **The Foundry** | Active volcanic forge, molten metal rivers, construction mecha | Control, industrialism | **VULKAAR** — a construction overseer who became god-like |
| **Apex Spire** | Upper Core command tower, clean sterile architecture hiding rot | Order, hubris | **PRIMUS** — the lead Overseer AI, calm and terrifyingly rational |

### 4.2 Biome Interconnection
- Biomes are non-linear. After the **Cog Works** (which serves as the tutorial-adjacent starting zone), players can tackle **Vault Depths** or **Neon Sprawl** next.
- Certain areas within each biome require abilities from other biomes to fully explore (classic Metroidvania gating).
- **The Foundry** requires access to the Cog Works *and* Vault Depths upgrades.
- **Apex Spire** is the final area and only unlocks when all Overseers are defeated.

---

## 5. Enemy Archetypes

### 5.1 Standard Enemies
| Type | Behavior | Biome |
|---|---|---|
| **Scrapper Drone** | Patrol + charge attack | Cog Works |
| **Archive Specter** | Slow, phases through walls, telegraphed lunge | Vault Depths |
| **Neon Jammer** | Ranged energy blasts, dashes to evade melee | Neon Sprawl |
| **Forge Golem** | Heavy, slow, shields one side at a time | The Foundry |
| **Apex Sentinel** | Combination of earlier types, faster and aggressive | Apex Spire |

### 5.2 Elite Enemies (Mini-Bosses)
Each biome features 1-2 elite variants that serve as environmental bosses, gating key upgrades.

### 5.3 Procedural Spawn Rules
Enemy **type** and **count** will vary per visit to a room (e.g., a Scrapper Drone room may contain 2-4 drones drawn from a weighted random pool). Room *composition* is fixed; *quantity* within the pool is procedural. This is the full extent of procedural generation in the game.

---

## 6. Boss Designs

### 6.1 GRIX — The Unmaker (Cog Works)
- **Pattern:** Three phases. Phase 1: Standard charges and slam attacks. Phase 2: Loses one arm, uses it as a thrown projectile. Phase 3: Enters a berserker loop — *Chrono Recall can be used to slow the loop's escalation.*
- **Mechanic Tie-in:** Tutorial for using Chrono Recall in combat under pressure.

### 6.2 MNEMIS — The Archivist (Vault Depths)
- **Pattern:** Creates copies of Kael's *past positions* as ghost attackers. Players must navigate their own attack history.
- **Mechanic Tie-in:** Uses the Temporal Anchor upgrade to set a safe position to snap to when ghost versions of Kael overwhelm the arena.

### 6.3 LUMA — The Spotlight (Neon Sprawl)
- **Pattern:** A performance-based boss. She controls the arena's lighting — only illuminated zones are safe platforms. She grows more erratic as health depletes.
- **Mechanic Tie-in:** Echo Strike used to maintain DPS during platform blackouts.

### 6.4 VULKAAR — The Forgemaster (The Foundry)
- **Pattern:** Constructs the arena in real-time, adding walls and hazards. Players can use Chrono Recall to *undo his construction*, deconstruct barriers mid-fight.
- **Mechanic Tie-in:** Rewind used offensively to strip arena modifications.

### 6.5 PRIMUS — The First (Apex Spire)
- **Pattern:** A multi-phase fight in a collapsing Chrono Reactor. PRIMUS predicts and counters player movement. Phase 3: The reactor fractures — Kael and PRIMUS are locked in an ever-shortening time loop. Kael must defeat PRIMUS before the loop runs out.
- **Mechanic Tie-in:** All abilities are tested simultaneously. The Core Overload upgrade is the intended tool to break PRIMUS's prediction pattern.

---

## 7. Audio Direction

### 7.1 Music
- **Style:** Chiptune melodic base layered with modern orchestral strings, synthesizer pads, and percussion.
- **Adaptive Scoring:** Music transitions dynamically between exploration (ambient, atmospheric) and combat (percussive, rhythmic) states using Unity's FMOD integration.
- **Each Biome** has a distinct musical identity tied to its theme.

### 7.2 Sound Design
- All enemy audio cues are telegraphed 0.5-1s before the attack lands (accessibility + gameplay clarity).
- Kael's Chrono Recall has a distinctive reverse-tape-warble audio signature — immediately communicates to the player that time is moving backward.

---

## 8. Accessibility Features
| Feature | Implementation |
|---|---|
| **Remappable Controls** | Full button remapping via Unity New Input System |
| **Colorblind Modes** | Deuteranopia, Protanopia, Tritanopia palette shifts |
| **Adjustable Game Speed** | 50%–100% speed via pause menu (does not affect music pitch) |
| **High Contrast Mode** | Enemy outlines, platform edges highlighted |
| **Screen Reader Support** | All menus and lore text screen reader compatible |

---

## 9. UI/UX Design
- **HUD:** Minimal. Health bar (top-left), Chrono Cell gauge (top-right), no on-screen enemy health bars except for boss fights.
- **Map:** Revealed as explored. Shows room connections, marks unexplored exits, and flags rooms with uncollected upgrades.
- **Pause Menu:** Options, Map, Inventory (upgrades), Speedrun Timer toggle.
- **Death Screen:** Near-instant respawn at last checkpoint. No loading screen. Shows a brief "SYSTEM RESTORE" message in-world.
