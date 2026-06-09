# Quick Start: Tournament Data

Get started tracking tournament data (World Cup, Champions League, etc.) in **5 minutes**.

## Step 1: Copy the Template

```bash
cp data/templates/matches.csv data/my-tournament/worldcup_2022.csv
```

## Step 2: Add Your Tournament Matches

Open the CSV and add matches with tournament-specific columns:

### For Group Stage Matches:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
World Cup,2022,1,GS,A,,20/11/2022,Qatar,Ecuador,0,2,A,Daniele Orsato
World Cup,2022,1,GS,A,,25/11/2022,Qatar,Senegal,1,3,A,Antonio Mateu Lahoz
World Cup,2022,1,GS,B,,21/11/2022,England,Iran,6,2,H,Raphael Claus
```

### For Knockout Rounds:

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
World Cup,2022,2,R16,,,03/12/2022,Netherlands,USA,3,1,H,Wilton Sampaio
World Cup,2022,3,QF,,,09/12/2022,Croatia,Brazil,1,1,D,Michael Oliver
World Cup,2022,4,SF,,,13/12/2022,Argentina,Croatia,3,0,H,Daniele Orsato
World Cup,2022,6,F,,,18/12/2022,Argentina,France,3,3,D,Szymon Marciniak
```

### For Two-Legged Ties (Champions League):

```csv
Div,Season,GW,Stage,Group,Leg,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
Champions League,2023-24,9,R16,,1,13/02/2024,PSG,Real Sociedad,2,0,H,Slavko Vinčić
Champions League,2023-24,10,R16,,2,06/03/2024,Real Sociedad,PSG,1,2,A,Szymon Marciniak
```

## Step 3: Load and Analyze

```csharp
using FootballDataTool.Services;
using FootballDataTool.Models;

var csvService = new CsvDataService();
var seasonData = csvService.LoadSeasonDataFromFile("data/my-tournament/worldcup_2022.csv");

// Get group stage matches
var groupStage = seasonData.Matches.Where(m => m.IsGroupStage).ToList();
Console.WriteLine($"Group stage matches: {groupStage.Count}");

// Get knockout matches
var knockout = seasonData.Matches.Where(m => m.IsKnockout).ToList();
Console.WriteLine($"Knockout matches: {knockout.Count}");

// Get matches from Group A
var groupA = seasonData.Matches.Where(m => m.Group == "A").ToList();
Console.WriteLine($"Group A: {groupA.Count} matches");

// Get the final
var final = seasonData.Matches.FirstOrDefault(m => m.Stage == TournamentStage.Final);
if (final != null)
{
    Console.WriteLine($"Final: {final.HomeTeam} {final.HomeGoals}-{final.AwayGoals} {final.AwayTeam}");
}

// Get semi-finals
var semis = seasonData.Matches.Where(m => m.Stage == TournamentStage.SemiFinal).ToList();
foreach (var semi in semis)
{
    Console.WriteLine($"Semi-final: {semi.HomeTeam} vs {semi.AwayTeam}");
}
```

## Stage Codes Reference

| Code | Stage |
|------|-------|
| `GS` | Group Stage |
| `R32` | Round of 32 |
| `R16` | Round of 16 |
| `QF` | Quarter-final |
| `SF` | Semi-final |
| `3P` | 3rd Place Playoff |
| `F` | Final |

## What's Next?

- **Add lineups, goals, and assists** — Tournament data supports all the same rich features as league data
- **Mix with squad data** — Add `squads.csv` to auto-calculate player ages
- **See full docs** — Read `TOURNAMENT-GUIDE.md` for complete tournament documentation

---

**That's it!** You're ready to track the World Cup, Champions League, or any tournament! 🏆⚽
