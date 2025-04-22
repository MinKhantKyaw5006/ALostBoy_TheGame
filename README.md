# Lost Boy Adventure
![Screenshot 2024-10-14 112219](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d2ff7b9774495cbd018c0d2790c01b6187f62146/Screenshot%202024-10-14%20112219.png)
[![Play on Itch.io](https://img.shields.io/badge/Play_on-Itch.io-red?style=for-the-badge&logo=itchdotio)](https://lucasmin.itch.io/alostboyversion10)



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
5. [Detail Architecture](#detail-architecture)
6. [Game Play](#game-play)
7. [Tech Stack](#tech-stack)
8. [Download and play my game at Itch.io](#downad-and-play-my-game-at-itch.io)


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
   ![projectstructure](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/0dbd391b1e91652d68f445884b6969049132bd4f/project%20structure.png)

## Features
### Player Controls & Combat System
- Four-directional melee attacks with combo potential
- Dash, jump, and double-jump mechanics for mobility
- Health and mana bars, with mana gained on enemy defeat
- Special directional superpowers consuming mana
![playercontrol](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/0dbd391b1e91652d68f445884b6969049132bd4f/playercontrol.png)

### Checkpoint Saving System
- Automatic save at checkpoints (position, inventory, state)
- Resume from last checkpoint on death, crash, or restart
- Pause menu options to resume, restart checkpoint, or restart chapter
![checkpoint](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/0dbd391b1e91652d68f445884b6969049132bd4f/checkpoint.png)

### Menu System
**Main Menu**:
- New Game (up to four save slots)
- Continue Game
- Load Game
- Quit
![main menu](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/e33b402e8e33bbe6c94e465fb1318dfa9ab61d82/main%20menu.png)
![save](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/e33b402e8e33bbe6c94e465fb1318dfa9ab61d82/save%20menu.png)


**Pause Menu**:
- Resume
- Restart from Checkpoint
- Restart Chapter
- Return to Main Menu (saves current checkpoint)
![pause](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/e33b402e8e33bbe6c94e465fb1318dfa9ab61d82/pause%20menu.png)

### Advanced Camera System
- Delayed follow for smooth movement feel
- Dynamic zooming and panning
- Rapid downward pan during falls
- Cinematic camera shake on impact
- Scene boundaries to constrain view
![cam1](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/0d76e86211f586ba139df5a5e8fd4f9ea10e5edd/cam%20pan%201.png)
![cam2](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/0d76e86211f586ba139df5a5e8fd4f9ea10e5edd/cam%20pan%202.png)
![cam3](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/0d76e86211f586ba139df5a5e8fd4f9ea10e5edd/cam%20pan%203.png)

### Enemy System
Seven enemy types with unique behaviors:
1. Patrolling ground enemies
2. Following walking & shooting enemies
3. Flying follow enemies
4. Flying shooting enemies
5. Vertical small spiders
6. Horizontal/diagonal big spiders
7. 360° rotating platform enemies
![enemy1](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/enemy%20one.png)
![enemy2](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/enemy%20two.png)
   

### Boss Fight AI
- Three-stage encounter with idle, run, and attack modes
- Attacks: lunge, triple slash, parry, jump slam, fireball barrages, airborne phase
- Stun windows for player counterattacks
- Randomized AI patterns for unpredictability
  ![boss1](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/boss%201.png)
  ![boss2](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/boss%202.png)
  ![boss3](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/boss%20three.png)
  ![boss4](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/boss%20four.png)
  ![boss5](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/bb170f26d8478949c41637f40a48a7adad21caf9/boss%20five.png)

### Level Design
- Stage 1: Cave
![lvl1](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/lvl1.png)
![scene1](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/scene1.png)

- Stage 2: Rainforest
![lvl2](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/lvl2.png)
![secen2](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/scene2.png)

- Stage 3: Mushroom Valley
![lvl3](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/lvl3.png)
![scene3](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/scene3.png)

- Stage 4: Boss Cave
![lvl4](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/lvl4.png)
![scene4](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/scene4.png)
  

### Scene Animation & Dialogue System
- Cinematic cutscenes with widescreen bars
- NPC dialogue UI
- Tutorial prompts for controls

### Interactable Objects & Challenges
- Unlockable doors: combat or collectible quests
- One-way and mission doors, teleportation portals
- Normal and falling platforms, spikes, traps, jump pads
- Fall damage and respawn logic
![mission](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/d26962b2834402c4099b0821be6d3ee7b3534651/mission.png)

### Sound & Graphic Design
- Four thematic background tracks
- Ambient sound effects (e.g., forest background)
- Player action SFX (jump, run, attack)
- Door interaction SFX

## Detail Architecture 
![major](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/10cf0f1d16377772cb6c7154b173fc13ec589755/majorsystem.png)
![minor](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/b22cd647d496818197ba50ae7247a36a68401199/minorsystem.png)

## Game Play
### 🎮 Gameplay Videos

[![Watch Gameplay 1](https://img.youtube.com/vi/VCyRH2romLA/0.jpg)](https://youtu.be/VCyRH2romLA)
*Gameplay Video 1*

[![Watch Gameplay 2](https://img.youtube.com/vi/fAzMbpfR3A8/0.jpg)](https://youtu.be/fAzMbpfR3A8)
*Gameplay Video 2*


## Tech Stack
The game was developed using the following tools:

![Sketchbook](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/53d6b8e8accf14ea583e2dcc04fa8fd0bf5b07e7/sketchbook.png)
![Blender](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/53d6b8e8accf14ea583e2dcc04fa8fd0bf5b07e7/blender.png)
![Spine](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/53d6b8e8accf14ea583e2dcc04fa8fd0bf5b07e7/spine.png)
![Unity](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/blob/53d6b8e8accf14ea583e2dcc04fa8fd0bf5b07e7/unity.png)

---

📄 **Game Development Report**  
We’ve documented the full development process, including design choices, tools used, workflow, and testing strategies.  
**[👉 Read the full report here (PDF)](https://github.com/MinKhantKyaw5006/ALostBoy_TheGame/raw/bda47ec1a717017cb5b2dc6a205286987131c0cd/LostBoy%20Game%20report.pdf)**  


## Download and play my game at Itch.io
[![Play on Itch.io](https://img.shields.io/badge/Play_on-Itch.io-red?style=for-the-badge&logo=itchdotio)](https://lucasmin.itch.io/alostboyversion10)

