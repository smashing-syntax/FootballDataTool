# Tournament Support - Feature Summary

## 🏆 Overview

FootballDataTool now fully supports **knockout tournaments** in addition to league/season data. You can track World Cup, Champions League, Europa League, domestic cups, and any other tournament format with group stages and/or knockout rounds.

---

## ✨ What's New

### 1. **Tournament Stage Model** (`TournamentStage.cs`)

New enum and extension methods for tournament stages:

- **Stages**: `League`, `GroupStage`, `RoundOf64`, `RoundOf32`, `RoundOf16`, `QuarterFinal`, `SemiFinal`, `ThirdPlacePlayoff`, `Final`
- **Display names**: "Group Stage", "Round of 16", "Quarter-Final", etc.
- **Short codes**: `GS`, `R16`, `QF`, `SF`, `F`, `3P`
- **Flexible parsing**: Accepts multiple formats (`R16`, `ROUND OF 16`, `LAST 16`, etc.)
- **Knockout detection**: `IsKnockout()` method to identify elimination rounds

### 2. **Match Model Updates** (`Match.cs`)

Three new properties added to `Match`:

```csharp
public TournamentStage Stage { get; set; } = TournamentStage.League;
public string? Group { get; set; }
public int? Leg { get; set; }
```

Plus convenient helper properties:
- `IsKnockout` - True if match is a knockout stage
- `IsGroupStage` - True if match is a group stage
- `IsLeague` - True if regular league match

### 3. **CSV Support** (`CsvMatchRecord.cs`, `CsvDataService.cs`)

Three new **optional** columns in match CSV:

| Column | Description | Example |
|--------|-------------|---------|
| `Stage` | Tournament stage | `GS`, `R16`, `QF`, `SF`, `F` |
| `Group` | Group identifier | `A`, `B`, `Group C` |
| `Leg` | Leg number for two-legged ties | `1`, `2` |

The CSV loader automatically:
- Maps `Stage` column to `TournamentStage` enum
- Parses flexible stage codes (e.g., `R16`, `ROUND OF 16`, `LAST 16`)
- Handles group and leg data
- Works with existing league CSVs (no breaking changes!)

### 4. **Tournament Query Methods** (`SeasonData.cs`)

New helper methods for tournament analysis:

```csharp
// Basic queries
List<Match> GetGroupStageMatches()
List<Match> GetKnockoutMatches()
List<Match> GetMatchesByGroup(string group)
List<Match> GetMatchesByStage(TournamentStage stage)
List<string> GetAllGroups()

// Group standings
List<TeamRecord> GetGroupStandings(string group)

// Two-legged tie support
List<(Match FirstLeg, Match SecondLeg)> GetTwoLeggedTies()
(int Team1Goals, int Team2Goals) GetAggregateScore(Match firstLeg, Match secondLeg)
```

### 5. **Updated Templates**

- **`matches.csv`** - Now includes `Stage`, `Group`, and `Leg` columns
- All existing templates remain backward compatible

### 6. **Example Files**

Three new example datasets in `data/examples/`:

- **`tournament_examples_groupstage.csv`** - Champions League group stage matches
- **`tournament_examples_knockout.csv`** - Champions League knockout with two-legged ties
- **`tournament_examples_worldcup.csv`** - World Cup with groups + single-elimination knockout

### 7. **Comprehensive Documentation**

- **`TOURNAMENT-GUIDE.md`** - Complete tournament usage guide with examples
- **`QUICK-START-TOURNAMENTS.md`** - 5-minute quick start for tournaments
- **`TEMPLATE-GUIDE.md`** - Updated with tournament column reference
- **`README.md`** (templates) - Updated to reference tournament support
- **`README.md`** (root) - Highlights tournament feature

### 8. **Code Examples** (`TournamentExamples.cs`)

New example file with three demonstrations:

1. **World Cup Example** - Group stage + knockout analysis
2. **Champions League Example** - Two-legged ties with aggregate scoring
3. **Champions League Group Stage Example** - Group standings and qualification
4. **Tournament Query Examples** - Various query patterns

---

## 🎯 Use Cases Supported

✅ **World Cup** - Group stage + single-elimination knockout  
✅ **Champions League** - Group stage + two-legged knockout ties  
✅ **Europa League** - Group stage + two-legged knockout ties  
✅ **FA Cup / Copa del Rey** - Single-elimination tournaments  
✅ **Olympics / Continental Championships** - Any tournament format  
✅ **Mixed datasets** - League + tournament data in same CSV  

---

## 💡 Key Design Decisions

### 1. **Backward Compatibility**
- All tournament columns are **optional**
- Existing league CSVs work without any changes
- `Stage` defaults to `League` if not specified
- No breaking changes to existing code

### 2. **Flexible Parsing**
- Stage codes support multiple formats (`R16`, `ROUND OF 16`, `Last 16`)
- Users can write stage names naturally
- Case-insensitive matching

### 3. **Progressive Enhancement**
- Tournament data works with minimal columns (just add `Stage`)
- Can add `Group` and `Leg` as needed
- All existing features (lineups, goals, assists) work for tournaments

### 4. **Two-Legged Tie Support**
- `Leg` field distinguishes first and second legs
- Helper methods calculate aggregate scores automatically
- Handles home/away reversals correctly

### 5. **Group Stage Support**
- `Group` field for group identifiers
- `GetGroupStandings()` calculates points/GD/position per group
- Works with any number of groups (A, B, C, ..., H)

---

## 📊 Example Queries

```csharp
var seasonData = csvService.LoadSeasonDataFromFile("worldcup_2022.csv");

// Get all group stage matches
var groupMatches = seasonData.GetGroupStageMatches();

// Get knockout matches
var knockoutMatches = seasonData.GetKnockoutMatches();

// Get Group A standings
var groupAStandings = seasonData.GetGroupStandings("A");

// Get all finals
var finals = seasonData.GetMatchesByStage(TournamentStage.Final);

// Get two-legged ties and aggregate scores
var ties = seasonData.GetTwoLeggedTies();
foreach (var (firstLeg, secondLeg) in ties)
{
    var (team1Goals, team2Goals) = seasonData.GetAggregateScore(firstLeg, secondLeg);
    Console.WriteLine($"{firstLeg.HomeTeam} {team1Goals}-{team2Goals} {firstLeg.AwayTeam} (agg)");
}

// Check match types
if (match.IsLeague)
    Console.WriteLine("League match");
else if (match.IsGroupStage)
    Console.WriteLine($"Group {match.Group} match");
else if (match.IsKnockout)
    Console.WriteLine($"{match.Stage.GetDisplayName()} match");
```

---

## 🚀 Getting Started

### Quick Start (3 steps):

1. **Copy the template**
   ```bash
   cp data/templates/matches.csv data/my-tournament/worldcup_2022.csv
   ```

2. **Add tournament data**
   ```csv
   Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR
   World Cup,2022,1,GS,A,,20/11/2022,Qatar,Ecuador,0,2,A
   World Cup,2022,2,R16,,,03/12/2022,Netherlands,USA,3,1,H
   World Cup,2022,6,F,,,18/12/2022,Argentina,France,3,3,D
   ```

3. **Load and query**
   ```csharp
   var data = csvService.LoadSeasonDataFromFile("worldcup_2022.csv");
   var final = data.GetMatchesByStage(TournamentStage.Final).FirstOrDefault();
   ```

**See:** `QUICK-START-TOURNAMENTS.md` for complete walkthrough

---

## 📁 Files Changed/Added

### New Files:
- `src/FootballDataTool/Models/TournamentStage.cs`
- `src/FootballDataTool/Examples/TournamentExamples.cs`
- `data/templates/TOURNAMENT-GUIDE.md`
- `data/templates/QUICK-START-TOURNAMENTS.md`
- `data/examples/tournament_examples_groupstage.csv`
- `data/examples/tournament_examples_knockout.csv`
- `data/examples/tournament_examples_worldcup.csv`
- `docs/TOURNAMENT-SUPPORT.md` (this file)

### Modified Files:
- `src/FootballDataTool/Models/Match.cs` - Added `Stage`, `Group`, `Leg`, and helper properties
- `src/FootballDataTool/Models/CsvMatchRecord.cs` - Added `Stage`, `Group`, `Leg` fields
- `src/FootballDataTool/Services/CsvDataService.cs` - Added tournament column mapping and parsing
- `src/FootballDataTool/Models/SeasonData.cs` - Added tournament query methods
- `data/templates/matches.csv` - Added `Stage`, `Group`, `Leg` columns
- `data/templates/README.md` - Updated to mention tournament support
- `data/templates/TEMPLATE-GUIDE.md` - Added tournament field documentation
- `README.md` - Highlighted tournament feature

---

## ✅ Testing

All changes tested and validated:
- ✅ Build successful
- ✅ No breaking changes to existing functionality
- ✅ Backward compatible with existing CSVs
- ✅ Tournament CSV examples load correctly
- ✅ Stage parsing works with multiple formats
- ✅ Group standings calculated correctly
- ✅ Aggregate scoring for two-legged ties works

---

## 🎉 Summary

Tournament support is **fully integrated** and **production-ready**. The tool now handles:

- ✅ Group stages with standings
- ✅ Knockout rounds (all stages from R64 to Final)
- ✅ Two-legged ties with aggregate scoring
- ✅ Single-elimination tournaments
- ✅ Mixed league + tournament datasets
- ✅ All existing features (lineups, goals, assists, etc.) work for tournaments

**The world is ready for the World Cup! 🏆⚽**
