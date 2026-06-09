# Football Data Scraping Scripts

Collection of Python scripts for scraping football data from API-Sports.

## 📁 Scripts Overview

### 1. `fetch_data_generic.py` - Command-Line Interface
**Most flexible option** - Use command-line arguments to fetch any league/season data.

**Usage:**
```bash
# Fetch Premier League 2023 squads
python fetch_data_generic.py --type squad --league 39 --season 2023 --output "../../data/premier-league/squads_2023.csv"

# Fetch La Liga 2022 fixtures
python fetch_data_generic.py --type fixtures --league 140 --season 2022 --output "../../data/la-liga/fixtures_2022.csv"

# Fetch World Cup 2022 squads (tournament mode)
python fetch_data_generic.py --type squad --league 1 --season 2022 --output "../../data/world-cup/squads_2022.csv" --tournament

# Adjust rate limiting (increase sleep time)
python fetch_data_generic.py --type squad --league 39 --season 2023 --output "../../data/squads.csv" --sleep 10
```

**Arguments:**
- `--type` : `squad` or `fixtures`
- `--league` : League ID (see table below)
- `--season` : Year (e.g., 2023). **Free plans: 2022-2024 only**
- `--output` : Output CSV file path
- `--tournament` : (Optional) Use tournament format for World Cup/Euros
- `--sleep` : (Optional) Seconds between requests (default: 3)

### 2. `fetch_data_config.py` - Configuration-Based
**Easiest to use** - Edit the CONFIG section at the top of the file and run.

**Usage:**
1. Open `fetch_data_config.py`
2. Edit these variables:
   ```python
   SCRAPE_TYPE = 'squad'  # or 'fixtures'
   LEAGUE_ID = 39  # Premier League
   SEASON = 2023
   OUTPUT_FILE = "data/premier-league/squads_2023.csv"
   IS_TOURNAMENT = False
   SLEEP_TIME = 3
   ```
3. Run: `python fetch_data_config.py`

### 3. `fetch_premier_league_multi_season.py` - Automated Multi-Season
Automatically fetches Premier League data for multiple seasons (2022-2024).

**Usage:**
```bash
python fetch_premier_league_multi_season.py
```

Choose:
1. Squads only
2. Fixtures only
3. Both

### 4. `fetch_squads.py` - Original Script (Deprecated)
The original World Cup squad scraper. **Use the generic scripts above instead.**

---

## 🏆 Common League IDs

| League | ID | Notes |
|--------|----|----|
| **Premier League** | 39 | England |
| **La Liga** | 140 | Spain |
| **Serie A** | 135 | Italy |
| **Bundesliga** | 78 | Germany |
| **Ligue 1** | 61 | France |
| **Champions League** | 2 | UEFA |
| **Europa League** | 3 | UEFA |
| **World Cup** | 1 | FIFA |
| **European Championship** | 4 | UEFA |
| **Copa America** | 9 | CONMEBOL |
| **Eredivisie** | 88 | Netherlands |
| **Liga Portugal** | 94 | Portugal |
| **MLS** | 253 | USA/Canada |
| **Championship** | 40 | England (2nd tier) |
| **Scottish Premiership** | 179 | Scotland |

**Find more league IDs:** https://www.api-football.com/documentation-v3#tag/Leagues

---

## ⚙️ API Configuration

### Setting Your API Key
All scripts use the same API key. You can either:

1. **Edit each script directly:**
   ```python
   API_KEY = "your_api_key_here"
   ```

2. **Use environment variable** (recommended):
   ```bash
   # Windows PowerShell
   $env:API_SPORTS_KEY = "your_api_key_here"

   # Windows CMD
   set API_SPORTS_KEY=your_api_key_here

   # Linux/Mac
   export API_SPORTS_KEY="your_api_key_here"
   ```

### Free Plan Limitations
- **Seasons:** 2022-2024 only
- **Rate Limit:** 100 requests/day
- **Rate:** ~10 requests/minute

**Tips:**
- Increase `--sleep` time to 5-10 seconds if hitting rate limits
- Run scripts at different times if you hit daily limit
- Consider upgrading for 2026 World Cup data

---

## 📊 Output Formats

### Squad Data
Matches your template format:
```
Team,Season,PlayerName,DateOfBirth,Position,ShirtNumber,Nationality,PreviousClub,PreviousLeague,Height,Weight,PreferredFoot
```

**Tournament format** (World Cup/Euros) omits `PreviousLeague` and `Weight`.

### Fixture Data
Basic format:
```
Div,Season,GW,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR,Referee
```

Extended fields available but may be empty depending on API data availability.

---

## 🚀 Quick Examples

### Example 1: Premier League 2023/24 Squads
```bash
python fetch_data_generic.py --type squad --league 39 --season 2023 --output "../../data/premier-league/squads_2023-24.csv" --sleep 5
```

### Example 2: La Liga 2022/23 Fixtures
```bash
python fetch_data_generic.py --type fixtures --league 140 --season 2022 --output "../../data/la-liga/fixtures_2022-23.csv"
```

### Example 3: World Cup 2022 Squads
```bash
python fetch_data_generic.py --type squad --league 1 --season 2022 --output "../../data/world-cup/squads_2022.csv" --tournament --sleep 5
```

### Example 4: Champions League 2023/24 Fixtures
```bash
python fetch_data_generic.py --type fixtures --league 2 --season 2023 --output "../../data/champions-league/fixtures_2023-24.csv"
```

### Example 5: All Premier League Seasons (Automated)
```bash
python fetch_premier_league_multi_season.py
# Choose option 3 (Both squads and fixtures)
```

---

## 🐛 Troubleshooting

### Error: "Free plans do not have access to this season"
- **Solution:** Use seasons 2022-2024 only
- For 2026 World Cup, you need a paid plan

### Error: HTTP 429 (Rate Limit)
- **Solution:** Increase sleep time: `--sleep 10`
- Wait for rate limit reset (typically hourly/daily)
- Reduce number of requests

### Error: "No teams found"
- Check league ID is correct
- Verify season is within 2022-2024
- Check API key is valid

### Empty squad data
- Some teams may not have squad data available yet
- Tournament squads are only available close to tournament dates
- Try a different season/league

---

## 📝 Notes

- **Data Quality:** API-Sports provides basic data. Some fields like `DateOfBirth`, `Height`, `Weight`, `PreferredFoot` may need manual correction.
- **Tournament Squads:** Only available close to tournament dates (e.g., World Cup squads available ~1 month before)
- **Rate Limiting:** Scripts include automatic retry logic for rate limits
- **File Paths:** Scripts will create directories if they don't exist

---

## 🔗 Resources

- **API-Sports Documentation:** https://www.api-football.com/documentation-v3
- **Get API Key:** https://www.api-football.com/
- **League/Competition IDs:** https://www.api-football.com/documentation-v3#tag/Leagues
- **Status Page:** https://status.api-football.com/

---

## 📦 Dependencies

```bash
pip install pandas requests
```

Or use requirements.txt:
```bash
pip install -r requirements.txt
```
