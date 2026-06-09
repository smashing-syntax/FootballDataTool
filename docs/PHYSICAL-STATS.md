# Physical Stats Analytics

## Overview

The FootballDataTool now supports physical attributes for players, enabling fun and insightful analytics like:
- Height brackets that perform best
- Average BMI by position
- Preferred foot distributions
- Tallest/shortest player comparisons

## Data Fields

Add these optional columns to your squad CSV files:

| Field | Description | Example |
|-------|-------------|---------|
| `Height` | Player height in centimeters | `185` |
| `Weight` | Player weight in kilograms | `78` |
| `PreferredFoot` | Left, Right, or Both | `Right` |

**Bonus:** BMI is automatically calculated if both height and weight are provided!

## Analytics Methods

### Position-Based Averages

```csharp
// Average height by position
var heightByPos = seasonData.GetAverageHeightByPosition();
// Returns: Dictionary<string, double>
// e.g., { "GK": 191.5, "CB": 186.2, "ST": 181.0, ... }

// Average weight by position
var weightByPos = seasonData.GetAverageWeightByPosition();

// Average BMI by position
var bmiByPos = seasonData.GetAverageBMIByPosition();
```

### Height Bracket Performance

Analyze which height ranges are most productive:

```csharp
var brackets = seasonData.GetPerformanceByHeightBracket();
// Returns: List<(string HeightBracket, int Players, int Goals, int Assists)>
// e.g., ("170-174cm", 8, 12, 9), ("185-189cm", 15, 6, 4), ...
```

This groups players into 5cm height brackets and shows their combined goals and assists.

### Preferred Foot Distribution

```csharp
var footDist = seasonData.GetPreferredFootDistribution();
// Returns: Dictionary<string, int>
// e.g., { "Right": 45, "Left": 18, "Both": 3 }
```

### Height Extremes

```csharp
var (tallest, shortest) = seasonData.GetHeightExtremes(5);
// Returns the 5 tallest and 5 shortest players
```

## Example Use Cases

### 1. Find Best Performing Height Range

```csharp
var brackets = seasonData.GetPerformanceByHeightBracket();
var mostProductive = brackets
    .OrderByDescending(b => b.Goals + b.Assists)
    .First();

Console.WriteLine($"Most productive: {mostProductive.HeightBracket}");
Console.WriteLine($"{mostProductive.Goals} goals, {mostProductive.Assists} assists");
```

### 2. Compare Team Physical Profiles

```csharp
foreach (var team in seasonData.Teams.Values)
{
    var avgHeight = team.FullSquad
        .Where(p => p.Height.HasValue)
        .Average(p => p.Height!.Value);

    var avgBMI = team.FullSquad
        .Where(p => p.BMI.HasValue)
        .Average(p => p.BMI!.Value);

    Console.WriteLine($"{team.Name}: {avgHeight:F1}cm, BMI {avgBMI:F2}");
}
```

### 3. Analyze Individual Player

```csharp
var player = seasonData.GetAllPlayers()
    .First(p => p.Name.Contains("Messi"));

Console.WriteLine($"{player.Name}:");
Console.WriteLine($"  Height: {player.Height}cm");
Console.WriteLine($"  Weight: {player.Weight}kg");
Console.WriteLine($"  BMI: {player.BMI:F2}");
Console.WriteLine($"  Preferred Foot: {player.PreferredFoot}");
```

## Example Output

See `src/FootballDataTool/Examples/PhysicalStatsExamples.cs` for complete working examples.

```
=== FIFA World Cup 2022 - Physical Stats Analysis ===

📏 Average Height by Position:
  GK : 191.0 cm
  CB : 186.5 cm
  ST : 181.2 cm
  LW : 175.0 cm

⚖️  Average Weight by Position:
  GK : 86.7 kg
  CB : 83.2 kg
  ST : 78.0 kg

💪 Average BMI by Position:
  GK : 23.76
  CB : 23.96
  ST : 23.75

🦶 Preferred Foot Distribution:
  Right : 38 players (82.6%)
  Left  :  8 players (17.4%)

=== Performance by Height Bracket ===

Height Range  | Players | Goals | Assists | Total
--------------|---------|-------|---------|-------
165-169cm     |       2 |     5 |       3 |     8
170-174cm     |       8 |    12 |       9 |    21
175-179cm     |      12 |     8 |       7 |    15
180-184cm     |      10 |     6 |       5 |    11
185-189cm     |       8 |     3 |       2 |     5
190-194cm     |       4 |     1 |       0 |     1
195-199cm     |       2 |     0 |       0 |     0

🏆 Most productive height bracket: 170-174cm
   8 players, 12 goals, 9 assists
```

## Fun Stats You Can Generate

- **"Short kings rule!"** - Show that 170-175cm players score more goals
- **Position stereotypes** - Confirm goalkeepers are tallest, wingers are shortest
- **BMI sweet spot** - Find the optimal BMI range for midfielders
- **Left-footer advantage** - Compare goal rates by preferred foot
- **Physical evolution** - Track how squad heights change over seasons
- **David vs Goliath** - Compare shortest vs tallest scorers

## Tips

1. **Optional data** - Physical attributes are completely optional. The tool works fine without them!
2. **Incomplete data** - If only some players have height/weight, analytics will work with whatever data exists
3. **Unit consistency** - Always use cm for height and kg for weight
4. **BMI interpretation** - Remember BMI doesn't account for muscle mass; elite athletes often have "high" BMIs!

---

For more examples, see:
- `src/FootballDataTool/Examples/PhysicalStatsExamples.cs`
- `data/examples/squad_examples_worldcup.csv` (sample data with physical stats)
