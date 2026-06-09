namespace FootballDataTool.Models;

public class Match
{
    // Core match data (required)
    public int Gameweek { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }

    // Tournament/knockout stage data (optional)
    /// <summary>
    /// Tournament stage (e.g., GroupStage, RoundOf16, Final). 
    /// Defaults to League for regular season matches.
    /// </summary>
    public TournamentStage Stage { get; set; } = TournamentStage.League;

    /// <summary>
    /// Group identifier for group stage matches (e.g., "A", "B", "Group A").
    /// Null for knockout stages and league matches.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Leg number for two-legged knockout ties (1 or 2). 
    /// Null for single-leg matches and league games.
    /// </summary>
    public int? Leg { get; set; }

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

    /// <summary>
    /// Returns true if this match is part of a knockout tournament (single elimination).
    /// </summary>
    public bool IsKnockout => Stage.IsKnockout();

    /// <summary>
    /// Returns true if this match is part of a group stage.
    /// </summary>
    public bool IsGroupStage => Stage == TournamentStage.GroupStage;

    /// <summary>
    /// Returns true if this is a regular league match.
    /// </summary>
    public bool IsLeague => Stage == TournamentStage.League;

    // Convenience properties that check extended data
    public bool HasLineupData => ExtendedData?.HomeStartingLineup.Count > 0 
                                 || ExtendedData?.AwayStartingLineup.Count > 0;

    public bool HasGoalEvents => ExtendedData?.Goals.Count > 0;

    public bool HasAttendanceData => ExtendedData?.Attendance.HasValue == true;

    // Optional reference to parent SeasonData for accessing team objects
    internal SeasonData? ParentSeason { get; set; }

    /// <summary>
    /// Get the home team's full season object (if SeasonData is available).
    /// Provides access to team-level aggregated data.
    /// </summary>
    public TeamSeason? GetHomeTeamObject() => ParentSeason?.GetTeam(HomeTeam);

    /// <summary>
    /// Get the away team's full season object (if SeasonData is available).
    /// Provides access to team-level aggregated data.
    /// </summary>
    public TeamSeason? GetAwayTeamObject() => ParentSeason?.GetTeam(AwayTeam);
}
