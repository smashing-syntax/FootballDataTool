# FootballDataTool

A C# .NET 8 console application that loads raw CSV football season data and visualises it as interactive tables and charts directly in your terminal.

## Features

| View | Description |
|------|-------------|
| **Home Form** | Table of each team's home results (W/D/L + score) per gameweek, colour-coded green/yellow/red |
| **Points Breakdown** | Cumulative points table by gameweek, final bar chart, and cross-group analysis (e.g. Top N vs Bottom N) |
| **Result Matrix** | N×N results grid in Home, Away, or Aggregate mode with head-to-head drill-down |
| **League Standings** | Full table (P/W/D/L/GF/GA/GD/Pts) filterable to any gameweek |

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

The tool accepts flexible CSV formats. Any of the following column naming conventions are supported:

| Data | Accepted column names |
|------|-----------------------|
| Gameweek | `GW`, `Gameweek`, `Round`, `Wk`, `Week`, `Matchday` |
| Home Team | `HomeTeam`, `Home Team`, `Home`, `HTeam` |
| Away Team | `AwayTeam`, `Away Team`, `Away`, `ATeam` |
| Home Goals | `FTHG`, `HomeGoals`, `HG`, `Home Goals`, `HGoals` |
| Away Goals | `FTAG`, `AwayGoals`, `AG`, `Away Goals`, `AGoals` |
| Date (optional) | `Date`, `MatchDate` |

### Example (simple format)

```csv
GW,HomeTeam,AwayTeam,HomeGoals,AwayGoals
1,Arsenal,Chelsea,2,1
1,Liverpool,Man City,1,0
```

### Example (football-data.co.uk format)

```csv
Div,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR
E0,12/08/2023,Arsenal,Chelsea,2,1,H
E0,12/08/2023,Liverpool,Man City,1,0,H
```

If no `Gameweek` column is present, gameweeks are inferred from dates (or file order) using a greedy round-robin algorithm.

## Project Structure

```
FootballDataTool/
├── src/FootballDataTool/
│   ├── Models/
│   │   ├── Match.cs           # Single match data
│   │   └── TeamRecord.cs      # Aggregated team stats
│   ├── Services/
│   │   ├── CsvDataService.cs  # Flexible CSV parser
│   │   └── MatchAnalyzer.cs   # Statistics & analysis engine
│   ├── Visualisers/
│   │   ├── HomeFormVisualiser.cs        # Home form by gameweek
│   │   ├── PointBreakdownVisualiser.cs  # Cumulative points & cross-group
│   │   └── ResultMatrixVisualiser.cs    # Results matrix
│   └── Program.cs             # Interactive menu (Spectre.Console)
├── tests/FootballDataTool.Tests/
│   ├── MatchAnalyzerTests.cs  # Unit tests for analytics
│   └── CsvDataServiceTests.cs # Unit tests for CSV parsing
└── data/
    └── sample_season.csv      # 6-team, 10-gameweek sample data
```

## Running Tests

```bash
dotnet test FootballDataTool.slnx
```

## Dependencies

- [Spectre.Console](https://spectreconsole.net/) — rich terminal tables, bar charts, and interactive prompts
- [CsvHelper](https://joshclose.github.io/CsvHelper/) — flexible CSV parsing
