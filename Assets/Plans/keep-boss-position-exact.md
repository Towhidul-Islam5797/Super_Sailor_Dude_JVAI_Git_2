# Project Overview
- **Game Title**: Super Sailor Dude
- **High-Level Concept**: 2D Action-Platformer with responsive character controls and bosses.
- **Render Pipeline**: UniversalRP

# Game Mechanics
## Core Gameplay Loop
The player fights mobs and confronts Tag King in Level 4.
## Controls and Input Methods
Event-driven keyboard inputs (WASD, Space, J, K) are active.

# UI
Standard health bars and select screens are configured.

# Key Asset & Context
- `Assets/_Scripts/_Enemies/Tag King/TagKingAppear.cs`: Handles Tag King entrance logic and positional shifting.

# Implementation Steps

### 1. Disable Shifting and Offset Logic in `TagKingAppear.cs`
- **Description**: Comment out the horizontal start offset and DOtween movement logic inside `AppearTagKing()` to temporarily disable any triggers or code that can teleport or shift Tag King away from his editor-placed position.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes
- **Details**:
  - Comment out the starting position offset logic inside `TagKingAppear.cs`.
  - Comment out the `DOMoveX` sliding animation so he remains precisely in his default location.

### 2. Diagnose & Stop Deactivation of Tag King at Scene Start
- **Description**: Inspect and intercept the deactivation of `Tag King` at start-of-scene runtime to identify whether another manager or active Trigger is setting him inactive, and disable the deactivating behavior.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - Create a temporary `TempObserver` script that hooks into `Tag King`'s lifecycle (`OnDisable`) and logs the stack trace to `TempLog.txt`.
  - Execute a Play Mode capture, read the stack trace to determine the exact script and method calling `SetActive(false)`, and disable the offending call.

# Verification & Testing
- Ensure the script compiles cleanly with no syntax errors.
- Confirm Tag King spawns and remains exactly at the placement coordinate set by the user in the Unity Editor.
