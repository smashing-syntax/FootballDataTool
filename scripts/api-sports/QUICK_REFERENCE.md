# 🚀 Quick Reference Card

## One-Line Commands

### Premier League
```bash
# Squads 2023/24
python fetch_data_generic.py --type squad --league 39 --season 2023 --output "../../data/premier-league/squads_2023-24.csv"

# Fixtures 2023/24
python fetch_data_generic.py --type fixtures --league 39 --season 2023 --output "../../data/premier-league/fixtures_2023-24.csv"

# All seasons (automated)
python fetch_premier_league_multi_season.py
```

### La Liga
```bash
# Squads 2023/24
python fetch_data_generic.py --type squad --league 140 --season 2023 --output "../../data/la-liga/squads_2023-24.csv"

# Fixtures 2023/24
python fetch_data_generic.py --type fixtures --league 140 --season 2023 --output "../../data/la-liga/fixtures_2023-24.csv"
```

### Serie A
```bash
# Squads 2023/24
python fetch_data_generic.py --type squad --league 135 --season 2023 --output "../../data/serie-a/squads_2023-24.csv"

# Fixtures 2023/24
python fetch_data_generic.py --type fixtures --league 135 --season 2023 --output "../../data/serie-a/fixtures_2023-24.csv"
```

### Bundesliga
```bash
# Squads 2023/24
python fetch_data_generic.py --type squad --league 78 --season 2023 --output "../../data/bundesliga/squads_2023-24.csv"

# Fixtures 2023/24
python fetch_data_generic.py --type fixtures --league 78 --season 2023 --output "../../data/bundesliga/fixtures_2023-24.csv"
```

### Champions League
```bash
# Squads 2023/24
python fetch_data_generic.py --type squad --league 2 --season 2023 --output "../../data/champions-league/squads_2023-24.csv"

# Fixtures 2023/24
python fetch_data_generic.py --type fixtures --league 2 --season 2023 --output "../../data/champions-league/fixtures_2023-24.csv"
```

### World Cup
```bash
# 2022 Squads (Qatar)
python fetch_data_generic.py --type squad --league 1 --season 2022 --output "../../data/world-cup/squads_2022.csv" --tournament --sleep 5

# 2022 Fixtures
python fetch_data_generic.py --type fixtures --league 1 --season 2022 --output "../../data/world-cup/fixtures_2022.csv"
```

---

## League IDs Cheat Sheet

```
39  = Premier League (England)
40  = Championship (England)
140 = La Liga (Spain)
135 = Serie A (Italy)
78  = Bundesliga (Germany)
61  = Ligue 1 (France)
88  = Eredivisie (Netherlands)
94  = Liga Portugal (Portugal)
144 = Jupiler Pro League (Belgium)
179 = Premiership (Scotland)
253 = MLS (USA/Canada)

2   = UEFA Champions League
3   = UEFA Europa League
848 = UEFA Conference League

1   = FIFA World Cup
4   = UEFA European Championship
9   = Copa America
```

---

## Common Patterns

### High rate limit protection
```bash
--sleep 10
```

### Tournament format (World Cup, Euros)
```bash
--tournament
```

### Multiple seasons loop (PowerShell)
```powershell
foreach ($season in 2022..2024) {
    python fetch_data_generic.py --type squad --league 39 --season $season --output "..\..\data\pl_$season.csv"
}
```

### Multiple leagues loop (PowerShell)
```powershell
$leagues = @{39="premier-league"; 140="la-liga"; 135="serie-a"}
foreach ($id in $leagues.Keys) {
    python fetch_data_generic.py --type fixtures --league $id --season 2023 --output "..\..\data\$($leagues[$id])\fixtures_2023.csv"
}
```

---

## Windows Quick Launch
```batch
scraper_launcher.bat
```

---

## Config-Based (Easiest)
1. Edit `fetch_data_config.py` (top section)
2. Run: `python fetch_data_config.py`
