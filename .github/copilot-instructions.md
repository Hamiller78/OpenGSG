# OpenGSG AI Instructions

> **Note for AI assistants**: This instructions file itself should NOT contain triple backtick code fences (```), as they break VS Copilot Chat parsing. Use bullet lists with **bold** formatting instead.

## Project Overview
Grand strategy game engine inspired by Paradox Interactive games (HOI4, EU4, CK3).
Built with .NET 10, C# 14.0, Windows Forms.

## Code Style & Conventions

### Naming
- Private fields: `_camelCase` with leading underscore
- Parameters: `camelCase` (e.g., `gamedataPath`)
- Classes: `PascalCase`
- Interfaces: `IPascalCase`

### Architecture Rules
1. **OpenGSGLibrary**: Generic GSG features only (no game-specific code)
2. **ColdWarGameLogic**: Game-specific implementations
3. **ColdWarPrototype**: UI layer (WinForms adapters)

### Data Loading Pattern
All loadable entities use `GameObject.SetData(string fileName, ILookup<string, object> parsedData)`:
- ALWAYS check `if (parsedData.Contains("key"))` before accessing
- Provide sensible defaults for missing data
- Support two-stage loading (call SetData twice for merging)

### Parser/Scanner
- Use `Scanner` → `Parser` → `ILookup<string, object>`
- Scanner handles: negative numbers, dates (YYYY.MM.DD), operators (>=, <, etc.)
- Parser appends operators to keys: `date>=` for `date >= 1950.01.09`

### Localization
- American English spelling: `LocalizationManager`, `localization/` folder
- File format: CSV with semicolons, UTF-8 with BOM
- Pattern: `key;ENGLISH;GERMAN;x`

### File Format Conventions
- **common/countries/{Name}.txt** - Static: tag, color
- **history/countries/{TAG} - {Name}.txt** - Dynamic: leader, government, opinions, diplomacy
- **history/provinces/{id}-{name}.txt** - Province starting state
- **localization/*.csv** - UTF-8 with BOM, semicolon-separated
- **events/*.txt** - Event definitions
- **gfx/event_pictures/{name}.png** - Event images
- **gfx/flags/{TAG}.png** - Country flags

### Threading & Simulation
- Use **GameSimulationThread** for background tick processing
- All UI updates from tick events must use `InvokeRequired` and `BeginInvoke`
- Static events (**TickHandler.EventTriggered**, **TickHandler.UIRefreshRequested**) marshal to UI thread
- Lock pattern preferred over `volatile` for thread safety
- `IsTickComplete()` gates tick advancement

### Code Snippet Format for Visual Studio
When providing code snippets:
- Use language and full file path in code fence header
- In explanatory text OUTSIDE code blocks, avoid backticks around filenames
- Instead use **bold** for file names: **WorldLoader.cs**, **CwpCountry.cs**
- This prevents VS from misinterpreting explanations as code snippet headers

## Common Patterns

### Adding New Country Properties
1. Add to **CwpCountry** (game-specific) or **Country** (generic)
2. Load in SetData() with null checks
3. Use American spelling for keywords: `add_opinion` not `add_opinion_modifier`

### Event System Patterns
- Base event classes in **OpenGSGLibrary.Events**
- Game-specific events inherit from **CountryEvent** or **NewsEvent**
- Triggers implement `IEventTrigger.Evaluate(object context)`
- Effects implement `IEventEffect.Execute(object context)`
- Modal **EventDialog** pauses simulation automatically

### Event Triggers
- Base triggers in **OpenGSGLibrary.Events**
- Game-specific triggers override ParseSingleTrigger()
- All triggers implement `IEventTrigger.Evaluate(object context)`

### Event Properties
- Events use `mean_time_to_happen = { days = X }` for randomized firing
- `trigger_only_once = yes` prevents re-firing
- `hidden = yes` auto-executes first option (no UI)
- Event pictures load from gfx/event_pictures/{name}.png
- MTTH: probability = 1/days per tick (e.g., days=3 → ~33% chance per day)

### Diplomatic Relations
- Base: **WarRelation**, **AllianceRelation** (OpenGSGLibrary)
- Game-specific: **GuaranteeRelation** (ColdWarGameLogic)
- Display bidirectionally but store unidirectionally

### Opinions
- Use `add_opinion = { target = TAG value = -50 }` (NOT `opinion`)
- Range: -100 (hostile) to +100 (friendly)
- Bidirectional but asymmetric (USA→USSR ≠ USSR→USA)
- Supports future **OpinionModifier** system with reasons and expiry dates

### Simulation Threading
- **GameSimulationThread** runs on background thread
- UI updates via static events: **EventTriggered**, **UIRefreshRequested**
- Lock all shared state access (don't use `volatile`)
- IsTickComplete() gates tick advancement

## Country Stats in History Files
- **soft_power** (0-100) - Cultural influence and diplomatic effectiveness (default: 50)
- **unrest** (0-100) - Internal stability, higher = more unstable (default: 0)
- **tech_level** (0-100) - Industrial/scientific advancement (default: 50)
- **military_strength** (0-100) - Optional override, normally calculated from armies/economy

## Paradox Compatibility
Match Paradox syntax where possible:
- `give_guarantee = TAG` (unidirectional)
- `declare_war = { target = TAG }` (initiator stored)
- `create_alliance = TAG` (bidirectional display)
- `add_opinion = { target = TAG value = -50 }` (NOT `opinion`)

## Don't Do This
- ❌ Add game-specific code to OpenGSGLibrary
- ❌ Hard-code file paths or formats
- ❌ Throw exceptions for missing optional data
- ❌ Use British spelling in code (use American in data files if matching Paradox)
- ❌ Use `volatile` for shared state (use locks instead)
- ❌ Use backticks for filenames in explanatory text when code snippets are present

## Current Focus
Building event system, diplomacy (guarantees, opinions), simulation threading, and data loading infrastructure.