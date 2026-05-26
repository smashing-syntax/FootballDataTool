using System.Text.RegularExpressions;
using FootballDataTool.Models;

namespace FootballDataTool.Services;

/// <summary>
/// Service for detecting and extracting season metadata from CSV data and filenames.
/// </summary>
public class MetadataDetectionService
{
    // League/Division mappings
    private static readonly Dictionary<string, (string League, string Country)> DivisionMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        // England
        ["E0"] = ("Premier League", "England"),
        ["E1"] = ("Championship", "England"),
        ["E2"] = ("League One", "England"),
        ["E3"] = ("League Two", "England"),
        ["EPL"] = ("Premier League", "England"),
        ["PL"] = ("Premier League", "England"),
        
        // Spain
        ["SP1"] = ("La Liga", "Spain"),
        ["SP2"] = ("La Liga 2", "Spain"),
        ["LaLiga"] = ("La Liga", "Spain"),
        
        // Italy
        ["I1"] = ("Serie A", "Italy"),
        ["I2"] = ("Serie B", "Italy"),
        ["SerieA"] = ("Serie A", "Italy"),
        
        // Germany
        ["D1"] = ("Bundesliga", "Germany"),
        ["D2"] = ("2. Bundesliga", "Germany"),
        
        // France
        ["F1"] = ("Ligue 1", "France"),
        ["F2"] = ("Ligue 2", "France"),
        
        // Scotland
        ["SC0"] = ("Premiership", "Scotland"),
        ["SC1"] = ("Championship", "Scotland"),
        
        // Netherlands
        ["N1"] = ("Eredivisie", "Netherlands"),
        
        // Portugal
        ["P1"] = ("Primeira Liga", "Portugal"),
        
        // Belgium
        ["B1"] = ("Pro League", "Belgium"),
        
        // Turkey
        ["T1"] = ("Süper Lig", "Turkey"),
        
        // Greece
        ["G1"] = ("Super League", "Greece"),
    };

    private static readonly Regex SeasonRangeRegex = new(@"\b(20\d{2})[-/]?(20\d{2}|\d{2})\b", RegexOptions.Compiled);
    private static readonly Regex SingleYearRegex = new(@"\b(20\d{2})\b", RegexOptions.Compiled);

    /// <summary>
    /// Detects season metadata from CSV records and optional filename.
    /// </summary>
    public static SeasonMetadata DetectMetadata(List<CsvMatchRecord> records, string? filename = null)
    {
        var metadata = new SeasonMetadata();

        // Extract division/league information
        var divisionField = records.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Division))?.Division;
        if (!string.IsNullOrWhiteSpace(divisionField))
        {
            if (DivisionMappings.TryGetValue(divisionField, out var leagueInfo))
            {
                metadata.League = leagueInfo.League;
                metadata.Country = leagueInfo.Country;
            }
            else
            {
                metadata.League = divisionField;
            }
        }

        // Extract season from data
        var seasonField = records.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Season))?.Season;
        if (!string.IsNullOrWhiteSpace(seasonField))
        {
            metadata.Season = NormalizeSeason(seasonField);
        }

        // Parse dates to determine date range
        var dates = records
            .Select(r => ParseDate(r.Date))
            .Where(d => d.HasValue)
            .Select(d => d!.Value)
            .ToList();

        if (dates.Any())
        {
            metadata.DateRange = (dates.Min(), dates.Max());

            // Infer season from date range if not already set
            if (metadata.Season == "Unknown" && dates.Any())
            {
                metadata.Season = InferSeasonFromDates(dates.Min(), dates.Max());
            }
        }

        // Try to extract metadata from filename
        if (!string.IsNullOrWhiteSpace(filename))
        {
            EnrichFromFilename(metadata, filename);
        }

        // Fallback: detect league from team names
        if (metadata.League == "Unknown" && metadata.Country == "Unknown")
        {
            DetectLeagueFromTeams(metadata, records);
        }

        metadata.TotalMatches = records.Count;

        return metadata;
    }

    /// <summary>
    /// Normalizes various season formats to a consistent format (e.g., "2023/24").
    /// </summary>
    private static string NormalizeSeason(string season)
    {
        // Match formats like "2023-24", "2023/24", "202324"
        var rangeMatch = SeasonRangeRegex.Match(season);
        if (rangeMatch.Success)
        {
            var year1 = rangeMatch.Groups[1].Value;
            var year2 = rangeMatch.Groups[2].Value;
            
            // Convert 2-digit year to 4-digit if needed
            if (year2.Length == 2)
            {
                var century = year1[..2];
                year2 = century + year2;
            }

            return $"{year1}/{year2[2..]}";
        }

        // Single year format
        var singleMatch = SingleYearRegex.Match(season);
        if (singleMatch.Success)
        {
            return singleMatch.Groups[1].Value;
        }

        return season;
    }

    /// <summary>
    /// Infers the season from a date range (football seasons typically run Aug-May).
    /// </summary>
    private static string InferSeasonFromDates(DateTime start, DateTime end)
    {
        // Football seasons typically start in August and end in May
        // If start is in second half of year, season is startYear/nextYear
        var startYear = start.Month >= 7 ? start.Year : start.Year - 1;
        var endYear = startYear + 1;

        return $"{startYear}/{endYear.ToString()[2..]}";
    }

    /// <summary>
    /// Attempts to extract season and league information from the filename.
    /// </summary>
    private static void EnrichFromFilename(SeasonMetadata metadata, string filename)
    {
        var name = Path.GetFileNameWithoutExtension(filename);

        // Try to extract season
        if (metadata.Season == "Unknown")
        {
            var seasonMatch = SeasonRangeRegex.Match(name);
            if (seasonMatch.Success)
            {
                metadata.Season = NormalizeSeason(seasonMatch.Value);
            }
            else
            {
                var yearMatch = SingleYearRegex.Match(name);
                if (yearMatch.Success)
                {
                    metadata.Season = yearMatch.Groups[1].Value;
                }
            }
        }

        // Try to extract division code
        if (metadata.League == "Unknown")
        {
            foreach (var kvp in DivisionMappings)
            {
                if (name.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.League = kvp.Value.League;
                    metadata.Country = kvp.Value.Country;
                    break;
                }
            }
        }

        // Try to extract league name directly
        if (metadata.League == "Unknown")
        {
            var leagueNames = DivisionMappings.Values.Select(v => v.League).Distinct();
            foreach (var league in leagueNames)
            {
                if (name.Contains(league.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                {
                    metadata.League = league;
                    metadata.Country = DivisionMappings.Values.First(v => v.League == league).Country;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Attempts to detect the league from well-known team names.
    /// </summary>
    private static void DetectLeagueFromTeams(SeasonMetadata metadata, List<CsvMatchRecord> records)
    {
        var allTeams = records
            .SelectMany(r => new[] { r.HomeTeam, r.AwayTeam })
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Premier League teams
        var plTeams = new[] { "Arsenal", "Chelsea", "Liverpool", "Man City", "Man Utd", "Manchester City", 
            "Manchester United", "Tottenham", "Spurs", "Everton", "Newcastle", "Aston Villa" };
        
        if (plTeams.Count(t => allTeams.Contains(t)) >= 3)
        {
            metadata.League = "Premier League";
            metadata.Country = "England";
            return;
        }

        // La Liga teams
        var laLigaTeams = new[] { "Barcelona", "Real Madrid", "Atletico Madrid", "Valencia", "Sevilla", "Villarreal" };
        if (laLigaTeams.Count(t => allTeams.Contains(t)) >= 3)
        {
            metadata.League = "La Liga";
            metadata.Country = "Spain";
            return;
        }

        // Serie A teams
        var serieATeams = new[] { "Juventus", "Inter", "AC Milan", "Roma", "Napoli", "Lazio" };
        if (serieATeams.Count(t => allTeams.Contains(t)) >= 3)
        {
            metadata.League = "Serie A";
            metadata.Country = "Italy";
            return;
        }

        // Bundesliga teams
        var bundesligaTeams = new[] { "Bayern Munich", "Borussia Dortmund", "RB Leipzig", "Leverkusen", "Wolfsburg" };
        if (bundesligaTeams.Count(t => allTeams.Contains(t)) >= 3)
        {
            metadata.League = "Bundesliga";
            metadata.Country = "Germany";
        }
    }

    private static DateTime? ParseDate(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr))
            return null;

        var formats = new[]
        {
            "dd/MM/yyyy", "dd/MM/yy", "yyyy-MM-dd", "MM/dd/yyyy", 
            "d/M/yyyy", "d/M/yy", "yyyy/MM/dd", "dd-MM-yyyy"
        };

        if (DateTime.TryParseExact(dateStr.Trim(), formats, 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        if (DateTime.TryParse(dateStr.Trim(), out date))
        {
            return date;
        }

        return null;
    }
}
