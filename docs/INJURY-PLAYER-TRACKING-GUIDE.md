# Injury and Player Tracking Features

This document describes the enhanced player tracking features in FootballDataTool, including injury tracking, minutes played analysis, nationality data, birthday/zodiac tracking, and previous club history.

## Overview

The tool now supports comprehensive player profiling beyond basic lineup information:

- **Injury Tracking**: Track players unavailable due to injuries with start/end dates
- **Minutes Played**: Record actual playing time for each player per match
- **Nationality**: Player nationalities for squad composition analysis
- **Birthdays & Zodiacs**: Fun player birthday tracking with Western and Chinese zodiac signs
- **Previous Clubs**: Track where players came from (club, league, and table position)

All features are **completely optional** and follow the progressive enhancement model.

---

## 1. Injury Tracking

### Model: `Injury`

Represents a player injury with dates and severity.

```csharp
public class Injury
{
    public Player Player { get; set; }
    public string InjuryType { get; set; }
    public DateTime InjuryDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public InjurySeverity Severity { get; set; }
    
    // Check if player was injured on a specific date
    public bool IsInjuredOn(DateTime date);
}
```

### Severity Levels

```csharp
public enum InjurySeverity
{
    Minor,              // 1-7 days
    Moderate,           // 1-4 weeks
    Serious,            // 1-3 months
    LongTerm,           // 3+ months
    CareerThreatening
}
```

### CSV Format

**Column Names**: `HomeInjuries`, `AwayInjuries`, `HInjuries`, `AInjuries`, `HomeOut`, `AwayOut`, `HomeMissing`, `AwayMissing`

**Format**: Semi-colon or comma separated entries:
```
Player Name (Injury Type, dd/MM/yyyy - dd/MM/yyyy)
```

**Examples**:
```csv
HomeInjuries
"Tomiyasu (Calf, 05/08/2023 - 20/08/2023); Timber (Knee, 06/08/2023 - )"
"De Bruyne (Hamstring, 09/08/2023 - 15/09/2023); Phillips (Calf, 05/08/2023 - 25/08/2023)"
```

- Leave end date empty if player still injured: `"Player (ACL, 01/01/2023 - )"`
- Severity is auto-calculated from duration

### Match Extended Data

Injuries are stored per match:

```csharp
public class MatchExtendedData
{
    public List<Injury> HomeInjuries { get; set; }
    public List<Injury> AwayInjuries { get; set; }
    
    // Computed properties
    public int HomeInjuryCount => HomeInjuries.Count;
    public int AwayInjuryCount => AwayInjuries.Count;
}
```

---

## 2. Minutes Played Tracking

### Model: `PlayerAppearance`

Detailed appearance record for each player in a match.

```csharp
public class PlayerAppearance
{
    public Player Player { get; set; }
    public string Team { get; set; }
    public bool IsStarting { get; set; }
    public int MinutesPlayed { get; set; }
    public int? ShirtNumber { get; set; }
    public string? Position { get; set; }
    public int? SubbedOn { get; set; }
    public int? SubbedOff { get; set; }
    public int Goals { get; set; }              // Auto-populated from goal events
    public int Assists { get; set; }            // Auto-populated from goal events
    public List<CardType> Cards { get; set; }

    public bool PlayedFullMatch => MinutesPlayed >= 90;
}
```

**Note**: `Goals` and `Assists` are automatically populated from parsed goal events. If your CSV has goalscorers with assists (e.g., `"Player 45' (assist: Assister)"`), the system will automatically link them to player appearances.

### CSV Format

**Column Names**: `HomeMinutes`, `AwayMinutes`, `HMinutes`, `AMinutes`, `HomeMinutesPlayed`, `AwayMinutesPlayed`

**Format**: Semi-colon or comma separated entries:
```
Player Name 90'
Player Name (65')
Player Name 45
```

**Example**:
```csv
HomeMinutesPlayed
"Ramsdale 90'; White 90'; Gabriel 90'; Saliba 90'; Zinchenko 90'; Partey 90'; Odegaard 90'; Saka 90'; Havertz 78'; Martinelli 90'; Jesus 90'; Nketiah (12'); Vieira (12'); Smith Rowe (0')"
```

### Automatic Generation

If no minutes data is provided, the parser automatically generates appearances:
- **Starting XI**: Assumed 90 minutes
- **Substitutes**: Assumed 0 minutes (unused)

### Match Extended Data

```csharp
public class MatchExtendedData
{
    public List<PlayerAppearance> HomeAppearances { get; set; }
    public List<PlayerAppearance> AwayAppearances { get; set; }
    
    // Computed properties
    public int HomeTotalMinutes => HomeAppearances.Sum(a => a.MinutesPlayed);
    public int AwayTotalMinutes => AwayAppearances.Sum(a => a.MinutesPlayed);
}
```

---

## 3. Nationality Tracking

### Player Model Enhancement

```csharp
public class Player
{
    public string? Nationality { get; set; }
}
```

### CSV Format

Nationalities can be included in player entries using various formats:

**In Lineups**:
```csv
HomeLineup
"1. Ramsdale (25, ENG, GK); 4. White (25, ENG, RB); 6. Gabriel (25, BRA, CB)"
```

**Future Enhancement**: Dedicated nationality columns will be supported in future updates.

### Usage

```csharp
var englishPlayers = match.ExtendedData.HomeStartingLineup
    .Where(p => p.Nationality == "ENG")
    .ToList();
```

---

## 4. Birthday & Zodiac Tracking

### Player Model Enhancement

```csharp
public class Player
{
    public DateTime? DateOfBirth { get; set; }
    
    // Computed properties
    public string? ZodiacSign { get; }        // Western zodiac (Aries to Pisces)
    public string? ChineseZodiac { get; }     // Chinese zodiac (Rat to Pig)
    
    // Check if it's player's birthday
    public bool IsBirthdayOn(DateTime date);
}
```

### Manager Model Enhancement

```csharp
public class Manager
{
    public DateTime? DateOfBirth { get; set; }
    
    public string? ZodiacSign { get; }
    public string? ChineseZodiac { get; }
    public bool IsBirthdayOn(DateTime date);
}
```

### Zodiac Signs

**Western Zodiac** (based on birth date):
- Aries, Taurus, Gemini, Cancer, Leo, Virgo, Libra, Scorpio, Sagittarius, Capricorn, Aquarius, Pisces

**Chinese Zodiac** (based on birth year):
- Rat, Ox, Tiger, Rabbit, Dragon, Snake, Horse, Goat, Monkey, Rooster, Dog, Pig

### Match Extended Data

Find birthdays on match day:

```csharp
public class MatchExtendedData
{
    // Players having birthday on match day
    public List<Player> PlayersWithBirthday(DateTime matchDate);
    
    // Managers having birthday on match day
    public List<Manager> ManagersWithBirthday(DateTime matchDate);
}
```

### Example Usage

```csharp
foreach (var match in matches)
{
    var birthdayPlayers = match.ExtendedData?.PlayersWithBirthday(match.Date ?? DateTime.Today);
    if (birthdayPlayers?.Any() == true)
    {
        Console.WriteLine($"🎂 Birthday players in {match.HomeTeam} vs {match.AwayTeam}:");
        foreach (var player in birthdayPlayers)
        {
            Console.WriteLine($"   {player.Name} ({player.ZodiacSign})");
        }
    }
}
```

---

## 5. Previous Club Tracking

### Player Model Enhancement

```csharp
public class Player
{
    public string? PreviousClub { get; set; }
    public string? PreviousLeague { get; set; }
    public int? PreviousClubPosition { get; set; }  // League position when player left
}
```

### JSON Format (Transfer Data)

The `Transfer` model already supports this:

```json
{
  "player": {
    "name": "Kai Havertz",
    "nationality": "Germany",
    "previousClub": "Chelsea",
    "previousLeague": "Premier League",
    "previousClubPosition": 12
  },
  "from": "Chelsea",
  "fromLeague": "Premier League",
  "fromPosition": 12,
  "type": "Permanent"
}
```

### Usage

```csharp
var fromRelegation = transfers
    .Where(t => t.Player.PreviousClubPosition >= 18)
    .ToList();

var fromChampions = transfers
    .Where(t => t.Player.PreviousClubPosition == 1)
    .ToList();
```

---

## Complete CSV Example

```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,HomeManager,AwayManager,HomeLineup,AwayLineup,Stadium,Attendance,HomeInjuries,AwayInjuries,HomeMinutesPlayed,AwayMinutesPlayed
E0,2023/24,1,11/08/2023,Arsenal,Nottm Forest,2,1,Mikel Arteta,Steve Cooper,"1. Ramsdale (25, GK); 4. White (25, RB); 6. Gabriel (25, CB); 2. Saliba (22, CB); 35. Zinchenko (26, LB); 5. Partey (30, DM); 8. Odegaard (24, CM); 7. Saka (21, RW); 29. Havertz (24, AM); 11. Martinelli (22, LW); 9. Jesus (26, ST)","1. Turner (29, GK); 32. Aina (27, RB); 26. McKenna (26, CB); 4. Worrall (26, CB); 7. Williams (22, LB); 22. Yates (26, DM); 5. Mangala (25, CM); 10. Gibbs-White (23, AM); 20. Danilo (22, RW); 11. Elanga (21, LW); 21. Awoniyi (26, ST)",Emirates Stadium,60184,"Tomiyasu (Calf, 05/08/2023 - 20/08/2023); Timber (Knee, 06/08/2023 - )","Shelvey (Calf, 01/08/2023 - 15/08/2023); Niakhate (Hamstring, 28/07/2023 - 18/08/2023)","Ramsdale 90'; White 90'; Gabriel 90'; Saliba 90'; Zinchenko 90'; Partey 90'; Odegaard 90'; Saka 90'; Havertz 78'; Martinelli 90'; Jesus 90'; Nketiah (12'); Vieira (12'); Smith Rowe (0')","Turner 90'; Aina 90'; McKenna 90'; Worrall 90'; Williams 90'; Yates 90'; Mangala 90'; Gibbs-White 90'; Danilo 65'; Elanga 90'; Awoniyi 90'; Kouyate (25'); Johnson (0')"
```

---

## Analysis Capabilities

### Injury Impact Analysis

```csharp
// Teams with most injuries
var injuryImpact = matches
    .GroupBy(m => m.HomeTeam)
    .Select(g => new {
        Team = g.Key,
        TotalInjuries = g.Sum(m => m.ExtendedData?.HomeInjuryCount ?? 0)
    })
    .OrderByDescending(x => x.TotalInjuries);

// Long-term injuries
var seriousInjuries = matches
    .SelectMany(m => m.ExtendedData?.HomeInjuries ?? new List<Injury>())
    .Where(i => i.Severity >= InjurySeverity.Serious);
```

### Minutes Analysis

```csharp
// Players with most minutes
var topPlayers = matches
    .SelectMany(m => m.ExtendedData?.HomeAppearances ?? new List<PlayerAppearance>())
    .GroupBy(a => a.Player.Name)
    .Select(g => new {
        Player = g.Key,
        TotalMinutes = g.Sum(a => a.MinutesPlayed),
        Appearances = g.Count(),
        AvgMinutes = g.Average(a => a.MinutesPlayed)
    })
    .OrderByDescending(x => x.TotalMinutes);
```

### Birthday Analysis

```csharp
// Zodiac sign distribution
var zodiacDistribution = players
    .Where(p => p.ZodiacSign != null)
    .GroupBy(p => p.ZodiacSign)
    .Select(g => new { Sign = g.Key, Count = g.Count() })
    .OrderByDescending(x => x.Count);
```

---

## Design Principles

1. **Progressive Enhancement**: All features are optional
2. **Backward Compatibility**: Existing CSVs without new fields work perfectly
3. **Flexible Formats**: Multiple input formats supported
4. **Automatic Calculation**: Severity, zodiac signs computed automatically
5. **Graceful Degradation**: Missing data doesn't break functionality

---

## See Also

- [AGE-FEATURE-SUMMARY.md](AGE-FEATURE-SUMMARY.md) - Player age tracking
- [TRANSFER-MANAGER-AGE-GUIDE.md](TRANSFER-MANAGER-AGE-GUIDE.md) - Transfer and manager features
- [EXTENDED-DATA-GUIDE.md](EXTENDED-DATA-GUIDE.md) - General extended data guide
- [CSV-FORMAT-GUIDE.md](CSV-FORMAT-GUIDE.md) - Complete CSV column reference
