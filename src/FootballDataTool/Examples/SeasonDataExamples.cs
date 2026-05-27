using FootballDataTool.Models;
using FootballDataTool.Services;

namespace FootballDataTool.Examples;

/// <summary>
/// Examples demonstrating the new SeasonData / TeamSeason architecture.
/// Shows both match-centric and team-centric approaches.
/// </summary>
public class SeasonDataExamples
{
    public static void RunExamples(string csvPath)
    {
        Console.WriteLine("=== Season Data Architecture Examples ===\n");
        
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile(csvPath);
        
        Console.WriteLine($"Loaded: {seasonData.Metadata.League} {seasonData.Metadata.Season}");
        Console.WriteLine($"  {seasonData.TotalMatches} matches, {seasonData.TotalTeams} teams\n");
        
        Example1_BasicTeamAccess(seasonData);
        Example2_SquadAnalysis(seasonData);
        Example3_FormAndMomentum(seasonData);
        Example4_InjuryImpact(seasonData);
        Example5_MatchToTeamNavigation(seasonData);
        Example6_SeasonWideStats(seasonData);
    }
    
    static void Example1_BasicTeamAccess(SeasonData seasonData)
    {
        Console.WriteLine("--- Example 1: Basic Team Access ---");
        
        // Get a specific team
        var arsenal = seasonData.GetTeam("Arsenal");
        if (arsenal != null)
        {
            Console.WriteLine($"\n{arsenal.Name} Season Stats:");
            Console.WriteLine($"  Matches: {arsenal.MatchesPlayed} (W{arsenal.TotalWins} D{arsenal.TotalDraws} L{arsenal.TotalLosses})");
            Console.WriteLine($"  Points: {arsenal.TotalPoints}");
            Console.WriteLine($"  Goals: {arsenal.GoalsScored} scored, {arsenal.GoalsConceded} conceded (GD: {arsenal.GoalDifference:+#;-#;0})");
            Console.WriteLine($"  Home: W{arsenal.HomeWins} D{arsenal.HomeDraws} L{arsenal.HomeLosses}");
            Console.WriteLine($"  Away: W{arsenal.AwayWins} D{arsenal.AwayDraws} L{arsenal.AwayLosses}");
            
            if (arsenal.HomeStadium != null)
                Console.WriteLine($"  Stadium: {arsenal.HomeStadium.Name}");
            
            if (arsenal.AverageHomeAttendance.HasValue)
                Console.WriteLine($"  Avg Attendance: {arsenal.AverageHomeAttendance:N0}");
        }
        Console.WriteLine();
    }
    
    static void Example2_SquadAnalysis(SeasonData seasonData)
    {
        Console.WriteLine("--- Example 2: Squad Analysis ---\n");
        
        // Find teams with youngest squads
        var youngestSquads = seasonData.Teams.Values
            .Where(t => t.AverageSquadAge.HasValue)
            .OrderBy(t => t.AverageSquadAge)
            .Take(3);
        
        Console.WriteLine("Youngest Squads:");
        foreach (var team in youngestSquads)
        {
            Console.WriteLine($"  {team.Name}: {team.AverageSquadAge:F1} years avg");
            Console.WriteLine($"    Youngest: {team.YoungestPlayer?.Name} ({team.YoungestPlayer?.Age})");
            Console.WriteLine($"    Oldest: {team.OldestPlayer?.Name} ({team.OldestPlayer?.Age})");
        }
        
        // Most used players for a team
        var arsenal = seasonData.GetTeam("Arsenal");
        if (arsenal?.FullSquad.Count > 0)
        {
            Console.WriteLine($"\n{arsenal.Name} Most Used Players:");
            var regularStarters = arsenal.MostUsedPlayers().Take(5);
            foreach (var (player, appearances) in regularStarters)
            {
                Console.WriteLine($"  {player.Name}: {appearances} starts");
            }
        }
        Console.WriteLine();
    }
    
    static void Example3_FormAndMomentum(SeasonData seasonData)
    {
        Console.WriteLine("--- Example 3: Form & Momentum ---\n");
        
        // Get current table
        var table = seasonData.GetCurrentTable();
        
        Console.WriteLine("Top 3 Teams (with form guide):");
        foreach (var record in table.Take(3))
        {
            var team = seasonData.GetTeam(record.TeamName);
            if (team != null)
            {
                var form = team.FormGuide(5);
                Console.WriteLine($"  {record.Position}. {record.TeamName} - {record.Points} pts");
                Console.WriteLine($"     Last 5: {string.Join(", ", form)}");
            }
        }
        
        // Points progression for a team
        var arsenal = seasonData.GetTeam("Arsenal");
        if (arsenal != null)
        {
            Console.WriteLine($"\n{arsenal.Name} Points Progression:");
            var progression = arsenal.PointsProgression().TakeLast(5);
            foreach (var (gw, points) in progression)
            {
                Console.WriteLine($"  GW{gw}: {points} points");
            }
        }
        Console.WriteLine();
    }
    
    static void Example4_InjuryImpact(SeasonData seasonData)
    {
        Console.WriteLine("--- Example 4: Injury Impact ---\n");
        
        if (!seasonData.HasInjuryData)
        {
            Console.WriteLine("  No injury data available in this dataset.\n");
            return;
        }
        
        // Teams most affected by injuries
        var injuryTable = seasonData.GetTeamsByInjuries();
        Console.WriteLine("Teams by Total Injuries:");
        foreach (var (team, count) in injuryTable.Take(5))
        {
            Console.WriteLine($"  {team}: {count} injuries");
            
            var teamObj = seasonData.GetTeam(team);
            if (teamObj != null)
            {
                var injuries = teamObj.AllInjuries();
                var longTerm = injuries.Count(i => i.Severity >= InjurySeverity.Serious);
                if (longTerm > 0)
                    Console.WriteLine($"    ({longTerm} serious/long-term)");
            }
        }
        Console.WriteLine();
    }
    
    static void Example5_MatchToTeamNavigation(SeasonData seasonData)
    {
        Console.WriteLine("--- Example 5: Match-to-Team Navigation ---\n");
        
        // Pick a match and navigate to team objects
        var match = seasonData.Matches.FirstOrDefault(m => m.HasLineupData);
        if (match != null)
        {
            Console.WriteLine($"Match: {match.HomeTeam} {match.ScoreString} {match.AwayTeam}");
            
            var homeTeam = match.GetHomeTeamObject();
            var awayTeam = match.GetAwayTeamObject();
            
            if (homeTeam != null)
            {
                Console.WriteLine($"  {homeTeam.Name}: {homeTeam.TotalPoints} pts, form: {string.Join("", homeTeam.FormGuide(3))}");
            }
            
            if (awayTeam != null)
            {
                Console.WriteLine($"  {awayTeam.Name}: {awayTeam.TotalPoints} pts, form: {string.Join("", awayTeam.FormGuide(3))}");
            }
        }
        Console.WriteLine();
    }
    
    static void Example6_SeasonWideStats(SeasonData seasonData)
    {
        Console.WriteLine("--- Example 6: Season-Wide Statistics ---\n");
        
        Console.WriteLine($"Total Goals: {seasonData.TotalGoals}");
        Console.WriteLine($"Avg Goals/Match: {seasonData.AverageGoalsPerMatch:F2}");
        Console.WriteLine($"Home Wins: {seasonData.HomeWins} ({seasonData.HomeWinPercentage:F1}%)");
        Console.WriteLine($"Away Wins: {seasonData.AwayWins} ({seasonData.AwayWinPercentage:F1}%)");
        Console.WriteLine($"Draws: {seasonData.Draws} ({seasonData.DrawPercentage:F1}%)");
        
        // Best/worst home records
        var bestHomeRecord = seasonData.Teams.Values
            .OrderByDescending(t => t.HomeWins)
            .ThenByDescending(t => t.HomeMatches.Sum(m => m.HomeGoals))
            .First();
        
        var worstHomeRecord = seasonData.Teams.Values
            .OrderBy(t => t.HomeWins)
            .ThenBy(t => t.HomeMatches.Sum(m => m.HomeGoals))
            .First();

        Console.WriteLine($"\nBest Home Record: {bestHomeRecord.Name} (W{bestHomeRecord.HomeWins} D{bestHomeRecord.HomeDraws} L{bestHomeRecord.HomeLosses})");
        Console.WriteLine($"Worst Home Record: {worstHomeRecord.Name} (W{worstHomeRecord.HomeWins} D{worstHomeRecord.HomeDraws} L{worstHomeRecord.HomeLosses})");

        // Zodiac distribution (if birthday data available)
        if (seasonData.HasLineupData)
        {
            var zodiacDist = seasonData.GetZodiacDistribution();
            if (zodiacDist.Any())
            {
                Console.WriteLine("\nMost Common Zodiac Signs:");
                foreach (var (sign, count) in zodiacDist.OrderByDescending(x => x.Value).Take(3))
                {
                    Console.WriteLine($"  {sign}: {count} players");
                }
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Example 7: Loading and analyzing transfer data (CSV or JSON)
    /// </summary>
    public static void Example7_TransferAnalysis(SeasonData seasonData, string transferCsvPath)
    {
        Console.WriteLine("--- Example 7: Transfer Analysis ---");

        // Load transfers from CSV (easier than JSON!)
        seasonData.LoadTransferDataFromCsv(transferCsvPath);

        Console.WriteLine($"\nTransfer data loaded for {seasonData.TeamTransferData.Count} teams\n");

        // Analyze a specific team's transfers
        var team = seasonData.GetTeam("Arsenal");
        if (team?.SeasonInfo != null)
        {
            Console.WriteLine($"{team.Name} Transfers:");

            // Summer signings
            Console.WriteLine($"\n  Summer Signings: {team.SeasonInfo.SummerSignings.Count}");
            foreach (var transfer in team.SeasonInfo.SummerSignings.Take(3))
            {
                var feeStr = transfer.Fee.HasValue 
                    ? $"{transfer.FeeCurrency} {transfer.Fee:N0}" 
                    : transfer.IsLoan ? "Loan" : "Free";

                Console.WriteLine($"    {transfer.Player.Name} ({transfer.PlayerAgeAtTransfer}) from {transfer.FromClub} - {feeStr}");
            }

            // Summer departures
            Console.WriteLine($"\n  Summer Departures: {team.SeasonInfo.SummerDepartures.Count}");
            foreach (var transfer in team.SeasonInfo.SummerDepartures.Take(3))
            {
                var feeStr = transfer.Fee.HasValue 
                    ? $"{transfer.FeeCurrency} {transfer.Fee:N0}" 
                    : "Free";

                Console.WriteLine($"    {transfer.Player.Name} ({transfer.PlayerAgeAtTransfer}) to {transfer.ToClub} - {feeStr}");
            }

            // Net spend
            var totalSpent = team.SeasonInfo.SummerSignings.Sum(t => t.Fee ?? 0) 
                           + team.SeasonInfo.WinterSignings.Sum(t => t.Fee ?? 0);
            var totalReceived = team.SeasonInfo.SummerDepartures.Sum(t => t.Fee ?? 0) 
                              + team.SeasonInfo.WinterDepartures.Sum(t => t.Fee ?? 0);
            var netSpend = totalSpent - totalReceived;

            Console.WriteLine($"\n  Total Spent: {team.SeasonInfo.SummerSignings.FirstOrDefault()?.FeeCurrency ?? "GBP"} {totalSpent:N0}");
            Console.WriteLine($"  Total Received: {team.SeasonInfo.SummerSignings.FirstOrDefault()?.FeeCurrency ?? "GBP"} {totalReceived:N0}");
            Console.WriteLine($"  Net Spend: {team.SeasonInfo.SummerSignings.FirstOrDefault()?.FeeCurrency ?? "GBP"} {netSpend:+#,0;-#,0;0}");
        }

        // League-wide transfer stats
        Console.WriteLine("\n\nLeague-Wide Transfer Analysis:");

        var allSignings = seasonData.TeamTransferData.Values
            .SelectMany(t => t.SummerSignings.Concat(t.WinterSignings))
            .ToList();

        Console.WriteLine($"  Total Signings: {allSignings.Count}");
        Console.WriteLine($"  Average Age: {allSignings.Where(t => t.PlayerAgeAtTransfer.HasValue).Average(t => t.PlayerAgeAtTransfer):F1}");

        var biggestSigning = allSignings
            .Where(t => t.Fee.HasValue)
            .OrderByDescending(t => t.Fee)
            .FirstOrDefault();

        if (biggestSigning != null)
        {
            Console.WriteLine($"  Biggest Signing: {biggestSigning.Player.Name} to {biggestSigning.ToClub} for {biggestSigning.FeeCurrency} {biggestSigning.Fee:N0}");
        }

        Console.WriteLine();
    }
}
