# 🕹️ Terminal GlyphGulper 2026

A high-performance, **multi-threaded** .NET console game built with modern C# practices. Navigate your avatar, eat snacks to evolve, and manage your "mood" before time runs out.



## ✨ Technical Highlights

* **Thread-Safe Rendering**: Uses a `BlockingCollection<Action>` producer-consumer pattern to prevent console cursor collisions and flickering.
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
   git clone https://github.com/ReddyWala/GlyphGulper.git

2. **Navigate to the folder:**

   ```bash
   cd GlyphGulper

3. **Ensure you have the latest code:**

   ```bash
   git pull origin main

4. **Run the game:**

   Option A: Run from the Root (Recommended):

   ```bash
   dotnet run --project src/GlyphGulper/GlyphGulper.csproj
   ```

   Option B: Run from the Project Folder:

   ```bash
   cd src/GlyphGulper
   dotnet run
   ```

   If the game doesn't start or looks strange, ensure your terminal supports ANSI Escape Codes (standard in VS Code, Windows Terminal, and macOS/Linux shells).If you've just renamed the project, you might need to perform a quick cleanup:

   ```bash
   dotnet clean && dotnet run --project src/GlyphGulper/GlyphGulper.csproj
   ```

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

# 📂 GlyphGulper Project Structure

The **GlyphGulper** project follows a clean, modular architecture that separates game logic from data and utility. It is organized into a src/ folder for the application and a tests/ folder for logic validation, all orchestrated by a root Solution (.sln) file.

## 🏗️ Directory Hierarchy

```text
GlyphGulper/
├── src/
│   └── GlyphGulper/                         # Main Project Folder
│       ├── Engine/                          # COORDINATION LAYER
│       │   ├── IGameEngine.cs               # Contract for the core game loop and state tracking
│       │   └── GameEngine.cs                # Central orchestrator; manages timing, input, and win/loss logic
│       ├── Entities/                        # DOMAIN LAYER (The "What")
│       │   ├── Food/
│       │   │   ├── IFood.cs                 # Contract for consumable objects
│       │   │   ├── Food.cs                  # Implementation of food behavior, respawning, and rendering
│       │   │   └── FoodStateManager.cs      # Logic for tier-based evolution (Apple -> Bread -> Luxury)
│       │   └── Player
│       │       ├── IPlayer.cs               # Contract for the user-controlled entity
│       │       ├── Player.cs                # Handles movement logic and sprite selection
│       │       └── PlayerStateManager.cs    # Manages hunger-driven mood transitions (Happy -> Dead)
│       ├── Extensions/                      # UTILITY LAYER
│       │   └── EnumExtensions.cs            # Helper methods for state cycling and metadata retrieval
│       ├── Models/                          # DATA LAYER (The "Specs")
│       │   ├── Constants/                   # Single Source of Truth for magic numbers and strings
│       │   │   └── GameConstants.cs         # Config for grid sizes, symbols, and game-balancing values
│       │   └── Enums/                       # Strongly-typed state definitions
│       │       ├── FoodState.cs             # Evolution tiers (Apple -> Bread -> Luxury)
│       │       ├── PlayerState.cs           # Vitality status (Happy, Neutral, Dead)
│       │       └── GameResult.cs            # Terminal game states (Win/Loss/Quit)
│       ├── Services/                        # INFRASTRUCTURE LAYER (The "How")
│       │   ├── Input/                       # Hardware abstraction for keyboard interaction
│       │   │   ├── IConsole.cs              # Mockable wrapper for System.Console
│       │   │   ├── IInputService.cs         # Non-blocking input polling logic
│       │   │   ├── ConsoleInputService.cs   # Wraps keyboard hardware access
│       │   │   └── WindowsConsole.cs        # OS-specific console implementation
│       │   ├── Rendering/                   # High-performance drawing
│       │   │   ├── IRenderManager.cs        # Contract to manage thread-safe drawing
│       │   │   └── RenderManager.cs         # Thread-safe, queue-based drawing for flicker-free visuals       
│       │   ├── Resolution/                  # Environmental awareness
│       │   │   ├── IResolutionManager.cs    # Contract to get coordinate boundaries and detect resizing events
│       │   │   └── ResolutionManager.cs     # Detects console resizing and boundary constraints
│       │   └── Timer/                       # Temporal logic
│       │        ├── IGameTimer.cs           # Contract for mockable, manual-tick timers
│       │        ├── IGameTimerFactory.cs    # Factory to decouple engine from timer instantiation
│       │        ├── GameTimer.cs            # High-precision timer driven by the main loop delta
│       │        └── GameTimerFactory.cs     # Production implementation of the timer creator
│       ├── GlyphGulper.csproj               # .NET Project configuration
│       └── Program.cs                       # The "Ignition". Configures DI and ignites the engine
├── tests/                                   # VERIFICATION LAYER
│   └── GlyphGulper.Tests/                   # Unit Testing Suite
│       ├── GlyphGulper.Tests.csproj         # Verifies food evolution logic
│       ├── FoodStateManagerTests.cs         # Ensures the FoodStateManager transitions correctly through its tiers
│       ├── GameEngineTestFactory.cs         # "Object Mother" for assembling engines with mocks
│       ├── GameEngineTests.cs               # Tests win/loss conditions and collision outcomes
│       ├── GameEngineTimerTests.cs          # Verifies engine response to temporal events
│       ├── GameTimerTestFactory.cs          # The "Object Mother" for assembling engines with mocks
│       ├── MockTimer.cs                     # Test-double for manual timer triggering
│       └── PlayerStateManagerTest.cs        # Verifies mood/vitality state transitions
├── .editorconfig                            # Enforces strict coding standards
├── .gitignore                               # Prevents /bin and /obj tracking
├── GlyphGulper.sln                          # Workspace orchestrator
├── README.md                                # Project documentation
├── CHANGELOG.md                             # History of versions
└── LICENSE                                  # MIT License
```

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
> *GlyphGulper is an open-source project created for the love of terminal-based UI and efficient C# design.*
