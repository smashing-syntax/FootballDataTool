using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FootballDataTool.Models;

namespace FootballDataTool.Services;

public class CsvDataService
{
    // Column name aliases supported for flexible header mapping
    private static readonly string[] GameweekColumns = ["GW", "Gameweek", "Round", "Wk", "Week", "Matchday"];
    private static readonly string[] HomeTeamColumns = ["HomeTeam", "Home Team", "Home", "HTeam", "HOMETEAM"];
    private static readonly string[] AwayTeamColumns = ["AwayTeam", "Away Team", "Away", "ATeam", "AWAYTEAM"];
    private static readonly string[] HomeGoalsColumns = ["FTHG", "HomeGoals", "HG", "Home Goals", "HGoals", "FullTimeHomeGoals"];
    private static readonly string[] AwayGoalsColumns = ["FTAG", "AwayGoals", "AG", "Away Goals", "AGoals", "FullTimeAwayGoals"];
    private static readonly string[] DateColumns = ["Date", "MatchDate", "Match Date"];

    public List<Match> LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        using var reader = new StreamReader(filePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.Trim(),
            MissingFieldFound = null,
            BadDataFound = null
        };
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord!
            .Select(h => h.Trim())
            .ToArray();

        // Resolve column indices
        int gameweekIdx = FindColumnIndex(headers, GameweekColumns);
        int homeTeamIdx = FindColumnIndex(headers, HomeTeamColumns);
        int awayTeamIdx = FindColumnIndex(headers, AwayTeamColumns);
        int homeGoalsIdx = FindColumnIndex(headers, HomeGoalsColumns);
        int awayGoalsIdx = FindColumnIndex(headers, AwayGoalsColumns);
        int dateIdx = FindColumnIndex(headers, DateColumns);

        if (homeTeamIdx < 0 || awayTeamIdx < 0)
            throw new InvalidDataException(
                "CSV must contain home team and away team columns. " +
                $"Supported names: [{string.Join(", ", HomeTeamColumns)}] and [{string.Join(", ", AwayTeamColumns)}].");

        if (homeGoalsIdx < 0 || awayGoalsIdx < 0)
            throw new InvalidDataException(
                "CSV must contain home goals and away goals columns. " +
                $"Supported names: [{string.Join(", ", HomeGoalsColumns)}] and [{string.Join(", ", AwayGoalsColumns)}].");

        var matches = new List<Match>();

        while (csv.Read())
        {
            var homeTeam = csv.GetField(homeTeamIdx)?.Trim() ?? string.Empty;
            var awayTeam = csv.GetField(awayTeamIdx)?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(homeTeam) || string.IsNullOrWhiteSpace(awayTeam))
                continue;

            if (!TryParseInt(csv.GetField(homeGoalsIdx), out int homeGoals) ||
                !TryParseInt(csv.GetField(awayGoalsIdx), out int awayGoals))
                continue;

            var match = new Match
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeGoals = homeGoals,
                AwayGoals = awayGoals
            };

            if (gameweekIdx >= 0 && TryParseInt(csv.GetField(gameweekIdx), out int gw))
                match.Gameweek = gw;

            if (dateIdx >= 0)
                match.Date = TryParseDate(csv.GetField(dateIdx));

            matches.Add(match);
        }

        if (matches.Count == 0)
            throw new InvalidDataException("No valid match rows found in the CSV file.");

        // Assign gameweeks if not provided in the data
        bool hasGameweeks = matches.Any(m => m.Gameweek > 0);
        if (!hasGameweeks)
            AssignGameweeks(matches);

        return matches;
    }

    /// <summary>
    /// Assigns gameweek numbers using a greedy round-robin algorithm based on match dates.
    /// When dates are not available, matches are grouped in order so each team appears once per gameweek.
    /// </summary>
    public static void AssignGameweeks(List<Match> matches)
    {
        // Sort by date if available, otherwise keep file order
        bool hasDates = matches.Any(m => m.Date.HasValue);
        var sorted = hasDates
            ? matches.OrderBy(m => m.Date).ThenBy(m => m.HomeTeam).ToList()
            : matches;

        int gameweek = 1;
        var teamsInCurrentGw = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var match in sorted)
        {
            if (teamsInCurrentGw.Contains(match.HomeTeam) || teamsInCurrentGw.Contains(match.AwayTeam))
            {
                gameweek++;
                teamsInCurrentGw.Clear();
            }

            match.Gameweek = gameweek;
            teamsInCurrentGw.Add(match.HomeTeam);
            teamsInCurrentGw.Add(match.AwayTeam);
        }
    }

    private static int FindColumnIndex(string[] headers, string[] aliases)
    {
        foreach (var alias in aliases)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (string.Equals(headers[i], alias, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
        }
        return -1;
    }

    private static bool TryParseInt(string? value, out int result)
    {
        result = 0;
        return !string.IsNullOrWhiteSpace(value) && int.TryParse(value.Trim(), out result);
    }

    private static DateTime? TryParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        string[] formats = ["dd/MM/yyyy", "dd/MM/yy", "yyyy-MM-dd", "MM/dd/yyyy", "d/M/yyyy", "d/M/yy"];
        if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt;

        if (DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return dt;

        return null;
    }
}
