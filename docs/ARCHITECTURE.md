# Discrete CSV Model Architecture

## Overview

FootballDataTool uses a **discrete CSV model** approach that separates raw CSV data from business logic. This design pattern provides maximum flexibility for handling various CSV formats while maintaining clean, maintainable code.

## Architecture Layers

### 1. Raw CSV Layer (`CsvMatchRecord`)

```csharp
public class CsvMatchRecord
{
    public string? Division { get; set; }
    public string? Season { get; set; }
    public string? Date { get; set; }
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
    public string? HomeGoals { get; set; }
    public string? AwayGoals { get; set; }
    // ... additional fields
    public Dictionary<string, string> AdditionalFields { get; set; }
}
```

**Purpose:**
- Captures raw CSV data as strings before any parsing or validation
- Preserves all source columns, including those not mapped to known fields
- Acts as an intermediate representation between CSV and domain models

**Benefits:**
- No data loss during ingestion
- Easy to debug parsing issues
- Supports dynamic/unknown columns
- Future-proof for new CSV formats

### 2. Metadata Detection Layer (`SeasonMetadata`)

```csharp
public class SeasonMetadata
{
    public string Season { get; set; }
    public string League { get; set; }
    public string Country { get; set; }
    public int TotalMatches { get; set; }
    public (DateTime? Start, DateTime? End) DateRange { get; set; }
}
```

**Detection Strategies:**

1. **Explicit Column Detection**
   - Looks for `Div`, `Season` columns in CSV
   - Maps division codes (E0 → Premier League)

2. **Filename Analysis**
   - Extracts season: "data_2023-24.csv" → "2023/24"
   - Detects league: "premier_league.csv" → Premier League

3. **Date Range Inference**
   - Analyzes match dates
   - Infers season from Aug-May pattern

4. **Team Name Recognition**
   - Identifies leagues from well-known teams
   - Example: Arsenal, Liverpool → Premier League

### 3. Business Model Layer (`Match`)

```csharp
public class Match
{
    public int Gameweek { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? Time { get; set; }
    public string? Referee { get; set; }
    
    // Computed properties
    public string Result { get; }
    public int HomePoints { get; }
    public int AwayPoints { get; }
}
```

**Purpose:**
- Strongly-typed domain model
- Contains business logic (points calculation, results)
- Used throughout the application for analysis

## Data Flow

```
CSV File
    ↓
[CsvDataService.LoadCsvRecords()]
    ↓
List<CsvMatchRecord> (Raw strings)
    ↓
[MetadataDetectionService.DetectMetadata()]
    ↓
SeasonMetadata (League/Season detected)
    ↓
[CsvDataService.TransformToMatches()]
    ↓
List<Match> (Validated business objects)
    ↓
[MatchAnalyzer] → Visualizers
```

## Key Design Decisions

### Why Discrete Models?

**Traditional Approach (Direct Mapping):**
```csharp
// Tightly coupled to CSV structure
public class Match
{
    [Name("FTHG")]  // CSV-specific annotations
    public int HomeGoals { get; set; }
}
```

**Discrete Model Approach:**
```csharp
// Step 1: Capture raw data
CsvMatchRecord rawRecord = LoadFromCsv();

// Step 2: Transform & validate
Match match = Transform(rawRecord);
```

**Benefits:**
1. **Flexibility** - Easy to support multiple CSV formats
2. **Validation** - Centralized parsing with error handling
3. **Testing** - Can test parsing independently from business logic
4. **Evolution** - New CSV formats don't require model changes
5. **Debugging** - Raw data available for inspection

### Column Name Aliasing

The `CsvDataService` supports multiple column names for each field:

```csharp
private static readonly string[] HomeGoalsColumns = 
    ["FTHG", "HomeGoals", "HG", "Home Goals", "HGoals"];
```

This allows the tool to work with:
- football-data.co.uk format (FTHG/FTAG)
- Custom spreadsheets (HomeGoals/AwayGoals)
- Abbreviated formats (HG/AG)
- Various data sources without modification

### Graceful Degradation

Missing optional fields don't break the application:

```csharp
if (columnMap["Time"] >= 0)
    record.Time = csv.GetField(columnMap["Time"]);
// Time field is optional - continues if not present
```

Gameweeks are inferred if not provided:
```csharp
bool hasGameweeks = matches.Any(m => m.Gameweek > 0);
if (!hasGameweeks)
    AssignGameweeks(matches);  // Round-robin inference
```

## Adding New CSV Formats

### Example: Adding UEFA Competition Support

1. **Add division mapping:**
```csharp
// In MetadataDetectionService.cs
["UCL"] = ("UEFA Champions League", "Europe"),
["UEL"] = ("UEFA Europa League", "Europe"),
```

2. **Add team recognition:**
```csharp
var uclTeams = new[] { "Real Madrid", "Bayern Munich", "PSG" };
if (uclTeams.Count(t => allTeams.Contains(t)) >= 3)
{
    metadata.League = "UEFA Champions League";
    metadata.Country = "Europe";
}
```

3. **Add column aliases (if needed):**
```csharp
private static readonly string[] PhaseColumns = 
    ["Phase", "Round", "Stage"];  // Group stage, Knockout, etc.
```

That's it! The discrete model handles everything else automatically.

## Testing Strategy

The architecture enables comprehensive testing:

### Unit Tests for Parsing
```csharp
[Fact]
public void LoadCsvRecords_WithVariousFormats_ParsesCorrectly()
{
    var csv = "FTHG,FTAG\n2,1\n";
    var records = service.LoadCsvRecords(csv);
    Assert.Equal("2", records[0].HomeGoals);
}
```

### Unit Tests for Metadata Detection
```csharp
[Fact]
public void DetectMetadata_WithE0Division_ReturnsPremierLeague()
{
    var records = new List<CsvMatchRecord>
    {
        new() { Division = "E0", Date = "12/08/2023" }
    };
    var metadata = MetadataDetectionService.DetectMetadata(records);
    Assert.Equal("Premier League", metadata.League);
}
```

### Integration Tests
```csharp
[Fact]
public void LoadFromFile_RealWorldCsv_ProducesValidMatches()
{
    var matches = service.LoadFromFile("data/premier_league_2023-24.csv");
    Assert.All(matches, m => Assert.InRange(m.HomeGoals, 0, 20));
}
```

## Performance Considerations

- **Single Pass Parsing** - CSV is read once, all data extracted
- **Lazy Metadata Detection** - Only runs when CSV is loaded
- **Efficient Column Mapping** - Index-based access after header parse
- **Minimal Allocations** - Reuses string arrays for column aliases

## Future Extensions

The discrete model enables easy future enhancements:

1. **Multiple File Formats**
   - Add JSON/XML parsers
   - Same `CsvMatchRecord` → `Match` transformation

2. **Data Enrichment**
   - Add weather data
   - Include betting odds
   - Link to external APIs

3. **Advanced Statistics**
   - xG (expected goals)
   - Possession percentages
   - Shot statistics

All without changing core business logic!

---

**Summary:** The discrete CSV model provides a robust, flexible foundation that separates data ingestion from business logic, enabling the tool to work with any conventional football data CSV format.
