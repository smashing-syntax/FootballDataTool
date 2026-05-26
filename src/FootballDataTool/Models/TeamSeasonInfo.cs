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
    public string? Manager { get; set; }
    
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
}

public class ManagerialChange
{
    public string OutgoingManager { get; set; } = string.Empty;
    public string IncomingManager { get; set; } = string.Empty;
    public DateTime ChangeDate { get; set; }
    public string? Reason { get; set; }
    public bool IsCaretaker { get; set; }
}
