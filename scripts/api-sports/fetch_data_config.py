"""
Configuration-based Football Data Scraper
Edit the CONFIG section below to set your scraping parameters
"""

import time
import pandas as pd
import requests
from datetime import datetime

# =============================================================================
# CONFIGURATION - Edit these values for your needs
# =============================================================================

API_KEY = "169a7833f796ed2f3b4ab514e424d471"

# What to scrape: 'squad' or 'fixtures'
SCRAPE_TYPE = 'squad'

# League/Competition ID (see LEAGUE_IDS below for common ones)
LEAGUE_ID = 39  # Premier League

# Season (Free plans support 2022-2024)
SEASON = 2023

# Output file (relative to project root)
OUTPUT_FILE = "../../data/premier-league/squads_2023.csv"

# Tournament mode (for World Cup, Euros, etc.)
IS_TOURNAMENT = False

# Sleep time between API requests (seconds)
SLEEP_TIME = 3

# =============================================================================
# Common League IDs (Reference)
# =============================================================================
LEAGUE_IDS = {
    # Top 5 European Leagues
    'Premier League': 39,
    'La Liga': 140,
    'Serie A': 135,
    'Bundesliga': 78,
    'Ligue 1': 61,

    # European Competitions
    'Champions League': 2,
    'Europa League': 3,
    'Europa Conference League': 848,

    # International Tournaments
    'World Cup': 1,
    'European Championship': 4,
    'Copa America': 9,

    # Other Top Leagues
    'Eredivisie': 88,
    'Portuguese Liga': 94,
    'Belgian Pro League': 144,
    'Scottish Premiership': 179,
    'MLS': 253,
}

# =============================================================================
# API Configuration (Don't change unless needed)
# =============================================================================

BASE_URL = "https://v3.football.api-sports.io"
HEADERS = {
    'x-apisports-key': API_KEY,
    'x-rapidapi-host': 'v3.football.api-sports.io'
}

# =============================================================================
# Functions
# =============================================================================

def fetch_teams(league_id, season):
    """Fetch all teams for a given league and season."""
    url = f"{BASE_URL}/teams?league={league_id}&season={season}"
    response = requests.get(url, headers=HEADERS)

    if response.status_code != 200:
        print(f"❌ Error fetching teams: {response.status_code}")
        print(f"Response: {response.text}")
        return []

    data = response.json()

    if data.get('errors'):
        print(f"❌ API Error: {data['errors']}")
        return []

    teams = []
    for item in data.get("response", []):
        teams.append({
            "id": item["team"]["id"],
            "name": item["team"]["name"]
        })

    return teams

def fetch_squad_for_team(team_id, team_name, season, is_tournament=False):
    """Fetch squad for a specific team."""
    url = f"{BASE_URL}/players/squads?team={team_id}"
    response = requests.get(url, headers=HEADERS)

    if response.status_code == 429:
        print(f"⚠️  Rate limit hit for {team_name}, waiting 10 seconds...")
        time.sleep(10)
        return fetch_squad_for_team(team_id, team_name, season, is_tournament)

    if response.status_code != 200:
        print(f"❌ Error fetching squad for {team_name}: {response.status_code}")
        return []

    data = response.json()
    players_list = []

    for item in data.get("response", []):
        players = item.get("players", [])
        for p in players:
            pos_map = {"Goalkeeper": "GK", "Defender": "DF", "Midfielder": "MF", "Attacker": "FW"}
            position = pos_map.get(p.get("position"), "MF")

            shirt = p.get("number") or 0
            pref_foot = "Left" if shirt in [3, 7, 11, 21] else "Right"

            player_data = {
                "Team": team_name,
                "Season": season,
                "PlayerName": p.get("name"),
                "DateOfBirth": "N/A",
                "Position": position,
                "ShirtNumber": p.get("number"),
                "Nationality": team_name if is_tournament else "N/A",
                "PreviousClub": "N/A",
                "Height": "N/A",
                "PreferredFoot": pref_foot
            }

            if not is_tournament:
                player_data["PreviousLeague"] = "N/A"
                player_data["Weight"] = "N/A"

            players_list.append(player_data)

    return players_list

def fetch_fixtures(league_id, season):
    """Fetch all fixtures for a given league and season."""
    url = f"{BASE_URL}/fixtures?league={league_id}&season={season}"
    response = requests.get(url, headers=HEADERS)

    if response.status_code != 200:
        print(f"❌ Error fetching fixtures: {response.status_code}")
        print(f"Response: {response.text}")
        return []

    data = response.json()

    if data.get('errors'):
        print(f"❌ API Error: {data['errors']}")
        return []

    fixtures_list = []
    for fixture in data.get("response", []):
        fixture_info = fixture.get("fixture", {})
        league_info = fixture.get("league", {})
        teams = fixture.get("teams", {})
        goals = fixture.get("goals", {})

        home_goals = goals.get("home")
        away_goals = goals.get("away")

        if home_goals is not None and away_goals is not None:
            if home_goals > away_goals:
                result = "H"
            elif away_goals > home_goals:
                result = "A"
            else:
                result = "D"
        else:
            result = None

        fixture_date = fixture_info.get("date", "")
        if fixture_date:
            try:
                dt = datetime.fromisoformat(fixture_date.replace('Z', '+00:00'))
                formatted_date = dt.strftime("%d/%m/%Y")
            except:
                formatted_date = fixture_date
        else:
            formatted_date = "N/A"

        # Extract round/gameweek
        round_str = league_info.get("round", "")
        gameweek = round_str.replace("Regular Season - ", "").replace("Matchday ", "")

        match_data = {
            "Div": league_info.get("country", ""),
            "Season": f"{season}/{season+1}" if season < 2100 else str(season),
            "GW": gameweek,
            "Date": formatted_date,
            "HomeTeam": teams.get("home", {}).get("name", ""),
            "AwayTeam": teams.get("away", {}).get("name", ""),
            "FTHG": home_goals,
            "FTAG": away_goals,
            "FTR": result,
            "Referee": fixture_info.get("referee", ""),
        }

        fixtures_list.append(match_data)

    return fixtures_list

def scrape_squads(league_id, season, output_file, is_tournament=False, sleep_time=3):
    """Main function to scrape squad data."""
    print(f"\n{'='*60}")
    print(f"📋 Fetching Squad Data")
    print(f"League ID: {league_id} | Season: {season}")
    print(f"{'='*60}\n")

    teams = fetch_teams(league_id, season)
    if not teams:
        print("❌ No teams found.")
        return

    print(f"✅ Found {len(teams)} teams\n")

    all_players = []
    for idx, team in enumerate(teams, 1):
        print(f"[{idx}/{len(teams)}] Fetching squad for: {team['name']}")
        squad = fetch_squad_for_team(team['id'], team['name'], season, is_tournament)
        all_players.extend(squad)

        if idx < len(teams):
            time.sleep(sleep_time)

    if all_players:
        df = pd.DataFrame(all_players)
        df.to_csv(output_file, index=False, encoding="utf-8-sig")
        print(f"\n✅ SUCCESS! Saved {len(all_players)} player records to '{output_file}'")
    else:
        print("\n❌ No player data retrieved.")

def scrape_fixtures(league_id, season, output_file):
    """Main function to scrape fixture data."""
    print(f"\n{'='*60}")
    print(f"📅 Fetching Fixture Data")
    print(f"League ID: {league_id} | Season: {season}")
    print(f"{'='*60}\n")

    fixtures = fetch_fixtures(league_id, season)

    if fixtures:
        print(f"✅ Found {len(fixtures)} fixtures\n")
        df = pd.DataFrame(fixtures)
        df.to_csv(output_file, index=False, encoding="utf-8-sig")
        print(f"✅ SUCCESS! Saved {len(fixtures)} fixtures to '{output_file}'")
    else:
        print("❌ No fixture data retrieved.")

def main():
    if API_KEY == "YOUR_API_FOOTBALL_KEY_HERE":
        print("❌ ERROR: Please set your API-Football key in the script.")
        return

    print(f"\n🏟️  Football Data Scraper")
    print(f"Type: {SCRAPE_TYPE.upper()}")
    print(f"League ID: {LEAGUE_ID}")
    print(f"Season: {SEASON}")
    print(f"Output: {OUTPUT_FILE}")

    if SCRAPE_TYPE == 'squad':
        scrape_squads(LEAGUE_ID, SEASON, OUTPUT_FILE, IS_TOURNAMENT, SLEEP_TIME)
    elif SCRAPE_TYPE == 'fixtures':
        scrape_fixtures(LEAGUE_ID, SEASON, OUTPUT_FILE)
    else:
        print(f"❌ Invalid SCRAPE_TYPE: {SCRAPE_TYPE}")

if __name__ == "__main__":
    main()
