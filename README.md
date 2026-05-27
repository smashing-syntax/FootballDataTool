# FootballDataTool

A C# .NET 8 console application that loads raw CSV football season data with **automatic league and season detection** and visualises it as interactive tables and charts directly in your terminal.

## Features

- 🔍 **Automatic Season Detection** - Detects season and league from CSV data or filename
- 🌍 **Multi-League Support** - Works with Premier League, La Liga, Serie A, Bundesliga, and more
- 📊 **Rich Visualizations**:

| View | Description |
|------|-------------|
| **Home Form** | Table of each team's home results (W/D/L + score) per gameweek, colour-coded green/yellow/red |
| **Points Breakdown** | Cumulative points table by gameweek, final bar chart, and cross-group analysis (e.g. Top N vs Bottom N) |
| **Result Matrix** | N×N results grid in Home, Away, or Aggregate mode with head-to-head drill-down |
| **League Standings** | Full table (P/W/D/L/GF/GA/GD/Pts) filterable to any gameweek |

- 🔬 **Extended Match Data** (optional):
  - Player lineups with ages, positions, and shirt numbers
  - Goalscorers and assists
  - Substitutions and cards (yellow/red)
  - Team managers with experience and nationalities
  - Stadium information and attendance
  - Match officials (referee, assistants, VAR)
  - Weather conditions

- 👥 **Player Profiling**:
  - Injury tracking with start/end dates and severity levels
  - Minutes played tracking per match
  - Nationality and previous club history
  - Birthday tracking with zodiac signs (Western & Chinese) 🎂

- 💰 **Transfer Analysis** (JSON data):
  - Transfer fees and contract details
  - Player ages at transfer
  - Net spend and financial analytics

- 🏗️ **Dual-View Architecture**:
  - **Match-Centric**: Simple list of matches for basic analysis (CSV-friendly)
  - **Team-Centric**: Rich team objects with aggregated squad, stats, and history
  - Seamlessly switch between views as needed
  - See [Season Aggregation Guide](docs/SEASON-AGGREGATION-GUIDE.md)


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

### Extended Data Columns (All Optional)

**Player & Team Data:**
- `HomeLineup`, `AwayLineup` - Starting XIs with optional ages/positions
- `HomeSubstitutes`, `AwaySubstitutes` - Bench players
- `HomeManager`, `AwayManager` - Team managers
- `HomeFormation`, `AwayFormation` - Tactical formations (e.g., "4-3-3")

**Match Events:**
- `HomeGoalscorers`, `AwayGoalscorers` - Goals with times and assisters
- `HomeSubstitutions`, `AwaySubstitutions` - Substitution events
- `HomeYellowCards`, `AwayYellowCards`, `HomeRedCards`, `AwayRedCards` - Disciplinary cards

**Injuries & Minutes:**
- `HomeInjuries`, `AwayInjuries` - Injured players with dates
- `HomeMinutesPlayed`, `AwayMinutesPlayed` - Playing time per player

**Venue & Context:**
- `Stadium`, `Attendance`, `StadiumCapacity`
- `Temperature`, `WeatherConditions`
- `AssistantReferee1`, `AssistantReferee2`, `FourthOfficial`, `VarReferee`

> See [CSV-FORMAT-GUIDE.md](docs/CSV-FORMAT-GUIDE.md) for complete column reference and [EXTENDED-DATA-GUIDE.md](docs/EXTENDED-DATA-GUIDE.md) for format specifications.

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
│   │   ├── CsvMatchRecord.cs          # Discrete CSV representation
│   │   ├── Match.cs                   # Business model
│   │   ├── MatchEvents.cs             # Extended match data
│   │   ├── Player.cs                  # Player with age, nationality, zodiac
│   │   ├── Manager.cs                 # Manager profile
│   │   ├── Transfer.cs                # Transfer records
│   │   ├── Injury.cs                  # Injury tracking & player appearances
│   │   ├── Stadium.cs                 # Venue information
│   │   ├── TeamSeasonInfo.cs          # Team season data (transfers, managers)
│   │   ├── SeasonMetadata.cs          # Detected league/season info
│   │   └── TeamRecord.cs              # Aggregated team stats
│   ├── Services/
│   │   ├── CsvDataService.cs              # Flexible CSV parser
│   │   ├── MetadataDetectionService.cs    # Auto-detection logic
│   │   ├── ExtendedDataParser.cs          # Parse lineups, events, injuries
│   │   ├── TeamDataLoader.cs              # Load transfer JSON data
│   │   └── MatchAnalyzer.cs               # Statistics & analysis engine
│   ├── Visualisers/
│   │   ├── HomeFormVisualiser.cs          # Home form by gameweek
│   │   ├── PointBreakdownVisualiser.cs    # Cumulative points & cross-group
│   │   ├── ResultMatrixVisualiser.cs      # Results matrix
│   │   ├── AgeProfileVisualiser.cs        # Age-based analytics
│   │   └── TransferAnalysisVisualiser.cs  # Transfer spending analysis
│   └── Program.cs                         # Interactive menu (Spectre.Console)
├── tests/FootballDataTool.Tests/
│   ├── MatchAnalyzerTests.cs      # Unit tests for analytics
│   └── CsvDataServiceTests.cs     # Unit tests for CSV parsing
├── data/
│   ├── sample_season.csv                      # 6-team, 10-gameweek sample
│   ├── premier_league_2023-24.csv             # Premier League basic
│   ├── premier_league_2023-24_with_ages.csv   # With player ages in lineups
│   ├── premier_league_2023-24_full_sample.csv # With injuries & minutes
│   ├── laliga_2022-23.csv                     # La Liga sample
│   ├── arsenal_2023-24_transfers.json         # Arsenal transfer window
│   └── chelsea_2023-24_transfers.json         # Chelsea transfer window
└── docs/
    ├── ARCHITECTURE.md                    # Discrete model explanation
    ├── CSV-FORMAT-GUIDE.md                # Complete CSV column reference
    ├── EXTENDED-DATA-GUIDE.md             # Extended fields format guide
    ├── AGE-FEATURE-SUMMARY.md             # Player age tracking
    ├── TRANSFER-MANAGER-AGE-GUIDE.md      # Transfer & manager features
    ├── INJURY-PLAYER-TRACKING-GUIDE.md    # Injury & player profiling 🆕
    └── DEVELOPER-GUIDE.md                 # Development guidelines
```

## Running Tests

```bash
dotnet test FootballDataTool.slnx
```

## Dependencies

- [Spectre.Console](https://spectreconsole.net/) — rich terminal tables, bar charts, and interactive prompts
- [CsvHelper](https://joshclose.github.io/CsvHelper/) — flexible CSV parsing
