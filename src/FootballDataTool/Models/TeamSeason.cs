namespace FootballDataTool.Models;

/// <summary>
/// Represents a team's complete season with aggregated data from all matches.
/// Built from match data to enable rich team-centric analysis.
/// </summary>
public class TeamSeason
{
    public string Name { get; set; } = string.Empty;
    
    // Match collections
    public List<Match> HomeMatches { get; set; } = new();
    public List<Match> AwayMatches { get; set; } = new();
    public List<Match> AllMatches => HomeMatches.Concat(AwayMatches).OrderBy(m => m.Gameweek).ToList();
    
    // Squad data (aggregated from lineups across all matches)
    public List<Player> FullSquad { get; set; } = new();
    public List<Manager> ManagerHistory { get; set; } = new();
    public Stadium? HomeStadium { get; set; }
    
    // Transfer data (optional - from JSON)
    public TeamSeasonInfo? SeasonInfo { get; set; }
    
    // Computed season statistics
    public int MatchesPlayed => HomeMatches.Count + AwayMatches.Count;
    public int HomeWins => HomeMatches.Count(m => m.Result == "H");
    public int HomeDraws => HomeMatches.Count(m => m.Result == "D");
    public int HomeLosses => HomeMatches.Count(m => m.Result == "A");
    public int AwayWins => AwayMatches.Count(m => m.Result == "A");
    public int AwayDraws => AwayMatches.Count(m => m.Result == "D");
    public int AwayLosses => AwayMatches.Count(m => m.Result == "H");
    
    public int TotalWins => HomeWins + AwayWins;
    public int TotalDraws => HomeDraws + AwayDraws;
    public int TotalLosses => HomeLosses + AwayLosses;
    public int TotalPoints => HomeMatches.Sum(m => m.HomePoints) + AwayMatches.Sum(m => m.AwayPoints);
    
    public int GoalsScored => HomeMatches.Sum(m => m.HomeGoals) + AwayMatches.Sum(m => m.AwayGoals);
    public int GoalsConceded => HomeMatches.Sum(m => m.AwayGoals) + AwayMatches.Sum(m => m.HomeGoals);
    public int GoalDifference => GoalsScored - GoalsConceded;
    
    // Average attendance (if data available)
    public double? AverageHomeAttendance => HomeMatches
        .Where(m => m.ExtendedData?.Attendance.HasValue == true)
        .Select(m => m.ExtendedData!.Attendance!.Value)
        .DefaultIfEmpty()
        .Average();
    
    // Squad statistics (if lineup data available)
    public double? AverageSquadAge => FullSquad.Where(p => p.Age.HasValue).Select(p => p.Age!.Value).DefaultIfEmpty().Average();
    public Player? OldestPlayer => FullSquad.Where(p => p.Age.HasValue).OrderByDescending(p => p.Age).FirstOrDefault();
    public Player? YoungestPlayer => FullSquad.Where(p => p.Age.HasValue).OrderBy(p => p.Age).FirstOrDefault();
    
    // Most used players (by appearances)
    public List<(Player Player, int Appearances)> MostUsedPlayers()
    {
        var playerAppearances = new Dictionary<string, (Player Player, int Count)>();
        
        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;
            
            var lineup = match.HomeTeam == Name 
                ? match.ExtendedData.HomeStartingLineup 
                : match.ExtendedData.AwayStartingLineup;
            
            foreach (var player in lineup)
            {
                if (!playerAppearances.ContainsKey(player.Name))
                    playerAppearances[player.Name] = (player, 0);
                
                playerAppearances[player.Name] = (player, playerAppearances[player.Name].Count + 1);
            }
        }
        
        return playerAppearances.Values
            .OrderByDescending(x => x.Count)
            .Select(x => (x.Player, x.Count))
            .ToList();
    }
    
    // Total minutes played by all players (if minutes data available)
    public int TotalMinutesPlayed()
    {
        var total = 0;
        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;

            var appearances = match.HomeTeam == Name 
                ? match.ExtendedData.HomeAppearances 
                : match.ExtendedData.AwayAppearances;

            total += appearances.Sum(a => a.MinutesPlayed);
        }
        return total;
    }

    /// <summary>
    /// Get top scorers for this team across the season.
    /// </summary>
    public List<(Player Player, int Goals)> TopScorers()
    {
        var playerGoals = new Dictionary<string, (Player Player, int Goals)>();

        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;

            var appearances = match.HomeTeam == Name 
                ? match.ExtendedData.HomeAppearances 
                : match.ExtendedData.AwayAppearances;

            foreach (var appearance in appearances.Where(a => a.Goals > 0))
            {
                if (!playerGoals.ContainsKey(appearance.Player.Name))
                    playerGoals[appearance.Player.Name] = (appearance.Player, 0);

                playerGoals[appearance.Player.Name] = (
                    appearance.Player, 
                    playerGoals[appearance.Player.Name].Goals + appearance.Goals
                );
            }
        }

        return playerGoals.Values
            .OrderByDescending(x => x.Goals)
            .ToList();
    }

    /// <summary>
    /// Get top assisters for this team across the season.
    /// </summary>
    public List<(Player Player, int Assists)> TopAssisters()
    {
        var playerAssists = new Dictionary<string, (Player Player, int Assists)>();

        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;

            var appearances = match.HomeTeam == Name 
                ? match.ExtendedData.HomeAppearances 
                : match.ExtendedData.AwayAppearances;

            foreach (var appearance in appearances.Where(a => a.Assists > 0))
            {
                if (!playerAssists.ContainsKey(appearance.Player.Name))
                    playerAssists[appearance.Player.Name] = (appearance.Player, 0);

                playerAssists[appearance.Player.Name] = (
                    appearance.Player, 
                    playerAssists[appearance.Player.Name].Assists + appearance.Assists
                );
            }
        }

        return playerAssists.Values
            .OrderByDescending(x => x.Assists)
            .ToList();
    }

    /// <summary>
    /// Get players with most goal contributions (goals + assists).
    /// </summary>
    public List<(Player Player, int Goals, int Assists, int Total)> TopContributors()
    {
        var playerStats = new Dictionary<string, (Player Player, int Goals, int Assists)>();

        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;

            var appearances = match.HomeTeam == Name 
                ? match.ExtendedData.HomeAppearances 
                : match.ExtendedData.AwayAppearances;

            foreach (var appearance in appearances)
            {
                if (!playerStats.ContainsKey(appearance.Player.Name))
                    playerStats[appearance.Player.Name] = (appearance.Player, 0, 0);

                var current = playerStats[appearance.Player.Name];
                playerStats[appearance.Player.Name] = (
                    appearance.Player,
                    current.Goals + appearance.Goals,
                    current.Assists + appearance.Assists
                );
            }
        }

        return playerStats.Values
            .Select(x => (x.Player, x.Goals, x.Assists, x.Goals + x.Assists))
            .OrderByDescending(x => x.Item4)
            .ToList();
    }

    // Total injuries across season
    public List<Injury> AllInjuries()
    {
        var injuries = new List<Injury>();
        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;
            
            var teamInjuries = match.HomeTeam == Name 
                ? match.ExtendedData.HomeInjuries 
                : match.ExtendedData.AwayInjuries;
            
            injuries.AddRange(teamInjuries);
        }
        return injuries.DistinctBy(i => (i.Player.Name, i.InjuryDate)).ToList();
    }
    
    // Form guide (last N matches)
    public List<string> FormGuide(int lastNMatches = 5)
    {
        return AllMatches
            .TakeLast(lastNMatches)
            .Select(m => m.HomeTeam == Name ? m.Result : ReverseResult(m.Result))
            .Select(r => r == "H" || r == "A" ? "W" : r == "D" ? "D" : "L")
            .ToList();
    }
    
    private static string ReverseResult(string result) => result switch
    {
        "H" => "A",
        "A" => "H",
        "D" => "D",
        _ => result
    };
    
    // Points progression by gameweek
    public List<(int Gameweek, int CumulativePoints)> PointsProgression()
    {
        var progression = new List<(int, int)>();
        var cumulative = 0;
        
        foreach (var match in AllMatches)
        {
            cumulative += match.HomeTeam == Name ? match.HomePoints : match.AwayPoints;
            progression.Add((match.Gameweek, cumulative));
        }
        
        return progression;
    }
    
    // Constructor
    public TeamSeason(string name, List<Match> allMatches)
    {
        Name = name;
        HomeMatches = allMatches.Where(m => m.HomeTeam == name).ToList();
        AwayMatches = allMatches.Where(m => m.AwayTeam == name).ToList();
        BuildSquadFromMatches();
        BuildManagerHistoryFromMatches();
        DetectHomeStadium();
    }
    
    private void BuildSquadFromMatches()
    {
        var players = new Dictionary<string, Player>();
        
        foreach (var match in AllMatches)
        {
            if (match.ExtendedData == null) continue;
            
            var lineup = match.HomeTeam == Name 
                ? match.ExtendedData.HomeStartingLineup.Concat(match.ExtendedData.HomeSubstitutes)
                : match.ExtendedData.AwayStartingLineup.Concat(match.ExtendedData.AwaySubstitutes);
            
            foreach (var player in lineup)
            {
                if (!players.ContainsKey(player.Name))
                    players[player.Name] = player;
            }
        }
        
        FullSquad = players.Values.ToList();
    }
    
    private void BuildManagerHistoryFromMatches()
    {
        var managers = new Dictionary<string, Manager>();
        
        foreach (var match in AllMatches.OrderBy(m => m.Date))
        {
            if (match.ExtendedData == null) continue;
            
            var managerDetails = match.HomeTeam == Name 
                ? match.ExtendedData.HomeManagerDetails 
                : match.ExtendedData.AwayManagerDetails;
            
            if (managerDetails != null && !managers.ContainsKey(managerDetails.Name))
                managers[managerDetails.Name] = managerDetails;
        }
        
        ManagerHistory = managers.Values.ToList();
    }
    
    private void DetectHomeStadium()
    {
        // Find most common home stadium
        var stadiumCounts = HomeMatches
            .Where(m => m.ExtendedData?.Stadium != null)
            .GroupBy(m => m.ExtendedData!.Stadium!.Name)
            .Select(g => new { Stadium = g.First().ExtendedData!.Stadium, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();
        
        HomeStadium = stadiumCounts?.Stadium;
    }
}
