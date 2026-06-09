# CSV Format Quick Reference

## Supported Data Sources

FootballDataTool is designed to work with CSV files from various sources. Here are the most common formats:

## 1. football-data.co.uk Format

**Most Popular Source:** http://www.football-data.co.uk/

### Standard Format
```csv
Div,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,HTHG,HTAG,HTR,Referee,HS,AS,HST,AST
E0,11/08/2023,Burnley,Man City,0,3,A,0,2,A,Michael Oliver,8,20,3,8
E0,12/08/2023,Arsenal,Nottm Forest,2,1,H,1,0,H,Simon Hooper,13,8,4,1
```

### Key Columns
- **Div**: Division code (E0=Premier League, SP1=La Liga, etc.)
- **Date**: Match date (dd/MM/yyyy)
- **HomeTeam/AwayTeam**: Team names
- **FTHG/FTAG**: Full Time Home/Away Goals
- **FTR**: Full Time Result (H/D/A)
- **HTHG/HTAG**: Half Time scores (ignored by tool)
- **Referee**: Match official
- **HS/AS**: Home/Away shots (ignored by tool)

**Detection:** Automatically recognized by `Div` column and division codes.

## 2. Simple Custom Format

### Minimal Format
```csv
GW,HomeTeam,AwayTeam,FTHG,FTAG
1,Arsenal,Chelsea,2,1
1,Liverpool,Man City,3,0
2,Chelsea,Liverpool,1,1
```

### With Dates
```csv
GW,Date,HomeTeam,AwayTeam,FTHG,FTAG
1,12/08/2023,Arsenal,Chelsea,2,1
1,12/08/2023,Liverpool,Man City,3,0
```

**Detection:** 
- Season inferred from dates
- League detected from team names
- Works with any date format (dd/MM/yyyy, yyyy-MM-dd, etc.)

## 3. Extended Format with Metadata

```csv
Division,Season,Gameweek,Date,Time,HomeTeam,AwayTeam,HomeGoals,AwayGoals,Referee
Premier League,2023/24,1,2023-08-11,20:00,Arsenal,Nottm Forest,2,1,Simon Hooper
Premier League,2023/24,1,2023-08-12,15:00,Burnley,Man City,0,3,Michael Oliver
```

**Detection:** Explicit metadata in every row ensures accurate detection.

## Division Codes Reference

### England
- **E0**: Premier League
- **E1**: Championship
- **E2**: League One
- **E3**: League Two
- **EC**: National League

### Spain
- **SP1**: La Liga
- **SP2**: Segunda División

### Italy
- **I1**: Serie A
- **I2**: Serie B

### Germany
- **D1**: Bundesliga
- **D2**: 2. Bundesliga

### France
- **F1**: Ligue 1
- **F2**: Ligue 2

### Scotland
- **SC0**: Premiership
- **SC1**: Championship
- **SC2**: League One
- **SC3**: League Two

### Netherlands
- **N1**: Eredivisie

### Belgium
- **B1**: Pro League

### Portugal
- **P1**: Primeira Liga

### Turkey
- **T1**: Süper Lig

### Greece
- **G1**: Super League

## Column Name Variations

The tool accepts multiple names for each field:

### Team Names
```
Home Team: HomeTeam | Home Team | Home | HTeam | HOMETEAM
Away Team: AwayTeam | Away Team | Away | ATeam | AWAYTEAM
```

### Goals
```
Home Goals: FTHG | HomeGoals | HG | Home Goals | HGoals | FullTimeHomeGoals
Away Goals: FTAG | AwayGoals | AG | Away Goals | AGoals | FullTimeAwayGoals
```

### Match Info
```
Gameweek: GW | Gameweek | Round | Wk | Week | Matchday
Date: Date | MatchDate | Match Date
Time: Time | KickOff | Kick Off
Division: Div | Division | League | Comp | Competition
Season: Season
Referee: Referee | Ref
```

## Date Format Support

All these formats are automatically recognized:

- `dd/MM/yyyy` - 12/08/2023 (UK format)
- `dd/MM/yy` - 12/08/23
- `yyyy-MM-dd` - 2023-08-12 (ISO format)
- `MM/dd/yyyy` - 08/12/2023 (US format)
- `d/M/yyyy` - 2/8/2023 (no leading zeros)
- `dd-MM-yyyy` - 12-08-2023

## Season Format Support

All these season formats are recognized:

- `2023/24` (standard)
- `2023-24` (alternative)
- `2023` (single year)
- `202324` (compact)
- `2023/2024` (full years)

## Filename Detection Examples

The tool extracts metadata from filenames:

### Division Code
- `E0_2023-24.csv` → Premier League, 2023/24
- `SP1.csv` → La Liga
- `bundesliga_2022.csv` → Bundesliga

### League Name
- `premier_league_2023-24.csv` → Premier League, 2023/24
- `laliga_2022_23.csv` → La Liga, 2022/23
- `SerieA2023.csv` → Serie A, 2023

### Season Only
- `season_2023-24.csv` → 2023/24 (league from team names)
- `football_2022_2023.csv` → 2022/23

## Creating Your Own CSV

### Minimum Requirements
1. Home team column
2. Away team column  
3. Home goals column
4. Away goals column

### Recommended Additions
1. Gameweek (for chronological ordering)
2. Date (for season inference)
3. Division/League (for explicit identification)
4. Season (for multi-season analysis)

### Example Template
```csv
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG
E0,2023/24,1,11/08/2023,Arsenal,Nottm Forest,2,1
E0,2023/24,1,12/08/2023,Bournemouth,West Ham,1,1
E0,2023/24,1,12/08/2023,Burnley,Man City,0,3
```

## Troubleshooting

### "CSV must contain home team and away team columns"
**Problem:** Column names not recognized  
**Solution:** Rename to one of: `HomeTeam`, `AwayTeam`, `Home Team`, `Away Team`

### "CSV must contain home goals and away goals columns"
**Problem:** Goal columns not recognized  
**Solution:** Rename to one of: `FTHG`, `FTAG`, `HomeGoals`, `AwayGoals`

### League shows as "Unknown"
**Solutions:**
1. Add `Div` column with division code (E0, SP1, etc.)
2. Add `League` column with league name
3. Use recognizable team names (Arsenal, Barcelona, etc.)
4. Include league name in filename

### Season shows as "Unknown"
**Solutions:**
1. Add `Season` column with format "2023/24"
2. Add `Date` column with match dates
3. Include season in filename (e.g., "data_2023-24.csv")

### Gameweeks are incorrect
**Solutions:**
1. Add `GW` or `Gameweek` column with explicit numbers
2. Ensure dates are in correct format for chronological ordering
3. Check that each team appears only once per gameweek

## Advanced Features

### Multiple Seasons in One File
```csv
Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG
2022/23,38,28/05/2023,Arsenal,Wolves,5,0
2023/24,1,11/08/2023,Burnley,Man City,0,3
```
Note: Tool currently processes as single dataset. Use separate files for proper multi-season analysis.

### Cup Competitions
For cup matches without gameweeks:
```csv
Round,Date,HomeTeam,AwayTeam,FTHG,FTAG
R16,14/03/2023,Man City,Leipzig,7,0
QF,11/04/2023,Man City,Bayern,3,0
```
The tool will auto-assign gameweek numbers based on match order.

## Data Source Recommendations

1. **football-data.co.uk** - Comprehensive historical data, updated weekly
2. **Official League APIs** - Real-time data (requires API integration)
3. **Custom Spreadsheets** - Perfect for amateur/local leagues
4. **Wikipedia** - Season tables can be copied to CSV format

---

**Remember:** The tool is designed to be forgiving. As long as you have team names and goals, it will work! Everything else is automatically detected or inferred.
