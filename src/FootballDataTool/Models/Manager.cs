namespace FootballDataTool.Models;

/// <summary>
/// Represents a football manager/head coach.
/// </summary>
public class Manager
{
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Manager's age (if known).
    /// </summary>
    public int? Age { get; set; }
    
    /// <summary>
    /// Date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Nationality.
    /// </summary>
    public string? Nationality { get; set; }
    
    /// <summary>
    /// Previous clubs managed.
    /// </summary>
    public List<string> PreviousClubs { get; set; } = new();
    
    /// <summary>
    /// Years of managerial experience.
    /// </summary>
    public int? YearsOfExperience { get; set; }
    
    /// <summary>
    /// Date appointed to current club.
    /// </summary>
    public DateTime? AppointmentDate { get; set; }
    
    /// <summary>
    /// Preferred formation(s).
    /// </summary>
    public List<string> PreferredFormations { get; set; } = new();
    
    /// <summary>
    /// Calculates age at a specific date.
    /// </summary>
    public int? CalculateAge(DateTime referenceDate)
    {
        if (!DateOfBirth.HasValue)
            return Age;
        
        var age = referenceDate.Year - DateOfBirth.Value.Year;
        if (referenceDate < DateOfBirth.Value.AddYears(age))
            age--;
        
        return age;
    }
    
    /// <summary>
    /// Returns formatted string with age if available.
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string> { Name };
        
        if (Age.HasValue)
            parts.Add($"({Age})");
        
        if (YearsOfExperience.HasValue)
            parts.Add($"[{YearsOfExperience}y exp]");
        
        return string.Join(" ", parts);
    }
}
