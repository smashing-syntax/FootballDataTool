namespace FootballDataTool.Models;

/// <summary>
/// Metadata about a football season detected from CSV data.
/// </summary>
public class SeasonMetadata
{
    /// <summary>
    /// The season identifier (e.g., "2023/24", "2023-24", or "2023").
    /// </summary>
    public string Season { get; set; } = "Unknown";

    /// <summary>
    /// The league/division name (e.g., "Premier League", "La Liga", "Serie A").
    /// </summary>
    public string League { get; set; } = "Unknown";

    /// <summary>
    /// The country/region of the league (e.g., "England", "Spain", "Italy").
    /// </summary>
    public string Country { get; set; } = "Unknown";

    /// <summary>
    /// Total number of matches in the dataset.
    /// </summary>
    public int TotalMatches { get; set; }

    /// <summary>
    /// Date range of the season.
    /// </summary>
    public (DateTime? Start, DateTime? End) DateRange { get; set; }

    public override string ToString()
    {
        var parts = new List<string>();
        
        if (League != "Unknown")
            parts.Add(League);
        
        if (Country != "Unknown" && !League.Contains(Country, StringComparison.OrdinalIgnoreCase))
            parts.Add($"({Country})");
        
        if (Season != "Unknown")
            parts.Add($"- {Season}");
        
        return parts.Count > 0 ? string.Join(" ", parts) : "Unknown Season";
    }
}
