# Tournament Data Guide

This guide explains how to use FootballDataTool for knockout tournaments like the Champions League, World Cup, Europa League, domestic cups, and other tournament competitions.

## Overview

The tool fully supports both **league/round-robin** competitions and **knockout tournament** competitions. Tournament support includes:

- **Group stages** (World Cup groups, Champions League groups)
- **Knockout rounds** (R64, R32, R16, Quarter-finals, Semi-finals)
- **Finals** and **3rd place playoffs**
- **Two-legged ties** (home/away legs in Champions League, Europa League)
- **Single-elimination tournaments** (World Cup knockout, FA Cup)

**Squad Data:** You can use the same `squads.csv` format for tournaments! See `data/examples/squad_examples_worldcup.csv` for a complete World Cup squad example with Argentina, France, Brazil, and England squads.

## Tournament-Specific Columns

When working with tournaments, you can use three additional optional columns in your match CSV:

| Column | Description | Example Values |
|--------|-------------|----------------|
| `Stage` | Tournament stage/round | `GS`, `R32`, `R16`, `QF`, `SF`, `F`, `3P` |
| `Group` | Group identifier (for group stages) | `A`, `B`, `Group A`, `Group C` |
| `Leg` | Leg number for two-legged ties | `1`, `2` |

### Stage Codes

The tool recognizes multiple formats for tournament stages:

| Stage | Recognized Codes | Display Name |
|-------|------------------|--------------|
| **Group Stage** | `GS`, `GROUP`, `GROUP STAGE`, `GROUPS` | Group Stage |
| **Round of 64** | `R64`, `ROUND OF 64`, `RO64` | Round of 64 |
| **Round of 32** | `R32`, `ROUND OF 32`, `RO32`, `LAST 32` | Round of 32 |
| **Round of 16** | `R16`, `ROUND OF 16`, `RO16`, `LAST 16` | Round of 16 |
| **Quarter-finals** | `QF`, `QUARTER FINAL`, `QUARTERS` | Quarter-Final |
| **Semi-finals** | `SF`, `SEMI FINAL`, `SEMIS` | Semi-Final |
| **3rd Place** | `3P`, `3RD`, `THIRD PLACE`, `3RD PLACE PLAYOFF` | 3rd Place Playoff |
| **Final** | `F`, `FINAL`, `FINALS` | Final |
| **League** | (leave blank or `L`) | League |

**Note:** If you leave the `Stage` column blank or omit it entirely, matches will be treated as regular league/season matches.

---

## Example Use Cases

### 1. Champions League Group Stage

For group stage matches, specify the stage as `GS` and include the group identifier:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
Champions League,2023-24,1,GS,A,,19/09/2023,Bayern Munich,Manchester United,4,3,H,Clément Turpin
Champions League,2023-24,1,GS,A,,19/09/2023,FC Copenhagen,Galatasaray,0,0,D,Davide Massa
Champions League,2023-24,1,GS,B,,19/09/2023,Arsenal,PSV Eindhoven,4,0,H,François Letexier
```

**Key points:**
- `Stage` = `GS` (Group Stage)
- `Group` = `A`, `B`, `C`, etc.
- `Leg` is left blank (not applicable for group stages)
- `GW` tracks matchday within the group stage

### 2. Champions League Knockout Rounds (Two-Legged Ties)

For two-legged knockout ties, use `Leg` to distinguish first and second legs:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
Champions League,2023-24,9,R16,,1,13/02/2024,Paris Saint-Germain,Real Sociedad,2,0,H,Slavko Vinčić
Champions League,2023-24,10,R16,,2,06/03/2024,Real Sociedad,Paris Saint-Germain,1,2,A,Szymon Marciniak
Champions League,2023-24,11,QF,,1,09/04/2024,Arsenal,Bayern Munich,2,2,D,Glenn Nyberg
Champions League,2023-24,12,QF,,2,17/04/2024,Bayern Munich,Arsenal,1,0,H,Slavko Vinčić
Champions League,2023-24,13,SF,,1,30/04/2024,PSG,Borussia Dortmund,0,1,A,Anthony Taylor
Champions League,2023-24,14,SF,,2,07/05/2024,Borussia Dortmund,PSG,1,0,H,Daniele Orsato
Champions League,2023-24,15,F,,,01/06/2024,Borussia Dortmund,Real Madrid,0,2,A,Slavko Vinčić
```

**Key points:**
- `Stage` = `R16`, `QF`, `SF`, or `F`
- `Group` is left blank (not applicable after group stages)
- `Leg` = `1` or `2` for two-legged ties
- `Leg` is left blank for single-match finals

### 3. World Cup (Single-Elimination Knockout)

For tournaments like the World Cup with single-elimination knockout rounds:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
World Cup,2022,1,GS,A,,20/11/2022,Qatar,Ecuador,0,2,A,Daniele Orsato
World Cup,2022,1,GS,A,,25/11/2022,Qatar,Senegal,1,3,A,Antonio Mateu Lahoz
World Cup,2022,2,R16,,,03/12/2022,Netherlands,USA,3,1,H,Wilton Sampaio
World Cup,2022,3,QF,,,09/12/2022,Croatia,Brazil,1,1,D,Michael Oliver
World Cup,2022,4,SF,,,13/12/2022,Argentina,Croatia,3,0,H,Daniele Orsato
World Cup,2022,5,3P,,,17/12/2022,Croatia,Morocco,2,1,H,Abdulrahman Al-Jassim
World Cup,2022,6,F,,,18/12/2022,Argentina,France,3,3,D,Szymon Marciniak
```

**Key points:**
- Group stage uses `Stage=GS` with group identifiers
- Knockout rounds use `Stage=R16`, `QF`, `SF`, `F`, `3P`
- `Leg` is left blank (World Cup knockout is single-elimination)
- The final and 3rd place playoff are played on different days

### 4. Domestic Cup (FA Cup, Copa del Rey)

For domestic knockout cups with single-elimination:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
FA Cup,2023-24,1,R64,,,06/01/2024,Arsenal,Liverpool,2,0,H,Michael Oliver
FA Cup,2023-24,2,R32,,,27/01/2024,Manchester United,Newport County,4,2,H,John Brooks
FA Cup,2023-24,3,R16,,,17/02/2024,Luton Town,Manchester City,2,6,A,Tim Robinson
FA Cup,2023-24,4,QF,,,16/03/2024,Manchester City,Newcastle,2,0,H,Peter Bankes
FA Cup,2023-24,5,SF,,,21/04/2024,Manchester City,Chelsea,1,0,H,Paul Tierney
FA Cup,2023-24,6,F,,,25/05/2024,Manchester City,Manchester United,2,1,H,Anthony Taylor
```

**Key points:**
- Use `R64`, `R32`, `R16`, `QF`, `SF`, `F` based on the cup format
- Leave `Group` and `Leg` blank
- `GW` tracks progression through the tournament

---

## Using Tournament Data in Code

### Querying Tournament Matches

```csharp
var seasonData = csvService.LoadSeasonDataFromFile("worldcup_2022.csv");

// Get all group stage matches
var groupMatches = seasonData.Matches.Where(m => m.IsGroupStage).ToList();

// Get all knockout matches
var knockoutMatches = seasonData.Matches.Where(m => m.IsKnockout).ToList();

// Get matches from a specific group
var groupA = seasonData.Matches.Where(m => m.Group == "A").ToList();

// Get matches from a specific stage
var finals = seasonData.Matches.Where(m => m.Stage == TournamentStage.Final).ToList();
var semis = seasonData.Matches.Where(m => m.Stage == TournamentStage.SemiFinal).ToList();

// Get two-legged tie matches
var firstLegs = seasonData.Matches.Where(m => m.Leg == 1).ToList();
var secondLegs = seasonData.Matches.Where(m => m.Leg == 2).ToList();

// Check if a match is league or tournament
foreach (var match in seasonData.Matches)
{
    if (match.IsLeague)
        Console.WriteLine($"{match.HomeTeam} vs {match.AwayTeam} - League Match");
    else if (match.IsGroupStage)
        Console.WriteLine($"{match.HomeTeam} vs {match.AwayTeam} - Group {match.Group}");
    else if (match.IsKnockout)
        Console.WriteLine($"{match.HomeTeam} vs {match.AwayTeam} - {match.Stage.GetDisplayName()}");
}
```

### Match Properties

Every `Match` object now includes:

| Property | Type | Description |
|----------|------|-------------|
| `Stage` | `TournamentStage` | Stage enum (League, GroupStage, RoundOf16, etc.) |
| `Group` | `string?` | Group identifier (e.g., "A", "B") |
| `Leg` | `int?` | Leg number (1 or 2 for two-legged ties) |
| `IsLeague` | `bool` | True if regular league match |
| `IsGroupStage` | `bool` | True if group stage match |
| `IsKnockout` | `bool` | True if knockout stage match |

---

## Tips and Best Practices

### 1. **Gameweek (GW) Column for Tournaments**

For tournaments, the `GW` column can be used flexibly:

- **Group stages**: GW = matchday (1, 2, 3, etc.)
- **Knockout rounds**: GW can track overall tournament progression or be left as sequential numbers
- The tool will auto-assign gameweeks if you leave them blank

### 2. **Mixing Leagues and Tournaments**

You can track both league and tournament matches in the same CSV if desired:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG
Premier League,2023-24,10,,,12/11/2023,Arsenal,Burnley,3,1
Champions League,2023-24,4,GS,B,,29/11/2023,Arsenal,RC Lens,6,0
Premier League,2023-24,11,,,25/11/2023,Brentford,Arsenal,0,1
```

Just use the `Stage` column to differentiate league from tournament matches.

### 3. **Two-Legged Ties**

- Always include both `Leg=1` and `Leg=2` for two-legged ties
- The home/away reversal is captured by the `HomeTeam` / `AwayTeam` columns
- Aggregate scores can be calculated by filtering matches with the same teams in the same stage

### 4. **Extra Time and Penalties**

Currently, the tool records the final score (including extra time). Penalty shootout details can be tracked in:
- The `ExtendedData` or goal events if you track every penalty kick
- Future enhancements may add explicit penalty shootout fields

---

## Squad Data for Tournaments

You can use **the exact same squad CSV format** for tournaments as you do for leagues!

### Example: World Cup Squads

```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,CurrentClub,ClubLeague
Argentina,2022,Lionel Messi,24/06/1987,FW,10,Argentina,Paris Saint-Germain,Ligue 1
Argentina,2022,Emiliano Martínez,02/09/1992,GK,23,Argentina,Aston Villa,Premier League
Argentina,2022,Enzo Fernández,17/01/2001,CM,24,Argentina,Benfica,Primeira Liga
France,2022,Kylian Mbappé,20/12/1998,LW,10,France,Paris Saint-Germain,Ligue 1
France,2022,Hugo Lloris,26/12/1986,GK,1,France,Tottenham,Premier League
Brazil,2022,Neymar,05/02/1992,LW,10,Brazil,Paris Saint-Germain,Ligue 1
```

**Column Aliases:** For tournaments, you can use `CurrentClub` and `ClubLeague` instead of `PreviousClub` and `PreviousLeague`. Both work - use whichever is clearer!

### Loading Tournament Squads

```csharp
// Load World Cup matches
var seasonData = csvService.LoadSeasonDataFromFile("worldcup_2022.csv");

// Load squad data (same format as leagues!)
seasonData.LoadSquadDataFromCsv("worldcup_2022_squads.csv");

// Player ages are automatically calculated for each match
var final = seasonData.GetMatchesByStage(TournamentStage.Final).First();
if (final.ExtendedData != null)
{
    foreach (var player in final.ExtendedData.HomeStartingLineup)
    {
        Console.WriteLine($"{player.Name} ({player.Nationality}) - Age: {player.Age}");
    }
}
```

### Key Points

✅ Use `Season` = tournament year (e.g., `2022` for World Cup 2022)  
✅ `Team` = national team or club name  
✅ Same enrichment logic - ages calculated per match  
✅ All squad features work: birthdays, zodiac signs, nationalities, clubs  
✅ **Column names**: Use `CurrentClub`/`ClubLeague` for tournaments, or `PreviousClub`/`PreviousLeague` for leagues - both work!  

**See:** `data/examples/squad_examples_worldcup.csv` for a complete example with Argentina, France, Brazil, and England squads from World Cup 2022.

---

## Example Files

Check out these example files in `data/examples/`:

### Match Data:
- **`tournament_examples_groupstage.csv`** — Champions League group stage matches
- **`tournament_examples_knockout.csv`** — Champions League knockout rounds with two-legged ties
- **`tournament_examples_worldcup.csv`** — World Cup with group stage + single-elimination knockout

### Squad Data:
- **`squad_examples_worldcup.csv`** — World Cup 2022 squads (Argentina, France, Brazil, England) with 100+ players

---

## Summary

Tournament support is **fully integrated** into FootballDataTool. You can:

✅ Track group stages with group identifiers  
✅ Track knockout rounds (R32, R16, QF, SF, Final)  
✅ Handle two-legged ties with leg numbers  
✅ Mix league and tournament data in one dataset  
✅ Query matches by stage, group, or leg  
✅ Use all existing features (lineups, goals, assists, etc.) for tournament matches  
✅ Use the same squad CSV format for national teams and clubs  

The world is ready for you to analyze the World Cup, Champions League, and beyond! 🏆⚽
