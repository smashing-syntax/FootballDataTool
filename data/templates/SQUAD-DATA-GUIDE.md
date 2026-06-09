# Squad Data Quick Reference

## 🎯 What Is It?

Squad CSV files contain player biographical data (birthdays, nationalities, etc.) entered **once per player** instead of repeating in every match.

---

## 📋 Format

### For League/Transfer Squads:
```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,PreviousClub,PreviousLeague,Height,Weight,PreferredFoot
Arsenal,2014/15,Mesut Ozil,1988-10-15,AM,11,Germany,Real Madrid,La Liga,180,76,Left
Chelsea,2014/15,Eden Hazard,1991-01-07,LW,10,Belgium,Lille,Ligue 1,173,74,Right
```

### For Tournament Squads:
```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,CurrentClub,ClubLeague,Height,Weight,PreferredFoot
Argentina,2022,Lionel Messi,1987-06-24,FW,10,Argentina,Paris Saint-Germain,Ligue 1,170,72,Left
France,2022,Kylian Mbappé,1998-12-20,LW,10,France,Paris Saint-Germain,Ligue 1,178,73,Right
```

**Note:** 
- `PreviousClub`/`PreviousLeague` and `CurrentClub`/`ClubLeague` are **aliases** - they map to the same field! Use whichever makes more sense for your data.
- Physical attributes (`Height`, `Weight`, `PreferredFoot`) are optional but enable fun analytics like height brackets and BMI by position!
- BMI is automatically calculated if both height and weight are provided.

---

## 💻 Usage

```csharp
// 1. Load matches
var seasonData = csvService.LoadSeasonDataFromFile("2014-15_matches.csv");

// 2. Load squad data (auto-enriches ALL matches!)
seasonData.LoadSquadDataFromCsv("2014-15_squads.csv");

// 3. Done! Ages calculated automatically
```

---

## ✨ Benefits

| Before | After |
|--------|-------|
| Enter age 38 times per player | Enter birthday once |
| Manual mid-season age updates | Automatic |
| Ages embedded in match CSV | Separated, cleaner |
| Error-prone repetition | Single source of truth |

---

## 📁 Templates

- `data/templates/squads_template.csv` - Examples (70+ players)
- `data/templates/squads_template_blank.csv` - Blank template

---

## 🎂 Auto-Calculations

- ✅ Age per match (based on match date + birthday)
- ✅ Mid-season birthdays tracked automatically
- ✅ Zodiac signs calculated
- ✅ Chinese zodiac calculated

---

## 🔄 Match CSV Simplified

**Before:**
```csv
HomeLineup,"1. Szczesny (24, GK); 3. Gibbs (24, LB)"
```

**After:**
```csv
HomeLineup,"Szczesny; Gibbs; Mertesacker"
```

Ages added automatically! 🎉

---

**See:** `docs/SQUAD-DATA-SUMMARY.md` for full details
