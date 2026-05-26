namespace FootballDataTool.Models;

public class Match
{
    // Core match data (required)
    public int Gameweek { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }

    // Basic metadata (optional)
    public DateTime? Date { get; set; }
    public TimeSpan? Time { get; set; }
    public string? Referee { get; set; }

    /// <summary>
    /// Extended match data including lineups, events, and context.
    /// Null if not available - tool works perfectly without it.
    /// </summary>
    public MatchExtendedData? ExtendedData { get; set; }

    // Computed properties
    public string Result => HomeGoals > AwayGoals ? "H" : HomeGoals < AwayGoals ? "A" : "D";
    public int HomePoints => HomeGoals > AwayGoals ? 3 : HomeGoals == AwayGoals ? 1 : 0;
    public int AwayPoints => AwayGoals > HomeGoals ? 3 : HomeGoals == AwayGoals ? 1 : 0;

    public string ScoreString => $"{HomeGoals}-{AwayGoals}";

    // Convenience properties that check extended data
    public bool HasLineupData => ExtendedData?.HomeStartingLineup.Count > 0 
                                 || ExtendedData?.AwayStartingLineup.Count > 0;

    public bool HasGoalEvents => ExtendedData?.Goals.Count > 0;

    public bool HasAttendanceData => ExtendedData?.Attendance.HasValue == true;
}
