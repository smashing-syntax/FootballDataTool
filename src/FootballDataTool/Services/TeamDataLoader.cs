using System.Text.Json;
using FootballDataTool.Models;

namespace FootballDataTool.Services;

/// <summary>
/// Service for loading team transfer and manager data from JSON files.
/// </summary>
public class TeamDataLoader
{
    /// <summary>
    /// Loads team season information from a JSON file.
    /// </summary>
    public static TeamSeasonInfo? LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        try
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<TeamSeasonInfo>(json, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading team data from {filePath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads all team season data from a directory.
    /// </summary>
    public static Dictionary<string, TeamSeasonInfo> LoadAllFromDirectory(string directory, string season)
    {
        var teamData = new Dictionary<string, TeamSeasonInfo>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(directory))
            return teamData;

        // Look for files matching pattern: *_season_transfers.json
        var pattern = $"*{season.Replace("/", "-")}*transfers.json";
        var files = Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var data = LoadFromJson(file);
            if (data != null && !string.IsNullOrWhiteSpace(data.TeamName))
            {
                teamData[data.TeamName] = data;
            }
        }

        return teamData;
    }

    /// <summary>
    /// Enriches match data with manager details from team season info.
    /// </summary>
    public static void EnrichMatchWithManagerData(Match match, Dictionary<string, TeamSeasonInfo> teamData)
    {
        if (match.ExtendedData == null)
            return;

        // Enrich home manager
        if (teamData.TryGetValue(match.HomeTeam, out var homeTeamData))
        {
            match.ExtendedData.HomeManagerDetails = GetManagerAtDate(homeTeamData, match.Date);
        }

        // Enrich away manager
        if (teamData.TryGetValue(match.AwayTeam, out var awayTeamData))
        {
            match.ExtendedData.AwayManagerDetails = GetManagerAtDate(awayTeamData, match.Date);
        }
    }

    /// <summary>
    /// Gets the manager for a team at a specific date (considering managerial changes).
    /// </summary>
    private static Manager? GetManagerAtDate(TeamSeasonInfo teamData, DateTime? matchDate)
    {
        if (!matchDate.HasValue)
            return teamData.CurrentManager ?? teamData.StartingManager;

        // Find the most recent managerial change before the match date
        var relevantChange = teamData.ManagerialChanges
            .Where(c => c.ChangeDate <= matchDate.Value)
            .OrderByDescending(c => c.ChangeDate)
            .FirstOrDefault();

        return relevantChange?.IncomingManager ?? teamData.StartingManager;
    }

    /// <summary>
    /// Gets transfer statistics for a team.
    /// </summary>
    public static TransferStatistics GetTransferStatistics(TeamSeasonInfo teamData)
    {
        var allSignings = teamData.SummerSignings.Concat(teamData.WinterSignings).ToList();
        var allDepartures = teamData.SummerDepartures.Concat(teamData.WinterDepartures).ToList();

        return new TransferStatistics
        {
            TotalSignings = allSignings.Count,
            TotalDepartures = allDepartures.Count,
            TotalSpent = allSignings.Where(t => t.Fee.HasValue).Sum(t => t.Fee!.Value),
            TotalReceived = allDepartures.Where(t => t.Fee.HasValue).Sum(t => t.Fee!.Value),
            NetSpend = teamData.NetSpend ?? 0,
            AverageAgeIn = GetAverageAge(allSignings.Select(t => t.PlayerAgeAtTransfer)),
            AverageAgeOut = GetAverageAge(allDepartures.Select(t => t.PlayerAgeAtTransfer)),
            MostExpensiveSigning = allSignings
                .Where(t => t.Fee.HasValue)
                .OrderByDescending(t => t.Fee)
                .FirstOrDefault(),
            HighestFeeReceived = allDepartures
                .Where(t => t.Fee.HasValue)
                .OrderByDescending(t => t.Fee)
                .FirstOrDefault()
        };
    }

    private static double? GetAverageAge(IEnumerable<int?> ages)
    {
        var validAges = ages.Where(a => a.HasValue).Select(a => a!.Value).ToList();
        return validAges.Any() ? validAges.Average() : null;
    }
}

/// <summary>
/// Summary statistics for transfer activity.
/// </summary>
public class TransferStatistics
{
    public int TotalSignings { get; set; }
    public int TotalDepartures { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalReceived { get; set; }
    public decimal NetSpend { get; set; }
    public double? AverageAgeIn { get; set; }
    public double? AverageAgeOut { get; set; }
    public Transfer? MostExpensiveSigning { get; set; }
    public Transfer? HighestFeeReceived { get; set; }

    public override string ToString()
    {
        var lines = new List<string>
        {
            $"Signings: {TotalSignings} | Departures: {TotalDepartures}",
            $"Spent: {TotalSpent:N0} | Received: {TotalReceived:N0}",
            $"Net Spend: {NetSpend:N0}"
        };

        if (AverageAgeIn.HasValue)
            lines.Add($"Avg Age In: {AverageAgeIn:F1}");

        if (AverageAgeOut.HasValue)
            lines.Add($"Avg Age Out: {AverageAgeOut:F1}");

        return string.Join(" | ", lines);
    }
}
