using FootballDataTool.Services;
using FootballDataTool.Models;

namespace FootballDataTool.Examples;

/// <summary>
/// Examples demonstrating tournament data usage (World Cup, Champions League, etc.).
/// </summary>
public static class TournamentExamples
{
    /// <summary>
    /// Load and analyze World Cup 2022 data.
    /// </summary>
    public static void WorldCupExample()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_worldcup.csv");

        Console.WriteLine($"=== FIFA World Cup {seasonData.Metadata.Season} ===\n");
        Console.WriteLine($"Total matches: {seasonData.TotalMatches}");

        // Group stage analysis
        var groupMatches = seasonData.GetGroupStageMatches();
        Console.WriteLine($"Group stage matches: {groupMatches.Count}");

        var groups = seasonData.GetAllGroups();
        Console.WriteLine($"Groups: {string.Join(", ", groups)}\n");

        // Analyze each group
        foreach (var group in groups)
        {
            Console.WriteLine($"=== Group {group} Standings ===");
            var standings = seasonData.GetGroupStandings(group);

            foreach (var team in standings)
            {
                Console.WriteLine($"{team.TeamName,-20} P:{team.Played,2} W:{team.Won,2} D:{team.Drawn,2} L:{team.Lost,2} GD:{team.GoalDifference,3} Pts:{team.Points,2}");
            }
            Console.WriteLine();
        }

        // Knockout stage analysis
        var knockoutMatches = seasonData.GetKnockoutMatches();
        Console.WriteLine($"=== Knockout Stage ({knockoutMatches.Count} matches) ===\n");

        // Round of 16
        var r16 = seasonData.GetMatchesByStage(TournamentStage.RoundOf16);
        Console.WriteLine("Round of 16:");
        foreach (var match in r16)
        {
            Console.WriteLine($"  {match.HomeTeam} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam}");
        }
        Console.WriteLine();

        // Quarter-finals
        var qf = seasonData.GetMatchesByStage(TournamentStage.QuarterFinal);
        Console.WriteLine("Quarter-finals:");
        foreach (var match in qf)
        {
            Console.WriteLine($"  {match.HomeTeam} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam}");
        }
        Console.WriteLine();

        // Semi-finals
        var sf = seasonData.GetMatchesByStage(TournamentStage.SemiFinal);
        Console.WriteLine("Semi-finals:");
        foreach (var match in sf)
        {
            Console.WriteLine($"  {match.HomeTeam} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam}");
        }
        Console.WriteLine();

        // Third place playoff
        var thirdPlace = seasonData.GetMatchesByStage(TournamentStage.ThirdPlacePlayoff).FirstOrDefault();
        if (thirdPlace != null)
        {
            Console.WriteLine($"3rd Place Playoff: {thirdPlace.HomeTeam} {thirdPlace.HomeGoals}-{thirdPlace.AwayGoals} {thirdPlace.AwayTeam}\n");
        }

        // Final
        var final = seasonData.GetMatchesByStage(TournamentStage.Final).FirstOrDefault();
        if (final != null)
        {
            Console.WriteLine($"🏆 FINAL: {final.HomeTeam} {final.HomeGoals}-{final.AwayGoals} {final.AwayTeam}");
            var winner = final.HomeGoals > final.AwayGoals ? final.HomeTeam : 
                        final.AwayGoals > final.HomeGoals ? final.AwayTeam : 
                        "Draw (check penalties)";
            Console.WriteLine($"   Winner: {winner}\n");
        }
    }

    /// <summary>
    /// Load and analyze Champions League knockout data with two-legged ties.
    /// </summary>
    public static void ChampionsLeagueExample()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_knockout.csv");

        Console.WriteLine($"=== UEFA Champions League {seasonData.Metadata.Season} Knockout Stage ===\n");

        // Get all two-legged ties
        var ties = seasonData.GetTwoLeggedTies();
        Console.WriteLine($"Two-legged ties: {ties.Count}\n");

        // Analyze each tie
        var stageGroups = ties.GroupBy(t => t.FirstLeg.Stage).OrderBy(g => g.Key);

        foreach (var stageGroup in stageGroups)
        {
            var stage = stageGroup.Key;
            Console.WriteLine($"=== {stage.GetDisplayName()} ===\n");

            foreach (var (firstLeg, secondLeg) in stageGroup)
            {
                var (team1Goals, team2Goals) = seasonData.GetAggregateScore(firstLeg, secondLeg);
                var team1 = firstLeg.HomeTeam;
                var team2 = firstLeg.AwayTeam;

                Console.WriteLine($"{team1} vs {team2}");
                Console.WriteLine($"  First leg:  {firstLeg.HomeTeam} {firstLeg.HomeGoals}-{firstLeg.AwayGoals} {firstLeg.AwayTeam}");
                Console.WriteLine($"  Second leg: {secondLeg.HomeTeam} {secondLeg.HomeGoals}-{secondLeg.AwayGoals} {secondLeg.AwayTeam}");
                Console.WriteLine($"  Aggregate:  {team1} {team1Goals}-{team2Goals} {team2}");

                var winner = team1Goals > team2Goals ? team1 : 
                           team2Goals > team1Goals ? team2 : 
                           "Draw (check away goals/penalties)";
                Console.WriteLine($"  Winner: {winner}\n");
            }
        }

        // Single-leg final
        var final = seasonData.GetMatchesByStage(TournamentStage.Final).FirstOrDefault();
        if (final != null)
        {
            Console.WriteLine($"=== {TournamentStage.Final.GetDisplayName()} ===");
            Console.WriteLine($"{final.HomeTeam} {final.HomeGoals}-{final.AwayGoals} {final.AwayTeam}");
            var winner = final.HomeGoals > final.AwayGoals ? final.HomeTeam : 
                        final.AwayGoals > final.HomeGoals ? final.AwayTeam : 
                        "Draw (check penalties)";
            Console.WriteLine($"🏆 Winner: {winner}\n");
        }
    }

    /// <summary>
    /// Load and analyze Champions League group stage.
    /// </summary>
    public static void ChampionsLeagueGroupStageExample()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_groupstage.csv");

        Console.WriteLine($"=== UEFA Champions League {seasonData.Metadata.Season} Group Stage ===\n");

        var groups = seasonData.GetAllGroups();
        Console.WriteLine($"Groups: {string.Join(", ", groups)}\n");

        foreach (var group in groups)
        {
            Console.WriteLine($"=== Group {group} ===");

            var groupMatches = seasonData.GetMatchesByGroup(group);
            Console.WriteLine($"Matches played: {groupMatches.Count}\n");

            Console.WriteLine("Results:");
            foreach (var match in groupMatches.OrderBy(m => m.Gameweek))
            {
                Console.WriteLine($"  MD{match.Gameweek}: {match.HomeTeam} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam}");
            }
            Console.WriteLine();

            var standings = seasonData.GetGroupStandings(group);
            Console.WriteLine("Standings:");
            int pos = 1;
            foreach (var team in standings)
            {
                string qualifier = pos <= 2 ? " ✓" : "";  // Top 2 qualify
                Console.WriteLine($"  {pos}. {team.TeamName,-20} P:{team.Played} W:{team.Won} D:{team.Drawn} L:{team.Lost} GD:{team.GoalDifference,3} Pts:{team.Points}{qualifier}");
                pos++;
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Query tournament matches by various criteria.
    /// </summary>
    public static void TournamentQueryExamples()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_worldcup.csv");

        Console.WriteLine("=== Tournament Query Examples ===\n");

        // Check match types
        foreach (var match in seasonData.Matches.Take(5))
        {
            if (match.IsLeague)
                Console.WriteLine($"{match.HomeTeam} vs {match.AwayTeam} - League match");
            else if (match.IsGroupStage)
                Console.WriteLine($"{match.HomeTeam} vs {match.AwayTeam} - Group {match.Group}");
            else if (match.IsKnockout)
                Console.WriteLine($"{match.HomeTeam} vs {match.AwayTeam} - {match.Stage.GetDisplayName()}");
        }
        Console.WriteLine();

        // Get all knockout matches
        var knockout = seasonData.GetKnockoutMatches();
        Console.WriteLine($"Total knockout matches: {knockout.Count}");

        // Get matches by specific stage
        var finals = seasonData.GetMatchesByStage(TournamentStage.Final);
        Console.WriteLine($"Finals: {finals.Count}");

        var semis = seasonData.GetMatchesByStage(TournamentStage.SemiFinal);
        Console.WriteLine($"Semi-finals: {semis.Count}");

        // Get all matches from a team
        var argentinaMatches = seasonData.GetMatchesForTeam("Argentina");
        Console.WriteLine($"\nArgentina's tournament:");
        foreach (var match in argentinaMatches)
        {
            var opponent = match.HomeTeam == "Argentina" ? match.AwayTeam : match.HomeTeam;
            var score = match.HomeTeam == "Argentina" ? $"{match.HomeGoals}-{match.AwayGoals}" : $"{match.AwayGoals}-{match.HomeGoals}";
            var stage = match.IsGroupStage ? $"Group {match.Group}" : match.Stage.GetDisplayName();
            Console.WriteLine($"  {stage,-20} vs {opponent,-20} {score}");
        }
    }
}
