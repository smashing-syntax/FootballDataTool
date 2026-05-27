# Templates Folder

This folder contains **blank templates** for creating your football season datasets.

## 📋 Quick Start

### For a Complete Season Dataset, You Need:

1. **`matches.csv`** - Match results and events
2. **`squads.csv`** - Player biographical data (birthdays, nationalities)
3. **`transfers.csv`** - Transfer movements (optional)

---

## 📄 Template Files

### 1. `matches.csv` - Match Data Template

**Purpose:** Record all match results and events for the season.

**Minimum Required Columns:**
- `HomeTeam`, `AwayTeam`, `FTHG`, `FTAG`

**Recommended Columns:**
- `Div`, `Season`, `GW`, `Date`, `FTR`, `Referee`

**All 33 Columns Available:**
Full match data including lineups, goalscorers, substitutions, cards, injuries, minutes played, etc.

**📖 See:** `TEMPLATE-GUIDE.md` for complete field reference

---

### 2. `squads.csv` - Squad Biographical Data Template 🆕

**Purpose:** Player biographies entered **once per player** (not repeated in every match).

**Benefits:**
- ✅ Enter birthday once (not 38 times!)
- ✅ Automatic age calculation per match
- ✅ Mid-season birthdays handled automatically
- ✅ 97% less repetition

**Required Columns:**
- `Team`, `Season`, `PlayerName`, `DateOfBirth`, `Position`

**Optional Columns:**
- `ShirtNumber`, `Nationality`, `PreviousClub`, `Height`, `PreferredFoot`, `JoinDate`

**📖 See:** `SQUAD-DATA-GUIDE.md` for details

---

### 3. `transfers.csv` - Transfer Data Template

**Purpose:** Record player transfers (signings and departures).

**Benefits:**
- ✅ Multiple teams in one file
- ✅ Auto-calculates ages from birthdays
- ✅ Easier than JSON format

**Required Columns:**
- `Team`, `Season`, `PlayerName`, `FromClub`, `ToClub`, `TransferDate`, `Window`, `Type`

**Optional Columns:**
- `Fee`, `FeeCurrency`, `ContractYears`, `Notes`, etc.

**📖 See:** `TRANSFER-CSV-GUIDE.md` for details

---

## 🚀 Quick Workflow

### Step 1: Copy Templates

```bash
# For 2014/15 Premier League season
cp data/templates/matches.csv data/premier-league/2014-15_matches.csv
cp data/templates/squads.csv data/premier-league/2014-15_squads.csv
cp data/templates/transfers.csv data/premier-league/2014-15_transfers.csv
```

### Step 2: Fill In Data

Open in Excel/Google Sheets and start filling in:

1. **Matches** - Start with basic results, add details later
2. **Squads** - All players with birthdays (enter once!)
3. **Transfers** - Summer and winter transfers

### Step 3: Load in Code

```csharp
// Load matches
var seasonData = csvService.LoadSeasonDataFromFile("data/premier-league/2014-15_matches.csv");

// Load squad data (auto-enriches matches with ages!)
seasonData.LoadSquadDataFromCsv("data/premier-league/2014-15_squads.csv");

// Load transfer data (optional)
seasonData.LoadTransferDataFromCsv("data/premier-league/2014-15_transfers.csv");

// Done! All data enriched and ready to analyze
```

---

## 📊 Progressive Enhancement

You don't need all fields at once! Start simple and add more detail over time:

| Stage | What to Add | Time | Result |
|-------|-------------|------|--------|
| **1. Basic** | 4 match columns | 1 hour | League table, results |
| **2. Squads** | Player birthdays | 1 hour | Auto age calculation! |
| **3. Goals** | Goalscorers | 2 hours | Top scorer charts |
| **4. Assists** | Assists in goals | 2 hours | Playmaker stats |
| **5. Lineups** | Starting 11s | 3 hours | Squad rotation analysis |
| **6. Full** | All 33 columns | 10+ hours | Complete analytics |

---

## 📁 File Naming Conventions

### Matches
```
{league}_{season}_matches.csv
Examples:
  - premier_league_2014-15_matches.csv
  - la_liga_2022-23_matches.csv
```

### Squads
```
{league}_{season}_squads.csv
OR
{season}_squads.csv  (if league obvious from folder)
Examples:
  - premier_league_2014-15_squads.csv
  - 2014-15_squads.csv
```

### Transfers
```
{league}_{season}_transfers.csv
OR
{season}_transfers.csv
Examples:
  - premier_league_2014-15_transfers.csv
  - 2014-15_transfers.csv
```

---

## 🎓 Key Concepts

### Separation of Concerns

**Before (Old Way):**
```csv
# Age repeated in EVERY match!
HomeLineup,"1. Szczesny (24, GK); 3. Gibbs (24, LB); 4. Mertesacker (29, CB)"
```

**After (New Way):**
```csv
# matches.csv - Just names
HomeLineup,"Szczesny; Gibbs; Mertesacker"

# squads.csv - Biographical data (separate file)
Arsenal,2014/15,Wojciech Szczesny,1990-04-18,Goalkeeper,1,Poland
```

**Ages calculated automatically!** 🎂

### Single Source of Truth

- **Matches CSV:** Event data (who played, who scored, when)
- **Squads CSV:** Biographical data (birthdays, nationalities) - entered once!
- **Transfers CSV:** Movement data (who joined/left, fees)

Each file has a single responsibility and is maintained independently.

---

## 📖 Documentation

| File | Purpose |
|------|---------|
| `TEMPLATE-GUIDE.md` | Complete field reference for matches.csv (500+ lines) |
| `SQUAD-DATA-GUIDE.md` | Squad CSV quick reference |
| `TRANSFER-CSV-GUIDE.md` | Transfer CSV complete guide |
| `QUICK-START.md` | Quick reference cheat sheet |

---

## 💡 Tips

### 1. Start Simple
Begin with just 4 columns in matches.csv:
```csv
HomeTeam,AwayTeam,FTHG,FTAG
Arsenal,Crystal Palace,2,1
```

Test it works, then add more fields!

### 2. Use Squads CSV
Don't repeat player ages 38 times! Use squads.csv:
```csv
Team,Season,PlayerName,DateOfBirth,Position
Arsenal,2014/15,Mesut Ozil,1988-10-15,Attacking Midfielder
```

System calculates ages automatically per match!

### 3. Test Frequently
Load and test after every 10 matches to catch errors early.

### 4. Copy Examples
Check `data/examples/` folder for reference data.

---

## 🌐 Data Sources

### Match Results
- [Football-Data.co.uk](https://www.football-data.co.uk/englandm.php) - Basic results (free)
- [FBref.com](https://fbref.com/) - Detailed stats
- Wikipedia - Season pages

### Player Birthdays
- [Transfermarkt](https://www.transfermarkt.com/) - Comprehensive player database
- [FBref.com](https://fbref.com/) - Squad pages
- Wikipedia - Squad lists

### Transfers
- [Transfermarkt](https://www.transfermarkt.com/) - Transfer fees and dates
- [Wikipedia](https://en.wikipedia.org/) - Season transfer pages
- Club official websites

---

## ✅ Examples

Check `data/examples/` folder for:
- `match_examples_basic.csv` - Basic match data
- `match_examples_extended.csv` - With managers and goals
- `match_examples_full.csv` - Complete with all fields
- `squad_examples.csv` - Arsenal, Chelsea, Man Utd squads
- `transfer_examples.csv` - 2023/24 transfers

---

## 🆘 Common Issues

**"Ages not calculated"**
→ Make sure you called `seasonData.LoadSquadDataFromCsv()`

**"Player not found in squad data"**
→ Check spelling matches exactly between matches.csv and squads.csv

**"Date parsing failed"**
→ Use `YYYY-MM-DD` format (e.g., `2014-08-16`)

**"Transfer not showing as signing"**
→ Make sure `ToClub` matches the `Team` column

---

## 📞 Need Help?

1. Check `TEMPLATE-GUIDE.md` for field formats
2. Check `QUICK-START.md` for quick reference
3. Look at examples in `data/examples/`
4. See full guides in `docs/` folder

---

**Ready to start?** Copy the templates and begin filling them in! 🚀
