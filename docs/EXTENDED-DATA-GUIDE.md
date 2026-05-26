# Extended Data Support

FootballDataTool now supports **rich extended match data** including lineups, goalscorers, substitutions, cards, managers, attendance, and much more. All extended fields are **completely optional** - the tool works perfectly with just basic match scores or with incredibly detailed data.

## Philosophy

- **Progressive Enhancement**: Start with basic CSV (teams + scores), add more detail as available
- **Graceful Degradation**: Missing extended data doesn't break anything
- **Flexible Formats**: Multiple ways to format each field type
- **Future-Proof**: Easy to add new data types without breaking existing CSVs

## Extended Field Categories

### Team Management

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Home Manager | `HomeManager`, `Home Manager`, `HManager` | Manager name | `Mikel Arteta` |
| Away Manager | `AwayManager`, `Away Manager`, `AManager` | Manager name | `Pep Guardiola` |

### Tactics

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Home Formation | `HomeFormation`, `Home Formation`, `HForm` | Formation code | `4-3-3`, `3-5-2` |
| Away Formation | `AwayFormation`, `Away Formation`, `AForm` | Formation code | `4-2-3-1` |

### Venue & Attendance

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Stadium | `Stadium`, `Venue`, `Ground` | Stadium name | `Old Trafford` |
| Capacity | `StadiumCapacity`, `Capacity` | Number | `74310` |
| Attendance | `Attendance`, `Crowd` | Number | `73738` |

### Match Officials

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Referee | `Referee`, `Ref` | Name | `Michael Oliver` |
| Assistant Referee 1 | `AR1`, `AssistantReferee1` | Name | `Stuart Burt` |
| Assistant Referee 2 | `AR2`, `AssistantReferee2` | Name | `Simon Bennett` |
| Fourth Official | `FourthOfficial`, `4thOfficial`, `FO` | Name | `Darren England` |
| VAR | `VAR`, `VarReferee` | Name | `Paul Tierney` |

### Goalscorers

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Home Goalscorers | `HomeGoalscorers`, `Home Goalscorers` | See below | `Saka 72'; Nketiah 86'` |
| Away Goalscorers | `AwayGoalscorers`, `Away Goalscorers` | See below | `Haaland 18' (pen)` |

#### Goalscorer Format Options

**Basic format:**
```
Saka 72'
Saka 72', Nketiah 86'
```

**With added time:**
```
Saka 45+2'
```

**With goal type:**
```
Saka 30' (pen)          # Penalty
Saka 25' (OG)           # Own goal
Saka 35' (FK)           # Free kick
```

**With assists:**
```
Saka 45' (assist: Odegaard)
Saka 67' (assist: Martinelli), Nketiah 82'
```

**Combined:**
```
Saka 30' (pen, assist: Odegaard); Jesus 67' (assist: Martinelli)
```

### Lineups

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Home Lineup | `HomeLineup`, `HomeXI`, `Home Lineup` | See below | Player list |
| Away Lineup | `AwayLineup`, `AwayXI`, `Away Lineup` | See below | Player list |
| Home Substitutes | `HomeSubstitutes`, `Home Subs`, `HSubs` | See below | Player list |
| Away Substitutes | `AwaySubstitutes`, `Away Subs`, `ASubs` | See below | Player list |

#### Lineup Format Options

**Simple list (comma separated):**
```
Ramsdale, White, Saliba, Gabriel, Zinchenko, Partey, Odegaard, Saka, Havertz, Martinelli, Jesus
```

**With shirt numbers (semi-colon separated):**
```
1. Ramsdale; 4. White; 6. Gabriel; 2. Saliba; 35. Zinchenko
```

**With player ages (parentheses):**
```
1. Ramsdale (25); 4. White (25); 6. Gabriel (25); 2. Saliba (22)
```

**With player ages (square brackets):**
```
Ramsdale [25], White [25], Gabriel [25], Saliba [22]
```

**With ages and positions:**
```
1. Ramsdale (25) [GK]; 4. White (25) [DEF]; 6. Gabriel (25) [DEF]
```

**Alternative format with position in parentheses:**
```
1. Ramsdale (25, GK); 4. White (25, DEF); 8. Odegaard (24, MID)
```

**Mixed format:**
```
1. Ramsdale (25), 4. White (25), 6. Gabriel, 2. Saliba (22), Zinchenko
```

### Substitutions

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Home Substitutions | `HomeSubstitutions`, `Home Substitutions` | See below | Substitution events |
| Away Substitutions | `AwaySubstitutions`, `Away Substitutions` | See below | Substitution events |

#### Substitution Format Options

**Using arrows:**
```
Trossard → Martinelli 65'
Jesus ← Nketiah 78'
```

**Using text arrows:**
```
Trossard -> Martinelli 65'
Martinelli <- Trossard 65'
```

**With added time:**
```
Jesus → Nketiah 90+2'
```

**Multiple substitutions (semi-colon separated):**
```
Trossard → Martinelli 65'; Nketiah → Jesus 78'; Smith Rowe → Odegaard 85'
```

### Cards

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Home Yellow Cards | `HomeYellowCards`, `HY`, `Home Yellow` | See below | Player(s) booked |
| Away Yellow Cards | `AwayYellowCards`, `AY`, `Away Yellow` | See below | Player(s) booked |
| Home Red Cards | `HomeRedCards`, `HR`, `Home Red` | See below | Player(s) sent off |
| Away Red Cards | `AwayRedCards`, `AR`, `Away Red` | See below | Player(s) sent off |

#### Card Format Options

**Single card:**
```
Xhaka 45'
```

**Multiple cards (comma or semi-colon separated):**
```
Xhaka 45', Partey 67'
Casemiro 23'; Fernandes 78'
```

**With added time:**
```
Fernandes 90+4'
```

### Weather

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Temperature | `Temperature`, `Temp` | Number (Celsius) | `18`, `12.5` |
| Weather Conditions | `Weather`, `WeatherConditions`, `Conditions` | Description | `Sunny`, `Rainy`, `Overcast` |

### Competition Context

| Field | Column Names | Format | Example |
|-------|-------------|--------|---------|
| Other Competitions | `OtherCompetitions`, `OtherFixtures`, `MidweekFixtures` | JSON or formatted | (Advanced - TBD) |

## Example: Full Extended CSV

**With player ages:**
```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,HomeManager,AwayManager,HomeLineup,Stadium,Attendance
E0,2023/24,1,11/08/2023,Arsenal,Nottm Forest,2,1,Mikel Arteta,Steve Cooper,"1. Ramsdale (25); 4. White (25); 6. Gabriel (25); 2. Saliba (22); 35. Zinchenko (26); 5. Partey (30); 8. Odegaard (24); 7. Saka (21); 29. Havertz (24); 11. Martinelli (22); 9. Jesus (26)",Emirates Stadium,60184
```

**With all match events:**
```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,HomeManager,AwayManager,Stadium,Capacity,Attendance,HomeFormation,AwayFormation,HomeGoalscorers,AwayGoalscorers,HomeYellowCards,HomeRedCards,Referee
E0,2023/24,1,11/08/2023,Arsenal,Nottm Forest,2,1,Mikel Arteta,Steve Cooper,Emirates Stadium,60704,60184,4-3-3,5-4-1,Saka 72' (assist: Odegaard); Nketiah 86',Awoniyi 82',White 34'; Xhaka 67',,Simon Hooper
E0,2023/24,1,12/08/2023,Man City,Burnley,3,0,Pep Guardiola,Vincent Kompany,Etihad Stadium,53400,53276,4-3-3,4-4-2,Haaland 4'; Rodri 36'; Haaland 60',,Rodri 44',Stones 88',John Brooks
```

## Example: Minimal CSV (Still Works!)

```csv
GW,HomeTeam,AwayTeam,FTHG,FTAG
1,Arsenal,Nottm Forest,2,1
1,Man City,Burnley,3,0
```

Both work perfectly - extended data is entirely optional!

## Team Season Data (Advanced)

For tracking transfers, managerial changes, and squad rosters, you can use separate JSON or CSV files:

### Team Transfers Example (`transfers_2023-24.json`)

```json
{
  "team": "Arsenal",
  "season": "2023/24",
  "summerSignings": [
    {
      "player": {"name": "Declan Rice", "position": "Midfielder"},
      "from": "West Ham",
      "date": "2023-07-15",
      "fee": 105000000,
      "currency": "GBP"
    },
    {
      "player": {"name": "Kai Havertz", "position": "Midfielder"},
      "from": "Chelsea",
      "date": "2023-06-28",
      "fee": 65000000,
      "currency": "GBP"
    }
  ],
  "summerDepartures": [
    {
      "player": {"name": "Granit Xhaka", "position": "Midfielder"},
      "to": "Bayer Leverkusen",
      "date": "2023-06-21",
      "fee": 25000000,
      "currency": "EUR"
    }
  ]
}
```

### Managerial Changes Example

```json
{
  "team": "Chelsea",
  "season": "2023/24",
  "managerialChanges": [
    {
      "date": "2023-04-02",
      "outgoing": "Graham Potter",
      "incoming": "Frank Lampard",
      "isCaretaker": true
    },
    {
      "date": "2023-07-01",
      "outgoing": "Frank Lampard",
      "incoming": "Mauricio Pochettino",
      "isCaretaker": false
    }
  ]
}
```

## Data Models Reference

All extended data is represented by these strongly-typed models:

- **`MatchExtendedData`** - Container for all extended match information
- **`Player`** - Player with name, number, position, nationality, **age**
- **`Stadium`** - Venue with capacity, location, surface type
- **`GoalEvent`** - Goal with scorer, assister, minute, type
- **`SubstitutionEvent`** - Substitution with players, minute, reason
- **`CardEvent`** - Card with player, minute, type (yellow/red)
- **`Transfer`** - Player transfer with clubs, date, fee, type
- **`ManagerialChange`** - Manager change with date, reason
- **`TeamSeasonInfo`** - Complete team information for a season
- **`WeatherConditions`** - Temperature, conditions, wind, humidity

### Player Age Support

The `Player` model includes comprehensive age support:

- **`Age`** - Direct age value (can be provided in lineup)
- **`DateOfBirth`** - Birth date for age calculation
- **`CalculateAge(referenceDate)`** - Method to calculate age at match time

**Age-based computed properties on `MatchExtendedData`:**
- `HomeAverageAge` - Average age of home starting XI
- `AwayAverageAge` - Average age of away starting XI
- `HomeYoungestPlayer` - Youngest home starter
- `HomeOldestPlayer` - Oldest home starter
- `AwayYoungestPlayer` - Youngest away starter
- `AwayOldestPlayer` - Oldest away starter

**Example usage in analytics:**
```csharp
if (match.ExtendedData?.HomeAverageAge.HasValue == true)
{
    Console.WriteLine($"Home team average age: {match.ExtendedData.HomeAverageAge:F1} years");
    Console.WriteLine($"Youngest: {match.ExtendedData.HomeYoungestPlayer}");
    Console.WriteLine($"Oldest: {match.ExtendedData.HomeOldestPlayer}");
}
```

## Future Extensions

The architecture supports easy addition of:

- **xG (Expected Goals)** per shot
- **Possession percentages**
- **Shot maps** (location, type, outcome)
- **Pass networks**
- **Heat maps**
- **Injury reports**
- **Player ratings**
- **Tactical analysis**
- **Pre-match odds**
- **Post-match analysis links**

Simply add new columns to your CSV or JSON files, and the discrete model architecture will handle them automatically!

## Benefits of Extended Data

When extended data is available, the tool can provide:

1. **Richer Visualizations**
   - Manager records and comparisons
   - Formation effectiveness analysis
   - Goalscorer charts and statistics
   - Attendance trends
   - Home advantage metrics
   - **Team age profiles and trends**
   - **Experience vs youth analysis**

2. **Advanced Analytics**
   - Impact of managerial changes on results
   - Correlation between attendance and performance
   - Formation vs opponent formation analysis
   - Substitution timing and effectiveness
   - Card accumulation and suspensions
   - **Age-based performance patterns**
   - **Squad age diversity analysis**
   - **Impact of young vs experienced players**

3. **Historical Context**
   - Track player transfers between clubs
   - Squad evolution throughout the season
   - Managerial tenure and results
   - Stadium moves or renovations

4. **Enhanced Reporting**
   - Match reports with full event timeline
   - Season summaries with key personnel
   - Transfer window impact analysis

## Best Practices

1. **Start Simple** - Begin with basic match data, add detail gradually
2. **Be Consistent** - Use the same format across all matches
3. **Document Sources** - Note where extended data comes from
4. **Validate Data** - Check that goalscorers, attendances, etc. are accurate
5. **Use Standards** - Follow conventional formats (e.g., "4-3-3" for formations)

## Data Sources

Good sources for extended data:

- **football-data.co.uk** - Basic stats included
- **Official club websites** - Lineups, attendance, match reports
- **Transfermarkt** - Transfer data, squad information
- **Wikipedia** - Season summaries, historical data
- **WhoScored** - Detailed match stats and events
- **FBref** - Advanced statistics
- **Official league APIs** - Real-time comprehensive data

---

**Remember:** You control the level of detail! From a simple 4-column CSV to a 50-column masterpiece, FootballDataTool adapts to your data.
