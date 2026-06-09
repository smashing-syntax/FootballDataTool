# World Cup Squad Example - README

## Overview

The file `squad_examples_worldcup.csv` demonstrates how to use the same squad CSV format for **international tournaments** like the World Cup.

## What's Included

This example contains **100+ players** from 4 national teams that reached the semi-finals of the 2022 FIFA World Cup:

- 🇦🇷 **Argentina** (23 players) - Champions
- 🇫🇷 **France** (23 players) - Runners-up
- 🇧🇷 **Brazil** (24 players) - Quarter-finalists
- 🏴󠁧󠁢󠁥󠁮󠁧󠁿 **England** (26 players) - Quarter-finalists

## Key Features

Each player entry includes:
- **Name** - Player's full name
- **Date of Birth** - For automatic age calculation
- **Position** - GK, CB, RB, LB, DM, CM, AM, LW, RW, ST, FW
- **Shirt Number** - Tournament squad number
- **Nationality** - National team
- **Current Club** - Club team at time of tournament
- **Club League** - League the club plays in
- **Height** - Player height in centimeters
- **Weight** - Player weight in kilograms
- **Preferred Foot** - Left, Right, or Both

**Note:** The columns are called `CurrentClub` and `ClubLeague` in this file (tournament naming), but you can also use `PreviousClub` and `PreviousLeague` (transfer naming) - they're aliases for the same fields!

## Notable Players Included

### Argentina
- Lionel Messi (PSG)
- Emiliano Martínez (Aston Villa)
- Enzo Fernández (Benfica)
- Julián Álvarez (Man City)
- Ángel Di María (Juventus)

### France
- Kylian Mbappé (PSG)
- Hugo Lloris (Tottenham)
- Antoine Griezmann (Atlético Madrid)
- Olivier Giroud (AC Milan)
- Aurélien Tchouaméni (Real Madrid)

### Brazil
- Neymar (PSG)
- Alisson (Liverpool)
- Casemiro (Man United)
- Vinícius Júnior (Real Madrid)
- Richarlison (Tottenham)

### England
- Harry Kane (Tottenham)
- Jordan Pickford (Everton)
- Jude Bellingham (Borussia Dortmund)
- Bukayo Saka (Arsenal)
- Declan Rice (West Ham)

## How to Use

### 1. Load Tournament Matches

```csharp
var csvService = new CsvDataService();
var seasonData = csvService.LoadSeasonDataFromFile("worldcup_2022.csv");
```

### 2. Load Squad Data

```csharp
seasonData.LoadSquadDataFromCsv("squad_examples_worldcup.csv");
```

### 3. Access Enriched Player Data

```csharp
// Get the final
var final = seasonData.GetMatchesByStage(TournamentStage.Final).First();

// Access Argentina's lineup (automatically enriched with ages)
if (final.ExtendedData != null)
{
    foreach (var player in final.ExtendedData.HomeStartingLineup)
    {
        Console.WriteLine($"{player.Name} - Age {player.Age} - {player.Position}");
        Console.WriteLine($"  Club: {player.PreviousClub} ({player.PreviousLeague})");
    }
}
```

## Age Calculation

The tool **automatically calculates** each player's age on the date of each match:

- Messi was **35 years old** during the World Cup final (18 Dec 2022)
- Mbappé was **23 years old** during the final
- Enzo Fernández was **21 years old** during the tournament

Ages are dynamically calculated, so if you add matches with different dates, ages adjust accordingly!

## Format Notes

- **Team**: Use the national team name (e.g., `Argentina`, not `ARG`)
- **Season**: Use the tournament year (`2022` for World Cup 2022)
- **CurrentClub** (or `PreviousClub`): The club the player was at during the tournament
- **ClubLeague** (or `PreviousLeague`): The league/country of that club

The tool accepts both naming conventions - use whichever makes more sense:
- 🏆 **Tournaments**: `CurrentClub`, `ClubLeague` (clearer for international competitions)
- 🔄 **Leagues/Transfers**: `PreviousClub`, `PreviousLeague` (makes sense for transfer history)

## Compatible With

✅ World Cup  
✅ Continental Championships (Euros, Copa América, AFCON, etc.)  
✅ Olympic Football  
✅ Any international tournament  

The **exact same format** also works for club tournaments (Champions League, Europa League, etc.)!

## See Also

- **`tournament_examples_worldcup.csv`** - World Cup match data example
- **`TOURNAMENT-GUIDE.md`** - Complete tournament documentation
- **`SQUAD-DATA-GUIDE.md`** - Full squad data reference

---

**The same squad CSV format works for leagues AND tournaments!** 🏆⚽
