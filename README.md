# Color Loop - Zestful Games Technical Test
Developed by: Shanmukha Srinivasa

A polished, fully functional vertical slice of the Pixel Flow core loop, built using Unity 6000.3.14f1.

## 🎮 Playable Build / Scene
* The main game scene is located in `Assets/Scenes/Game.unity`.
* Simply open the scene and hit Play!

## ✅ Features Implemented

### Must-Have Components
* **Dynamic Grid & Queues:** System scales dynamically based on level data. 
* **Character Logic:** Minimum 10 shots per character; ammo counts precisely match the remaining cubes on the board.
* **Core Loop:** Max 5 characters on track. Tap to deploy, tap to redeploy from the resting area.
* **Level Progression:** 10 distinct, playable levels with escalating difficulty.
* **Win/Lose States:** Clear the board to win; fill the resting area to trigger a game over/revive state.

### Nice-to-Have Components (Added Polish)
* **Auto-Finish System:** Intelligently fast-forwards the game (`Time.timeScale = 2.5f`) when the remaining character count fits safely within the resting area limit.
* **Tactical Blocks:** Added "Armored" blocks (require 2 hits) and "Bomb" blocks (AoE explosions that trigger chain reactions).
* **Power-Ups:** Implemented "Shuffle" (reorders waiting queues) and "Expand Loop" (increases max track capacity).
* **Revive Mechanic:** Freezes game state and accurately refunds orphaned ammo back into the deployment queues.
* **Juice:** Integrated color-matched particle bursts, screen shake, and UI animations.

## 🧠 Technical Highlights
* **Data-Driven Level Design:** The `GridManager` builds levels dynamically from string arrays in the Inspector, making new level creation instant and scalable.
* **Dynamic Ammo Sync:** Complex AOE bomb interactions actively search the queues and resting lines to strip orphaned ammo from characters, preventing soft-locks.
