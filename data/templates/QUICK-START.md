# Quick Reference: Creating Your Dataset

## 📋 Three Simple Files

To create a complete season dataset, you need just **3 CSV files**:

1. **`matches.csv`** - Match results and events
2. **`squads.csv`** - Player biographies (birthdays, nationalities)
3. **`transfers.csv`** - Transfer movements (optional)

---

## 🚀 Quick Start (3 Steps)

### Step 1: Copy Templates (30 seconds)

```bash
cp data/templates/matches.csv data/premier-league/2014-15_matches.csv
cp data/templates/squads.csv data/premier-league/2014-15_squads.csv
cp data/templates/transfers.csv data/premier-league/2014-15_transfers.csv
```

### Step 2: Fill In Data (1-10 hours depending on detail)

Open in Excel/Google Sheets and start with the basics:

**matches.csv** - Minimum 4 columns:
```csv
HomeTeam,AwayTeam,FTHG,FTAG
Man United,Swansea,1,2
Arsenal,Crystal Palace,2,1
```

**squads.csv** - Player birthdays (enter once!):
```csv
Team,Season,PlayerName,DateOfBirth,Position
Arsenal,2014/15,Mesut Ozil,1988-10-15,Attacking Midfielder
Arsenal,2014/15,Aaron Ramsey,1990-12-26,Central Midfielder
```

### Step 3: Load & Analyze

```csharp
var data = csvService.LoadSeasonDataFromFile("2014-15_matches.csv");
data.LoadSquadDataFromCsv("2014-15_squads.csv");  // Ages auto-calculated!
data.LoadTransferDataFromCsv("2014-15_transfers.csv");
```

**Done!** ✅

---

## 📂 Template Locations

All blank templates are in `data/templates/`:
- **`matches.csv`** - Match data template
- **`squads.csv`** - Squad data template  
- **`transfers.csv`** - Transfer data template

**Examples** are in `data/examples/`:
- `match_examples_basic.csv`
- `match_examples_full.csv`
- `squad_examples.csv`
- `transfer_examples.csv`

---

## 📊 Progressive Enhancement

| Stage | Fields | Time | What You Get |
|-------|--------|------|--------------|
| **1. Basic** | 4-10 | 1 hour | League table, results |
| **2. Goals** | +2 | 2 hours | Goal charts, top scorers |
| **3. Assists** | +0 | 2 hours | Playmaker rankings |
| **4. Lineups** | +4 | 5 hours | Squad age analysis |
| **5. Full** | +15 | 10+ hours | Complete analytics |
| **6. Squads** | CSV | 1 hour | Auto age calculation! 🆕 |
| **7. Transfers** | CSV | 2 hours | Transfer analysis |

---

## 👥 Squad Data (NEW - Enter Birthdays Once!) 🆕

Instead of entering ages in every match, create ONE squad file:

```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality
Arsenal,2014/15,Mesut Ozil,1988-10-15,Attacking Midfielder,11,Germany
Arsenal,2014/15,Aaron Ramsey,1990-12-26,Central Midfielder,16,Wales
Chelsea,2014/15,Eden Hazard,1991-01-07,Winger,10,Belgium
```

**Then match lineups become simple:**
```csv
HomeLineup,"Szczesny; Gibbs; Mertesacker; Ramsey; Ozil"
```

**Ages calculated automatically!** 🎂

**Templates**:
- `data/templates/squads_template.csv` (70+ player examples)
- `data/templates/squads_template_blank.csv` (blank)
- `data/templates/SQUAD-DATA-GUIDE.md` (quick guide)

**Benefits**:
- ✅ Enter birthday ONCE (not 38 times!)
- ✅ Automatic age calculation per match
- ✅ Mid-season birthdays handled
- ✅ 97% less repetition!

---

## 🔄 Transfer Data (CSV Format!)

### Quick Add Transfers
```csv
Team,Season,PlayerName,FromClub,ToClub,TransferDate,Window,Type,Fee,FeeCurrency
Man United,2014/15,Ander Herrera,Athletic Bilbao,Man United,2014-06-26,Summer,Permanent,29000000,GBP
Man United,2014/15,Angel Di Maria,Real Madrid,Man United,2014-08-26,Summer,Permanent,59700000,GBP
Man United,2014/15,Radamel Falcao,Monaco,Man United,2014-09-01,Summer,Loan,6000000,GBP
```

**Templates**: 
- `data/templates/transfers_template.csv` (with examples)
- `data/templates/transfers_template_blank.csv` (blank)
- `data/templates/TRANSFER-CSV-GUIDE.md` (full guide)

**Benefits**:
- ✅ Much easier than JSON!
- ✅ Multiple teams in one file
- ✅ Auto-calculates ages
- ✅ Excel/Google Sheets friendly

---

## 🌐 Data Sources Quick Links

- **Basic Results**: [football-data.co.uk](https://www.football-data.co.uk/englandm.php)
- **Lineups/Ages**: [FBref.com](https://fbref.com/en/comps/9/2014-2015/2014-2015-Premier-League-Stats)
- **Assists**: FBref goal descriptions
- **Injuries**: [PhysioRoom.com](https://www.physioroom.com)
- **Minutes**: FBref player stats pages

---

## 💡 Top Tips

1. **Start Simple**: 4 fields → Test → Add more
2. **Copy Templates**: Use `data/templates/` as starting point
3. **Consistent Names**: "Man United" everywhere (not "Man Utd")
4. **Test Often**: Load after every 10 matches
5. **Semicolons**: Use `;` to separate multiple entries
6. **Quote Commas**: `"Player (25, GK)"` needs quotes

---

## 🆘 Common Issues

**"Team not recognized"**
→ Check spelling consistency

**"Date parsing failed"**
→ Use dd/MM/yyyy format (16/08/2014)

**"Assists not showing"**
→ Format: `"Player 45' (assist: Helper)"`

**"Ages not parsed"**
→ Format: `"Player (25)"` or `"Player [25]"`

---

## 📁 File Paths

```
data/
├── templates/
│   └── TEMPLATE-GUIDE.md          ← Full field reference
├── premier-league/
│   └── premier_league_2014-15_basic.csv    ← Your dataset here
└── README.md                       ← Data folder guide
```

---

## 🚀 Example: First 10 Matches

**Time**: 30 minutes  
**Source**: football-data.co.uk  
**Fields**: Div, Season, GW, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, Referee

**Result**: Working league table!

Then enrich with:
- Week 2: Add managers, formations
- Week 3: Add goals with assists
- Week 4: Add lineups with ages
- Week 5: Complete with injuries, minutes

---

**For Full Guide**: See `data/templates/TEMPLATE-GUIDE.md`  
**For Examples**: See `data/templates/match_template_*.csv`
