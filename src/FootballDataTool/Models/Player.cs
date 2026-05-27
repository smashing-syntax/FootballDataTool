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
    /// Previous club before current team.
    /// </summary>
    public string? PreviousClub { get; set; }

    /// <summary>
    /// Previous league before current team.
    /// </summary>
    public string? PreviousLeague { get; set; }

    /// <summary>
    /// Previous club's league position when player left (if known).
    /// </summary>
    public int? PreviousClubPosition { get; set; }

    /// <summary>
    /// Zodiac sign based on date of birth.
    /// </summary>
    public string? ZodiacSign => DateOfBirth.HasValue ? CalculateZodiacSign(DateOfBirth.Value) : null;

    /// <summary>
    /// Chinese zodiac based on birth year.
    /// </summary>
    public string? ChineseZodiac => DateOfBirth.HasValue ? CalculateChineseZodiac(DateOfBirth.Value.Year) : null;

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

    /// <summary>
    /// Checks if it's the player's birthday on a given date.
    /// </summary>
    public bool IsBirthdayOn(DateTime date)
    {
        if (!DateOfBirth.HasValue)
            return false;

        return DateOfBirth.Value.Month == date.Month && DateOfBirth.Value.Day == date.Day;
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

        if (Nationality != null)
            parts.Add($"🌍{Nationality}");

        return string.Join(" ", parts);
    }

    private static string CalculateZodiacSign(DateTime birthDate)
    {
        var month = birthDate.Month;
        var day = birthDate.Day;

        return (month, day) switch
        {
            ( >= 3 and <= 4, _) when (month == 3 && day >= 21) || (month == 4 && day <= 19) => "Aries",
            ( >= 4 and <= 5, _) when (month == 4 && day >= 20) || (month == 5 && day <= 20) => "Taurus",
            ( >= 5 and <= 6, _) when (month == 5 && day >= 21) || (month == 6 && day <= 20) => "Gemini",
            ( >= 6 and <= 7, _) when (month == 6 && day >= 21) || (month == 7 && day <= 22) => "Cancer",
            ( >= 7 and <= 8, _) when (month == 7 && day >= 23) || (month == 8 && day <= 22) => "Leo",
            ( >= 8 and <= 9, _) when (month == 8 && day >= 23) || (month == 9 && day <= 22) => "Virgo",
            ( >= 9 and <= 10, _) when (month == 9 && day >= 23) || (month == 10 && day <= 22) => "Libra",
            ( >= 10 and <= 11, _) when (month == 10 && day >= 23) || (month == 11 && day <= 21) => "Scorpio",
            ( >= 11 and <= 12, _) when (month == 11 && day >= 22) || (month == 12 && day <= 21) => "Sagittarius",
            ( >= 12 or 1, _) when (month == 12 && day >= 22) || (month == 1 && day <= 19) => "Capricorn",
            ( >= 1 and <= 2, _) when (month == 1 && day >= 20) || (month == 2 && day <= 18) => "Aquarius",
            ( >= 2 and <= 3, _) when (month == 2 && day >= 19) || (month == 3 && day <= 20) => "Pisces",
            _ => "Unknown"
        };
    }

    private static string CalculateChineseZodiac(int year)
    {
        var animals = new[] { "Rat", "Ox", "Tiger", "Rabbit", "Dragon", "Snake", 
                             "Horse", "Goat", "Monkey", "Rooster", "Dog", "Pig" };
        var index = (year - 4) % 12;
        if (index < 0) index += 12;
        return animals[index];
    }
}

