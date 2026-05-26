namespace FootballDataTool.Models;

/// <summary>
/// Represents a raw CSV match record with all available fields from the source data.
/// This is the discrete representation before transformation into the Match model.
/// Supports both basic match data and rich extended fields.
/// </summary>
public class CsvMatchRecord
{
    // Core match data
    public string? Division { get; set; }
    public string? Season { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Gameweek { get; set; }
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
    public string? HomeGoals { get; set; }
    public string? AwayGoals { get; set; }
    public string? Result { get; set; }

    // Match officials
    public string? Referee { get; set; }
    public string? AssistantReferee1 { get; set; }
    public string? AssistantReferee2 { get; set; }
    public string? FourthOfficial { get; set; }
    public string? VarReferee { get; set; }

    // Team management
    public string? HomeManager { get; set; }
    public string? AwayManager { get; set; }

    // Formations
    public string? HomeFormation { get; set; }
    public string? AwayFormation { get; set; }

    // Venue & attendance
    public string? Stadium { get; set; }
    public string? StadiumCapacity { get; set; }
    public string? Attendance { get; set; }

    // Goalscorers (semi-colon or comma separated)
    public string? HomeGoalscorers { get; set; }
    public string? AwayGoalscorers { get; set; }

    // Lineups (semi-colon or comma separated player names)
    public string? HomeLineup { get; set; }
    public string? AwayLineup { get; set; }
    public string? HomeSubstitutes { get; set; }
    public string? AwaySubstitutes { get; set; }

    // Substitutions (formatted strings)
    public string? HomeSubstitutions { get; set; }
    public string? AwaySubstitutions { get; set; }

    // Cards
    public string? HomeYellowCards { get; set; }
    public string? AwayYellowCards { get; set; }
    public string? HomeRedCards { get; set; }
    public string? AwayRedCards { get; set; }

    // Competition context
    public string? OtherCompetitions { get; set; }  // JSON or formatted string

    // Weather
    public string? Temperature { get; set; }
    public string? WeatherConditions { get; set; }

    // Additional optional fields common in football datasets
    public Dictionary<string, string> AdditionalFields { get; set; } = new();
}
