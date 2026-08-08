# Project Overview
- **Game Title**: Super Sailor Dude
- **High-Level Concept**: 2D Action-Platformer where players battle enemies and bosses.
- **Tone / Art Direction**: 2D pixel art style.
- **Render Pipeline**: UniversalRP

# Game Mechanics
## Core Gameplay Loop
The player navigates platforms, performs combat combos, and double jumps across hazards.
## Controls and Input Methods
Event-driven inputs via the New Input System. Keyboard mappings (WASD, Space, J, K) are now implemented.

# UI
Player Selection Panel controls character choice.

# Key Asset & Context
- `Assets/_Scripts/_Player/_Player States/PlayerStateManager.cs`: Central input handler and state coordinator for the player character.
- `Assets/Animations/Player/Super Sailor/Super Sailor.controller`: Super Sailor Dude's Animator Controller.
- `Assets/Animations/Player/Ninja/Ninja.controller`: Ninja's Animator Controller.
- `Assets/Animations/Tag King/Tag King.controller`: Tag King's Animator Controller.

# Implementation Steps

### 1. Enable Looping for Player Idle Animations
- **Description**: Slicing player spritesheets is done, but the `Idle.anim` clips are currently set with `loopTime = false`, causing characters to freeze on their last idle frame. Set `loopTime` of both `Idle` clips to `true`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes
- **Details**:
  - Load `Assets/Animations/Player/Super Sailor/States/Idle.anim` and `Assets/Animations/Player/Ninja/States/Idle.anim`.
  - Set `loopTime` of their `AnimationClipSettings` to `true`.

### 2. Implement Responsive Air-To-Ground Transitions
- **Description**: Currently, landing out of a `Jump` is stuck waiting for `Exit Time = 1.0` (2.8 seconds!), causing the player to freeze in a crouching/falling pose. Synchronize grounding to the animator and add instant transitions to `Idle`/`Run`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - Add `isGrounded_b` (Bool) parameter to both `Super Sailor.controller` and `Ninja.controller`.
  - In `PlayerStateManager.cs`'s `SwitchState()`, add:
    ```csharp
    animator.SetBool("isGrounded_b", isGrounded);
    ```
  - In `Super Sailor.controller` and `Ninja.controller`, remove the old `Jump -> Idle` exit-time transition.
  - Create the following transitions out of `Jump`:
    - `Jump -> Idle`: `Has Exit Time = False`, `Transition Duration = 0`, Conditions: `isGrounded_b == True`, `run_b == False`.
    - `Jump -> Run`: `Has Exit Time = False`, `Transition Duration = 0`, Conditions: `isGrounded_b == True`, `run_b == True`.

### 3. Snappier Attack Execution & Move Interruptions
- **Description**: Speed up Super Sailor's extremely long punch (0.87s) and kick (2s) animations so they execute instantly, and allow movement to immediately interrupt active punch/kick poses.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - In `Super Sailor.controller`, set state speed of `Punch` to `2.2` and state speed of `Kick` to `3.2`.
  - In both controllers, add transitions from `Punch -> Run` and `Kick -> Run` with `Has Exit Time = False`, `Transition Duration = 0`, and Condition `run_b == True`.

### 4. Smooth out Tag King Animations & Zero Transition Durations
- **Description**: In 2D games, non-zero cross-fade durations in the animator cause ghosting, double-frame blending, and jitter. Set transition durations of Tag King's animator to `0`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes
- **Details**:
  - In `Tag King.controller`, set `Transition Duration = 0` for ALL transitions.
  - Ensure looping is active for `Walk` and `Spray` states, and disabled for `Hurt`, `Die`, and `Flip`.

### 5. Deactivate Tag King Appear Trigger
- **Description**: The `Tag King Appear` trigger script deactivates Tag King at start and waits for the player to reach Y coordinates at `x = 438.78` which are unreachable. Deactivate this trigger so Tag King remains active right where he is initially placed.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes
- **Details**:
  - Find `Tag King Appear` root GameObject in the scene and set it inactive.

# Verification & Testing
- Run automated Play Mode tests to verify:
  1. Jump states immediately transition to running/idle upon contact with tiles.
  2. Idle animations loop continuously without stopping.
  3. Holding horizontal keys during an attack cancels the punch/kick into a run immediately.
  4. Tag King is present in the scene and active at start at `x = 73.08`.
