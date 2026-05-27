namespace FootballDataTool.Models;

/// <summary>
/// Extended match data including lineups, events, and context.
/// All fields are optional to support varying levels of data richness.
/// </summary>
public class MatchExtendedData
{
    // Team Management
    /// <summary>
    /// Home team manager (string for simple cases).
    /// </summary>
    public string? HomeManager { get; set; }

    /// <summary>
    /// Away team manager (string for simple cases).
    /// </summary>
    public string? AwayManager { get; set; }

    /// <summary>
    /// Home team manager (full Manager object with age/details).
    /// </summary>
    public Manager? HomeManagerDetails { get; set; }

    /// <summary>
    /// Away team manager (full Manager object with age/details).
    /// </summary>
    public Manager? AwayManagerDetails { get; set; }

    // Lineups
    public List<Player> HomeStartingLineup { get; set; } = new();
    public List<Player> AwayStartingLineup { get; set; } = new();
    public List<Player> HomeSubstitutes { get; set; } = new();
    public List<Player> AwaySubstitutes { get; set; } = new();
    
    // Formation
    public string? HomeFormation { get; set; }
    public string? AwayFormation { get; set; }
    
    // Match Events
    public List<GoalEvent> Goals { get; set; } = new();
    public List<SubstitutionEvent> Substitutions { get; set; } = new();
    public List<CardEvent> Cards { get; set; } = new();
    
    // Venue & Attendance
    public Stadium? Stadium { get; set; }
    public int? Attendance { get; set; }
    
    // Competition Context
    public List<OtherCompetitionFixture> OtherFixturesThisWeek { get; set; } = new();
    
    /// <summary>
    /// Number of days since last match for home team.
    /// </summary>
    public int? HomeDaysSinceLastMatch { get; set; }
    
    /// <summary>
    /// Number of days since last match for away team.
    /// </summary>
    public int? AwayDaysSinceLastMatch { get; set; }
    
    // Match Officials
    public string? Referee { get; set; }
    public List<string> AssistantReferees { get; set; } = new();
    public string? FourthOfficial { get; set; }
    public string? VarReferee { get; set; }

    // Weather (optional but interesting!)
    public WeatherConditions? Weather { get; set; }

    // Injuries & Availability
    /// <summary>
    /// Players injured/unavailable for home team in this match.
    /// </summary>
    public List<Injury> HomeInjuries { get; set; } = new();

    /// <summary>
    /// Players injured/unavailable for away team in this match.
    /// </summary>
    public List<Injury> AwayInjuries { get; set; } = new();

    // Player Appearances & Minutes
    /// <summary>
    /// Detailed appearance records for home team players (including minutes played).
    /// </summary>
    public List<PlayerAppearance> HomeAppearances { get; set; } = new();

    /// <summary>
    /// Detailed appearance records for away team players (including minutes played).
    /// </summary>
    public List<PlayerAppearance> AwayAppearances { get; set; } = new();

    // Computed properties for lineup analysis

    /// <summary>
    /// Average age of home starting XI (if age data is available).
    /// </summary>
    public double? HomeAverageAge => CalculateAverageAge(HomeStartingLineup);

    /// <summary>
    /// Average age of away starting XI (if age data is available).
    /// </summary>
    public double? AwayAverageAge => CalculateAverageAge(AwayStartingLineup);

    /// <summary>
    /// Gets the youngest player in the home starting lineup.
    /// </summary>
    public Player? HomeYoungestPlayer => HomeStartingLineup
        .Where(p => p.Age.HasValue)
        .OrderBy(p => p.Age)
        .FirstOrDefault();

    /// <summary>
    /// Gets the oldest player in the home starting lineup.
    /// </summary>
    public Player? HomeOldestPlayer => HomeStartingLineup
        .Where(p => p.Age.HasValue)
        .OrderByDescending(p => p.Age)
        .FirstOrDefault();

    /// <summary>
    /// Gets the youngest player in the away starting lineup.
    /// </summary>
    public Player? AwayYoungestPlayer => AwayStartingLineup
        .Where(p => p.Age.HasValue)
        .OrderBy(p => p.Age)
        .FirstOrDefault();

    /// <summary>
    /// Gets the oldest player in the away starting lineup.
    /// </summary>
    public Player? AwayOldestPlayer => AwayStartingLineup
        .Where(p => p.Age.HasValue)
        .OrderByDescending(p => p.Age)
        .FirstOrDefault();

    /// <summary>
    /// Total injuries affecting home team for this match.
    /// </summary>
    public int HomeInjuryCount => HomeInjuries.Count;

    /// <summary>
    /// Total injuries affecting away team for this match.
    /// </summary>
    public int AwayInjuryCount => AwayInjuries.Count;

    /// <summary>
    /// Total minutes played by all home team players (sum of all appearances).
    /// </summary>
    public int HomeTotalMinutes => HomeAppearances.Sum(a => a.MinutesPlayed);

    /// <summary>
    /// Total minutes played by all away team players (sum of all appearances).
    /// </summary>
    public int AwayTotalMinutes => AwayAppearances.Sum(a => a.MinutesPlayed);

    /// <summary>
    /// Players having birthday on match day (both teams).
    /// </summary>
    public List<Player> PlayersWithBirthday(DateTime matchDate)
    {
        var allPlayers = HomeStartingLineup
            .Concat(HomeSubstitutes)
            .Concat(AwayStartingLineup)
            .Concat(AwaySubstitutes);

        return allPlayers.Where(p => p.IsBirthdayOn(matchDate)).ToList();
    }

    /// <summary>
    /// Managers having birthday on match day.
    /// </summary>
    public List<Manager> ManagersWithBirthday(DateTime matchDate)
    {
        var managers = new List<Manager>();
        if (HomeManagerDetails?.IsBirthdayOn(matchDate) == true)
            managers.Add(HomeManagerDetails);
        if (AwayManagerDetails?.IsBirthdayOn(matchDate) == true)
            managers.Add(AwayManagerDetails);
        return managers;
    }

    private static double? CalculateAverageAge(List<Player> lineup)
    {
        var ages = lineup.Where(p => p.Age.HasValue).Select(p => p.Age!.Value).ToList();
        return ages.Any() ? ages.Average() : null;
    }
}

/// <summary>
/// Represents a goal scored in a match.
/// </summary>
public class GoalEvent
{
    public Player Scorer { get; set; } = new();
    public Player? Assister { get; set; }
    public int Minute { get; set; }
    public int? AddedTimeMinute { get; set; }
    public string TeamScoring { get; set; } = string.Empty;
    public GoalType Type { get; set; }
    
    public string DisplayTime => AddedTimeMinute.HasValue 
        ? $"{Minute}+{AddedTimeMinute}'" 
        : $"{Minute}'";
    
    public override string ToString()
    {
        var typeStr = Type switch
        {
            GoalType.OwnGoal => " (OG)",
            GoalType.Penalty => " (PEN)",
            GoalType.DirectFreeKick => " (FK)",
            _ => ""
        };
        
        var assistStr = Assister != null ? $" (assist: {Assister.Name})" : "";
        return $"{DisplayTime} {Scorer.Name}{typeStr}{assistStr}";
    }
}

public enum GoalType
{
    Open,               // Regular goal from open play
    Penalty,
    DirectFreeKick,
    OwnGoal,
    Header,
    Volley,
    LongRange          // Outside the box
}

/// <summary>
/// Represents a substitution during a match.
/// </summary>
public class SubstitutionEvent
{
    public Player PlayerOff { get; set; } = new();
    public Player PlayerOn { get; set; } = new();
    public int Minute { get; set; }
    public int? AddedTimeMinute { get; set; }
    public string Team { get; set; } = string.Empty;
    public string? Reason { get; set; }  // Tactical, Injury, etc.
    
    public string DisplayTime => AddedTimeMinute.HasValue 
        ? $"{Minute}+{AddedTimeMinute}'" 
        : $"{Minute}'";
    
    public override string ToString() => 
        $"{DisplayTime} {Team}: {PlayerOn.Name} ↔ {PlayerOff.Name}";
}

/// <summary>
/// Represents a card shown during a match.
/// </summary>
public class CardEvent
{
    public Player Player { get; set; } = new();
    public int Minute { get; set; }
    public int? AddedTimeMinute { get; set; }
    public string Team { get; set; } = string.Empty;
    public CardType Type { get; set; }
    public string? Reason { get; set; }
    
    public string DisplayTime => AddedTimeMinute.HasValue 
        ? $"{Minute}+{AddedTimeMinute}'" 
        : $"{Minute}'";
}

public enum CardType
{
    Yellow,
    Red,
    SecondYellow
}

/// <summary>
/// Information about other competitions a team is playing in during the same week.
/// </summary>
public class OtherCompetitionFixture
{
    public string Team { get; set; } = string.Empty;
    public string Competition { get; set; } = string.Empty;
    public DateTime FixtureDate { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public bool IsHome { get; set; }
    public string? Result { get; set; }  // If already played
    public string? Stage { get; set; }   // Group Stage, R16, QF, etc.
}

public class WeatherConditions
{
    public decimal? Temperature { get; set; }  // Celsius
    public string? Conditions { get; set; }    // Sunny, Rainy, Snowy, etc.
    public int? WindSpeed { get; set; }        // km/h
    public int? Humidity { get; set; }         // Percentage
}
