# Data Folder Organization

This folder contains all match data, transfer data, and templates for FootballDataTool.

## Folder Structure

```
data/
├── templates/           # Blank templates for creating datasets 📝
│   ├── matches.csv                  # Match data template (all columns)
│   ├── squads.csv                   # Squad biographical data template
│   ├── transfers.csv                # Transfer data template
│   ├── README.md                    # Template folder guide 🆕
│   ├── TEMPLATE-GUIDE.md            # Complete field reference (matches)
│   ├── SQUAD-DATA-GUIDE.md          # Squad data quick reference
│   ├── TRANSFER-CSV-GUIDE.md        # Transfer CSV complete guide
│   └── QUICK-START.md               # Quick reference cheat sheet
│
├── examples/            # Example data for reference 📚
│   ├── match_examples_basic.csv     # Basic match results (10 matches)
│   ├── match_examples_extended.csv  # With managers and goals
│   ├── match_examples_full.csv      # Complete with all fields
│   ├── squad_examples.csv           # Arsenal/Chelsea/Man Utd squads
│   └── transfer_examples.csv        # 2023/24 transfer examples
│
├── premier-league/      # Premier League data
│   ├── 2023-24_matches.csv
│   ├── 2023-24_squads.csv
│   ├── 2023-24_transfers.csv
│   ├── arsenal_2023-24_transfers.json  (legacy JSON format still supported)
│   └── chelsea_2023-24_transfers.json
│
├── la-liga/             # La Liga data
│   └── laliga_2022-23.csv
│
└── archive/             # Deprecated/test files
    └── sample_season.csv
```

---

## Quick Start

### 1. Copy Blank Templates

```bash
# For 2014/15 Premier League season
cp data/templates/matches.csv data/premier-league/2014-15_matches.csv
cp data/templates/squads.csv data/premier-league/2014-15_squads.csv
cp data/templates/transfers.csv data/premier-league/2014-15_transfers.csv
```

### 2. Fill In Your Data

Open in Excel/Google Sheets:
1. **`matches.csv`** - Match results and events
2. **`squads.csv`** - Player birthdays and biographies (enter once!)
3. **`transfers.csv`** - Transfer movements (optional)

### 3. Load in Code

```csharp
// Load match data
var seasonData = csvService.LoadSeasonDataFromFile("data/premier-league/2014-15_matches.csv");

// Load squad data (auto-calculates ages!)
seasonData.LoadSquadDataFromCsv("data/premier-league/2014-15_squads.csv");

// Load transfers (optional)
seasonData.LoadTransferDataFromCsv("data/premier-league/2014-15_transfers.csv");
```

**📖 See:** `data/templates/README.md` for complete template guide

---

## 📊 Progressive Enhancement

Start simple, add detail over time:

| Stage | Files | What to Add | Time |
|-------|-------|-------------|------|
| **1. Basic** | matches.csv | 4 columns (teams, goals) | 1 hour |
| **2. Squads** | +squads.csv | Player birthdays | 1 hour |
| **3. Goals** | matches.csv | Goalscorers | 2 hours |
| **4. Full** | matches.csv | All 33 columns | 10+ hours |

You decide how much detail you want!

---

## File Naming Conventions

### Match Data
```
{league}_{season}_matches.csv
Examples:
  - premier_league_2014-15_matches.csv
  - la_liga_2022-23_matches.csv
```

### Squad Data 🆕
```
{league}_{season}_squads.csv  OR  {season}_squads.csv
Examples:
  - premier_league_2014-15_squads.csv
  - 2014-15_squads.csv (if in league-specific folder)
```

### Transfer Data
```
{league}_{season}_transfers.csv  OR  {season}_transfers.csv
Examples:
  - 2014-15_transfers.csv (all teams in one file - easier!)
  - premier_league_2023-24_transfers.csv
```

**Legacy JSON format** (per team) still supported:
```
{team}_{season}_transfers.json
Examples:
  - arsenal_2014-15_transfers.json
```

---

## Data Variants

Different levels of data richness:

| Variant | Description | Fields | Example |
|---------|-------------|--------|---------|
| `_basic` | Minimum match results | 4-10 | `premier_league_2023-24_basic.csv` |
| `_with_ages` | Basic + player ages | 10-15 | `premier_league_2023-24_with_ages.csv` |
| `_extended` | + managers, formations, goals | 15-20 | `premier_league_2023-24_extended.csv` |
| `_full` | Everything (injuries, minutes) | 25-35+ | `premier_league_2023-24_full_sample.csv` |

**You decide how much detail you want!** The tool works with any variant.

---

## Creating a 2014/15 Premier League Dataset

### Recommended Approach

**Step 1: Get Basic Data (5 minutes)**
- Download from [football-data.co.uk](https://www.football-data.co.uk/englandm.php)
- File: `E0.csv` for 2014/15 season
- Rename to `premier_league_2014-15_basic.csv`
- Move to `data/premier-league/`

**Step 2: Test It**
```csharp
var seasonData = csvService.LoadSeasonDataFromFile("data/premier-league/premier_league_2014-15_basic.csv");
Console.WriteLine($"Loaded {seasonData.TotalMatches} matches");
```

**Step 3: Enrich (Optional)**
- Copy to `premier_league_2014-15_extended.csv`
- Add fields incrementally (managers, goals with assists, lineups)
- Use `templates/TEMPLATE-GUIDE.md` as reference

---

## Data Sources

### Recommended Sources

**Match Results:**
- ✅ [football-data.co.uk](https://www.football-data.co.uk) - FREE CSV downloads
- ✅ [FBref.com](https://fbref.com) - Detailed stats, lineups, assists
- ✅ Premier League official site

**Player Ages:**
- ✅ [Transfermarkt](https://www.transfermarkt.com) - Player birthdates
- ✅ [FBref.com](https://fbref.com) - Ages in lineups

**Assists:**
- ✅ [FBref.com](https://fbref.com) - Goal descriptions with assists
- ✅ WhoScored.com
- ✅ ESPN match reports

**Injuries:**
- ✅ [PhysioRoom.com](https://www.physioroom.com) - Injury database
- ✅ Official club sites
- ✅ Premier Injuries

**Minutes Played:**
- ✅ [FBref.com](https://fbref.com) - Player stats pages
- ✅ WhoScored.com
- ✅ Premier League official stats

---

## File Format Tips

### ✅ DO:
- Keep team names **consistent** across files
- Use **semicolons (;)** to separate multiple entries
- Save as **UTF-8 CSV**
- Include **column headers** in first row
- Quote fields containing commas: `"Player (25, GK)"`

### ❌ DON'T:
- Mix spellings (e.g., "Man United" vs "Man Utd")
- Use Excel's auto-formatting (dates become numbers)
- Include blank lines
- Use different date formats in same file

---

## Progressive Data Collection

### Week 1: Foundation (Quick Start)
- Download basic match results from football-data.co.uk
- Load and verify the data works
- Generate league table

### Week 2: Managers & Formations
- Add manager names for each team
- Add formations (if available)
- Add goalscorers

### Week 3: Player Ages
- Add ages to lineups
- Calculate squad age analytics

### Week 4: Assists & Minutes
- Add assists to goals
- Add minutes played data
- Calculate player contribution stats

### Week 5: Injuries & Polish
- Add injury data
- Add weather, officials
- Complete the dataset

**Start simple, enrich over time!**

---

## Validation

Test your data after adding it:

```csharp
var seasonData = csvService.LoadSeasonDataFromFile("your_file.csv");

// Check what data you have
Console.WriteLine($"Matches: {seasonData.TotalMatches}");
Console.WriteLine($"Teams: {seasonData.TotalTeams}");
Console.WriteLine($"Has lineups: {seasonData.HasLineupData}");
Console.WriteLine($"Has injuries: {seasonData.HasInjuryData}");
Console.WriteLine($"Has assists: {seasonData.Matches.Count(m => m.ExtendedData?.Goals.Any(g => g.Assister != null))}");

// Verify metadata detection
Console.WriteLine($"Detected: {seasonData.Metadata.League} {seasonData.Metadata.Season}");
```

---

## Example: Creating 2014/15 Full Season

1. **Download basic data** → `premier_league_2014-15_basic.csv`
2. **Copy template** → `premier_league_2014-15_extended.csv`
3. **Add 10 matches with full data** (test richness)
4. **Gradually complete all 380 matches**
5. **Save milestone versions** (`_v1.csv`, `_v2.csv`, etc.)

---

## Need Help?

- **Field reference**: See `templates/TEMPLATE-GUIDE.md`
- **Format specs**: See `docs/CSV-FORMAT-GUIDE.md`
- **Extended data**: See `docs/EXTENDED-DATA-GUIDE.md`
- **Examples**: Look at files in `premier-league/`

---

**Last Updated**: January 2025  
**FootballDataTool**: v4.0
