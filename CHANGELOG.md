# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-23

### Added
- **Core Engine**: Implemented asynchronous game loop in `GameEngine.cs`.
- **Thread-Safe Rendering**: Created `RenderManager` using `ConcurrentQueue` to prevent console flickering and cursor race conditions.
- **Player Mechanics**: Added `Player` class with boundary clamping and "Mood" state logic `(^-^)` vs `(X_X)`.
- **Food System**: Created `Food` entity with randomized spawning and "Evolution" scaling every 5 snacks.
- **UI & HUD**: Added a status bar with a real-time `Stopwatch` timer, score counter, and missed-item tracker.
- **Project Infrastructure**: Added `.gitignore`, `tasks.json`, and `README.md`.

### Changed
- Include a dedicated UI heartbeat timer for consistent clock updates.
- Optimized sprite rendering using the "Clear-before-Move" pattern to eliminate ghosting trails.

### Fixed
- Resolved thread-safety issues where the cursor would jump during simultaneous movement and score updates.