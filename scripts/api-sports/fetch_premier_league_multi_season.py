"""
Quick-start script for fetching Premier League data across multiple seasons
"""

import subprocess
import os

# Configuration
LEAGUE_ID = 39  # Premier League
SEASONS = [2022, 2023, 2024]  # Available seasons on free plan
OUTPUT_DIR = "../../data/premier-league"
SLEEP_TIME = 5  # Seconds between requests

# Get the project root directory (two levels up from script location)
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.abspath(os.path.join(SCRIPT_DIR, "..", ".."))
OUTPUT_DIR = os.path.join(PROJECT_ROOT, "data", "premier-league")

# Ensure output directory exists
os.makedirs(OUTPUT_DIR, exist_ok=True)

def fetch_squads():
    """Fetch squad data for all seasons."""
    print("\n" + "="*60)
    print("🔵 FETCHING PREMIER LEAGUE SQUAD DATA")
    print("="*60)

    for season in SEASONS:
        output_file = f"{OUTPUT_DIR}/squads_{season}-{season+1}.csv"
        print(f"\n📋 Fetching squads for {season}/{season+1} season...")

        cmd = [
            "python", "fetch_data_generic.py",
            "--type", "squad",
            "--league", str(LEAGUE_ID),
            "--season", str(season),
            "--output", output_file,
            "--sleep", str(SLEEP_TIME)
        ]

        subprocess.run(cmd)
        print(f"✅ Completed {season}/{season+1}")

def fetch_fixtures():
    """Fetch fixture data for all seasons."""
    print("\n" + "="*60)
    print("🔵 FETCHING PREMIER LEAGUE FIXTURE DATA")
    print("="*60)

    for season in SEASONS:
        output_file = f"{OUTPUT_DIR}/fixtures_{season}-{season+1}.csv"
        print(f"\n📅 Fetching fixtures for {season}/{season+1} season...")

        cmd = [
            "python", "fetch_data_generic.py",
            "--type", "fixtures",
            "--league", str(LEAGUE_ID),
            "--season", str(season),
            "--output", output_file
        ]

        subprocess.run(cmd)
        print(f"✅ Completed {season}/{season+1}")

def main():
    print("""
╔══════════════════════════════════════════════════════════════╗
║        Premier League Multi-Season Data Fetcher              ║
║                                                              ║
║  This will fetch data for seasons: 2022/23, 2023/24, 2024/25║
╚══════════════════════════════════════════════════════════════╝
    """)

    choice = input("What would you like to fetch?\n1. Squads\n2. Fixtures\n3. Both\n\nEnter choice (1-3): ")

    if choice == "1":
        fetch_squads()
    elif choice == "2":
        fetch_fixtures()
    elif choice == "3":
        fetch_squads()
        fetch_fixtures()
    else:
        print("❌ Invalid choice")

if __name__ == "__main__":
    main()
