using CsvHelper;
using CsvHelper.Configuration;
using FootballDataTool.Models;
using System.Globalization;

namespace FootballDataTool.Services;

/// <summary>
/// Service for loading transfer data from CSV files (easier than JSON for users).
/// </summary>
public class TransferCsvLoader
{
    /// <summary>
    /// Loads transfers from a CSV file.
    /// Returns dictionary of team name -> TeamSeasonInfo with transfers populated.
    /// </summary>
    public static Dictionary<string, TeamSeasonInfo> LoadFromCsv(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Transfer CSV file not found: {filePath}");

        var teamData = new Dictionary<string, TeamSeasonInfo>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = csv.GetRecords<TransferCsvRecord>().ToList();

        // Group by team and season
        var grouped = records.GroupBy(r => new { r.Team, r.Season });

        foreach (var group in grouped)
        {
            var teamInfo = new TeamSeasonInfo
            {
                TeamName = group.Key.Team,
                Season = group.Key.Season
            };

            // Separate into signings and departures
            foreach (var record in group)
            {
                var transfer = MapToTransfer(record);

                // Determine if it's a signing or departure based on ToClub
                if (record.ToClub.Equals(record.Team, StringComparison.OrdinalIgnoreCase))
                {
                    // This team is the destination -> signing
                    if (record.Window.Equals("Summer", StringComparison.OrdinalIgnoreCase))
                        teamInfo.SummerSignings.Add(transfer);
                    else if (record.Window.Equals("Winter", StringComparison.OrdinalIgnoreCase))
                        teamInfo.WinterSignings.Add(transfer);
                }
                else if (record.FromClub.Equals(record.Team, StringComparison.OrdinalIgnoreCase))
                {
                    // This team is the source -> departure
                    if (record.Window.Equals("Summer", StringComparison.OrdinalIgnoreCase))
                        teamInfo.SummerDepartures.Add(transfer);
                    else if (record.Window.Equals("Winter", StringComparison.OrdinalIgnoreCase))
                        teamInfo.WinterDepartures.Add(transfer);
                }
            }

            teamData[group.Key.Team] = teamInfo;
        }

        return teamData;
    }

    private static Transfer MapToTransfer(TransferCsvRecord record)
    {
        var transfer = new Transfer
        {
            FromClub = record.FromClub,
            ToClub = record.ToClub,
            TransferDate = ParseDate(record.TransferDate),
            Window = ParseWindow(record.Window),
            Type = ParseType(record.Type),
            IsLoan = record.IsLoan.Equals("Yes", StringComparison.OrdinalIgnoreCase) 
                     || record.IsLoan.Equals("True", StringComparison.OrdinalIgnoreCase),
            Player = new Player
            {
                Name = record.PlayerName,
                ShirtNumber = ParseInt(record.ShirtNumber),
                Position = record.Position,
                Nationality = record.Nationality,
                DateOfBirth = ParseDate(record.DateOfBirth),
                PreviousClub = record.PreviousClub
            }
        };

        // Calculate age if not provided
        if (!string.IsNullOrWhiteSpace(record.PlayerAge))
        {
            transfer.PlayerAgeAtTransfer = int.Parse(record.PlayerAge);
        }
        else if (transfer.Player.DateOfBirth.HasValue)
        {
            var age = transfer.TransferDate.Year - transfer.Player.DateOfBirth.Value.Year;
            if (transfer.Player.DateOfBirth.Value.Date > transfer.TransferDate.AddYears(-age))
                age--;
            transfer.PlayerAgeAtTransfer = age;
            transfer.Player.Age = age;
        }

        // Financial data
        if (!string.IsNullOrWhiteSpace(record.Fee) && decimal.TryParse(record.Fee, out decimal fee))
        {
            transfer.Fee = fee;
            transfer.FeeCurrency = string.IsNullOrWhiteSpace(record.FeeCurrency) ? "GBP" : record.FeeCurrency;
        }

        // Contract data
        transfer.ContractYears = ParseInt(record.ContractYears);
        transfer.ContractExpiry = ParseDate(record.ContractExpiry);

        // Notes
        transfer.Notes = record.Notes;

        return transfer;
    }

    private static DateTime ParseDate(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr))
            return DateTime.MinValue;

        // Try multiple date formats
        string[] formats = 
        [
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "dd-MM-yyyy"
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

        return DateTime.MinValue;
    }

    private static int? ParseInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return int.TryParse(value, out int result) ? result : null;
    }

    private static TransferWindow ParseWindow(string? window)
    {
        if (string.IsNullOrWhiteSpace(window))
            return TransferWindow.Summer;

        return window.ToLower() switch
        {
            "summer" => TransferWindow.Summer,
            "winter" => TransferWindow.Winter,
            "january" => TransferWindow.Winter,
            _ => TransferWindow.Summer
        };
    }

    private static TransferType ParseType(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return TransferType.Permanent;

        return type.ToLower() switch
        {
            "permanent" => TransferType.Permanent,
            "loan" => TransferType.Loan,
            "free" => TransferType.FreeTransfer,
            "free transfer" => TransferType.FreeTransfer,
            "loan return" => TransferType.LoanReturn,
            "contract expiry" => TransferType.ContractExpiry,
            "retirement" => TransferType.Retirement,
            _ => TransferType.Permanent
        };
    }
}

/// <summary>
/// CSV record for transfer data (mapped from CSV columns).
/// </summary>
public class TransferCsvRecord
{
    public string Team { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string ShirtNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string FromClub { get; set; } = string.Empty;
    public string ToClub { get; set; } = string.Empty;
    public string TransferDate { get; set; } = string.Empty;
    public string Window { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Fee { get; set; } = string.Empty;
    public string FeeCurrency { get; set; } = string.Empty;
    public string IsLoan { get; set; } = string.Empty;
    public string PlayerAge { get; set; } = string.Empty;
    public string ContractYears { get; set; } = string.Empty;
    public string ContractExpiry { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string PreviousClub { get; set; } = string.Empty;
}
