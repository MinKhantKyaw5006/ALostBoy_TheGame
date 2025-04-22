# Lost Boy Adventure
![Screenshot 2024-10-14 112219](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d2ff7b9774495cbd018c0d2790c01b6187f62146/Screenshot%202024-10-14%20112219.png)



## Table of Contents
1. [Introduction](#introduction)
2. [Scope](#scope)
   - [Covered](#covered)
   - [Not Covered](#not-covered)
3. [Project Overview & Methodology](#project-overview--methodology)
4. [Features](#features)
   - [Player Controls & Combat System](#player-controls--combat-system)
   - [Checkpoint Saving System](#checkpoint-saving-system)
   - [Menu System](#menu-system)
   - [Advanced Camera System](#advanced-camera-system)
   - [Enemy System](#enemy-system)
   - [Boss Fight AI](#boss-fight-ai)
   - [Level Design](#level-design)
   - [Scene Animation & Dialogue System](#scene-animation--dialogue-system)
   - [Interactable Objects & Challenges](#interactable-objects--challenges)
   - [Sound & Graphic Design](#sound--graphic-design)
5. [Project Structure](#project-structure)
6. [Installation](#installation)
7. [Usage](#usage)
8. [Tech Stack](#tech-stack)
9. [License](#license)

## Introduction
Game development education often lacks guidance on originality and hands-on practice, resulting in student projects that are simplistic remakes of popular titles. **Lost Boy Adventure** aims to address this gap by equipping students with the tools, knowledge, and creative freedom to develop engaging, original 2D games featuring unique characters, narratives, and mechanics inspired by industry standards.

## Scope
### Covered
| Functional Scope       | Brief Explanation                                 |
|------------------------|---------------------------------------------------|
| 6 Major Functions      | Core gameplay systems required for completeness   |
| 6 Minor Functions      | Support systems for visuals and mechanics         |
| Game Level Design      | Four distinct stages with unique environments     |
| Asset Creation         | Resources for players, enemies, bosses, and items |
| Animation Methods      | Manual and Unity-built animation workflows        |
| Basic Sound Design     | Effects, background audio, and thematic tracks    |

### System Architecture
![System](https://raw.githubusercontent.com/MinKhantKyaw5006/ALostBoy_TheGame/71adca0393dcf14bf4d4dcf52e906c27548a6ce5/system.png)

![Subsystem](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/b8a25d9ecfea32a00386c91cf009c603551b01f8/subsystem.png)


### Not Covered
- Extensive sound design beyond basic effects and themes
- Fully original asset creation (uses free resources with minor modifications)
- More than four levels
- Database or cloud save systems (all data is local)
- Multi-platform builds (Windows only)

## Project Overview & Methodology
We follow a structured pipeline:
1. **Asset Preparation**: Create or source assets from the Unity Asset Store, then slice and import into Unity.
2. ![asset]( https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/87459420dceb72bf67c7bad6ee1709370297f9bf/asset%20preparation.png)
3. **Animation**: Combine manual rig-and-mesh workflows (Spine 3D) with Unity’s animation system for efficiency and quality.
4. ![asset](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/87459420dceb72bf67c7bad6ee1709370297f9bf/character%20design.png)
5. ![animation](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/87459420dceb72bf67c7bad6ee1709370297f9bf/character%20animation.png)
6. **Development**: Set up a 2D Unity project, organize folders, and implement systems iteratively using Git for version control.

## Features
### Player Controls & Combat System
- Four-directional melee attacks with combo potential
- Dash, jump, and double-jump mechanics for mobility
- Health and mana bars, with mana gained on enemy defeat
- Special directional superpowers consuming mana

### Checkpoint Saving System
- Automatic save at checkpoints (position, inventory, state)
- Resume from last checkpoint on death, crash, or restart
- Pause menu options to resume, restart checkpoint, or restart chapter

### Menu System
**Main Menu**:
- New Game (up to four save slots)
- Continue Game
- Load Game
- Quit

**Pause Menu**:
- Resume
- Restart from Checkpoint
- Restart Chapter
- Return to Main Menu (saves current checkpoint)

### Advanced Camera System
- Delayed follow for smooth movement feel
- Dynamic zooming and panning
- Rapid downward pan during falls
- Cinematic camera shake on impact
- Scene boundaries to constrain view

### Enemy System
Seven enemy types with unique behaviors:
1. Patrolling ground enemies
2. Following walking & shooting enemies
3. Flying follow enemies
4. Flying shooting enemies
5. Vertical small spiders
6. Horizontal/diagonal big spiders
7. 360° rotating platform enemies

### Boss Fight AI
- Three-stage encounter with idle, run, and attack modes
- Attacks: lunge, triple slash, parry, jump slam, fireball barrages, airborne phase
- Stun windows for player counterattacks
- Randomized AI patterns for unpredictability

### Level Design
- Stage 1: Cave
- Stage 2: Rainforest
- Stage 3: Mushroom Valley
- Stage 4: Boss Cave

### Scene Animation & Dialogue System
- Cinematic cutscenes with widescreen bars
- NPC dialogue UI
- Tutorial prompts for controls

### Interactable Objects & Challenges
- Unlockable doors: combat or collectible quests
- One-way and mission doors, teleportation portals
- Normal and falling platforms, spikes, traps, jump pads
- Fall damage and respawn logic

### Sound & Graphic Design
- Four thematic background tracks
- Ambient sound effects (e.g., forest background)
- Player action SFX (jump, run, attack)
- Door interaction SFX

## Project Structure
```
Assets/
  ├─ Scripts/
  ├─ Graphics/
  ├─ Animations/
  ├─ Audio/
  ├─ Scenes/
  └─ Prefabs/
Packages/
ProjectSettings/
.gitignore
README.md
```



