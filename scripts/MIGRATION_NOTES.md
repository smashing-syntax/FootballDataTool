# Scripts Organization - Change Log

## What Changed?

All Python data scraping scripts have been organized into a dedicated `scripts/` folder for better project structure.

### Before:
```
FootballDataTool/
├── fetch_data_generic.py
├── fetch_data_config.py
├── fetch_premier_league_multi_season.py
├── fetch_squads.py
├── scraper_launcher.bat
├── requirements.txt
├── SCRAPING_SCRIPTS_README.md
├── QUICK_REFERENCE.md
└── WorldCupSquads - Sheet1.csv
```

### After:
```
FootballDataTool/
├── scripts/
│   ├── README.md                              # Overview of all scripts
│   └── api-sports/                           # API-Sports scrapers
│       ├── fetch_data_generic.py             # CLI-based scraper
│       ├── fetch_data_config.py              # Config-based scraper
│       ├── fetch_premier_league_multi_season.py  # Multi-season automation
│       ├── fetch_squads.py                   # Original (deprecated)
│       ├── scraper_launcher.bat              # Windows launcher
│       ├── requirements.txt                  # Python dependencies
│       ├── README.md                         # Detailed docs
│       └── QUICK_REFERENCE.md                # Quick reference
└── data/
    └── world-cup/
        └── squads_2022.csv                   # Moved from root
```

## Updates Made

### 1. **Path Updates**
All output paths in scripts now use relative paths from the scripts folder:
- Old: `"data/premier-league/squads_2023.csv"`
- New: `"../../data/premier-league/squads_2023.csv"`

### 2. **Documentation**
- `SCRAPING_SCRIPTS_README.md` → `scripts/api-sports/README.md`
- Added `scripts/README.md` as main entry point
- Updated all examples with new paths
- Added scripts section to main project README

### 3. **Data Files**
- `WorldCupSquads - Sheet1.csv` → `data/world-cup/squads_2022.csv`

## Usage

### From Root Directory
```bash
cd scripts/api-sports
python fetch_data_generic.py --type squad --league 39 --season 2023 --output "../../data/premier-league/squads_2023.csv"
```

### Quick Start Options

1. **Windows Users**: Run `scripts/api-sports/scraper_launcher.bat`
2. **Config-Based**: Edit `scripts/api-sports/fetch_data_config.py` then run it
3. **Multi-Season**: Run `scripts/api-sports/fetch_premier_league_multi_season.py`

## Benefits

✅ **Better Organization** - Separates tooling from main C# project  
✅ **Clear Structure** - Easy to find and understand scripts  
✅ **Extensible** - Easy to add new script categories (web scrapers, data processors, etc.)  
✅ **Documentation** - All docs in one place  
✅ **Future-Proof** - Ready for additional automation tools  

## No Breaking Changes

All scripts work exactly the same - just run them from the new location!
