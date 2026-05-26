# Developer Quick Reference

## Adding New Extended Fields

### Step 1: Add to CsvMatchRecord

```csharp
// In src/FootballDataTool/Models/CsvMatchRecord.cs
public string? MyNewField { get; set; }
```

### Step 2: Add Column Aliases

```csharp
// In src/FootballDataTool/Services/CsvDataService.cs
private static readonly string[] MyNewFieldColumns = 
    ["MyNewField", "My New Field", "MNF"];
```

### Step 3: Add to Column Map

```csharp
// In LoadCsvRecords method
var columnMap = new Dictionary<string, int>
{
    // ... existing mappings ...
    ["MyNewField"] = FindColumnIndex(headers, MyNewFieldColumns)
};
```

### Step 4: Add to Switch Statement

```csharp
// In LoadCsvRecords method
switch (kvp.Key)
{
    // ... existing cases ...
    case "MyNewField": record.MyNewField = value; break;
}
```

### Step 5: Parse in ExtendedDataParser (if needed)

```csharp
// In src/FootballDataTool/Services/ExtendedDataParser.cs
public static MatchExtendedData? ParseExtendedData(CsvMatchRecord record, Match match)
{
    // ... existing code ...
    
    // Parse your new field
    if (!string.IsNullOrWhiteSpace(record.MyNewField))
    {
        extendedData.MyParsedData = ParseMyNewField(record.MyNewField);
    }
}
```

## Adding New League Support

### Add Division Mapping

```csharp
// In src/FootballDataTool/Services/MetadataDetectionService.cs
private static readonly Dictionary<string, (string League, string Country)> DivisionMappings = new()
{
    // ... existing mappings ...
    ["X1"] = ("Your League Name", "Your Country"),
};
```

### Add Team Recognition (Optional)

```csharp
// In DetectLeagueFromTeams method
var yourLeagueTeams = new[] { "Team1", "Team2", "Team3" };
if (yourLeagueTeams.Count(t => allTeams.Contains(t)) >= 3)
{
    metadata.League = "Your League Name";
    metadata.Country = "Your Country";
    return;
}
```

## Common Parsing Patterns

### Parse Delimited List
```csharp
private static List<string> ParseList(string? data, char delimiter = ';')
{
    if (string.IsNullOrWhiteSpace(data))
        return new List<string>();
    
    return data.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToList();
}
```

### Parse Time with Added Time
```csharp
private static (int Minute, int? AddedTime) ParseMatchTime(string timeStr)
{
    var match = Regex.Match(timeStr, @"(\d+)(?:\+(\d+))?");
    if (!match.Success) return (0, null);
    
    var minute = int.Parse(match.Groups[1].Value);
    var addedTime = match.Groups[2].Success 
        ? (int?)int.Parse(match.Groups[2].Value) 
        : null;
    
    return (minute, addedTime);
}
```

### Parse Player with Number
```csharp
private static Player ParsePlayer(string playerStr)
{
    var match = Regex.Match(playerStr, @"^(\d+)\.\s*(.+)$");
    if (match.Success)
    {
        return new Player
        {
            ShirtNumber = int.Parse(match.Groups[1].Value),
            Name = match.Groups[2].Value.Trim()
        };
    }
    
    return new Player { Name = playerStr.Trim() };
}
```

## Testing Extended Data

### Create Test CSV
```csv
Div,Season,GW,HomeTeam,AwayTeam,FTHG,FTAG,MyNewField
E0,2023/24,1,Arsenal,Chelsea,2,1,TestValue
```

### Test Parsing
```csharp
[Fact]
public void ParseMyNewField_WithValidData_ReturnsCorrectValue()
{
    var record = new CsvMatchRecord { MyNewField = "TestValue" };
    var match = new Match();
    
    var extendedData = ExtendedDataParser.ParseExtendedData(record, match);
    
    Assert.NotNull(extendedData);
    Assert.Equal("ExpectedValue", extendedData.MyParsedData);
}
```

## Model Design Guidelines

### Use Nullable Types for Optional Data
```csharp
public int? OptionalNumber { get; set; }
public DateTime? OptionalDate { get; set; }
public string? OptionalString { get; set; }
```

### Provide Default Values for Collections
```csharp
public List<Player> Players { get; set; } = new();
public Dictionary<string, string> Metadata { get; set; } = new();
```

### Add ToString() Overrides for Display
```csharp
public override string ToString() => 
    $"{Name} ({Age} years old)";
```

### Use Enums for Fixed Options
```csharp
public enum CardType
{
    Yellow,
    Red,
    SecondYellow
}
```

## Common Regex Patterns

```csharp
// Player with number: "10. Messi"
@"^(\d+)\.\s*(.+)$"

// Time with added time: "45+2'"
@"(\d+)(?:\+(\d+))?['\s]*"

// Goal with details: "Messi 45' (assist: Xavi)"
@"^(.+?)\s*[(\[]?(\d+)(?:\+(\d+))?[)\]]?['\s]*"

// Substitution: "Messi → Suarez 60'"
@"^(.+?)\s*(?:←|<-|->|→)\s*(.+?)\s*[(\[]?(\d+)(?:\+(\d+))?[)\]]?"

// Formation: "4-3-3", "3-5-2"
@"^\d{1}-\d{1}-\d{1}(?:-\d{1})?$"
```

## Performance Tips

### Use Compiled Regex
```csharp
private static readonly Regex MyRegex = 
    new(@"pattern", RegexOptions.Compiled);
```

### Avoid Repeated String Operations
```csharp
// Bad
if (data.ToLower().Contains("x") || data.ToLower().Contains("y"))

// Good
var lower = data.ToLower();
if (lower.Contains("x") || lower.Contains("y"))
```

### Use Spans for String Manipulation (Advanced)
```csharp
ReadOnlySpan<char> span = data.AsSpan();
// ... work with span ...
```

## Error Handling Patterns

### Graceful Parsing
```csharp
private static int? TryParseInt(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
        return null;
    
    return int.TryParse(value.Trim(), out int result) ? result : null;
}
```

### Defensive Null Checks
```csharp
public void ProcessMatch(Match match)
{
    if (match?.ExtendedData?.Goals == null)
        return;
    
    foreach (var goal in match.ExtendedData.Goals)
    {
        // Process goal
    }
}
```

### Try-Parse Pattern
```csharp
private static GoalEvent? TryParseGoal(string entry)
{
    try
    {
        // Parsing logic
        return new GoalEvent { /* ... */ };
    }
    catch
    {
        return null;  // Graceful failure
    }
}
```

## CSV Column Naming Best Practices

1. **Use consistent casing** - `HomeTeam` not `hometeam`
2. **Avoid special characters** - Use `HomeGoals` not `Home Goals (FT)`
3. **Be descriptive** - `HomeGoalscorers` not `HG`
4. **Use standard abbreviations** - `GW` for Gameweek, `FTHG` for Full Time Home Goals
5. **Group related columns** - `Home*`, `Away*` prefixes

## Documentation Template

When adding new features, document them like this:

```markdown
### New Field Name

**Column Names:** `FieldName`, `Field Name`, `FN`
**Format:** Description of expected format
**Example:** `Sample Value`
**Optional:** Yes/No
**Parsed Into:** `ModelClass.PropertyName`

#### Format Options

- **Option 1:** Description and example
- **Option 2:** Description and example

#### Example CSV

\`\`\`csv
HomeTeam,AwayTeam,FTHG,FTAG,NewField
Arsenal,Chelsea,2,1,Sample Value
\`\`\`
```

## Git Commit Message Format

```
feat: Add support for [feature name]

- Add [ModelClass] with [properties]
- Extend CsvMatchRecord with [fields]
- Add parsing logic in [Service]
- Add column aliases: [list]
- Add tests for [scenarios]
- Update documentation

Closes #[issue-number]
```

## Quick Commands

```bash
# Build
dotnet build

# Run
dotnet run --project src/FootballDataTool

# Run with specific CSV
dotnet run --project src/FootballDataTool -- data/your-file.csv

# Run tests
dotnet test

# Clean
dotnet clean

# Restore packages
dotnet restore
```

## Useful VS Code Snippets

### Add Property
```json
"CSV Property": {
    "prefix": "csvprop",
    "body": [
        "public string? ${1:PropertyName} { get; set; }"
    ]
}
```

### Add Column Alias
```json
"Column Alias": {
    "prefix": "colalias",
    "body": [
        "private static readonly string[] ${1:Field}Columns = [\"${1:Field}\", \"${2:Alias1}\", \"${3:Alias2}\"];"
    ]
}
```

---

**Happy Coding! ⚽**
