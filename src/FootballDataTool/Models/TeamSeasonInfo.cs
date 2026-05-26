namespace FootballDataTool.Models;

/// <summary>
/// Comprehensive team information for a specific season.
/// </summary>
public class TeamSeasonInfo
{
    public string TeamName { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;

    /// <summary>
    /// Manager/head coach at the start of the season.
    /// </summary>
    public Manager? StartingManager { get; set; }

    /// <summary>
    /// Current manager (may differ from starting manager if changes occurred).
    /// </summary>
    public Manager? CurrentManager { get; set; }

    /// <summary>
    /// All managerial changes during the season.
    /// </summary>
    public List<ManagerialChange> ManagerialChanges { get; set; } = new();

    /// <summary>
    /// Players signed in the summer transfer window.
    /// </summary>
    public List<Transfer> SummerSignings { get; set; } = new();

    /// <summary>
    /// Players signed in the winter transfer window.
    /// </summary>
    public List<Transfer> WinterSignings { get; set; } = new();

    /// <summary>
    /// Players who left in the summer.
    /// </summary>
    public List<Transfer> SummerDepartures { get; set; } = new();

    /// <summary>
    /// Players who left in the winter.
    /// </summary>
    public List<Transfer> WinterDepartures { get; set; } = new();

    /// <summary>
    /// Full squad at any point (can be updated throughout season).
    /// </summary>
    public List<Player> Squad { get; set; } = new();

    /// <summary>
    /// Home stadium information.
    /// </summary>
    public Stadium? HomeStadium { get; set; }

    /// <summary>
    /// Total transfer spending in the season.
    /// </summary>
    public decimal? TotalSpending => 
        SummerSignings.Concat(WinterSignings)
            .Where(t => t.Fee.HasValue)
            .Sum(t => t.Fee!.Value);

    /// <summary>
    /// Total transfer income in the season.
    /// </summary>
    public decimal? TotalIncome => 
        SummerDepartures.Concat(WinterDepartures)
            .Where(t => t.Fee.HasValue)
            .Sum(t => t.Fee!.Value);

    /// <summary>
    /// Net transfer spend (spending - income).
    /// </summary>
    public decimal? NetSpend => 
        TotalSpending.HasValue && TotalIncome.HasValue
            ? TotalSpending.Value - TotalIncome.Value
            : null;

    /// <summary>
    /// Average age of squad (if age data available).
    /// </summary>
    public double? AverageSquadAge
    {
        get
        {
            var ages = Squad.Where(p => p.Age.HasValue).Select(p => p.Age!.Value).ToList();
            return ages.Any() ? ages.Average() : null;
        }
    }
}

public class ManagerialChange
{
    /// <summary>
    /// Outgoing manager details.
    /// </summary>
    public Manager? OutgoingManager { get; set; }

    /// <summary>
    /// Incoming manager details.
    /// </summary>
    public Manager? IncomingManager { get; set; }

    /// <summary>
    /// Date of the managerial change.
    /// </summary>
    public DateTime ChangeDate { get; set; }

    /// <summary>
    /// Reason for the change (sacked, resigned, mutual consent, etc.).
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether the incoming manager is a caretaker/interim.
    /// </summary>
    public bool IsCaretaker { get; set; }

    /// <summary>
    /// Record before the change (if available).
    /// </summary>
    public string? RecordBeforeChange { get; set; }

    /// <summary>
    /// Age of outgoing manager at time of dismissal.
    /// </summary>
    public int? OutgoingManagerAge => OutgoingManager?.CalculateAge(ChangeDate);

    /// <summary>
    /// Age of incoming manager at time of appointment.
    /// </summary>
    public int? IncomingManagerAge => IncomingManager?.CalculateAge(ChangeDate);
}
