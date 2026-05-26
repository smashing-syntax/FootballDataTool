namespace FootballDataTool.Models;

/// <summary>
/// Represents a football player with optional detailed information.
/// </summary>
public class Player
{
    public string Name { get; set; } = string.Empty;
    public int? ShirtNumber { get; set; }
    public string? Position { get; set; }
    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Player ID for linking across datasets (e.g., from external APIs).
    /// </summary>
    public string? PlayerId { get; set; }

    public override string ToString() => ShirtNumber.HasValue 
        ? $"{ShirtNumber}. {Name}" 
        : Name;
}
