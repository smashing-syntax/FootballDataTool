namespace FootballDataTool.Models;

/// <summary>
/// Represents the stage of a knockout tournament.
/// </summary>
public enum TournamentStage
{
    /// <summary>
    /// League/regular season match (not a tournament).
    /// </summary>
    League = 0,

    /// <summary>
    /// Group stage (e.g., World Cup Group A, Champions League Group Stage).
    /// </summary>
    GroupStage = 1,

    /// <summary>
    /// Round of 64 (rare, some tournaments).
    /// </summary>
    RoundOf64 = 2,

    /// <summary>
    /// Round of 32.
    /// </summary>
    RoundOf32 = 3,

    /// <summary>
    /// Round of 16 (Last 16).
    /// </summary>
    RoundOf16 = 4,

    /// <summary>
    /// Quarter-finals.
    /// </summary>
    QuarterFinal = 5,

    /// <summary>
    /// Semi-finals.
    /// </summary>
    SemiFinal = 6,

    /// <summary>
    /// Third place playoff.
    /// </summary>
    ThirdPlacePlayoff = 7,

    /// <summary>
    /// Final.
    /// </summary>
    Final = 8
}

/// <summary>
/// Extension methods for TournamentStage enum.
/// </summary>
public static class TournamentStageExtensions
{
    /// <summary>
    /// Gets the display name for a tournament stage.
    /// </summary>
    public static string GetDisplayName(this TournamentStage stage)
    {
        return stage switch
        {
            TournamentStage.League => "League",
            TournamentStage.GroupStage => "Group Stage",
            TournamentStage.RoundOf64 => "Round of 64",
            TournamentStage.RoundOf32 => "Round of 32",
            TournamentStage.RoundOf16 => "Round of 16",
            TournamentStage.QuarterFinal => "Quarter-Final",
            TournamentStage.SemiFinal => "Semi-Final",
            TournamentStage.ThirdPlacePlayoff => "3rd Place Playoff",
            TournamentStage.Final => "Final",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets the short code for CSV (R32, R16, QF, SF, F).
    /// </summary>
    public static string GetShortCode(this TournamentStage stage)
    {
        return stage switch
        {
            TournamentStage.League => "L",
            TournamentStage.GroupStage => "GS",
            TournamentStage.RoundOf64 => "R64",
            TournamentStage.RoundOf32 => "R32",
            TournamentStage.RoundOf16 => "R16",
            TournamentStage.QuarterFinal => "QF",
            TournamentStage.SemiFinal => "SF",
            TournamentStage.ThirdPlacePlayoff => "3P",
            TournamentStage.Final => "F",
            _ => "?"
        };
    }

    /// <summary>
    /// Parses a tournament stage from string (flexible formats).
    /// </summary>
    public static TournamentStage Parse(string stageStr)
    {
        if (string.IsNullOrWhiteSpace(stageStr))
            return TournamentStage.League;

        stageStr = stageStr.Trim().ToUpperInvariant();

        return stageStr switch
        {
            // Group stage variations
            "GS" or "GROUP" or "GROUP STAGE" or "GROUPS" => TournamentStage.GroupStage,

            // Round of 64
            "R64" or "ROUND OF 64" or "RO64" => TournamentStage.RoundOf64,

            // Round of 32
            "R32" or "ROUND OF 32" or "RO32" or "LAST 32" => TournamentStage.RoundOf32,

            // Round of 16
            "R16" or "ROUND OF 16" or "RO16" or "LAST 16" => TournamentStage.RoundOf16,

            // Quarter-finals
            "QF" or "QUARTER FINAL" or "QUARTER-FINAL" or "QUARTERFINAL" or "QUARTERS" => TournamentStage.QuarterFinal,

            // Semi-finals
            "SF" or "SEMI FINAL" or "SEMI-FINAL" or "SEMIFINAL" or "SEMIS" => TournamentStage.SemiFinal,

            // Third place
            "3P" or "3RD" or "THIRD" or "3RD PLACE" or "THIRD PLACE" or "3RD PLACE PLAYOFF" => TournamentStage.ThirdPlacePlayoff,

            // Final
            "F" or "FINAL" or "FINALS" => TournamentStage.Final,

            _ => TournamentStage.League
        };
    }

    /// <summary>
    /// Determines if this stage is a knockout stage (single elimination).
    /// </summary>
    public static bool IsKnockout(this TournamentStage stage)
    {
        return stage is 
            TournamentStage.RoundOf64 or
            TournamentStage.RoundOf32 or
            TournamentStage.RoundOf16 or
            TournamentStage.QuarterFinal or
            TournamentStage.SemiFinal or
            TournamentStage.ThirdPlacePlayoff or
            TournamentStage.Final;
    }
}
