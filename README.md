# FootballDataTool

A C# .NET 8 console application that loads raw CSV football season data with **automatic league and season detection** and visualises it as interactive tables and charts directly in your terminal.

## Features

- 🔍 **Automatic Season Detection** - Detects season and league from CSV data or filename
- 🌍 **Multi-League Support** - Works with Premier League, La Liga, Serie A, Bundesliga, and more
- 📊 **Rich Visualizations**:
- 📊 **Rich Visualizations**:

| View | Description |
|------|-------------|
| **Home Form** | Table of each team's home results (W/D/L + score) per gameweek, colour-coded green/yellow/red |
| **Points Breakdown** | Cumulative points table by gameweek, final bar chart, and cross-group analysis (e.g. Top N vs Bottom N) |
| **Result Matrix** | N×N results grid in Home, Away, or Aggregate mode with head-to-head drill-down |
| **League Standings** | Full table (P/W/D/L/GF/GA/GD/Pts) filterable to any gameweek |

## Supported Leagues

The tool automatically recognizes division codes from popular football data sources:

| Code | League | Country |
|------|--------|---------|
| E0, EPL, PL | Premier League | England |
| E1 | Championship | England |
| SP1, LaLiga | La Liga | Spain |
| I1, SerieA | Serie A | Italy |
| D1 | Bundesliga | Germany |
| F1 | Ligue 1 | France |
| SC0 | Premiership | Scotland |
| N1 | Eredivisie | Netherlands |
| P1 | Primeira Liga | Portugal |

*League detection also works from team names or filename patterns!*

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build & Run

```bash
# Clone the repo
git clone https://github.com/smashing-syntax/FootballDataTool.git
cd FootballDataTool

# Build
dotnet build FootballDataTool.slnx

# Run with the bundled sample data
dotnet run --project src/FootballDataTool

# Run with your own CSV file
dotnet run --project src/FootballDataTool -- /path/to/your/season.csv
```

The app automatically uses `data/sample_season.csv` when no path is provided.

## CSV Format

The tool accepts flexible CSV formats with automatic metadata detection. **Any conventional football data CSV will work!**

### Required Columns

| Data | Accepted column names |
|------|-----------------------|
| Home Team | `HomeTeam`, `Home Team`, `Home`, `HTeam` |
| Away Team | `AwayTeam`, `Away Team`, `Away`, `ATeam` |
| Home Goals | `FTHG`, `HomeGoals`, `HG`, `Home Goals`, `HGoals` |
| Away Goals | `FTAG`, `AwayGoals`, `AG`, `Away Goals`, `AGoals` |

### Optional Columns (Auto-Detected)

| Data | Accepted column names |
|------|-----------------------|
| Division/League | `Div`, `Division`, `League`, `Comp`, `Competition` |
| Season | `Season` (e.g., "2023/24", "2023-24") |
| Gameweek | `GW`, `Gameweek`, `Round`, `Wk`, `Week`, `Matchday` |
| Date | `Date`, `MatchDate` (formats: dd/MM/yyyy, yyyy-MM-dd, etc.) |
| Time | `Time`, `KickOff` |
| Referee | `Referee`, `Ref` |

### Example (simple format)

```csv
GW,Date,HomeTeam,AwayTeam,FTHG,FTAG
1,12/08/2023,Arsenal,West Ham,2,1
1,12/08/2023,Chelsea,Spurs,2,2
```

### Example (football-data.co.uk format with metadata)

```csv
Div,Season,Date,HomeTeam,AwayTeam,FTHG,FTAG
E0,2023/24,11/08/2023,Arsenal,Nottm Forest,2,1
E0,2023/24,12/08/2023,Bournemouth,West Ham,1,1
```

**How Metadata Detection Works:**
1. Checks for explicit `Div` or `Season` columns in CSV
2. Extracts from filename (e.g., "E0_2023-24.csv", "premier_league_2023.csv")
3. Infers season from date range (Aug-May football season pattern)
4. Detects league from well-known team names (e.g., Arsenal → Premier League)

If no `Gameweek` column is present, gameweeks are inferred from dates (or file order) using a greedy round-robin algorithm.

## Project Structure

```
FootballDataTool/
├── src/FootballDataTool/
│   ├── Models/
│   │   ├── CsvMatchRecord.cs      # Discrete CSV representation
│   │   ├── Match.cs               # Business model
│   │   ├── SeasonMetadata.cs      # Detected league/season info
│   │   └── TeamRecord.cs          # Aggregated team stats
│   ├── Services/
│   │   ├── CsvDataService.cs              # Flexible CSV parser
│   │   ├── MetadataDetectionService.cs    # Auto-detection logic
│   │   └── MatchAnalyzer.cs               # Statistics & analysis engine
│   ├── Visualisers/
│   │   ├── HomeFormVisualiser.cs          # Home form by gameweek
│   │   ├── PointBreakdownVisualiser.cs    # Cumulative points & cross-group
│   │   └── ResultMatrixVisualiser.cs      # Results matrix
│   └── Program.cs                 # Interactive menu (Spectre.Console)
├── tests/FootballDataTool.Tests/
│   ├── MatchAnalyzerTests.cs      # Unit tests for analytics
│   └── CsvDataServiceTests.cs     # Unit tests for CSV parsing
└── data/
    ├── sample_season.csv          # 6-team, 10-gameweek sample
    ├── premier_league_2023-24.csv # Premier League sample
    └── laliga_2022-23.csv         # La Liga sample
```

## Running Tests

```bash
dotnet test FootballDataTool.slnx
```

## Dependencies

- [Spectre.Console](https://spectreconsole.net/) — rich terminal tables, bar charts, and interactive prompts
- [CsvHelper](https://joshclose.github.io/CsvHelper/) — flexible CSV parsing
