# Season-Level Aggregation Architecture

## Overview

FootballDataTool now supports **dual-view architecture**:
1. **Match-Centric View** (original) - List of matches for simple analysis
2. **Team-Centric View** (new) - Aggregated team seasons for rich analysis

This gives you the best of both worlds: **CSV-friendly simplicity** with **object-oriented richness** when you need it.

---

## Architecture Diagram

```
CSV File
    ↓
CsvDataService.LoadSeasonDataFromFile()
    ↓
SeasonData (top-level container)
    ├── Matches (List<Match>)           ← Match-centric view
    ├── Teams (Dict<string, TeamSeason>) ← Team-centric view (aggregated)
    └── Metadata (SeasonMetadata)
```

### Key Design Principle: **Progressive Enhancement**

- ✅ Matches remain match-centric with **string team names** (CSV-friendly)
- ✅ Optional aggregation layer builds **rich team objects** from matches
- ✅ No breaking changes - existing code works as-is
- ✅ Team objects built on-demand via `SeasonData.BuildTeamAggregates()`

---

## Core Models

### 1. SeasonData (New)

Top-level container for all season data.

```csharp
public class SeasonData
{
    public SeasonMetadata Metadata { get; set; }
    public List<Match> Matches { get; set; }
    public Dictionary<string, TeamSeason> Teams { get; private set; }
    
    // Factory method
    public static SeasonData FromMatches(List<Match> matches, SeasonMetadata metadata);
    
    // Team access
    public TeamSeason? GetTeam(string teamName);
    
    // Season-wide statistics
    public int TotalMatches { get; }
    public double AverageGoalsPerMatch { get; }
    public List<TeamRecord> GetCurrentTable();
    public List<TeamRecord> GetTableAtGameweek(int gameweek);
    
    // Rich queries
    public List<Match> GetHeadToHeadMatches(string team1, string team2);
    public List<(string Team, int InjuryCount)> GetTeamsByInjuries();
    public List<(Player Player, Match Match)> GetMatchDayBirthdays();
    public Dictionary<string, int> GetZodiacDistribution();
    public List<(Player Player, string Team, int Goals)> GetTopScorers(int count = 10);
    public List<(Player Player, string Team, int Assists)> GetTopAssisters(int count = 10);
    public List<(Player Player, string Team, int Goals, int Assists, int Total)> GetTopContributors(int count = 10);
}
```

### 2. TeamSeason (New)

Represents a team's complete season with aggregated data.

```csharp
public class TeamSeason
{
    public string Name { get; set; }
    
    // Match collections
    public List<Match> HomeMatches { get; set; }
    public List<Match> AwayMatches { get; set; }
    public List<Match> AllMatches { get; }
    
    // Squad data (aggregated from lineups)
    public List<Player> FullSquad { get; set; }
    public List<Manager> ManagerHistory { get; set; }
    public Stadium? HomeStadium { get; set; }
    
    // Transfer data (optional - from JSON)
    public TeamSeasonInfo? SeasonInfo { get; set; }
    
    // Season statistics
    public int TotalPoints { get; }
    public int GoalsScored { get; }
    public int GoalDifference { get; }
    public double? AverageSquadAge { get; }
    public double? AverageHomeAttendance { get; }
    
    // Rich analysis methods
    public List<(Player Player, int Appearances)> MostUsedPlayers();
    public List<(Player Player, int Goals)> TopScorers();
    public List<(Player Player, int Assists)> TopAssisters();
    public List<(Player Player, int Goals, int Assists, int Total)> TopContributors();
    public List<Injury> AllInjuries();
    public List<string> FormGuide(int lastNMatches = 5);
    public List<(int Gameweek, int CumulativePoints)> PointsProgression();
}
```

### 3. Match (Enhanced)

Matches can now optionally reference their parent season for team object access.

```csharp
public class Match
{
    public string HomeTeam { get; set; }  // Still a string (CSV-friendly)
    public string AwayTeam { get; set; }
    
    internal SeasonData? ParentSeason { get; set; }  // Optional link
    
    // New convenience methods
    public TeamSeason? GetHomeTeamObject();
    public TeamSeason? GetAwayTeamObject();
}
```

---

## Usage Examples

### 1. Simple Match-Centric Analysis (Original Way)

```csharp
// Load matches (works as before)
var csvService = new CsvDataService();
var matches = csvService.LoadFromFile("data/premier_league_2023-24.csv");

// Analyze matches directly
var arsenalMatches = matches.Where(m => 
    m.HomeTeam == "Arsenal" || m.AwayTeam == "Arsenal").ToList();

var totalGoals = matches.Sum(m => m.HomeGoals + m.AwayGoals);
```

### 2. Rich Team-Centric Analysis (New Way)

```csharp
// Load season data (new method)
var csvService = new CsvDataService();
var seasonData = csvService.LoadSeasonDataFromFile("data/premier_league_2023-24.csv");

// Access team objects
var arsenal = seasonData.GetTeam("Arsenal");
Console.WriteLine($"Arsenal: {arsenal.TotalPoints} points, {arsenal.GoalsScored} goals");

// Squad analysis
var oldestPlayer = arsenal.OldestPlayer;
var youngSquad = seasonData.Teams.Values
    .OrderBy(t => t.AverageSquadAge)
    .Take(5);

// Most used players
var regularStarters = arsenal.MostUsedPlayers().Take(11);
foreach (var (player, appearances) in regularStarters)
{
    Console.WriteLine($"{player.Name}: {appearances} starts");
}

// Top scorers and assisters
var topScorers = arsenal.TopScorers().Take(5);
foreach (var (player, goals) in topScorers)
{
    Console.WriteLine($"{player.Name}: {goals} goals");
}

var topAssisters = arsenal.TopAssisters().Take(5);
foreach (var (player, assists) in topAssisters)
{
    Console.WriteLine($"{player.Name}: {assists} assists");
}

// Top contributors (goals + assists)
var topContributors = arsenal.TopContributors().Take(5);
foreach (var (player, goals, assists, total) in topContributors)
{
    Console.WriteLine($"{player.Name}: {goals}G + {assists}A = {total}");
}

// Form guide
var recentForm = arsenal.FormGuide(5);  // ["W", "W", "D", "L", "W"]
Console.WriteLine($"Last 5: {string.Join(", ", recentForm)}");
```

### 3. Cross-Team Comparisons

```csharp
// Injury impact analysis
var injuryTable = seasonData.GetTeamsByInjuries();
foreach (var (team, count) in injuryTable.Take(5))
{
    Console.WriteLine($"{team}: {count} injuries");
}

// Top scorers across the league
var topScorers = seasonData.GetTopScorers(10);
foreach (var (player, team, goals) in topScorers)
{
    Console.WriteLine($"{player.Name} ({team}): {goals} goals");
}

// Top assisters across the league
var topAssisters = seasonData.GetTopAssisters(10);
foreach (var (player, team, assists) in topAssisters)
{
    Console.WriteLine($"{player.Name} ({team}): {assists} assists");
}

// Top contributors (goals + assists)
var topContributors = seasonData.GetTopContributors(10);
foreach (var (player, team, goals, assists, total) in topContributors)
{
    Console.WriteLine($"{player.Name} ({team}): {goals}G + {assists}A = {total}");
}

// Squad age comparison
var youngestSquads = seasonData.Teams.Values
    .Where(t => t.AverageSquadAge.HasValue)
    .OrderBy(t => t.AverageSquadAge)
    .Take(5);

// Best home attendance
var bestAttendance = seasonData.Teams.Values
    .Where(t => t.AverageHomeAttendance.HasValue)
    .OrderByDescending(t => t.AverageHomeAttendance)
    .Take(5);
```

### 4. Match-to-Team Navigation (Hybrid)

```csharp
// Start with a match
var match = seasonData.Matches.First();

// Get team objects for that match
var homeTeam = match.GetHomeTeamObject();
var awayTeam = match.GetAwayTeamObject();

// Rich analysis on those teams
Console.WriteLine($"Home team season points: {homeTeam?.TotalPoints}");
Console.WriteLine($"Away team form: {string.Join(", ", awayTeam?.FormGuide(5) ?? new())}");
```

### 5. Transfer Data Integration

```csharp
// Load season with transfer data
var seasonData = csvService.LoadSeasonDataFromFile("data/premier_league_2023-24.csv");
seasonData.LoadTransferData(
    "data/arsenal_2023-24_transfers.json",
    "data/chelsea_2023-24_transfers.json"
);

// Access transfer data
var arsenal = seasonData.GetTeam("Arsenal");
if (arsenal?.SeasonInfo != null)
{
    Console.WriteLine($"Net spend: {arsenal.SeasonInfo.NetSpend:C}");
    Console.WriteLine($"Signings: {arsenal.SeasonInfo.SummerSignings.Count}");
}
```

---

## Benefits of This Architecture

### ✅ Keeps CSV Simplicity
- Matches still use **string team names** (not complex objects)
- CSV parsing remains **fast and simple**
- **No memory duplication** in match list

### ✅ Adds Team-Level Richness
- Full **team objects with aggregated data**
- **Type-safe team references** when needed
- **Rich queries** across team data

### ✅ Progressive Enhancement
- Works with **minimal CSV data** (4 columns)
- Optional **extended data** (lineups, injuries, etc.)
- Optional **transfer data** (JSON)
- Optional **team aggregations** (built on-demand)

### ✅ Backward Compatible
- Existing code works without changes
- `LoadFromFile()` still returns `List<Match>`
- New `LoadSeasonDataFromFile()` returns `SeasonData`

### ✅ Efficient
- Team objects built **once** from matches
- **Dictionary lookup** for fast team access
- **Lazy evaluation** for computed properties
- **No data duplication** (teams reference same match objects)

---

## Migration Guide

### Existing Code (No Changes Needed)

```csharp
// This still works exactly as before
var matches = csvService.LoadFromFile("data/season.csv");
var analyzer = new MatchAnalyzer(matches);
var standings = analyzer.GetStandings();
```

### Enhanced Code (Opt-In)

```csharp
// Use new method for team-level analysis
var seasonData = csvService.LoadSeasonDataFromFile("data/season.csv");

// Access matches as before
foreach (var match in seasonData.Matches) { }

// Or use team-centric view
var arsenal = seasonData.GetTeam("Arsenal");
```

---

## Performance Characteristics

| Operation | Time Complexity | Notes |
|-----------|----------------|-------|
| Load CSV | O(n) | Same as before |
| Build team aggregates | O(n × m) | n=matches, m=avg squad size |
| Get team by name | O(1) | Dictionary lookup |
| Team statistics | O(1) | Pre-computed |
| Most used players | O(n log n) | n=matches for team |

**Memory**: 
- Match list: ~100 bytes/match × 380 matches = ~38KB
- Team objects: ~20 teams × ~2KB = ~40KB
- **Total overhead: ~40KB** (negligible)

---

## Design Rationale

### Why Not Make Match.HomeTeam/AwayTeam Objects?

**Considered:**
```csharp
public class Match
{
    public Team HomeTeam { get; set; }  // Rich object
    public Team AwayTeam { get; set; }
}
```

**Problems:**
1. ❌ CSV is flat - teams are hierarchical (impedance mismatch)
2. ❌ Team objects duplicated 38 times per season (memory waste)
3. ❌ Breaks progressive enhancement (requires full team data)
4. ❌ Complex CSV parsing (need to build teams first)

**Our Solution:**
```csharp
public class Match
{
    public string HomeTeam { get; set; }  // Simple string
    public TeamSeason? GetHomeTeamObject() { }  // Optional rich object
}
```

**Benefits:**
1. ✅ CSV-friendly (strings map naturally)
2. ✅ No duplication (team objects built once)
3. ✅ Progressive enhancement (strings work with minimal data)
4. ✅ Simple CSV parsing (strings are trivial)
5. ✅ Optional richness (team objects on-demand)

---

## See Also

- [ARCHITECTURE.md](ARCHITECTURE.md) - Original discrete CSV architecture
- [EXTENDED-DATA-GUIDE.md](EXTENDED-DATA-GUIDE.md) - Extended match data formats
- [TRANSFER-MANAGER-AGE-GUIDE.md](TRANSFER-MANAGER-AGE-GUIDE.md) - Transfer data integration
- [INJURY-PLAYER-TRACKING-GUIDE.md](INJURY-PLAYER-TRACKING-GUIDE.md) - Player profiling features
