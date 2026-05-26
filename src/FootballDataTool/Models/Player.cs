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
    /// Player's age (can be provided directly or calculated from DateOfBirth).
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// Player ID for linking across datasets (e.g., from external APIs).
    /// </summary>
    public string? PlayerId { get; set; }

    /// <summary>
    /// Calculates age based on DateOfBirth and a reference date.
    /// </summary>
    public int? CalculateAge(DateTime referenceDate)
    {
        if (!DateOfBirth.HasValue)
            return Age; // Return explicit age if no birth date

        var age = referenceDate.Year - DateOfBirth.Value.Year;
        if (referenceDate < DateOfBirth.Value.AddYears(age))
            age--;

        return age;
    }

    public override string ToString()
    {
        var parts = new List<string>();

        if (ShirtNumber.HasValue)
            parts.Add($"{ShirtNumber}.");

        parts.Add(Name);

        if (Age.HasValue)
            parts.Add($"({Age})");
        else if (Position != null)
            parts.Add($"[{Position}]");

        return string.Join(" ", parts);
    }
}
