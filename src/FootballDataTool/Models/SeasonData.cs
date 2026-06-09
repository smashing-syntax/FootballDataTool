using FootballDataTool.Services;

namespace FootballDataTool.Models;

/// <summary>
/// Top-level container for all season data with team-level aggregation.
/// Provides both match-centric access (for simple analysis) and 
/// team-centric access (for rich analysis) to the same data.
/// </summary>
public class SeasonData
{
    /// <summary>
    /// Season and league metadata (auto-detected or provided).
    /// </summary>
    public SeasonMetadata Metadata { get; set; } = new();
    
    /// <summary>
    /// All matches in chronological order (match-centric view).
    /// This is the primary data structure - all other data is derived from this.
    /// </summary>
    public List<Match> Matches { get; set; } = new();
    
    /// <summary>
    /// Team-level aggregations built from matches (team-centric view).
    /// Keyed by team name for fast lookup.
    /// </summary>
    public Dictionary<string, TeamSeason> Teams { get; private set; } = new();
    
    /// <summary>
    /// Optional team transfer and manager data (from JSON/CSV files).
    /// Keyed by team name.
    /// </summary>
    public Dictionary<string, TeamSeasonInfo> TeamTransferData { get; set; } = new();

    /// <summary>
    /// Optional squad biographical data (from CSV files).
    /// Keyed by team name. Contains player birthdays, nationalities, etc.
    /// </summary>
    public Dictionary<string, List<Player>> SquadData { get; set; } = new();

    // Computed season-wide statistics
    public int TotalMatches => Matches.Count;
    public int TotalGameweeks => Matches.Max(m => m.Gameweek);
    public int TotalTeams => Teams.Count;
    public int TotalGoals => Matches.Sum(m => m.HomeGoals + m.AwayGoals);
    public double AverageGoalsPerMatch => TotalMatches > 0 ? (double)TotalGoals / TotalMatches : 0;
    
    public int HomeWins => Matches.Count(m => m.Result == "H");
    public int AwayWins => Matches.Count(m => m.Result == "A");
    public int Draws => Matches.Count(m => m.Result == "D");
    
    public double HomeWinPercentage => TotalMatches > 0 ? (double)HomeWins / TotalMatches * 100 : 0;
    public double AwayWinPercentage => TotalMatches > 0 ? (double)AwayWins / TotalMatches * 100 : 0;
    public double DrawPercentage => TotalMatches > 0 ? (double)Draws / TotalMatches * 100 : 0;
    
    // Extended data availability
    public bool HasLineupData => Matches.Any(m => m.HasLineupData);
    public bool HasInjuryData => Matches.Any(m => m.ExtendedData?.HomeInjuries.Count > 0);
    public bool HasMinutesData => Matches.Any(m => m.ExtendedData?.HomeAppearances.Count > 0);
    public bool HasAttendanceData => Matches.Any(m => m.HasAttendanceData);
    public bool HasTransferData => TeamTransferData.Any();
    public bool HasSquadData => SquadData.Any();

    /// <summary>
    /// Build team aggregations from match data.
    /// Call this after loading matches to enable team-centric analysis.
    /// </summary>
    public void BuildTeamAggregates()
    {
        var teamNames = Matches
            .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
            .Distinct()
            .OrderBy(name => name)
            .ToList();
        
        Teams = teamNames.ToDictionary(
            name => name,
            name => new TeamSeason(name, Matches)
        );
        
        // Link transfer data if available
        foreach (var (teamName, teamSeason) in Teams)
        {
            if (TeamTransferData.TryGetValue(teamName, out var transferData))
                teamSeason.SeasonInfo = transferData;
        }
    }
    
    /// <summary>
    /// Get a team by name (fast dictionary lookup).
    /// </summary>
    public TeamSeason? GetTeam(string teamName) => 
        Teams.TryGetValue(teamName, out var team) ? team : null;
    
    /// <summary>
    /// Get all matches involving a specific team.
    /// </summary>
    public List<Match> GetMatchesForTeam(string teamName) =>
        Matches.Where(m => m.HomeTeam == teamName || m.AwayTeam == teamName).ToList();
    
    /// <summary>
    /// Get head-to-head matches between two teams.
    /// </summary>
    public List<Match> GetHeadToHeadMatches(string team1, string team2) =>
        Matches.Where(m => 
            (m.HomeTeam == team1 && m.AwayTeam == team2) ||
            (m.HomeTeam == team2 && m.AwayTeam == team1)
        ).ToList();
    
    /// <summary>
    /// Get league table at a specific gameweek.
    /// </summary>
    public List<TeamRecord> GetTableAtGameweek(int gameweek)
    {
        var matchesUpToGW = Matches.Where(m => m.Gameweek <= gameweek).ToList();
        var analyzer = new MatchAnalyzer(matchesUpToGW);
        return analyzer.GetStandings();
    }

    /// <summary>
    /// Get current league table (all matches).
    /// </summary>
    public List<TeamRecord> GetCurrentTable()
    {
        var analyzer = new MatchAnalyzer(Matches);
        return analyzer.GetStandings();
    }
    
    /// <summary>
    /// Get teams ranked by a specific metric.
    /// </summary>
    public List<TeamSeason> GetTeamsRankedBy(Func<TeamSeason, int> metric, bool descending = true)
    {
        var query = Teams.Values.AsEnumerable();
        return descending 
            ? query.OrderByDescending(metric).ToList()
            : query.OrderBy(metric).ToList();
    }
    
    /// <summary>
    /// Get teams with most injuries across the season.
    /// </summary>
    public List<(string Team, int InjuryCount)> GetTeamsByInjuries()
    {
        return Teams.Values
            .Select(t => (t.Name, t.AllInjuries().Count))
            .OrderByDescending(x => x.Item2)
            .ToList();
    }
    
    /// <summary>
    /// Get players who had birthdays during matches.
    /// </summary>
    public List<(Player Player, Match Match)> GetMatchDayBirthdays()
    {
        var birthdays = new List<(Player, Match)>();
        
        foreach (var match in Matches.Where(m => m.Date.HasValue))
        {
            if (match.ExtendedData == null) continue;
            
            var birthdayPlayers = match.ExtendedData.PlayersWithBirthday(match.Date!.Value);
            foreach (var player in birthdayPlayers)
                birthdays.Add((player, match));
        }
        
        return birthdays;
    }
    
    /// <summary>
    /// Get all unique players across the season.
    /// </summary>
    public List<Player> GetAllPlayers()
    {
        return Teams.Values
            .SelectMany(t => t.FullSquad)
            .DistinctBy(p => p.Name)
            .ToList();
    }
    
    /// <summary>
    /// Get zodiac sign distribution across all players.
    /// </summary>
    public Dictionary<string, int> GetZodiacDistribution()
    {
        return GetAllPlayers()
            .Where(p => p.ZodiacSign != null)
            .GroupBy(p => p.ZodiacSign!)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Get top scorers across the entire season (all teams).
    /// </summary>
    public List<(Player Player, string Team, int Goals)> GetTopScorers(int count = 10)
    {
        var allScorers = Teams.Values
            .SelectMany(t => t.TopScorers().Select(s => (s.Player, t.Name, s.Goals)))
            .OrderByDescending(x => x.Goals)
            .Take(count)
            .ToList();

        return allScorers;
    }

    /// <summary>
    /// Get top assisters across the entire season (all teams).
    /// </summary>
    public List<(Player Player, string Team, int Assists)> GetTopAssisters(int count = 10)
    {
        var allAssisters = Teams.Values
            .SelectMany(t => t.TopAssisters().Select(a => (a.Player, t.Name, a.Assists)))
            .OrderByDescending(x => x.Assists)
            .Take(count)
            .ToList();

        return allAssisters;
    }

    /// <summary>
    /// Get players with most goal contributions (goals + assists) across the season.
    /// </summary>
    public List<(Player Player, string Team, int Goals, int Assists, int Total)> GetTopContributors(int count = 10)
    {
        var allContributors = Teams.Values
            .SelectMany(t => t.TopContributors().Select(c => (c.Player, t.Name, c.Goals, c.Assists, c.Total)))
            .OrderByDescending(x => x.Total)
            .Take(count)
            .ToList();

        return allContributors;
    }

    /// <summary>
    /// Factory method: Create SeasonData from matches.
    /// </summary>
    public static SeasonData FromMatches(List<Match> matches, SeasonMetadata metadata)
    {
        var seasonData = new SeasonData
        {
            Matches = matches,
            Metadata = metadata
        };

        // Link matches back to season for GetHomeTeamObject() / GetAwayTeamObject()
        foreach (var match in matches)
            match.ParentSeason = seasonData;

        seasonData.BuildTeamAggregates();
        return seasonData;
    }
    
    /// <summary>
    /// Load transfer data from JSON files for specific teams.
    /// </summary>
    public void LoadTransferData(params string[] jsonFilePaths)
    {
        foreach (var path in jsonFilePaths)
        {
            var teamInfo = TeamDataLoader.LoadFromJson(path);
            if (teamInfo != null)
                TeamTransferData[teamInfo.TeamName] = teamInfo;
        }

        // Re-link to team seasons
        foreach (var (teamName, teamSeason) in Teams)
        {
            if (TeamTransferData.TryGetValue(teamName, out var transferData))
                teamSeason.SeasonInfo = transferData;
        }
    }

    /// <summary>
    /// Load transfer data from CSV file (easier for users than JSON).
    /// CSV can contain transfers for multiple teams in one file.
    /// </summary>
    public void LoadTransferDataFromCsv(string csvFilePath)
    {
        var transferData = TransferCsvLoader.LoadFromCsv(csvFilePath);

        foreach (var (teamName, teamInfo) in transferData)
        {
            TeamTransferData[teamName] = teamInfo;
        }

        // Re-link to team seasons
        foreach (var (teamName, teamSeason) in Teams)
        {
            if (TeamTransferData.TryGetValue(teamName, out var transferInfo))
                teamSeason.SeasonInfo = transferInfo;
        }
    }

    /// <summary>
    /// Load squad biographical data from CSV file.
    /// Enriches all match lineups with player birthdays, ages, positions, etc.
    /// This automatically calculates ages per match based on birthdays!
    /// </summary>
    public void LoadSquadDataFromCsv(string csvFilePath)
    {
        SquadData = SquadCsvLoader.LoadFromCsv(csvFilePath);

        // Enrich all existing match lineups with squad data
        EnrichMatchesWithSquadData();
    }

    /// <summary>
    /// Enriches all matches with squad biographical data.
    /// Automatically calculates player ages based on match date and birthday.
    /// </summary>
    private void EnrichMatchesWithSquadData()
    {
        if (!SquadData.Any())
            return;

        foreach (var match in Matches)
        {
            if (match.ExtendedData == null || !match.Date.HasValue)
                continue;

            // Enrich home team players
            if (SquadData.TryGetValue(match.HomeTeam, out var homeSquad))
            {
                EnrichPlayerList(match.ExtendedData.HomeStartingLineup, homeSquad, match.Date.Value);
                EnrichPlayerList(match.ExtendedData.HomeSubstitutes, homeSquad, match.Date.Value);

                foreach (var appearance in match.ExtendedData.HomeAppearances)
                {
                    SquadCsvLoader.EnrichPlayer(appearance.Player, homeSquad, match.Date.Value);
                }
            }

            // Enrich away team players
            if (SquadData.TryGetValue(match.AwayTeam, out var awaySquad))
            {
                EnrichPlayerList(match.ExtendedData.AwayStartingLineup, awaySquad, match.Date.Value);
                EnrichPlayerList(match.ExtendedData.AwaySubstitutes, awaySquad, match.Date.Value);

                foreach (var appearance in match.ExtendedData.AwayAppearances)
                {
                    SquadCsvLoader.EnrichPlayer(appearance.Player, awaySquad, match.Date.Value);
                }
            }
        }

        // Rebuild team aggregates to pick up enriched data
        BuildTeamAggregates();
    }

    private static void EnrichPlayerList(List<Player> players, List<Player> squadData, DateTime matchDate)
    {
        foreach (var player in players)
        {
            SquadCsvLoader.EnrichPlayer(player, squadData, matchDate);
        }
    }

    // ========== TOURNAMENT-SPECIFIC QUERIES ==========

    /// <summary>
    /// Get all group stage matches.
    /// </summary>
    public List<Match> GetGroupStageMatches() =>
        Matches.Where(m => m.IsGroupStage).ToList();

    /// <summary>
    /// Get all knockout stage matches.
    /// </summary>
    public List<Match> GetKnockoutMatches() =>
        Matches.Where(m => m.IsKnockout).ToList();

    /// <summary>
    /// Get matches from a specific group.
    /// </summary>
    public List<Match> GetMatchesByGroup(string group) =>
        Matches.Where(m => m.Group != null && m.Group.Equals(group, StringComparison.OrdinalIgnoreCase)).ToList();

    /// <summary>
    /// Get matches from a specific tournament stage.
    /// </summary>
    public List<Match> GetMatchesByStage(TournamentStage stage) =>
        Matches.Where(m => m.Stage == stage).ToList();

    /// <summary>
    /// Get all groups present in the dataset.
    /// </summary>
    public List<string> GetAllGroups() =>
        Matches.Where(m => m.Group != null)
            .Select(m => m.Group!)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

    /// <summary>
    /// Get group standings (sorted by points, then goal difference).
    /// Only includes group stage matches from the specified group.
    /// </summary>
    public List<TeamRecord> GetGroupStandings(string group)
    {
        var groupMatches = GetMatchesByGroup(group);
        if (!groupMatches.Any())
            return new List<TeamRecord>();

        var analyzer = new MatchAnalyzer(groupMatches);
        return analyzer.GetStandings();
    }

    /// <summary>
    /// Get all two-legged ties (matches with Leg 1 or 2).
    /// Returns tuples of (first leg, second leg).
    /// </summary>
    public List<(Match FirstLeg, Match SecondLeg)> GetTwoLeggedTies()
    {
        var firstLegs = Matches.Where(m => m.Leg == 1).ToList();
        var secondLegs = Matches.Where(m => m.Leg == 2).ToList();

        var ties = new List<(Match FirstLeg, Match SecondLeg)>();

        foreach (var firstLeg in firstLegs)
        {
            var secondLeg = secondLegs.FirstOrDefault(sl =>
                sl.Stage == firstLeg.Stage &&
                ((sl.HomeTeam == firstLeg.AwayTeam && sl.AwayTeam == firstLeg.HomeTeam) ||
                 (sl.HomeTeam == firstLeg.HomeTeam && sl.AwayTeam == firstLeg.AwayTeam)));

            if (secondLeg != null)
                ties.Add((firstLeg, secondLeg));
        }

        return ties;
    }

    /// <summary>
    /// Calculate aggregate score for a two-legged tie.
    /// Returns (team1 total goals, team2 total goals).
    /// </summary>
    public (int Team1Goals, int Team2Goals) GetAggregateScore(Match firstLeg, Match secondLeg)
    {
        // Determine which team is "team1" and "team2"
        string team1 = firstLeg.HomeTeam;
        string team2 = firstLeg.AwayTeam;

        int team1Goals = firstLeg.HomeGoals;
        int team2Goals = firstLeg.AwayGoals;

        // Add second leg goals
        if (secondLeg.HomeTeam == team1)
        {
            team1Goals += secondLeg.HomeGoals;
            team2Goals += secondLeg.AwayGoals;
        }
        else
        {
            team1Goals += secondLeg.AwayGoals;
            team2Goals += secondLeg.HomeGoals;
        }

        return (team1Goals, team2Goals);
    }

    // ========== PHYSICAL STATS ANALYTICS ==========

    /// <summary>
    /// Get average height by position across all players.
    /// </summary>
    public Dictionary<string, double> GetAverageHeightByPosition()
    {
        return GetAllPlayers()
            .Where(p => p.Height.HasValue && !string.IsNullOrWhiteSpace(p.Position))
            .GroupBy(p => p.Position!)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(g.Average(p => p.Height!.Value), 1)
            );
    }

    /// <summary>
    /// Get average weight by position across all players.
    /// </summary>
    public Dictionary<string, double> GetAverageWeightByPosition()
    {
        return GetAllPlayers()
            .Where(p => p.Weight.HasValue && !string.IsNullOrWhiteSpace(p.Position))
            .GroupBy(p => p.Position!)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(g.Average(p => p.Weight!.Value), 1)
            );
    }

    /// <summary>
    /// Get average BMI by position across all players.
    /// </summary>
    public Dictionary<string, double> GetAverageBMIByPosition()
    {
        return GetAllPlayers()
            .Where(p => p.BMI.HasValue && !string.IsNullOrWhiteSpace(p.Position))
            .GroupBy(p => p.Position!)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(g.Average(p => p.BMI!.Value), 2)
            );
    }

    /// <summary>
    /// Get preferred foot distribution across all players.
    /// </summary>
    public Dictionary<string, int> GetPreferredFootDistribution()
    {
        return GetAllPlayers()
            .Where(p => !string.IsNullOrWhiteSpace(p.PreferredFoot))
            .GroupBy(p => p.PreferredFoot!)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Get height brackets analysis (e.g., which height ranges perform best).
    /// Groups players by 5cm brackets and returns goal/assist counts.
    /// </summary>
    public List<(string HeightBracket, int Players, int Goals, int Assists)> GetPerformanceByHeightBracket()
    {
        var results = new List<(string, int, int, int)>();

        var playersWithHeight = GetAllPlayers()
            .Where(p => p.Height.HasValue)
            .ToList();

        if (!playersWithHeight.Any())
            return results;

        var minHeight = playersWithHeight.Min(p => p.Height!.Value);
        var maxHeight = playersWithHeight.Max(p => p.Height!.Value);

        // Create 5cm brackets
        for (int h = (minHeight / 5) * 5; h <= maxHeight; h += 5)
        {
            var bracket = $"{h}-{h + 4}cm";
            var playersInBracket = playersWithHeight
                .Where(p => p.Height!.Value >= h && p.Height.Value < h + 5)
                .ToList();

            if (!playersInBracket.Any())
                continue;

            int totalGoals = 0;
            int totalAssists = 0;

            // Get goals and assists from team stats
            foreach (var team in Teams.Values)
            {
                var scorers = team.TopScorers();
                var assisters = team.TopAssisters();

                foreach (var player in playersInBracket)
                {
                    var scorer = scorers.FirstOrDefault(s => 
                        s.Player.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase));
                    if (scorer != default)
                        totalGoals += scorer.Goals;

                    var assister = assisters.FirstOrDefault(a => 
                        a.Player.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase));
                    if (assister != default)
                        totalAssists += assister.Assists;
                }
            }

            results.Add((bracket, playersInBracket.Count, totalGoals, totalAssists));
        }

        return results.OrderBy(r => int.Parse(r.Item1.Split('-')[0].Replace("cm", ""))).ToList();
    }

    /// <summary>
    /// Get tallest and shortest players with their stats.
    /// </summary>
    public (List<Player> Tallest, List<Player> Shortest) GetHeightExtremes(int count = 5)
    {
        var playersWithHeight = GetAllPlayers().Where(p => p.Height.HasValue).ToList();

        var tallest = playersWithHeight
            .OrderByDescending(p => p.Height!.Value)
            .Take(count)
            .ToList();

        var shortest = playersWithHeight
            .OrderBy(p => p.Height!.Value)
            .Take(count)
            .ToList();

        return (tallest, shortest);
    }
}
