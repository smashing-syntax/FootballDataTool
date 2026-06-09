# Scripts

Collection of utility scripts for the FootballDataTool project.

## 📁 Structure

```
scripts/
├── api-sports/          # API-Sports data scraping scripts
│   ├── README.md        # Detailed documentation
│   ├── QUICK_REFERENCE.md
│   ├── requirements.txt
│   ├── fetch_data_generic.py          # CLI-based scraper (most flexible)
│   ├── fetch_data_config.py           # Config-based scraper (easiest)
│   ├── fetch_premier_league_multi_season.py  # Multi-season automation
│   ├── fetch_squads.py                # Original World Cup scraper (deprecated)
│   └── scraper_launcher.bat           # Windows GUI launcher
```

## 🚀 Quick Start

### API-Sports Scrapers

Navigate to the api-sports folder for football data scraping:

```bash
cd scripts/api-sports
```

**Option 1: Command-Line (Most Flexible)**
```bash
python fetch_data_generic.py --type squad --league 39 --season 2023 --output "../../data/premier-league/squads_2023.csv"
```

**Option 2: Configuration-Based (Easiest)**
1. Edit `fetch_data_config.py` configuration section
2. Run: `python fetch_data_config.py`

**Option 3: Windows Launcher**
```bash
scraper_launcher.bat
```

See [api-sports/README.md](api-sports/README.md) for complete documentation.

---

## 📝 Adding New Scripts

When adding new scripts:
1. Create a subfolder if it's a new category (e.g., `web-scrapers/`, `data-processors/`)
2. Include a README.md with usage instructions
3. Add a requirements.txt if Python dependencies are needed
4. Update this main README with a link

---

## 🔗 Related Documentation

- **API-Sports Scripts**: [api-sports/README.md](api-sports/README.md)
- **Quick Reference**: [api-sports/QUICK_REFERENCE.md](api-sports/QUICK_REFERENCE.md)
- **Data Templates**: [../data/templates/](../data/templates/)
- **Main Project**: [../README.md](../README.md)
