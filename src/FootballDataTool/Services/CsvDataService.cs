using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FootballDataTool.Models;

namespace FootballDataTool.Services;

public class CsvDataService
{
    // Column name aliases for flexible header mapping - Core fields
    private static readonly string[] DivisionColumns = ["Div", "Division", "League", "Comp", "Competition"];
    private static readonly string[] SeasonColumns = ["Season"];
    private static readonly string[] GameweekColumns = ["GW", "Gameweek", "Round", "Wk", "Week", "Matchday"];
    private static readonly string[] HomeTeamColumns = ["HomeTeam", "Home Team", "Home", "HTeam", "HOMETEAM"];
    private static readonly string[] AwayTeamColumns = ["AwayTeam", "Away Team", "Away", "ATeam", "AWAYTEAM"];
    private static readonly string[] HomeGoalsColumns = ["FTHG", "HomeGoals", "HG", "Home Goals", "HGoals", "FullTimeHomeGoals"];
    private static readonly string[] AwayGoalsColumns = ["FTAG", "AwayGoals", "AG", "Away Goals", "AGoals", "FullTimeAwayGoals"];
    private static readonly string[] DateColumns = ["Date", "MatchDate", "Match Date"];
    private static readonly string[] TimeColumns = ["Time", "KickOff", "Kick Off"];
    private static readonly string[] ResultColumns = ["FTR", "Res", "Result", "FullTimeResult"];
    private static readonly string[] RefereeColumns = ["Referee", "Ref"];

    // Extended field columns
    private static readonly string[] HomeManagerColumns = ["HomeManager", "Home Manager", "HManager"];
    private static readonly string[] AwayManagerColumns = ["AwayManager", "Away Manager", "AManager"];
    private static readonly string[] HomeFormationColumns = ["HomeFormation", "Home Formation", "HForm"];
    private static readonly string[] AwayFormationColumns = ["AwayFormation", "Away Formation", "AForm"];
    private static readonly string[] StadiumColumns = ["Stadium", "Venue", "Ground"];
    private static readonly string[] StadiumCapacityColumns = ["StadiumCapacity", "Capacity", "VenueCapacity"];
    private static readonly string[] AttendanceColumns = ["Attendance", "Crowd"];
    private static readonly string[] HomeGoalscorersColumns = ["HomeGoalscorers", "Home Goalscorers", "HGoalscorers"];
    private static readonly string[] AwayGoalscorersColumns = ["AwayGoalscorers", "Away Goalscorers", "AGoalscorers"];
    private static readonly string[] HomeLineupColumns = ["HomeLineup", "Home Lineup", "HomeXI", "HLineup"];
    private static readonly string[] AwayLineupColumns = ["AwayLineup", "Away Lineup", "AwayXI", "ALineup"];
    private static readonly string[] HomeSubstitutesColumns = ["HomeSubstitutes", "Home Subs", "HSubs"];
    private static readonly string[] AwaySubstitutesColumns = ["AwaySubstitutes", "Away Subs", "ASubs"];
    private static readonly string[] HomeSubstitutionsColumns = ["HomeSubstitutions", "Home Substitutions"];
    private static readonly string[] AwaySubstitutionsColumns = ["AwaySubstitutions", "Away Substitutions"];
    private static readonly string[] HomeYellowCardsColumns = ["HomeYellowCards", "HY", "Home Yellow"];
    private static readonly string[] AwayYellowCardsColumns = ["AwayYellowCards", "AY", "Away Yellow"];
    private static readonly string[] HomeRedCardsColumns = ["HomeRedCards", "HR", "Home Red"];
    private static readonly string[] AwayRedCardsColumns = ["AwayRedCards", "AR", "Away Red"];
    private static readonly string[] AssistantReferee1Columns = ["AR1", "AssistantReferee1", "Assistant1"];
    private static readonly string[] AssistantReferee2Columns = ["AR2", "AssistantReferee2", "Assistant2"];
    private static readonly string[] FourthOfficialColumns = ["FourthOfficial", "4thOfficial", "FO"];
    private static readonly string[] VarRefereeColumns = ["VAR", "VarReferee", "VideoReferee"];
    private static readonly string[] TemperatureColumns = ["Temperature", "Temp"];
    private static readonly string[] WeatherConditionsColumns = ["Weather", "WeatherConditions", "Conditions"];
    private static readonly string[] OtherCompetitionsColumns = ["OtherCompetitions", "OtherFixtures", "MidweekFixtures"];

    public SeasonMetadata? Metadata { get; private set; }

    /// <summary>
    /// Loads match data from a CSV file and automatically detects season/league metadata.
    /// </summary>
    public List<Match> LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        // Step 1: Load raw CSV records
        var csvRecords = LoadCsvRecords(filePath);

        if (csvRecords.Count == 0)
            throw new InvalidDataException("No valid records found in the CSV file.");

        // Step 2: Detect metadata
        Metadata = MetadataDetectionService.DetectMetadata(csvRecords, filePath);

        // Step 3: Transform to Match objects
        var matches = TransformToMatches(csvRecords);

        // Step 4: Assign gameweeks if not provided
        bool hasGameweeks = matches.Any(m => m.Gameweek > 0);
        if (!hasGameweeks)
            AssignGameweeks(matches);

        return matches;
    }

    /// <summary>
    /// Loads raw CSV records with dynamic column mapping.
    /// </summary>
    private List<CsvMatchRecord> LoadCsvRecords(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.Trim(),
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null
        };
        using var csv = new CsvReader(reader, config);

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord!
            .Select(h => h.Trim())
            .ToArray();

        // Resolve column indices for all possible fields
        var columnMap = new Dictionary<string, int>
        {
            // Core fields
            ["Division"] = FindColumnIndex(headers, DivisionColumns),
            ["Season"] = FindColumnIndex(headers, SeasonColumns),
            ["Gameweek"] = FindColumnIndex(headers, GameweekColumns),
            ["HomeTeam"] = FindColumnIndex(headers, HomeTeamColumns),
            ["AwayTeam"] = FindColumnIndex(headers, AwayTeamColumns),
            ["HomeGoals"] = FindColumnIndex(headers, HomeGoalsColumns),
            ["AwayGoals"] = FindColumnIndex(headers, AwayGoalsColumns),
            ["Date"] = FindColumnIndex(headers, DateColumns),
            ["Time"] = FindColumnIndex(headers, TimeColumns),
            ["Result"] = FindColumnIndex(headers, ResultColumns),
            ["Referee"] = FindColumnIndex(headers, RefereeColumns),

            // Extended fields - all optional
            ["HomeManager"] = FindColumnIndex(headers, HomeManagerColumns),
            ["AwayManager"] = FindColumnIndex(headers, AwayManagerColumns),
            ["HomeFormation"] = FindColumnIndex(headers, HomeFormationColumns),
            ["AwayFormation"] = FindColumnIndex(headers, AwayFormationColumns),
            ["Stadium"] = FindColumnIndex(headers, StadiumColumns),
            ["StadiumCapacity"] = FindColumnIndex(headers, StadiumCapacityColumns),
            ["Attendance"] = FindColumnIndex(headers, AttendanceColumns),
            ["HomeGoalscorers"] = FindColumnIndex(headers, HomeGoalscorersColumns),
            ["AwayGoalscorers"] = FindColumnIndex(headers, AwayGoalscorersColumns),
            ["HomeLineup"] = FindColumnIndex(headers, HomeLineupColumns),
            ["AwayLineup"] = FindColumnIndex(headers, AwayLineupColumns),
            ["HomeSubstitutes"] = FindColumnIndex(headers, HomeSubstitutesColumns),
            ["AwaySubstitutes"] = FindColumnIndex(headers, AwaySubstitutesColumns),
            ["HomeSubstitutions"] = FindColumnIndex(headers, HomeSubstitutionsColumns),
            ["AwaySubstitutions"] = FindColumnIndex(headers, AwaySubstitutionsColumns),
            ["HomeYellowCards"] = FindColumnIndex(headers, HomeYellowCardsColumns),
            ["AwayYellowCards"] = FindColumnIndex(headers, AwayYellowCardsColumns),
            ["HomeRedCards"] = FindColumnIndex(headers, HomeRedCardsColumns),
            ["AwayRedCards"] = FindColumnIndex(headers, AwayRedCardsColumns),
            ["AssistantReferee1"] = FindColumnIndex(headers, AssistantReferee1Columns),
            ["AssistantReferee2"] = FindColumnIndex(headers, AssistantReferee2Columns),
            ["FourthOfficial"] = FindColumnIndex(headers, FourthOfficialColumns),
            ["VarReferee"] = FindColumnIndex(headers, VarRefereeColumns),
            ["Temperature"] = FindColumnIndex(headers, TemperatureColumns),
            ["WeatherConditions"] = FindColumnIndex(headers, WeatherConditionsColumns),
            ["OtherCompetitions"] = FindColumnIndex(headers, OtherCompetitionsColumns)
        };

        // Validate required columns
        if (columnMap["HomeTeam"] < 0 || columnMap["AwayTeam"] < 0)
            throw new InvalidDataException(
                "CSV must contain home team and away team columns. " +
                $"Supported: [{string.Join(", ", HomeTeamColumns)}] and [{string.Join(", ", AwayTeamColumns)}].");

        if (columnMap["HomeGoals"] < 0 || columnMap["AwayGoals"] < 0)
            throw new InvalidDataException(
                "CSV must contain home goals and away goals columns. " +
                $"Supported: [{string.Join(", ", HomeGoalsColumns)}] and [{string.Join(", ", AwayGoalsColumns)}].");

        var records = new List<CsvMatchRecord>();

        while (csv.Read())
        {
            var record = new CsvMatchRecord();

            // Map known columns
            foreach (var kvp in columnMap.Where(x => x.Value >= 0))
            {
                var value = csv.GetField(kvp.Value)?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                switch (kvp.Key)
                {
                    // Core fields
                    case "Division": record.Division = value; break;
                    case "Season": record.Season = value; break;
                    case "Gameweek": record.Gameweek = value; break;
                    case "HomeTeam": record.HomeTeam = value; break;
                    case "AwayTeam": record.AwayTeam = value; break;
                    case "HomeGoals": record.HomeGoals = value; break;
                    case "AwayGoals": record.AwayGoals = value; break;
                    case "Date": record.Date = value; break;
                    case "Time": record.Time = value; break;
                    case "Result": record.Result = value; break;
                    case "Referee": record.Referee = value; break;

                    // Extended fields
                    case "HomeManager": record.HomeManager = value; break;
                    case "AwayManager": record.AwayManager = value; break;
                    case "HomeFormation": record.HomeFormation = value; break;
                    case "AwayFormation": record.AwayFormation = value; break;
                    case "Stadium": record.Stadium = value; break;
                    case "StadiumCapacity": record.StadiumCapacity = value; break;
                    case "Attendance": record.Attendance = value; break;
                    case "HomeGoalscorers": record.HomeGoalscorers = value; break;
                    case "AwayGoalscorers": record.AwayGoalscorers = value; break;
                    case "HomeLineup": record.HomeLineup = value; break;
                    case "AwayLineup": record.AwayLineup = value; break;
                    case "HomeSubstitutes": record.HomeSubstitutes = value; break;
                    case "AwaySubstitutes": record.AwaySubstitutes = value; break;
                    case "HomeSubstitutions": record.HomeSubstitutions = value; break;
                    case "AwaySubstitutions": record.AwaySubstitutions = value; break;
                    case "HomeYellowCards": record.HomeYellowCards = value; break;
                    case "AwayYellowCards": record.AwayYellowCards = value; break;
                    case "HomeRedCards": record.HomeRedCards = value; break;
                    case "AwayRedCards": record.AwayRedCards = value; break;
                    case "AssistantReferee1": record.AssistantReferee1 = value; break;
                    case "AssistantReferee2": record.AssistantReferee2 = value; break;
                    case "FourthOfficial": record.FourthOfficial = value; break;
                    case "VarReferee": record.VarReferee = value; break;
                    case "Temperature": record.Temperature = value; break;
                    case "WeatherConditions": record.WeatherConditions = value; break;
                    case "OtherCompetitions": record.OtherCompetitions = value; break;
                }
            }

            // Store additional unmapped fields
            for (int i = 0; i < headers.Length; i++)
            {
                if (!columnMap.Values.Contains(i))
                {
                    var value = csv.GetField(i)?.Trim();
                    if (!string.IsNullOrWhiteSpace(value))
                        record.AdditionalFields[headers[i]] = value;
                }
            }

            // Only add records with valid teams
            if (!string.IsNullOrWhiteSpace(record.HomeTeam) && !string.IsNullOrWhiteSpace(record.AwayTeam))
                records.Add(record);
        }

        return records;
    }

    /// <summary>
    /// Transforms CSV records into Match objects.
    /// </summary>
    private List<Match> TransformToMatches(List<CsvMatchRecord> records)
    {
        var matches = new List<Match>();

        foreach (var record in records)
        {
            if (!TryParseInt(record.HomeGoals, out int homeGoals) ||
                !TryParseInt(record.AwayGoals, out int awayGoals))
                continue;

            var match = new Match
            {
                HomeTeam = record.HomeTeam!,
                AwayTeam = record.AwayTeam!,
                HomeGoals = homeGoals,
                AwayGoals = awayGoals,
                Date = TryParseDate(record.Date),
                Time = TryParseTime(record.Time),
                Referee = record.Referee
            };

            if (TryParseInt(record.Gameweek, out int gw))
                match.Gameweek = gw;

            // Parse extended data if available
            match.ExtendedData = ExtendedDataParser.ParseExtendedData(record, match);

            matches.Add(match);
        }

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

    private static readonly string[] DateFormats =
        ["dd/MM/yyyy", "dd/MM/yy", "yyyy-MM-dd", "MM/dd/yyyy", "d/M/yyyy", "d/M/yy", "yyyy/MM/dd", "dd-MM-yyyy"];

    private static DateTime? TryParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateTime.TryParseExact(value.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt;

        if (DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return dt;

        return null;
    }

    private static TimeSpan? TryParseTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Try formats like "15:00", "3:00 PM", "15:00:00"
        if (TimeSpan.TryParse(value.Trim(), CultureInfo.InvariantCulture, out var time))
            return time;

        // Try parsing as DateTime and extract time
        if (DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt.TimeOfDay;

        return null;
    }
}
