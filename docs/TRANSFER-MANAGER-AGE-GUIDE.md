# Transfer and Manager Age Features

## Overview

Comprehensive age tracking for players, managers, and transfers, enabling advanced analytics on squad planning, transfer strategy, and managerial profiles.

## Key Features

### 1. Player Transfer Ages

**Track player age at time of transfer:**
- Age when signing for a club
- Age when leaving a club
- Age-based transfer value analysis

**Benefits:**
- Identify clubs that buy young vs experienced
- Analyze transfer strategy (development vs immediate impact)
- Value analysis (price per year, age-adjusted ROI)

### 2. Manager Ages

**Comprehensive manager profiling:**
- Current age
- Years of experience
- Age at appointment
- Age at dismissal (if applicable)

**Benefits:**
- Compare young vs experienced managers
- Track managerial career progression
- Identify coaching trends

### 3. Transfer Prices

**Complete transfer financial tracking:**
- Transfer fees
- Currency support (GBP, EUR, USD, etc.)
- Contract length
- Fee per year calculation
- Net spend analysis

## Models

### Enhanced Transfer Model

```csharp
public class Transfer
{
    public Player Player { get; set; }
    public string FromClub { get; set; }
    public string ToClub { get; set; }
    public DateTime TransferDate { get; set; }
    
    // Financial
    public decimal? Fee { get; set; }
    public string? FeeCurrency { get; set; }
    public int? ContractYears { get; set; }
    public decimal? FeePerYear { get; } // Computed
    
    // Age tracking
    public int? PlayerAgeAtTransfer { get; set; }
    
    // Type
    public TransferType Type { get; set; }
    public bool IsLoan { get; set; }
}
```

### Manager Model

```csharp
public class Manager
{
    public string Name { get; set; }
    public int? Age { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public int? YearsOfExperience { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public List<string> PreferredFormations { get; set; }
    
    public int? CalculateAge(DateTime referenceDate);
}
```

### Enhanced TeamSeasonInfo

```csharp
public class TeamSeasonInfo
{
    public Manager? StartingManager { get; set; }
    public List<Transfer> SummerSignings { get; set; }
    public List<Transfer> SummerDepartures { get; set; }
    public List<Transfer> WinterSignings { get; set; }
    public List<Transfer> WinterDepartures { get; set; }
    
    // Computed properties
    public decimal? TotalSpending { get; }
    public decimal? TotalIncome { get; }
    public decimal? NetSpend { get; }
    public double? AverageSquadAge { get; }
}
```

## JSON Format

### Team Transfer Data

```json
{
  "team": "Arsenal",
  "season": "2023/24",
  "startingManager": {
    "name": "Mikel Arteta",
    "age": 41,
    "dateOfBirth": "1982-03-26",
    "nationality": "Spain",
    "yearsOfExperience": 5,
    "appointmentDate": "2019-12-20",
    "preferredFormations": ["4-3-3", "4-2-3-1"]
  },
  "summerSignings": [
    {
      "player": {
        "name": "Declan Rice",
        "position": "Midfielder",
        "dateOfBirth": "1999-01-14"
      },
      "fromClub": "West Ham United",
      "toClub": "Arsenal",
      "transferDate": "2023-07-15",
      "fee": 105000000,
      "feeCurrency": "GBP",
      "playerAgeAtTransfer": 24,
      "contractYears": 5,
      "type": "Permanent",
      "notes": "Club record signing"
    }
  ],
  "summerDepartures": [
    {
      "player": {
        "name": "Granit Xhaka",
        "position": "Midfielder"
      },
      "fromClub": "Arsenal",
      "toClub": "Bayer Leverkusen",
      "transferDate": "2023-06-21",
      "fee": 25000000,
      "feeCurrency": "EUR",
      "playerAgeAtTransfer": 30,
      "notes": "End of 7-year career"
    }
  ]
}
```

### Managerial Change

```json
{
  "managerialChanges": [
    {
      "outgoingManager": {
        "name": "Graham Potter",
        "age": 48,
        "dateOfBirth": "1975-05-20"
      },
      "incomingManager": {
        "name": "Frank Lampard",
        "age": 44,
        "dateOfBirth": "1978-06-20"
      },
      "changeDate": "2023-04-02",
      "reason": "Poor results",
      "isCaretaker": true,
      "recordBeforeChange": "11W 8D 12L"
    }
  ]
}
```

## Analytics Enabled

### 1. Transfer Strategy Analysis

**Age Profile at Signing:**
```
Arsenal - Avg age in: 24.3 years (youth focus)
Man City - Avg age in: 26.8 years (peak/experience)
```

**Age Profile at Departure:**
```
Arsenal - Avg age out: 27.5 years (sell before decline)
Liverpool - Avg age out: 29.2 years (maximize value)
```

### 2. Financial Analysis

**Net Spend Rankings:**
```
Team         Spent    Income    Net
Chelsea      £300m    £120m     £180m
Arsenal      £200m    £60m      £140m
Liverpool    £120m    £150m     -£30m
```

**Value for Age:**
```
Player          Age    Fee      Per Year
Declan Rice     24     £105m    £21m
Caicedo         21     £115m    £14.4m (8-year)
```

### 3. Manager Demographics

**Age Distribution:**
```
Under 40:  15% (Young tactical minds)
40-50:     45% (Prime experience)
50-60:     30% (Veteran leaders)
60+:       10% (Legendary status)
```

**Experience vs Age:**
```
Mikel Arteta:  41 years, 5y experience (rapid rise)
Pep Guardiola: 52 years, 15y experience (established)
```

### 4. Squad Age Trends

**Before/After Transfer Window:**
```
Arsenal Squad Age:
Before: 26.8 years
After:  25.3 years (rejuvenation)
```

### 5. Career Stage Analysis

**Players by transfer age:**
- **18-21**: Potential/development signings
- **22-25**: Prime years, high value
- **26-29**: Peak performance, experience
- **30+**: Veterans, short-term fixes

## Use Cases

### Recruitment Strategy

```csharp
// Teams that buy young
var youthFocused = teamData
    .Where(t => t.Value.SummerSignings.Any())
    .Select(t => new
    {
        Team = t.Key,
        AvgAgeIn = t.Value.SummerSignings
            .Average(s => s.PlayerAgeAtTransfer ?? 0)
    })
    .Where(x => x.AvgAgeIn < 23)
    .OrderBy(x => x.AvgAgeIn);
```

### Transfer Value Analysis

```csharp
// Best value signings (fee per year)
var valueSignings = allTransfers
    .Where(t => t.FeePerYear.HasValue)
    .OrderBy(t => t.FeePerYear)
    .Take(10);
```

### Managerial Trends

```csharp
// Youngest managers
var youngManagers = teamData
    .Where(t => t.Value.StartingManager?.Age < 45)
    .OrderBy(t => t.Value.StartingManager.Age);

// Most experienced for their age
var earlyAchievers = teamData
    .Select(t => t.Value.StartingManager)
    .Where(m => m?.Age.HasValue == true && m.YearsOfExperience.HasValue)
    .OrderByDescending(m => (double)m.YearsOfExperience.Value / m.Age.Value);
```

### Age at Departure Analysis

```csharp
// When teams sell players
var departureAges = teamData
    .SelectMany(t => t.Value.SummerDepartures.Concat(t.Value.WinterDepartures))
    .Where(d => d.PlayerAgeAtTransfer.HasValue)
    .GroupBy(d => d.FromClub)
    .Select(g => new
    {
        Club = g.Key,
        AvgDepartureAge = g.Average(d => d.PlayerAgeAtTransfer.Value)
    });
```

## Sample Data Files

- `data/arsenal_2023-24_transfers.json` - Arsenal's complete transfer window
- `data/chelsea_2023-24_transfers.json` - Chelsea with managerial change

## Visualization Features

### Transfer Analysis View

- Spending overview table
- Age in/out comparison
- Detailed team breakdowns
- Individual transfer listings

### Manager Comparison View

- Manager profile table
- Age and experience stats
- Preferred formations
- Nationality distribution

## Integration with Match Data

Managers automatically linked to matches:

```csharp
// Match extended data includes manager details
if (match.ExtendedData?.HomeManagerDetails != null)
{
    var manager = match.ExtendedData.HomeManagerDetails;
    var ageAtMatch = manager.CalculateAge(match.Date);
    Console.WriteLine($"{manager.Name} was {ageAtMatch} at this match");
}
```

## Future Analytics

With age data, you can analyze:

1. **Transfer ROI**
   - Age-adjusted performance value
   - Peak years utilization
   - Resale value prediction

2. **Squad Evolution**
   - Age trend over multiple windows
   - Generational transitions
   - Youth integration success

3. **Managerial Success**
   - Age vs win rate correlation
   - Experience impact on tactics
   - Youngest/oldest title winners

4. **Market Trends**
   - Average transfer age by position
   - Fee inflation by age bracket
   - Contract length trends

5. **Career Trajectories**
   - Manager career progression paths
   - Player peak performance ages
   - Longevity by position

## Best Practices

### Recording Transfer Ages

1. **Use date of birth when possible** - Most accurate
2. **Calculate age at exact transfer date**
3. **Note: Ages are at time of transfer, not current**

### Financial Data

1. **Always specify currency** - Avoid exchange rate confusion
2. **Use actual fee, not potential add-ons** - Unless clearly stated
3. **Mark undisclosed as null, not zero**

### Manager Data

1. **Include date of birth for historical accuracy**
2. **Track years of experience separately from age**
3. **Note previous clubs for context**

## Summary

The transfer and manager age features enable:
- ✅ Complete transfer financial tracking
- ✅ Age-based recruitment analysis
- ✅ Managerial demographic profiling
- ✅ Squad age evolution tracking
- ✅ Value-for-age analytics
- ✅ Career stage identification
- ✅ Net spend calculations
- ✅ Transfer strategy patterns

Perfect for understanding how clubs build squads, the age profiles they target, and the experience levels they value in both players and managers.

---

**Files:** `Transfer.cs`, `Manager.cs`, `TeamSeasonInfo.cs`, `TeamDataLoader.cs`
**Visualizers:** `TransferAnalysisVisualiser.cs`
**Sample Data:** `arsenal_2023-24_transfers.json`, `chelsea_2023-24_transfers.json`
