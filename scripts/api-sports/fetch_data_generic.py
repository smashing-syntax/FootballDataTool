"""
Generic Football Data Scraper for API-Sports
Supports both squad and fixture data across any league/competition/season

Usage:
    python fetch_data_generic.py --type squad --league 39 --season 2023 --output "data/premier-league/squads_2023.csv"
    python fetch_data_generic.py --type fixtures --league 39 --season 2023 --output "data/premier-league/fixtures_2023.csv"
    python fetch_data_generic.py --type squad --league 1 --season 2022 --output "data/world-cup/squads_2022.csv" --tournament
"""

import argparse
import time
import pandas as pd
import requests
from datetime import datetime

# API Configuration
API_KEY = "169a7833f796ed2f3b4ab514e424d471"
BASE_URL = "https://v3.football.api-sports.io"

HEADERS = {
    'x-apisports-key': API_KEY,
    'x-rapidapi-host': 'v3.football.api-sports.io'
}

# Common League IDs (for reference)
LEAGUE_IDS = {
    'premier_league': 39,
    'la_liga': 140,
    'serie_a': 135,
    'bundesliga': 78,
    'ligue_1': 61,
    'champions_league': 2,
    'europa_league': 3,
    'world_cup': 1,
    'euros': 4
}

def fetch_teams(league_id, season):
    """Fetch all teams for a given league and season."""
    url = f"{BASE_URL}/teams?league={league_id}&season={season}"
    response = requests.get(url, headers=HEADERS)

    if response.status_code != 200:
        print(f"❌ Error fetching teams: {response.status_code}")
        print(f"Response: {response.text}")
        return []

    data = response.json()

    # Check for API errors
    if data.get('errors'):
        print(f"❌ API Error: {data['errors']}")
        return []

    teams = []
    for item in data.get("response", []):
        teams.append({
            "id": item["team"]["id"],
            "name": item["team"]["name"],
            "code": item["team"].get("code", ""),
            "country": item["team"].get("country", "")
        })

    print(f"✅ Found {len(teams)} teams")
    return teams

def fetch_squad_for_team(team_id, team_name, season, is_tournament=False):
    """Fetch squad/roster for a specific team."""
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
            # Map positions
            pos_map = {"Goalkeeper": "GK", "Defender": "DF", "Midfielder": "MF", "Attacker": "FW"}
            position = pos_map.get(p.get("position"), "MF")

            # Determine preferred foot (simplified heuristic)
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
                "PreviousLeague": "N/A" if not is_tournament else None,
                "Height": "N/A",
                "Weight": "N/A" if not is_tournament else None,
                "PreferredFoot": pref_foot
            }

            # Remove None values for tournament format
            player_data = {k: v for k, v in player_data.items() if v is not None}
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

    # Check for API errors
    if data.get('errors'):
        print(f"❌ API Error: {data['errors']}")
        return []

    fixtures_list = []
    for fixture in data.get("response", []):
        fixture_info = fixture.get("fixture", {})
        league_info = fixture.get("league", {})
        teams = fixture.get("teams", {})
        goals = fixture.get("goals", {})
        score = fixture.get("score", {})

        # Determine result
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

        # Parse date
        fixture_date = fixture_info.get("date", "")
        if fixture_date:
            try:
                dt = datetime.fromisoformat(fixture_date.replace('Z', '+00:00'))
                formatted_date = dt.strftime("%d/%m/%Y")
            except:
                formatted_date = fixture_date
        else:
            formatted_date = "N/A"

        match_data = {
            "Div": league_info.get("country", ""),
            "Season": f"{season}/{season+1}" if season < 2100 else str(season),
            "GW": league_info.get("round", "").replace("Regular Season - ", ""),
            "Stage": league_info.get("round", ""),
            "Group": None,
            "Leg": None,
            "Date": formatted_date,
            "HomeTeam": teams.get("home", {}).get("name", ""),
            "AwayTeam": teams.get("away", {}).get("name", ""),
            "FTHG": home_goals,
            "FTAG": away_goals,
            "FTR": result,
            "Referee": fixture_info.get("referee", ""),
            "AR1": None,
            "AR2": None,
            "FourthOfficial": None,
            "VAR": None,
            "HomeManager": None,
            "AwayManager": None,
            "HomeFormation": None,
            "AwayFormation": None,
            "HomeLineup": None,
            "AwayLineup": None,
            "HomeSubstitutes": None,
            "AwaySubstitutes": None,
            "Stadium": fixture_info.get("venue", {}).get("name", ""),
            "Attendance": None,
            "StadiumCapacity": None,
            "Temperature": None,
            "WeatherConditions": None,
            "HomeGoalscorers": None,
            "AwayGoalscorers": None,
            "HomeSubstitutions": None,
            "AwaySubstitutions": None,
            "HomeYellowCards": None,
            "AwayYellowCards": None,
            "HomeRedCards": None,
            "AwayRedCards": None,
            "HomeMinutesPlayed": None,
            "AwayMinutesPlayed": None,
            "HomeInjuries": None,
            "AwayInjuries": None
        }

        # Remove None values
        match_data = {k: v for k, v in match_data.items() if v is not None}
        fixtures_list.append(match_data)

    print(f"✅ Found {len(fixtures_list)} fixtures")
    return fixtures_list

def scrape_squads(league_id, season, output_file, is_tournament=False, sleep_time=3):
    """Main function to scrape squad data."""
    print(f"\n{'='*60}")
    print(f"📋 Fetching Squad Data")
    print(f"League ID: {league_id} | Season: {season}")
    print(f"{'='*60}\n")

    # Fetch teams
    teams = fetch_teams(league_id, season)
    if not teams:
        print("❌ No teams found. Check your league ID, season, or API subscription.")
        return

    # Fetch squads
    all_players = []
    for idx, team in enumerate(teams, 1):
        print(f"[{idx}/{len(teams)}] Fetching squad for: {team['name']}")
        squad = fetch_squad_for_team(team['id'], team['name'], season, is_tournament)
        all_players.extend(squad)

        # Rate limiting
        if idx < len(teams):
            time.sleep(sleep_time)

    # Save to CSV
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
        df = pd.DataFrame(fixtures)
        df.to_csv(output_file, index=False, encoding="utf-8-sig")
        print(f"\n✅ SUCCESS! Saved {len(fixtures)} fixtures to '{output_file}'")
    else:
        print("\n❌ No fixture data retrieved.")

def main():
    parser = argparse.ArgumentParser(
        description="Generic Football Data Scraper for API-Sports",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  # Fetch Premier League 2023 squads
  python fetch_data_generic.py --type squad --league 39 --season 2023 --output "data/premier-league/squads_2023.csv"

  # Fetch La Liga 2022 fixtures
  python fetch_data_generic.py --type fixtures --league 140 --season 2022 --output "data/la-liga/fixtures_2022.csv"

  # Fetch World Cup 2022 squads (tournament mode)
  python fetch_data_generic.py --type squad --league 1 --season 2022 --output "data/world-cup/squads_2022.csv" --tournament

Common League IDs:
  39  - Premier League
  140 - La Liga
  135 - Serie A
  78  - Bundesliga
  61  - Ligue 1
  2   - Champions League
  1   - World Cup
  4   - European Championship
        """
    )

    parser.add_argument('--type', type=str, required=True, choices=['squad', 'fixtures'],
                        help='Type of data to scrape (squad or fixtures)')
    parser.add_argument('--league', type=int, required=True,
                        help='League ID (e.g., 39 for Premier League, 1 for World Cup)')
    parser.add_argument('--season', type=int, required=True,
                        help='Season year (e.g., 2023). Note: Free plans support 2022-2024')
    parser.add_argument('--output', type=str, required=True,
                        help='Output CSV file path')
    parser.add_argument('--tournament', action='store_true',
                        help='Use tournament format (for World Cup, Euros, etc.)')
    parser.add_argument('--sleep', type=int, default=3,
                        help='Sleep time between requests in seconds (default: 3)')

    args = parser.parse_args()

    # Validate API key
    if API_KEY == "YOUR_API_FOOTBALL_KEY_HERE":
        print("❌ ERROR: Please set your API-Football key in the script.")
        return

    # Execute scraping
    if args.type == 'squad':
        scrape_squads(args.league, args.season, args.output, args.tournament, args.sleep)
    elif args.type == 'fixtures':
        scrape_fixtures(args.league, args.season, args.output)

if __name__ == "__main__":
    main()
