# Project Overview
- **Game Title**: Super Sailor Dude
- **High-Level Concept**: 2D Action-Platformer where players choose between two characters to battle enemies and bosses.
- **Players**: Single player (with local character selection)
- **Inspiration / Reference Games**: Retro arcade beat 'em ups and platformers.
- **Tone / Art Direction**: 2D pixel art style.
- **Target Platform**: Standalone Windows 64
- **Screen Orientation / Resolution**: Landscape 1920x1080
- **Render Pipeline**: UniversalRP

# Game Mechanics
## Core Gameplay Loop
The player selects a hero, navigates platform-based levels, defeats minion enemies (Dock Rats, Raccoons) using melee attacks (punch/kick), and conquers powerful bosses with unique combat styles.
## Controls and Input Methods
Event-driven inputs via the New Input System. Controls are mapped to Left/Right movement, Jumping, Punching, and Kicking.

# UI
The Player Selection Panel allows choosing either character before starting.

# Key Asset & Context
- `Assets/_Imported/_InputAction/Player.inputactions`: The active input asset mapping gamepad controls to player actions.
- `Assets/Animations/Player/Super Sailor/Super Sailor.controller`: Super Sailor Dude's Animator Controller, handling state transitions.
- `Assets/_Scripts/_Player/_Player States/PlayerStateManager.cs`: Central input handler and state coordinator for the player character.
- `Assets/_Scripts/_Player/_Player States/RunState.cs`: StateMachineBehaviour handling running physics movement.

# Implementation Steps

### 1. Configure Keyboard WASD/Arrow Bindings in `Player.inputactions`
- **Description**: Edit the Input Action asset to add keyboard-compatible controls so WASD/Arrows and key presses can be mapped to actions.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - Add a 1D Axis composite for "Keyboard WASD/Arrows" under `Left Right` action:
    - negative: `<Keyboard>/a`
    - positive: `<Keyboard>/d`
  - Add the following bindings to `Jump` action:
    - `<Keyboard>/space`
    - `<Keyboard>/w`
    - `<Keyboard>/upArrow`
  - Add `<Keyboard>/j` binding to `Punch` action.
  - Add `<Keyboard>/k` binding to `Kick` action.

### 2. Fix Animator Transitions & Parameters
- **Description**: Modify transitions in `Super Sailor.controller` and `Ninja.controller` to be responsive and immediate.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - For `Punch -> Idle` transition in `Super Sailor.controller`: Change `Has Exit Time` to `True`, set `Exit Time = 1.0` (or `0.9`), set `Transition Duration = 0`. Remove any empty conditions.
  - Ensure all other animation transitions (such as `Idle -> Run`, `Run -> Idle`) have `Transition Duration = 0` to ensure snappy sprite swaps.

### 3. Improve Input Processing in `PlayerStateManager.cs`
- **Description**: Correct state synchronization to cleanly transition out of the run state.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - In `SwitchState()`, add an `else` block to explicitly set `run_b` to `false` when `moveInput == 0`. This prevents players from being locked in run/slide loops.

### 4. Correct Run Speed in `RunState.cs`
- **Description**: Edit the state-machine run behaviour to read dynamically from the character's configured moveSpeed.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - Replace hardcoded `_speed` multiplier (`2.0f`) with `_stateManager.moveSpeed` to allow customized movement velocities.

# Verification & Testing
- Use automated Play Mode testing to simulate keyboard keystrokes and verify transitions, speed levels, and instant movement responses.
- Verify through manual checks that W/A/S/D moves the player instantly, J punches, K kicks, and Space jumps.
