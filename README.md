# 🕹️ Terminal Glyph-Gulper 2026

A high-performance, **multi-threaded** .NET console game built with modern C# practices. Navigate your avatar, eat snacks to evolve, and manage your "mood" before time runs out.



## ✨ Technical Highlights

* **Thread-Safe Rendering**: Uses a `ConcurrentQueue<Action>` producer-consumer pattern to prevent console cursor collisions and flickering.
* **Asynchronous Engine**: The game logic runs on a dedicated background task, keeping the input thread responsive.
* **State-Driven Design**: Implements `Enums` for Game States, Player Moods, and Food Types with safe validation logic.
* **Graceful Shutdown**: Utilizes `CancellationTokenSource` to ensure the background thread closes properly and restores the terminal cursor upon exit.

---

## 🚀 Getting Started

### Prerequisites
* **.NET 8.0 SDK** or later.
* A modern terminal (Windows Terminal, iTerm2, or VS Code Integrated Terminal recommended).

### Installation & Run
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/yourusername/terminal-Glyph-Gulper.git](https://github.com/yourusername/terminal-Glyph-Gulper.git)

2. **Navigate to the folder:**

   ```bash
   cd terminal-Glyph-Gulper

3. **Run the game:**

   ```bash
   dotnet run

## 🕹️ How to Play

### Controls
| Key | Action |
| :--- | :--- |
| **Arrow Keys** | Move your player |
| **ESC** | Quit the game safely |

### Game Rules
* **Eat Snacks**: Move over the food (e.g., `@@@@@`) to collect points.
* **Evolve**: Every 5 snacks, your food "upgrades" and the vanishing timer speeds up.
* **Watch the Mood**: If you miss too many snacks, your player's expression changes from `(^-^)` to `(X_X)`.
* **Win Condition**: Reach **20 snacks** with minimal misses to secure a win.

---

## 🏗️ Project Structure

* **`GameEngine.cs`**: Orchestrates the main loop, collision detection, and game rules.
* **`RenderManager.cs`**: A thread-safe utility that batches `Console` calls to prevent UI corruption.
* **`Player.cs`**: Encapsulates movement logic, boundary clamping, and character states.
* **`Food.cs`**: Handles randomized spawning and evolution logic.

---

## 🚀 Future Improvements
* **High Score System**: Persistence using `System.Text.Json`.
* **Multi-Food Spawning**: Modifying the engine to handle a `List<Food>` instead of a single instance.
* **Color Themes**: Adding `ConsoleColor` support to the `RenderManager`.