# OpenGSG AI Instructions

> **CRITICAL: Code Snippet Format Rules**
> 
> This instructions file should NOT contain triple backtick code fences (```), as they break VS Copilot Chat parsing.
> 
> When providing code snippets in chat responses:
> - Use language and full file path in code fence header: ```csharp ..\Path\To\File.cs
> - In explanatory text OUTSIDE code blocks, NEVER use backticks around filenames
> - Use **bold** for file names in explanations: **WorldLoader.cs**, **CwpCountry.cs**
> - This prevents VS from misinterpreting explanations as code snippet headers
> - Example file paths in markdown lists should use **bold**, not code format

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

### Parser/Scanner Rules
- Use `Scanner` → `Parser` → `ILookup<string, object>`
- Scanner handles: negative numbers, dates (YYYY.MM.DD), operators (>=, <, etc.)
- Parser appends operators to keys: `date>=` for `date >= 1950.01.09`
- **ALWAYS use InvariantCulture for number parsing** - No locale-dependent behavior
- Decimal numbers (0.007) returned as strings from Scanner
- Integers returned as int tokens
- `IsSimpleNumber()` validates integers and simple floats (not dates)

### Localization
- American English spelling: `LocalizationManager`, `localization/` folder
- File format: CSV with semicolons, UTF-8 with BOM
- Pattern: `key;ENGLISH;GERMAN;x`

### File Format Conventions
- **common/countries/{Name}.txt** - Static: tag, color
- **common/units/{unitType}.txt** - Unit definitions (strength, speed, costs)
- **history/countries/{TAG} - {Name}.txt** - Dynamic: leader, government, opinions, diplomacy
- **history/provinces/{id}-{name}.txt** - Province starting state
- **history/units/{TAG}.txt** - Country starting military forces
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

## Event System Architecture

### Country Events vs News Events
- **country_event**: Evaluates independently for each country (triggers + MTTH rolled per country)
- **news_event**: Evaluates globally once (triggers + MTTH rolled once), then filtered by recipients

### News Event Structure
- **trigger** block: Global conditions (date, etc.) - evaluated once per tick
- **recipients** block: Filter which countries receive the event - evaluated per country
- If no recipients specified, all countries receive the event
- Example: News event with date trigger fires globally, then recipients like `tag = USSR` filter who sees it

### Country Event Structure
- **trigger** block: Conditions evaluated independently for each country
- MTTH rolled separately for each country that passes triggers
- Example: Event with `tag = USA` only evaluates for USA

### Event Properties
- Events use `mean_time_to_happen = { days = X }` for randomized firing
- `trigger_only_once = yes` prevents re-firing
- `is_triggered_only = yes` means event ONLY fires when triggered by effects
- `hidden = yes` auto-executes first option (no UI)
- Event pictures load from **gfx/event_pictures/{name}.png**
- MTTH: probability = 1/days per tick (e.g., days=3 → ~33% chance per day)

### Event Evaluation Flow
1. **TickHandler.FinishTick()** calls **EventEvaluator.EvaluateAllEvents()**
2. Process scheduled events first (delayed triggers)
3. For **country events**:
   - Loop through all countries
   - Evaluate triggers in each country's context
   - Roll MTTH for each country that passes triggers
   - Show to player or auto-execute for AI
4. For **news events**:
   - Evaluate global triggers once (in player/first country context)
   - Roll MTTH once
   - If event fires:
     - Loop through all countries
     - Check recipients filter for each country
     - Show to player country (once) or auto-execute for AI countries

## Country Flags System

### Flag Operations
- **set_country_flag = flag_name** - Sets a flag on a country (persists for game session)
- **clr_country_flag = flag_name** - Clears a country flag
- **has_country_flag = flag_name** - Trigger checking if country has flag
- Flags stored in **Country.Flags** (HashSet<string>)
- Use for: event chains, one-time triggers, state tracking

### Scoped Flag Operations
Example: `USSR = { set_country_flag = my_flag }` sets flag on USSR
Example: `USA = { has_country_flag = my_flag }` checks USA's flag

### Implementation
- **SetCountryFlagEffect** - Executes in country context
- **ClearCountryFlagEffect** - Removes flag
- **HasCountryFlagTrigger** - Evaluates flag presence
- **CountryScopeTrigger** - Changes context to specific country for nested triggers
- **CountryScopeEffect** - Changes context to specific country for nested effects

## Conditional Effects & Random Triggers

### if/else Effects
Events can have conditional branching based on triggers:
- **if** block contains **limit** (conditions) and effects
- **else** block contains alternative effects
- Parsed in **GameEvent.ParseEffectsFromBlock()**
- Uses **ConditionalEffect** class

Example pattern:
- if = { limit = { random < 0.7 } effects }
- else = { alternative_effects }

### Random Triggers
- **random < X** - Probability trigger (0.0-1.0 range)
- Operators supported: `<`, `<=`, `>`, `>=`, `==`
- Injectable **IRandom** interface for deterministic testing
- Uses **RandomTrigger** class
- Parser recognizes `random<` syntax (operator appended to key)

### Immediate Effects Block
- **immediate = { }** - Effects execute BEFORE UI display
- Used for routing logic, preprocessing, hidden state changes
- Executes in **EventEvaluator.HandleEventFiring()** before showing dialog
- Common pattern: Router events with immediate if/else branching

## Delayed Event Triggering

### Scheduling Events
Effects can schedule future events with delay:
- Syntax: `country_event = { id = event.2 days = 21 }`
- Syntax: `news_event = { id = event.3 days = 7 }`
- Uses **TickHandler.ScheduleEvent(eventId, countryTag, days, isNewsEvent)**

### Implementation
- **ScheduledEvent** class stores: EventId, TargetCountryTag, FireDate, IsNewsEvent
- **TickHandler._scheduledEvents** list holds pending events
- **ProcessScheduledEvents()** fires due events each tick
- Parsed in **TriggerEventEffect** via `Days` property

### Use Cases
- Delayed consequences (reactions take time)
- Chain events with natural gaps
- Simulate travel time, news propagation

## Military System (Refactored)

### Deleted Classes (DO NOT USE)
- ❌ **Army** - Replaced by MilitaryFormation
- ❌ **ArmyManager** - Functionality moved to loaders and Country.Military
- ❌ **Branch** - No longer needed
- ❌ **Division** - No longer needed
- ❌ **Province.Units** property - Removed, units stored in Country.Military

### New Architecture
- **Unit** - Unit type definition from **common/units/*.txt**
  - Properties: `TypeId`, `Type` (army/air), `Strength`, `Speed`, `ProductionCost`, `MaintenanceCost`
  - Loaded via **UnitLoader.LoadUnits()**
- **MilitaryFormation** - Army or air force group at a location
  - Properties: `Branch`, `Location`, `Units` (Dictionary<unitType, count>), `Readiness`
  - Methods: `GetTotalStrength()`, `GetMaintenanceCost()`
- **NationMilitary** - Country's military forces
  - Properties: `Tag`, `Armies` (List<MilitaryFormation>), `AirForces` (List<MilitaryFormation>)
  - Methods: `GetTotalStrength()`, `GetTotalMaintenanceCost()`
  - Loaded via **MilitaryLoader.LoadMilitaries()**
- **Country.Military** - Reference to NationMilitary
- **Country.MilitaryStrength** - Calculated total strength (base class property)

### Unit Definition File Format
Example from **common/units/infantry.txt**:
- type = army
- strength = 2
- speed = 10
- production_cost = 4
- maintenance_cost = 1

### Military History File Format
Example from **history/units/USA.txt**:
- army = { location = 10  infantry = 6  armor = 3  readiness = active }
- air_forces = { location = 10  fighter = 5  readiness = ready }

### Loading Flow
1. **WorldLoader.LoadUnitDefinitions()** loads from **common/units/**
2. **WorldLoader.LoadMilitaries()** loads from **history/units/**
3. Attach **NationMilitary** to **Country.Military**
4. Calculate **Country.MilitaryStrength** using unit definitions

### Querying Units in Province
Query pattern (no longer stored in Province):
- `country.Military?.Armies.Where(a => a.Location == provinceId)`
- **ArmyList** view helper provides **GetFormationsInProvince()**

## Testability Patterns

### Dependency Injection
- Use interfaces for external dependencies:
  - **IEventTriggerNotifier** - for event UI notifications
  - **IRandom** - for random number generation
- Production implementations: **TickHandlerEventNotifier**, **SystemRandom**
- Test implementations: **TestEventNotifier**, **DeterministicRandom**, **SeededRandom**

### Deterministic Testing
- ALWAYS use **DeterministicRandom** or **SeededRandom** for tests involving randomness
- Document expected values when using seeded randoms (e.g., "with seed 12345, produces exactly 8 fires")
- Never write non-deterministic tests (no random variance in assertions)
- Use reflection for testing private properties when necessary

### Unit Test Structure (NUnit)
- Use **[TestFixture]** for test classes
- Use **[Test]** for test methods
- Assertion style: `Assert.That(actual, Is.EqualTo(expected))`
- Test naming: `MethodName_Scenario_ExpectedBehavior`
- Test pattern: Arrange-Act-Assert with clear comments

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
- Game-specific triggers override `ParseSingleTrigger()`
- All triggers implement `IEventTrigger.Evaluate(object context)`

### Event Effects
- Base effects in **OpenGSGLibrary.Events**
- Game-specific effects override `ParseSingleEffect()`
- All effects implement `IEventEffect.Execute(object context)`
- Scoped effects use context switching: **CountryScopeEffect**

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
- `IsTickComplete()` gates tick advancement

## UI Patterns

### Province Selection (Pin-on-Click)
- **MainWindow** tracks `_pinnedProvinceId` state
- First click: Pin to province (views stop updating on hover)
- Second click on same province: Unpin (resume hover updates)
- Pattern allows detailed inspection without mouse interference

### View Helpers
- **ProvinceInfo**, **CountryInfo**, **ArmyList**, **DiplomacyInfo**, **ActiveCountryInfo**
- Subscribe to **MouseController** events: **HoveredProvinceChanged**, **ProvinceClicked**
- Respect pin state before updating

### Mouse Controller
- Converts screen coordinates to map coordinates using scaling factor
- Reads province ID from pixel color: `GetPixelRgb()` → `GetProvinceNumber()`
- Raises events: **HoveredProvinceChanged**, **ProvinceClicked**, **HoveredCountryChanged**
- Refactored helper: `GetProvinceIdAtMousePosition()` for DRY

## Country Stats in History Files
- **soft_power** (0-100) - Cultural influence and diplomatic effectiveness (default: 50)
- **unrest** (0-100) - Internal stability, higher = more unstable (default: 0)
- **civil_tech** (0-100) - Civilian industrial/scientific advancement (default: 50)
- **military_tech** (0-100) - Military equipment/doctrine advancement (default: 50)
- **military_strength** (0-100) - Optional override, normally calculated from armies

## Province Cores
- **add_core = TAG** - Add a core claim (parsed in Province.SetData)
- Used for: territorial claims, unrest modifiers, casus belli, liberation
- Owner is typically (but not always) included in cores
- Multiple claimants represent contested territories (Korea, China, Vietnam)

## Code Organization Rules
- One class per file (including small helper classes like EventArgs)
- Event args classes: **TickEventArgs.cs**, **GameEventTriggeredEventArgs.cs**, **ProvinceEventArgs.cs**
- Interfaces: **IEventTriggerNotifier.cs**, **IRandom.cs**
- Test implementations: Alongside interfaces or in test projects

## Paradox Compatibility
Match Paradox syntax where possible:
- `give_guarantee = TAG` (unidirectional)
- `declare_war = { target = TAG }` (initiator stored)
- `create_alliance = TAG` (bidirectional display)
- `add_opinion = { target = TAG value = -50 }` (NOT `opinion`)
- `set_country_flag = flag_name`
- `country_event = { id = X days = Y }` (delayed triggers)

## Don't Do This
- ❌ Add game-specific code to OpenGSGLibrary
- ❌ Hard-code file paths or formats
- ❌ Throw exceptions for missing optional data
- ❌ Use British spelling in code (use American in data files if matching Paradox)
- ❌ Use `volatile` for shared state (use locks instead)
- ❌ Use backticks for filenames in explanatory text when code snippets are present
- ❌ Use **trigger** block for news event recipients (use **recipients** instead)
- ❌ Write non-deterministic tests
- ❌ Use static dependencies in testable code
- ❌ Put multiple classes in one file
- ❌ Use `Random` directly in production code (use `IRandom` interface)
- ❌ Parse numbers without `InvariantCulture` (causes locale bugs)
- ❌ Use Army, ArmyManager, Branch, Division classes (deleted - use new military system)
- ❌ Use `ArmyManager` or `WorldState.GetArmyManager()` (removed)
- ❌ Store units in `Province.Units` (removed - units in `Country.Military`)
- ❌ Use `GetValueOrDefault` on IDictionary (use `TryGetValue` instead)
- ❌ Use triple backtick code fences in this instructions file (breaks VS Chat parser)

## Current Focus
Event system (flags, conditional effects, delayed triggers), military system refactoring, UI polish (pin-on-click), and data loading infrastructure.