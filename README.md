# WuCru Recompiled

A decompiled and restored Unity project for **LEGO Ninjago: Wu Cru**, patched and upgraded for modern Unity versions.

## Overview

This repository contains the reconstructed Unity project and source code for LEGO Ninjago: Wu Cru, including critical decompilation fixes, shader restorations, character model synchronization fixes, and gameplay repairs.

### Fixes & Improvements

- **Character Visual & Movement Desync Fixed**:
  - Removed erroneous Root Motion evaluation on child Animator components (`Minifig_Animations_Master`), preventing the 3D character mesh from locking to the world origin during movement.
  - Corrected `m_ApplyRootMotion: 0` across player and enemy prefabs to match friendly NPCs.
  - Enabled `updateWhenOffscreen = true` on `SkinnedMeshRenderer`s so character and enemy meshes never freeze or cull when moving.
  - Hardened `Running.cs` and `Idle.cs` state machine behaviours with null-safe hierarchical character lookups.
- **Shader & UI Restorations**:
  - Rebuilt missing ring rendering logic in `Custom_SimpleRing.shader` for NGUI / character roster slot portraits.
  - Rebuilt circular alpha anti-aliasing in `Custom_Blob.shader`.
  - Restored proper character face decal textures and headpiece UV layering.
- **Save State & Dev Tools**:
  - Added Editor menu item `Tools > Clear PlayerPrefs (Reset Save)` to quickly reset progress and test fresh game states.

## Project Structure

- `WuCru/ExportedProject/`: Main Unity project (open this in Unity 2022.3 LTS).
  - `Assets/`: Restored scripts, scenes, prefabs, materials, shaders, and resources.
  - `ProjectSettings/`: Unity project settings and input configurations.
  - `Packages/`: Unity package manifest.
- `WuCru/AuxiliaryFiles/`: Decompilation mapping and auxiliary assemblies.

## Requirements

- **Unity 2022.3 LTS** (tested on 2022.3.62f2).
