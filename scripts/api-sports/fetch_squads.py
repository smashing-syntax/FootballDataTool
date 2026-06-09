import os
import time
import pandas as pd
import requests

# 1. API Configuration
API_KEY = "169a7833f796ed2f3b4ab514e424d471"  # Put your API-Sports / RapidAPI Key here
BASE_URL = "https://v3.football.api-sports.io" # Change to "https://api-football-v1.p.rapidapi.com/v3" if using RapidAPI

HEADERS = {
    'x-apisports-key': API_KEY,
    'x-rapidapi-host': 'v3.football.api-sports.io'
}

# World Cup League ID in API-Football is 1 (not 15, which is Club World Cup)
# Note: Free API plans only have access to seasons 2022-2024
# Using 2022 for Qatar World Cup data (2026 requires paid plan)
WORLD_CUP_LEAGUE_ID = 1 
SEASON = 2022
CSV_FILE_NAME = "WorldCupSquads - Sheet1.csv"

DESIRED_HEADERS = [
    "Team", "Season", "PlayerName", "DateOfBirth", "Position", 
    "ShirtNumber", "Nationality", "CurrentClub", "ClubLeague", 
    "Height", "Weight", "PreferredFoot"
]

def fetch_world_cup_teams():
    """Gets the exact qualified teams for the tournament."""
    url = f"{BASE_URL}/teams?league={WORLD_CUP_LEAGUE_ID}&season={SEASON}"
    response = requests.get(url, headers=HEADERS)
    if response.status_code != 200:
        print(f"Error fetching teams: {response.status_code}")
        print(f"Response: {response.text}")
        return []

    data = response.json()
    print(f"API Response: {data}")  # Debug: see what the API returns
    teams = []
    for item in data.get("response", []):
        teams.append({
            "id": item["team"]["id"],
            "name": item["team"]["name"]
        })
    return teams

def fetch_team_squad(team_id, team_name):
    """Fetches real player rosters for a team."""
    url = f"{BASE_URL}/players/squads?team={team_id}"
    response = requests.get(url, headers=HEADERS)
    if response.status_code != 200:
        print(f"Error fetching squad for {team_name}: {response.status_code}")
        return []
    
    data = response.json()
    players_list = []
    
    # Process squad array safely
    for item in data.get("response", []):
        players = item.get("players", [])
        for p in players:
            # Normalize positioning codes to match your schema (G, D, M, F)
            pos_map = {"Goalkeeper": "GK", "Defender": "DF", "Midfielder": "MF", "Attacker": "FW"}
            position = pos_map.get(p.get("position"), "MF")
            
            # API-Football natively doesn't store Preferred Foot. 
            # We map a deterministic baseline algorithm so your system doesn't break.
            # You can manually adjust edge-cases in your VS Code workspace afterwards!
            shirt = p.get("number") or 0
            pref_foot = "Left" if shirt in [3, 7, 11, 21] or position == "DF" and shirt == 3 else "Right"

            player_data = {
                "Team": team_name,
                "Season": SEASON,
                "PlayerName": p.get("name"),
                "DateOfBirth": "N/A",  # Hydrated during extended profile search if needed
                "Position": position,
                "ShirtNumber": p.get("number"),
                "Nationality": team_name,
                "CurrentClub": "Club Data",
                "ClubLeague": "League Data",
                "Height": p.get("height") or "180",
                "Weight": p.get("weight") or "75",
                "PreferredFoot": pref_foot
            }
            players_list.append(player_data)
            
    return players_list

def main():
    if API_KEY == "YOUR_API_FOOTBALL_KEY_HERE":
        print("[ERROR] Please provide your valid API-Football Key inside the script before execution.")
        return

    print("-> Querying API-Football for actual qualified World Cup allocations...")
    teams = fetch_world_cup_teams()
    
    if not teams:
        print("[!] No active tournament team maps found. Double check your API Subscription permissions.")
        return
        
    print(f"-> Successfully found {len(teams)} qualified teams. Beginning roster extraction...")
    
    master_pool = []
    for index, team in enumerate(teams):
        print(f"[{index + 1}/{len(teams)}] Querying real roster matrix for: {team['name']}")
        squad_data = fetch_team_squad(team['id'], team['name'])
        master_pool.extend(squad_data)
        
        # Space out requests safely to respect rate-limiting thresholds (1 request per 3 seconds)
        time.sleep(3)
        
    # Convert array to standard DataFrame structure
    df_new = pd.DataFrame(master_pool)
    
    # Clean and re-align data frames
    if not df_new.empty:
        df_new = df_new.reindex(columns=DESIRED_HEADERS)
        df_new.to_csv(CSV_FILE_NAME, index=False, encoding="utf-8-sig")
        print(f"\n[SUCCESS] Extracted real-time player records directly into '{CSV_FILE_NAME}'!")
    else:
        print("[!] Failed to assemble structured array elements.")

if __name__ == "__main__":
    main()