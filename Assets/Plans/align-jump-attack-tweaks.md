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
- `Assets/_Scripts/_Sticker/StickerTrigger.cs`: Handles player freezing and sticker collection sequence.
- `Assets/Animations/Player/Super Sailor/Super Sailor.controller`: Super Sailor Dude's Animator Controller.

# Implementation Steps

### 1. Re-slice/Re-align All Player Texture Pivots to BottomCenter
- **Description**: Slicing player sprites tightly crops transparency but leaves each animation state sequence with varying center heights relative to the character's feet. Changing the Sprite Pivot of all 273 player frame PNGs to `BottomCenter` (and updating metadata) forces the character's feet to be mathematically located exactly at local `(0, 0)` on every frame, eliminating air-walking and floating forever.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes
- **Details**:
  - Run a utility script to locate all `.png` textures under `Assets/Animations/Super Sailor Dude` and `Assets/Animations/Ninja`.
  - Use `SerializedObject` on `TextureImporter` to set `m_SpriteSheet.m_Sprites` element `m_Alignment` to `7` (BottomCenter) and `m_Pivot` to `(0.5, 0.0)`.
  - Save and reimport each texture.

### 2. Align Player Colliders to Sprite Feet and Ground Check
- **Description**: Now that all sprites are aligned to BottomCenter (meaning the feet are precisely at `y = 0` in local space), adjust CapsuleCollider2D height, size, and vertical offset on the child `Body` of "Super Sailor Dude" and "Ninja" so the bottom of the collider aligns perfectly with `y = 0`. Adjust Ground Check position accordingly.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: Yes
- **Details**:
  - Super Sailor Dude local CapsuleCollider2D on `Body`: Set size to `(3.23, 9.04)` and offset to `(0.49, 4.52)`.
  - Super Sailor Dude `Ground Check` child local position: Set to `(-0.05, -0.10, 0.0)`. Set `groundCheckRadious = 0.2f`.
  - Ninja local CapsuleCollider2D on `Body`: Set size to `(3.23, 9.40)` and offset to `(0.49, 4.70)`.
  - Ninja `Ground Check` child local position: Set to `(-0.05, -0.10, 0.0)`. Set `groundCheckRadious = 0.2f`.

### 3. Implement Double Jump and Jump State Updates
- **Description**: Add a double jump mechanic allowing players to trigger a second jump while in mid-air.
- **Assigned role**: developer
- **Dependencies**: Step 2
- **Parallelizable**: No
- **Details**:
  - Add fields `public bool canDoubleJump = true;` and `private bool hasDoubleJumped = false;` to `PlayerStateManager.cs`.
  - Reset `hasDoubleJumped = false` in `FixedUpdate()` if `isGrounded` is true.
  - Update `SwitchState()` to trigger double jump when `jumpPressed` is true and `isGrounded` is false, and consume the input cleanly.

### 4. Snappy Attack Cooldowns & Fix Animator Transition Deadlocks
- **Description**: 
  - Fix the animator deadlock where the transition `Punch -> Idle` has `Has Exit Time = False` and no conditions, which traps the player in the punch state indefinitely.
  - Implement a highly responsive button-press attack cooldown (250ms) to ensure at least a few milliseconds of a punch or kick are played before the player is allowed to trigger the next move.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No
- **Details**:
  - Modify transitions in `Super Sailor.controller` and `Ninja.controller` for `Punch -> Idle` and `Kick -> Idle` states:
    - Set `Has Exit Time = True`.
    - Set `Exit Time = 1.0`.
    - Set `Transition Duration = 0.0` (instant snappy reset).
    - Ensure conditions array is completely empty.
  - In `PlayerStateManager.cs`, introduce `private float lastAttackTime = -10f;` and `public float attackCooldown = 0.25f;`.
  - Update input callbacks `Punch()` and `Kick()` to verify `Time.time - lastAttackTime >= attackCooldown` before allowing triggers, then immediately update `lastAttackTime = Time.time`. This provides crisp, immediate actions with a perfect minimum execution time.

### 5. Temporarily Disable the Sticker System
- **Description**: Disable sticker triggers in the active scene.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes
- **Details**:
  - Deactivate the root `Sticker Triggers` GameObject in `"Level 4 Flooded Shipyard"`.

# Verification & Testing
- Deploy automated Play Mode tests to verify:
  1. No visual floating (checking world positions relative to ground tile alignment).
  2. Double jumping operates exactly once per airtime and correctly resets upon landing.
  3. Attack spamming is ignored while an attack animation is already playing.
