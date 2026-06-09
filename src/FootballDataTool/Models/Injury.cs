namespace FootballDataTool.Models;

/// <summary>
/// Represents a player injury record.
/// </summary>
public class Injury
{
    public Player Player { get; set; } = new();
    public string InjuryType { get; set; } = string.Empty;
    public DateTime InjuryDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? ExpectedReturnDate { get; set; }
    public int? EstimatedDaysOut { get; set; }
    public InjurySeverity Severity { get; set; }
    public string? Notes { get; set; }
    
    /// <summary>
    /// Actual days missed (if return date is known).
    /// </summary>
    public int? ActualDaysMissed => 
        ReturnDate.HasValue 
            ? (ReturnDate.Value - InjuryDate).Days 
            : null;
    
    /// <summary>
    /// Checks if player was injured on a specific date.
    /// </summary>
    public bool IsInjuredOn(DateTime date)
    {
        if (date < InjuryDate)
            return false;
        
        if (ReturnDate.HasValue && date >= ReturnDate.Value)
            return false;
        
        return true;
    }
    
    public override string ToString()
    {
        var status = ReturnDate.HasValue 
            ? $"Recovered ({ActualDaysMissed} days)" 
            : ExpectedReturnDate.HasValue 
                ? $"Out until {ExpectedReturnDate:dd/MM/yyyy}" 
                : "Out indefinitely";
        
        return $"{Player.Name} - {InjuryType} ({status})";
    }
}

public enum InjurySeverity
{
    Minor,      // 1-7 days
    Moderate,   // 1-4 weeks
    Serious,    // 1-3 months
    LongTerm,   // 3+ months
    CareerThreatening
}

/// <summary>
/// Represents a player's appearance in a match with detailed statistics.
/// </summary>
public class PlayerAppearance
{
    public Player Player { get; set; } = new();
    public string Team { get; set; } = string.Empty;
    public bool IsStarting { get; set; }
    public int MinutesPlayed { get; set; }
    public int? ShirtNumber { get; set; }
    public string? Position { get; set; }
    
    /// <summary>
    /// Minute substituted on (if substitute).
    /// </summary>
    public int? SubbedOn { get; set; }
    
    /// <summary>
    /// Minute substituted off (if substituted).
    /// </summary>
    public int? SubbedOff { get; set; }
    
    /// <summary>
    /// Goals scored in this appearance.
    /// </summary>
    public int Goals { get; set; }
    
    /// <summary>
    /// Assists in this appearance.
    /// </summary>
    public int Assists { get; set; }
    
    /// <summary>
    /// Cards received.
    /// </summary>
    public List<CardType> Cards { get; set; } = new();
    
    /// <summary>
    /// Whether player completed full 90+ minutes.
    /// </summary>
    public bool PlayedFullMatch => MinutesPlayed >= 90;
    
    public override string ToString()
    {
        var starter = IsStarting ? "Started" : $"Sub ({SubbedOn}')";
        var mins = MinutesPlayed > 0 ? $"{MinutesPlayed}'" : "Unused";
        return $"{Player.Name} - {starter}, {mins}";
    }
}
