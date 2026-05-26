using FootballDataTool.Models;

namespace FootballDataTool.Services;

public class MatchAnalyzer
{
    private readonly List<Match> _matches;

    public MatchAnalyzer(List<Match> matches)
    {
        _matches = matches ?? throw new ArgumentNullException(nameof(matches));
    }

    /// <summary>Returns all unique team names, sorted alphabetically.</summary>
    public List<string> GetTeams() =>
        _matches
            .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>Returns all gameweek numbers in ascending order.</summary>
    public List<int> GetGameweeks() =>
        _matches
            .Select(m => m.Gameweek)
            .Distinct()
            .OrderBy(gw => gw)
            .ToList();

    /// <summary>
    /// Returns the league standings at the end of the specified gameweek.
    /// If gameweek is 0 or negative, returns the final standings.
    /// </summary>
    public List<TeamRecord> GetStandings(int upToGameweek = 0)
    {
        var relevant = upToGameweek > 0
            ? _matches.Where(m => m.Gameweek <= upToGameweek)
            : _matches;

        var records = new Dictionary<string, TeamRecord>(StringComparer.OrdinalIgnoreCase);

        foreach (var team in GetTeams())
            records[team] = new TeamRecord { TeamName = team };

        foreach (var match in relevant)
        {
            records[match.HomeTeam].AddResult(match.HomeGoals, match.AwayGoals);
            records[match.AwayTeam].AddResult(match.AwayGoals, match.HomeGoals);
        }

        var standings = records.Values
            .OrderByDescending(r => r.Points)
            .ThenByDescending(r => r.GoalDifference)
            .ThenByDescending(r => r.GoalsFor)
            .ThenBy(r => r.TeamName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (int i = 0; i < standings.Count; i++)
            standings[i].Position = i + 1;

        return standings;
    }

    /// <summary>
    /// Returns home-only matches grouped by team, then by gameweek.
    /// Teams that had no home game in a given gameweek will have a null entry.
    /// </summary>
    public Dictionary<string, Dictionary<int, Match?>> GetHomeFormByGameweek()
    {
        var teams = GetTeams();
        var gameweeks = GetGameweeks();

        var result = new Dictionary<string, Dictionary<int, Match?>>(StringComparer.OrdinalIgnoreCase);

        foreach (var team in teams)
        {
            var gwMap = new Dictionary<int, Match?>();
            foreach (var gw in gameweeks)
                gwMap[gw] = null;

            foreach (var match in _matches.Where(m =>
                string.Equals(m.HomeTeam, team, StringComparison.OrdinalIgnoreCase)))
            {
                gwMap[match.Gameweek] = match;
            }

            result[team] = gwMap;
        }

        return result;
    }

    /// <summary>
    /// Returns cumulative points for every team after each gameweek.
    /// Result: team → list of (gameweek, cumulative points) pairs, sorted by gameweek.
    /// </summary>
    public Dictionary<string, List<(int Gameweek, int CumulativePoints)>> GetCumulativePoints()
    {
        var teams = GetTeams();
        var gameweeks = GetGameweeks();

        var running = teams.ToDictionary(t => t, _ => 0, StringComparer.OrdinalIgnoreCase);
        var result = teams.ToDictionary(t => t,
            _ => new List<(int, int)>(),
            StringComparer.OrdinalIgnoreCase);

        foreach (var gw in gameweeks)
        {
            var gwMatches = _matches.Where(m => m.Gameweek == gw);

            foreach (var match in gwMatches)
            {
                running[match.HomeTeam] += match.HomePoints;
                running[match.AwayTeam] += match.AwayPoints;
            }

            foreach (var team in teams)
                result[team].Add((gw, running[team]));
        }

        return result;
    }

    /// <summary>
    /// Returns a results matrix based on the requested view mode.
    /// The outer key is the attacking/row team, the inner key is the opponent/column team.
    /// The value is (goalsScored, goalsConceded) from the row-team's perspective.
    /// null means those two teams have not met in the given mode.
    /// </summary>
    public Dictionary<string, Dictionary<string, (int Scored, int Conceded)?>> GetResultMatrix(MatrixMode mode)
    {
        var teams = GetTeams();

        var matrix = new Dictionary<string, Dictionary<string, (int, int)?>>(StringComparer.OrdinalIgnoreCase);
        foreach (var team in teams)
        {
            matrix[team] = new Dictionary<string, (int, int)?>(StringComparer.OrdinalIgnoreCase);
            foreach (var opp in teams)
                matrix[team][opp] = null;
        }

        foreach (var match in _matches)
        {
            switch (mode)
            {
                case MatrixMode.Home:
                    // Row = home team, column = away team
                    matrix[match.HomeTeam][match.AwayTeam] = (match.HomeGoals, match.AwayGoals);
                    break;

                case MatrixMode.Away:
                    // Row = away team, column = home team
                    matrix[match.AwayTeam][match.HomeTeam] = (match.AwayGoals, match.HomeGoals);
                    break;

                case MatrixMode.Aggregate:
                    // Accumulate goals in both directions
                    AddToMatrix(matrix, match.HomeTeam, match.AwayTeam, match.HomeGoals, match.AwayGoals);
                    AddToMatrix(matrix, match.AwayTeam, match.HomeTeam, match.AwayGoals, match.HomeGoals);
                    break;
            }
        }

        return matrix;
    }

    /// <summary>
    /// Calculates how many points each team in <paramref name="group1"/> took against each team
    /// in <paramref name="group2"/>, and vice versa. Matches between teams in the same group
    /// are excluded.
    /// </summary>
    public CrossGroupResult GetCrossGroupPoints(IEnumerable<string> group1, IEnumerable<string> group2)
    {
        var g1 = new HashSet<string>(group1, StringComparer.OrdinalIgnoreCase);
        var g2 = new HashSet<string>(group2, StringComparer.OrdinalIgnoreCase);

        int g1Points = 0, g2Points = 0;
        int g1Goals = 0, g2Goals = 0;

        foreach (var match in _matches)
        {
            bool homeInG1 = g1.Contains(match.HomeTeam);
            bool awayInG1 = g1.Contains(match.AwayTeam);
            bool homeInG2 = g2.Contains(match.HomeTeam);
            bool awayInG2 = g2.Contains(match.AwayTeam);

            bool isCrossMatch = (homeInG1 && awayInG2) || (homeInG2 && awayInG1);
            if (!isCrossMatch) continue;

            if (homeInG1)
            {
                g1Points += match.HomePoints;
                g2Points += match.AwayPoints;
                g1Goals += match.HomeGoals;
                g2Goals += match.AwayGoals;
            }
            else
            {
                g2Points += match.HomePoints;
                g1Points += match.AwayPoints;
                g2Goals += match.HomeGoals;
                g1Goals += match.AwayGoals;
            }
        }

        return new CrossGroupResult
        {
            Group1Teams = g1.OrderBy(t => t).ToList(),
            Group2Teams = g2.OrderBy(t => t).ToList(),
            Group1Points = g1Points,
            Group2Points = g2Points,
            Group1Goals = g1Goals,
            Group2Goals = g2Goals
        };
    }

    /// <summary>
    /// Returns the top N teams by points in the final standings.
    /// </summary>
    public List<string> GetTopTeams(int count)
    {
        var standings = GetStandings();
        return standings.Take(count).Select(r => r.TeamName).ToList();
    }

    /// <summary>
    /// Returns the bottom N teams by points in the final standings.
    /// </summary>
    public List<string> GetBottomTeams(int count)
    {
        var standings = GetStandings();
        return standings.TakeLast(count).Select(r => r.TeamName).ToList();
    }

    private static void AddToMatrix(
        Dictionary<string, Dictionary<string, (int, int)?>> matrix,
        string rowTeam,
        string colTeam,
        int scored,
        int conceded)
    {
        var current = matrix[rowTeam][colTeam];
        matrix[rowTeam][colTeam] = current.HasValue
            ? (current.Value.Item1 + scored, current.Value.Item2 + conceded)
            : (scored, conceded);
    }
}

public enum MatrixMode
{
    Home,
    Away,
    Aggregate
}

public class CrossGroupResult
{
    public List<string> Group1Teams { get; set; } = [];
    public List<string> Group2Teams { get; set; } = [];
    public int Group1Points { get; set; }
    public int Group2Points { get; set; }
    public int Group1Goals { get; set; }
    public int Group2Goals { get; set; }
}
