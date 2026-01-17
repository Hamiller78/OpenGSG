# OpenGSG AI Instructions

## Project Overview
Grand strategy game engine inspired by Paradox Interactive games (HOI4, EU4, CK3).
Built with .NET 10, C# 14.0, Windows Forms.

## Code Style & Conventions

### Naming
- Private fields: `camelCase_` with trailing underscore
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

## Common Patterns

### Adding New Country Properties
1. Add to `CwpCountry` (game-specific) or `Country` (generic)
2. Load in `SetData()` with null checks
3. Use American spelling for keywords: `add_opinion` not `add_opinion_modifier`

### Event Triggers
- Base triggers in `OpenGSGLibrary.Events`
- Game-specific triggers override `ParseSingleTrigger()`
- All triggers implement `IEventTrigger.Evaluate(object context)`

### Diplomatic Relations
- Base: `WarRelation`, `AllianceRelation` (OpenGSGLibrary)
- Game-specific: `GuaranteeRelation` (ColdWarGameLogic)
- Display bidirectionally but store unidirectionally

## Paradox Compatibility
Match Paradox syntax where possible:
- `give_guarantee = TAG`
- `declare_war = { target = TAG }`
- `add_opinion = { target = TAG value = -50 }`

## Don't Do This
- ❌ Add game-specific code to OpenGSGLibrary
- ❌ Hard-code file paths or formats
- ❌ Throw exceptions for missing optional data
- ❌ Use British spelling in code (use in data files if matching Paradox)

## Current Focus
Building event system, diplomacy (guarantees, opinions), and data loading infrastructure.
See ARCHITECTURE.md for detailed design decisions.

### Threading & Simulation
- Use `GameSimulationThread` for background tick processing
- All UI updates from tick events must use `InvokeRequired` and `BeginInvoke`
- Static events (`TickHandler.EventTriggered`, `TickHandler.UIRefreshRequested`) marshal to UI thread
- Lock pattern preferred over `volatile` for thread safety

### Event System
- Events use `mean_time_to_happen = { days = X }` for randomized firing
- `trigger_only_once = yes` prevents re-firing
- `hidden = yes` auto-executes first option (no UI)
- Event pictures load from `gfx/event_pictures/{name}.png`
- MTTH: probability = 1/days per tick (e.g., days=3 → ~33% chance per day)

### Opinions
- Use `add_opinion = { target = TAG value = -50 }` (NOT `opinion`)
- Range: -100 (hostile) to +100 (friendly)
- Bidirectional but asymmetric (USA→USSR ≠ USSR→USA)
- Supports future `OpinionModifier` system with reasons and expiry dates

### Event System Patterns
- Base event classes in `OpenGSGLibrary.Events`
- Game-specific events inherit from `CountryEvent` or `NewsEvent`
- Triggers implement `IEventTrigger.Evaluate(object context)`
- Effects implement `IEventEffect.Execute(object context)`
- Modal `EventDialog` pauses simulation automatically

### Simulation Threading
- `GameSimulationThread` runs on background thread
- UI updates via static events: `EventTriggered`, `UIRefreshRequested`
- Lock all shared state access (don't use `volatile`)
- `IsTickComplete()` gates tick advancement