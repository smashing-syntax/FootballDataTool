using CsvHelper;
using CsvHelper.Configuration;
using FootballDataTool.Models;
using System.Globalization;

namespace FootballDataTool.Services;

/// <summary>
/// Service for loading squad/biographical data from CSV files.
/// Separates player biographies from match events for cleaner data architecture.
/// </summary>
public class SquadCsvLoader
{
    /// <summary>
    /// Loads squad data from CSV file.
    /// Returns dictionary of team name -> list of players with biographical data.
    /// </summary>
    public static Dictionary<string, List<Player>> LoadFromCsv(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Squad CSV file not found: {filePath}");

        var squadData = new Dictionary<string, List<Player>>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = csv.GetRecords<SquadCsvRecord>().ToList();

        // Group by team
        foreach (var group in records.GroupBy(r => r.Team, StringComparer.OrdinalIgnoreCase))
        {
            var players = group.Select(MapToPlayer).ToList();
            squadData[group.Key] = players;
        }

        return squadData;
    }

    /// <summary>
    /// Loads squad data and returns as dictionary keyed by season.
    /// Useful when loading multiple seasons.
    /// </summary>
    public static Dictionary<string, Dictionary<string, List<Player>>> LoadBySeasonFromCsv(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Squad CSV file not found: {filePath}");

        var seasonSquads = new Dictionary<string, Dictionary<string, List<Player>>>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = csv.GetRecords<SquadCsvRecord>().ToList();

        // Group by season, then by team
        foreach (var seasonGroup in records.GroupBy(r => r.Season, StringComparer.OrdinalIgnoreCase))
        {
            var teamSquads = new Dictionary<string, List<Player>>(StringComparer.OrdinalIgnoreCase);

            foreach (var teamGroup in seasonGroup.GroupBy(r => r.Team, StringComparer.OrdinalIgnoreCase))
            {
                var players = teamGroup.Select(MapToPlayer).ToList();
                teamSquads[teamGroup.Key] = players;
            }

            seasonSquads[seasonGroup.Key] = teamSquads;
        }

        return seasonSquads;
    }

    private static Player MapToPlayer(SquadCsvRecord record)
    {
        var player = new Player
        {
            Name = record.PlayerName,
            Position = record.Position,
            Nationality = record.Nationality,
            PreviousClub = record.PreviousClub
        };

        // Parse date of birth
        if (!string.IsNullOrWhiteSpace(record.DateOfBirth))
        {
            player.DateOfBirth = ParseDate(record.DateOfBirth);

            // Calculate current age (will be recalculated per match)
            if (player.DateOfBirth.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - player.DateOfBirth.Value.Year;
                if (player.DateOfBirth.Value.Date > today.AddYears(-age))
                    age--;
                player.Age = age;
            }
        }

        // Parse shirt number
        if (!string.IsNullOrWhiteSpace(record.ShirtNumber))
        {
            if (int.TryParse(record.ShirtNumber, out int number))
                player.ShirtNumber = number;
        }

        return player;
    }

    private static DateTime? ParseDate(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr))
            return null;

        // Try multiple date formats
        string[] formats = 
        [
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "dd-MM-yyyy",
            "yyyy/MM/dd"
        ];

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out DateTime result))
                return result;
        }

        // Fallback to standard parsing
        if (DateTime.TryParse(dateStr, out DateTime fallback))
            return fallback;

        return null;
    }

    /// <summary>
    /// Enriches a player object with data from squad records.
    /// Used to add biographical data to players parsed from match lineups.
    /// </summary>
    public static void EnrichPlayer(Player player, List<Player> squadPlayers, DateTime matchDate)
    {
        // Find matching player in squad data
        var squadPlayer = squadPlayers.FirstOrDefault(p => 
            p.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase));

        if (squadPlayer == null)
            return;

        // Enrich with biographical data (only if not already set)
        player.DateOfBirth ??= squadPlayer.DateOfBirth;
        player.Position ??= squadPlayer.Position;
        player.Nationality ??= squadPlayer.Nationality;
        player.PreviousClub ??= squadPlayer.PreviousClub;
        player.ShirtNumber ??= squadPlayer.ShirtNumber;

        // Calculate age at match date
        if (player.DateOfBirth.HasValue)
        {
            var age = matchDate.Year - player.DateOfBirth.Value.Year;
            if (player.DateOfBirth.Value.Date > matchDate.AddYears(-age))
                age--;
            player.Age = age;

            // Zodiac signs are automatically calculated from DateOfBirth via properties
        }
    }
}

/// <summary>
/// CSV record for squad data (mapped from CSV columns).
/// </summary>
public class SquadCsvRecord
{
    public string Team { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string ShirtNumber { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string PreviousClub { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty; // Not used yet, for future
    public string PreferredFoot { get; set; } = string.Empty; // Not used yet, for future
    public string JoinDate { get; set; } = string.Empty; // Not used yet, for future
}
