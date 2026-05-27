# CSV Template Field Reference Guide

This guide explains how to fill in each field in the template CSVs for FootballDataTool.

---

## Template Files Available

1. **`match_template_basic.csv`** - Minimum required fields (quick start)
2. **`match_template_extended.csv`** - With managers, formations, goals with assists
3. **`match_template_full.csv`** - Complete example with lineups, ages, injuries, minutes
4. **`match_template_blank.csv`** - Empty template with all possible columns

---

## Field Reference

### ✅ REQUIRED FIELDS (Minimum 4)

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| `HomeTeam` | Text | `Arsenal` | Team name (consistent spelling required) |
| `AwayTeam` | Text | `Chelsea` | Team name (consistent spelling required) |
| `FTHG` | Number | `2` | Full-time home goals |
| `FTAG` | Number | `1` | Full-time away goals |

---

### 📊 RECOMMENDED FIELDS (For Rich Analysis)

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| `Div` | Code | `E0` | Division code (E0=Premier League, SP1=La Liga) |
| `Season` | Text | `2014/15` | Season in format YYYY/YY |
| `GW` | Number | `1` | Gameweek number (auto-assigned if missing) |
| `Date` | Date | `16/08/2014` | Match date (dd/MM/yyyy or yyyy-MM-dd) |
| `FTR` | Letter | `H` | Full-time result (H=Home win, A=Away win, D=Draw) |
| `Referee` | Text | `Mike Dean` | Match referee name |

---

### 👥 MANAGERS & FORMATIONS

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| `HomeManager` | Text | `Arsene Wenger` | Home team manager |
| `AwayManager` | Text | `Jose Mourinho` | Away team manager |
| `HomeFormation` | Text | `4-2-3-1` | Home formation (e.g., 4-4-2, 3-5-2) |
| `AwayFormation` | Text | `4-3-3` | Away formation |

---

### ⚽ GOALS & ASSISTS

**Format Options:**
```
"Player 45'"                                    // Basic
"Player 45' (pen)"                              // With goal type
"Player 45' (assist: Assister)"                 // With assist
"Player 45+2' (assist: Assister)"               // With added time
"Player1 45'; Player2 67' (assist: Helper)"     // Multiple goals (semicolon separated)
```

**Goal Type Indicators:**
- `(pen)` or `(p)` = Penalty
- `(og)` or `(own)` = Own goal
- `(fk)` or `(free kick)` = Direct free kick

| Field | Format | Example |
|-------|--------|---------|
| `HomeGoalscorers` | Text | `"Koscielny 16'; Ramsey 90' (assist: Ozil)"` |
| `AwayGoalscorers` | Text | `"Drogba 28'; Hazard 72' (assist: Fabregas)"` |

---

### 👕 LINEUPS (With Ages & Positions)

**Format Options:**
```
"Player Name"                           // Name only
"1. Player Name"                        // With shirt number
"1. Player Name (25)"                   // With age
"1. Player Name (25, GK)"              // With age and position
"Player Name (25) [GK]"                // Alternative format
```

**Separate players with semicolons (;) or commas (,)**

**Position Codes:**
- GK = Goalkeeper
- LB/RB = Left/Right Back
- CB = Centre Back
- LWB/RWB = Left/Right Wing Back
- DM/CM/AM = Defensive/Central/Attacking Midfielder
- LM/RM = Left/Right Midfielder
- LW/RW = Left/Right Winger
- ST/CF = Striker/Centre Forward

| Field | Format | Example |
|-------|--------|---------|
| `HomeLineup` | Text | `"1. Szczesny (24, GK); 3. Gibbs (24, LB); 6. Koscielny (28, CB)"` |
| `AwayLineup` | Text | `"1. Courtois (22, GK); 2. Ivanovic (30, RB); 24. Cahill (28, CB)"` |
| `HomeSubstitutes` | Text | `"9. Podolski (29); 7. Rosicky (33); 20. Flamini (30)"` |
| `AwaySubstitutes` | Text | `"13. Cech (32, GK); 8. Oscar (22); 11. Drogba (36)"` |

---

### 🔄 SUBSTITUTIONS

**Format:**
```
"Player On ← Player Off 60'"
"Player On <- Player Off (60)"
"Player On -> Player Off 60+2'"
```

| Field | Format | Example |
|-------|--------|---------|
| `HomeSubstitutions` | Text | `"Cazorla ← Rosicky 78'; Ozil ← Campbell 82'"` |
| `AwaySubstitutions` | Text | `"Oscar ← Schurrle 65'; Drogba ← Costa 72'"` |

---

### 📇 CARDS

**Format:**
```
"Player 45'"
"Player1 28'; Player2 67'"
```

| Field | Format | Example |
|-------|--------|---------|
| `HomeYellowCards` | Text | `"Arteta 45'; Flamini 89'"` |
| `AwayYellowCards` | Text | `"Matic 34'; Fabregas 78'"` |
| `HomeRedCards` | Text | `"Flamini 89'"` |
| `AwayRedCards` | Text | `""` (empty if none) |

---

### 🏟️ VENUE & ATTENDANCE

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| `Stadium` | Text | `Emirates Stadium` | Stadium name |
| `Attendance` | Number | `60039` | Match attendance |
| `StadiumCapacity` | Number | `60704` | Stadium capacity |

---

### 🏥 INJURIES

**Format:**
```
"Player (Injury Type, dd/MM/yyyy - dd/MM/yyyy)"
"Player (Injury Type, 01/08/2014 - )"              // Still injured (no return date)
```

**Separate multiple injuries with semicolons**

| Field | Format | Example |
|-------|--------|---------|
| `HomeInjuries` | Text | `"Wilshere (Ankle, 01/08/2014 - 15/09/2014); Walcott (Knee, 15/01/2014 - 25/10/2014)"` |
| `AwayInjuries` | Text | `"Remy (Calf, 10/08/2014 - 28/08/2014)"` |

**Severity Auto-Calculated:**
- ≤ 7 days = Minor
- 8-28 days = Moderate
- 29-90 days = Serious
- 90+ days = Long-term

---

### ⏱️ MINUTES PLAYED

**Format:**
```
"Player 90'"
"Player (65')"
"Player 90'; Player2 65'; Player3 (25')"
```

| Field | Format | Example |
|-------|--------|---------|
| `HomeMinutesPlayed` | Text | `"Szczesny 90'; Gibbs 90'; Koscielny 90'; Ozil 82'; Rosicky (12')"` |
| `AwayMinutesPlayed` | Text | `"Courtois 90'; Ivanovic 90'; Cahill 90'; Oscar (25')"` |

**Note:** If omitted, system assumes:
- Starting XI = 90 minutes
- Substitutes = 0 minutes

---

### 🌤️ WEATHER (Optional)

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| `Temperature` | Number | `18` | Temperature in Celsius |
| `WeatherConditions` | Text | `Sunny` | Weather description (Sunny, Rainy, Cloudy, etc.) |

---

### 👔 MATCH OFFICIALS (Extended)

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| `AR1` | Text | `Simon Beck` | Assistant Referee 1 |
| `AR2` | Text | `Jake Collin` | Assistant Referee 2 |
| `FourthOfficial` | Text | `Kevin Friend` | Fourth Official |
| `VAR` | Text | `Stuart Attwell` | VAR official (2019/20 onwards) |

---

## Quick Start Examples

### Minimal (4 fields only)
```csv
HomeTeam,AwayTeam,FTHG,FTAG
Arsenal,Chelsea,2,0
Liverpool,Man United,3,1
```

### Basic (10 fields)
```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
E0,2014/15,1,16/08/2014,Arsenal,Chelsea,2,0,H,Mike Dean
E0,2014/15,1,16/08/2014,Liverpool,Man United,3,1,H,Martin Atkinson
```

### With Goals & Assists
```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,HomeGoalscorers,AwayGoalscorers
E0,2014/15,1,16/08/2014,Arsenal,Chelsea,2,0,"Sanchez 34'; Ozil 67' (assist: Ramsey)",""
E0,2014/15,1,16/08/2014,Liverpool,Man United,3,1,"Sturridge 12'; Gerrard 45' (pen); Sterling 72' (assist: Coutinho)","Rooney 28' (assist: Di Maria)"
```

### Full Example (All Fields)
See `match_template_full.csv` for complete example!

---

## Data Sources Recommendations

### 🌐 Where to Get Data

**Basic Match Results:**
- football-data.co.uk (FREE CSV downloads)
- Premier League official site
- ESPN, BBC Sport

**Lineups & Ages:**
- FBref.com (detailed stats)
- Transfermarkt (player ages/values)
- Soccerway

**Assists:**
- FBref.com (goal descriptions)
- WhoScored.com
- ESPN match reports

**Injuries:**
- PhysioRoom.com
- Official club sites
- Premier Injuries

**Minutes Played:**
- FBref.com (player stats pages)
- WhoScored.com
- Official Premier League stats

---

## Tips for Data Entry

### ✅ DO:
- Keep team names **consistent** (e.g., always "Man United" or always "Manchester United")
- Use **semicolons (;)** to separate multiple entries
- Include **ages in parentheses** for lineup data: `Player (25)`
- Add **assists** where possible: `Player 45' (assist: Helper)`
- Date format: **dd/MM/yyyy** (e.g., 16/08/2014)

### ❌ DON'T:
- Mix team name spellings (e.g., "Man United" vs "Man Utd")
- Use tabs - stick to commas in CSV
- Forget to quote fields with commas inside (e.g., "Player (25, GK)")
- Leave spaces after commas in lists (semicolons preferred)

---

## Testing Your Data

Once you've filled in data, test it:

```csharp
var seasonData = csvService.LoadSeasonDataFromFile("your_file.csv");
Console.WriteLine($"Loaded {seasonData.TotalMatches} matches");
Console.WriteLine($"Has lineups: {seasonData.HasLineupData}");
Console.WriteLine($"Has assists: {seasonData.Matches.Any(m => m.ExtendedData?.Goals.Any(g => g.Assister != null))}");
```

---

## Progressive Enhancement

Start simple, add richness incrementally:

1. **Week 1**: Basic match results (4 fields) → Get table working
2. **Week 2**: Add managers, formations, goals
3. **Week 3**: Add lineups with ages
4. **Week 4**: Add assists, injuries, minutes
5. **Week 5**: Polish with weather, all officials

**You don't need all fields!** The tool works with any subset.

---

## Need Help?

- See `docs/CSV-FORMAT-GUIDE.md` for complete column reference
- See `docs/EXTENDED-DATA-GUIDE.md` for format specifications
- See sample files in `data/premier-league/` for real examples
- Run the tool with your partial data - it will tell you what it found!

---

**Created**: January 2025  
**For**: FootballDataTool v4.0  
**Templates**: `/data/templates/`
