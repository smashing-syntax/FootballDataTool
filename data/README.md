# Data Folder Organization

This folder contains all match data, transfer data, and templates for FootballDataTool.

## Folder Structure

```
data/
├── templates/           # Template CSV files for creating your own datasets
│   ├── match_template_basic.csv       # Minimum required fields
│   ├── match_template_extended.csv    # With managers, formations, goals
│   ├── match_template_full.csv        # Complete example with all fields
│   ├── match_template_blank.csv       # Empty template (all columns)
│   ├── transfers_template.csv         # Transfer data template with examples 🆕
│   ├── transfers_template_blank.csv   # Blank transfer template 🆕
│   ├── TEMPLATE-GUIDE.md              # Comprehensive field reference
│   └── TRANSFER-CSV-GUIDE.md          # Transfer CSV format guide 🆕
│
├── premier-league/      # Premier League match and transfer data
│   ├── premier_league_2023-24_basic.csv        # Basic match results
│   ├── premier_league_2023-24_with_ages.csv    # With player ages in lineups
│   ├── premier_league_2023-24_extended.csv     # With extended match data
│   ├── premier_league_2023-24_full_sample.csv  # Full example (injuries, minutes)
│   ├── arsenal_2023-24_transfers.json          # Arsenal transfer window (JSON)
│   ├── chelsea_2023-24_transfers.json          # Chelsea transfer window (JSON)
│   └── 2023-24_transfers.csv                   # All PL transfers (CSV) 🆕
│
├── la-liga/             # La Liga match data
│   └── laliga_2022-23.csv
│
└── archive/             # Sample/test files
    └── sample_season.csv
```

---

## Quick Start

### 1. Use Existing Data

Load any existing dataset:
```csharp
var seasonData = csvService.LoadSeasonDataFromFile("data/premier-league/premier_league_2023-24_basic.csv");
```

### 2. Create Your Own Dataset

**Option A: Start from Blank Template**
1. Copy `templates/match_template_blank.csv`
2. Rename to your season (e.g., `premier_league_2014-15.csv`)
3. Fill in the fields (see `templates/TEMPLATE-GUIDE.md`)
4. Save in appropriate league folder

**Option B: Start from Example**
1. Copy `templates/match_template_full.csv`
2. Replace the example data with your matches
3. Remove columns you don't have data for

**Option C: Quick Basic Dataset**
1. Use `templates/match_template_basic.csv` as reference
2. Only fill in required fields: `HomeTeam`, `AwayTeam`, `FTHG`, `FTAG`
3. Add more fields as you gather data

---

## Naming Conventions

### Match Data Files
- **Format**: `{league}_{season}_{variant}.csv`
- **Examples**:
  - `premier_league_2014-15_basic.csv`
  - `premier_league_2014-15_full.csv`
  - `laliga_2022-23.csv`
  - `bundesliga_2023-24_with_ages.csv`

### Transfer Data Files
- **CSV Format** (recommended for manual entry): `{season}_transfers.csv` or `{league}_{season}_transfers.csv` 🆕
- **JSON Format** (per team): `{team}_{season}_transfers.json`
- **Examples**:
  - `2014-15_transfers.csv` (all teams in one file - easier!)
  - `arsenal_2014-15_transfers.json`
  - `chelsea_2023-24_transfers.json`
  - `premier_league_2023-24_transfers.csv`

**Why CSV?** Much easier to fill out in Excel/Google Sheets! Multiple teams in one file!

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
