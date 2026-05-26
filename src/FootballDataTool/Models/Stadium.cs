namespace FootballDataTool.Models;

/// <summary>
/// Represents a football stadium.
/// </summary>
public class Stadium
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Capacity { get; set; }
    
    /// <summary>
    /// Stadium ID for linking across datasets.
    /// </summary>
    public string? StadiumId { get; set; }
    
    /// <summary>
    /// Year opened/renovated.
    /// </summary>
    public int? YearOpened { get; set; }
    
    public bool HasRoof { get; set; }
    public string? SurfaceType { get; set; }

    public override string ToString() => 
        $"{Name} ({Capacity:N0} capacity)";
}
