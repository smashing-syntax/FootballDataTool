# Squad Data Quick Reference

## 🎯 What Is It?

Squad CSV files contain player biographical data (birthdays, nationalities, etc.) entered **once per player** instead of repeating in every match.

---

## 📋 Format

```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,PreviousClub,Height,PreferredFoot,JoinDate
Arsenal,2014/15,Mesut Ozil,1988-10-15,Attacking Midfielder,11,Germany,Real Madrid,180,Left,2013-09-02
Chelsea,2014/15,Eden Hazard,1991-01-07,Winger,10,Belgium,Lille,173,Both,2012-06-04
```

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
