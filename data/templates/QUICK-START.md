# Quick Reference: Creating a 2014/15 Dataset

## 📋 Cheat Sheet

### Minimum Required (Works Immediately)
```csv
HomeTeam,AwayTeam,FTHG,FTAG
Man United,Swansea,1,2
Leicester,Everton,2,2
Arsenal,Crystal Palace,2,1
```

### Recommended Basic
```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
E0,2014/15,1,16/08/2014,Man United,Swansea,1,2,A,Mike Dean
E0,2014/15,1,16/08/2014,Arsenal,Crystal Palace,2,1,H,Martin Atkinson
```

---

## 🎯 Quick Start Steps

### 1. Download Basic Data (5 minutes)
```bash
# Go to: https://www.football-data.co.uk/englandm.php
# Download: 2014-15 > E0.csv
# Save as: data/premier-league/premier_league_2014-15_basic.csv
```

### 2. Test It
```bash
dotnet run --project src/FootballDataTool -- data/premier-league/premier_league_2014-15_basic.csv
```

### 3. Enrich (Optional)
Copy `data/templates/match_template_extended.csv` and follow the guide!

---

## 🔑 Field Formats (Quick Reference)

### Goals with Assists
```
"Player1 45'; Player2 67' (assist: Helper)"
"Rooney 45' (pen); Van Persie 67' (assist: Mata)"
```

### Lineups with Ages
```
"1. De Gea (23, GK); 2. Rafael (24, RB); 5. Ferdinand (35, CB)"
```

### Injuries
```
"Player (Type, dd/MM/yyyy - dd/MM/yyyy)"
"Rooney (Groin, 01/08/2014 - 20/08/2014); RVP (Knee, 10/08/2014 - )"
```

### Minutes
```
"De Gea 90'; Rooney 90'; Mata 78'; Kagawa (12')"
```

---

## 📊 Progressive Enhancement

| Stage | Fields | Time | What You Get |
|-------|--------|------|--------------|
| **1. Basic** | 4-10 | 1 hour | League table, results |
| **2. Goals** | +2 | 2 hours | Goal charts, top scorers |
| **3. Assists** | +0 | 2 hours | Playmaker rankings |
| **4. Lineups** | +4 | 5 hours | Squad age analysis |
| **5. Full** | +15 | 10+ hours | Complete analytics |
| **6. Transfers** | CSV | 2 hours | Transfer analysis 🆕 |

---

## 🔄 Transfer Data (NEW - CSV Format!)

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
