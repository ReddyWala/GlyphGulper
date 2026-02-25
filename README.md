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
   git clone https://github.com/ReddyWala/Glyph-Gulper.git

2. **Navigate to the folder:**

   ```bash
   cd Glyph-Gulper

3. **Ensure you have the latest code:**

   ```bash
   git pull origin main

4. **Run the game:**

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

# 📂 Glyph-Gulper Project Structure

The **Glyph-Gulper** project follows a clean, modular architecture that separates game logic from data and utility. It is organized into a src/ folder for the application and a tests/ folder for logic validation, all orchestrated by a root Solution (.sln) file.

## 🏗️ Directory Hierarchy

```text
Glyph-Gulper/
├── src/
│   ├── Engine/                   # The "Brain". Contains Orchestration Logic
│   │   ├── GameEngine.cs         # Heart of the game (The Loop)
│   │   └── RenderManager.cs      # High-performance drawing engine
│   ├── Entities/                 # The "Actors". Contains Game Objects & State Managers
│   │   ├── Player.cs             # Player logic
│   │   ├── PlayerStateManager.cs # Manages hunger/mood transitions
│   │   ├── Food.cs               # Food logic
│   │   └── FoodStateManager.cs   # Manages food transitions
│   ├── Extensions/               # The "Toolbelt". Contains Helper Methods (Logic Add-ons)
│   │   ├── ConsoleExtensions.cs  # Positioning helpers
│   │   └── EnumExtensions.cs     # GetNextState & DisplayName logic
│   ├── Models/                   # The "Definitions". Contains Data Contracts
│   │   ├── Constants/            # Global Settings
│   │   │   └── GameConstants.cs  # Speeds, symbols, and grid sizes
│   │   └── Enums/                # State Definitions
│   │       ├── FoodState.cs      # Evolution tiers (Apple -> Bread -> Luxury)
│   │       ├── PlayerState.cs    # Vitality status (Happy, Neutral, Dead)
│   │       └── GameResult.cs     # Win/Loss/Quit states
│   └── Program.cs                # The "Ignition". Application Entry Point
├── tests/                        # Unit tests for engine logic
├── .editorconfig                 # Enforces strict coding standards across the project
├── .gitignore                    # Prevents /bin and /obj from being tracked
├── Glyph-Gulper.csproj           # .NET Project configuration
├── Glyph-Gulper.sln              # Workspace orchestrator and project linker
├── README.md                     # Project documentation
├── CHANGELOG.md                  # History of versions
└── LICENSE                       # MIT License

---

## 🌟 Credits & Acknowledgments

### 🛠️ Built With
* **[.NET 10.0](https://dotnet.microsoft.com/)** - The primary framework and runtime.
* **[C#](https://learn.microsoft.com/en-us/dotnet/csharp/)** - Utilizing modern features like String Interpolation, Enums with Display Attributes, and Record types.

### 🏛️ Architecture & Inspiration
* **Classic Arcade Games** - Core gameplay loop inspired by the "Snake" and "Pac-Man" era of terminal-based gaming.
* **[Microsoft Learn Portal](https://learn.microsoft.com/)** - Core architectural guidance on .NET project structure and coding conventions.

### 🤝 Contributions
* **Project Lead:** ReddyWala - *Core Engine and Logic*
* **Community & Tools:**
    * **AI Collaboration** - Refactoring assisted by **Gemini** and **GitHub Copilot**.
    * Microsoft's **dotnet-format** for code cleanliness.
    * The **.editorconfig** standard for cross-editor consistency.

---

## 🚀 Future Improvements
* **High Score System**: Persistence using `System.Text.Json`.
* **Multi-Food Spawning**: Modifying the engine to handle a `List<Food>` instead of a single instance.
* **Color Themes**: Adding `ConsoleColor` support to the `RenderManager`.

---
> *Glyph-Gulper is an open-source project created for the love of terminal-based UI and efficient C# design.*
