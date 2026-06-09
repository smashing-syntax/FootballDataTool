# Squad Column Aliases - Implementation Summary

## ✨ What Was Added

Column aliases have been added to the squad CSV loader to make naming clearer for different use cases:

### Club/League Context (Transfer History)
```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,PreviousClub,PreviousLeague
```
- **`PreviousClub`** - The club a player came from (transfer context)
- **`PreviousLeague`** - The league that club plays in

### Tournament Context (International/Current Club)
```csv
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,CurrentClub,ClubLeague
```
- **`CurrentClub`** - The player's club team (tournament context)
- **`ClubLeague`** - The league that club plays in

## 🔄 How It Works

The column names are **aliases** - they all map to the same underlying field in the `Player` model (`PreviousClub` and `PreviousLeague`).

### Accepted Column Names

For club data:
- `PreviousClub`, `CurrentClub`, `ClubTeam`, or `Club`

For league data:
- `PreviousLeague`, `CurrentLeague`, `ClubLeague`, or `League`

## 💻 Implementation

### SquadCsvRecord.cs
Uses CsvHelper's `[Name]` attribute to define multiple acceptable column names:

```csharp
[Name("PreviousClub", "CurrentClub", "ClubTeam", "Club")]
public string PreviousClub { get; set; } = string.Empty;

[Name("PreviousLeague", "CurrentLeague", "ClubLeague", "League")]
public string PreviousLeague { get; set; } = string.Empty;
```

### Backward Compatibility

✅ **All existing CSVs still work** - no breaking changes  
✅ **Old column names** (`PreviousClub`) still supported  
✅ **New column names** (`CurrentClub`) now also work  
✅ **User can choose** whichever makes more sense for their data  

## 📁 Files Modified

### Code Changes:
- ✅ `src/FootballDataTool/Services/SquadCsvLoader.cs` - Added `[Name]` attributes for aliases

### New Templates:
- ✅ `data/templates/squads_tournament.csv` - Tournament-focused blank template

### Updated Examples:
- ✅ `data/examples/squad_examples_worldcup.csv` - Now uses `CurrentClub`/`ClubLeague`

### Updated Documentation:
- ✅ `data/templates/SQUAD-DATA-GUIDE.md` - Shows both naming conventions
- ✅ `data/templates/TOURNAMENT-GUIDE.md` - Mentions column aliases
- ✅ `data/templates/README.md` - Lists both templates
- ✅ `data/examples/SQUAD-WORLDCUP-README.md` - Explains alias options

## 🎯 Use Cases

### League Squads
```csv
Arsenal,2014/15,Mesut Ozil,1988-10-15,AM,11,Germany,Real Madrid,La Liga
```
↑ Using `PreviousClub` makes sense (he transferred from Real Madrid)

### World Cup Squads
```csv
Argentina,2022,Lionel Messi,1987-06-24,FW,10,Argentina,Paris Saint-Germain,Ligue 1
```
↑ Using `CurrentClub` makes more sense (his club at time of tournament)

### Champions League Squads
```csv
Bayern Munich,2023-24,Harry Kane,1993-07-28,ST,9,England,Tottenham,Premier League
```
↑ Could use either - `PreviousClub` for transfer history or `CurrentClub` for current context

## ✅ Benefits

1. **Clarity** - Users can choose naming that makes sense for their data
2. **Flexibility** - Same tool handles leagues and tournaments naturally
3. **No Breaking Changes** - All existing CSVs continue to work
4. **Intuitive** - Tournament CSVs can use "current" language instead of "previous"

## 🧪 Testing

✅ Build successful  
✅ Backward compatible with existing CSVs  
✅ World Cup example updated and validated  
✅ Documentation updated across all guides  

---

**Result:** Squad data now has clearer, context-appropriate column naming while maintaining full backward compatibility! 🎉
