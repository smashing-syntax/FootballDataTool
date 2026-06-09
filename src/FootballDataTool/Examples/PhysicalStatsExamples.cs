using FootballDataTool.Services;
using FootballDataTool.Models;

namespace FootballDataTool.Examples;

/// <summary>
/// Examples demonstrating physical stats analytics (height, weight, BMI, etc.).
/// </summary>
public static class PhysicalStatsExamples
{
    /// <summary>
    /// Analyze World Cup squads' physical characteristics.
    /// </summary>
    public static void WorldCupPhysicalStats()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_worldcup.csv");

        // Load squad data with physical stats
        seasonData.LoadSquadDataFromCsv("data/examples/squad_examples_worldcup.csv");

        Console.WriteLine("=== FIFA World Cup 2022 - Physical Stats Analysis ===\n");

        // Average height by position
        Console.WriteLine("📏 Average Height by Position:");
        var heightByPosition = seasonData.GetAverageHeightByPosition();
        foreach (var (position, avgHeight) in heightByPosition.OrderByDescending(x => x.Value))
        {
            Console.WriteLine($"  {position,-3}: {avgHeight} cm");
        }
        Console.WriteLine();

        // Average weight by position
        Console.WriteLine("⚖️  Average Weight by Position:");
        var weightByPosition = seasonData.GetAverageWeightByPosition();
        foreach (var (position, avgWeight) in weightByPosition.OrderByDescending(x => x.Value))
        {
            Console.WriteLine($"  {position,-3}: {avgWeight} kg");
        }
        Console.WriteLine();

        // Average BMI by position
        Console.WriteLine("💪 Average BMI by Position:");
        var bmiByPosition = seasonData.GetAverageBMIByPosition();
        foreach (var (position, avgBMI) in bmiByPosition.OrderByDescending(x => x.Value))
        {
            Console.WriteLine($"  {position,-3}: {avgBMI}");
        }
        Console.WriteLine();

        // Tallest and shortest players
        var (tallest, shortest) = seasonData.GetHeightExtremes(5);

        Console.WriteLine("🏔️  Tallest Players:");
        foreach (var player in tallest)
        {
            Console.WriteLine($"  {player.Name,-25} {player.Height}cm ({player.Position}) - {player.Nationality}");
        }
        Console.WriteLine();

        Console.WriteLine("🐜 Shortest Players:");
        foreach (var player in shortest)
        {
            Console.WriteLine($"  {player.Name,-25} {player.Height}cm ({player.Position}) - {player.Nationality}");
        }
        Console.WriteLine();

        // Preferred foot distribution
        Console.WriteLine("🦶 Preferred Foot Distribution:");
        var footDist = seasonData.GetPreferredFootDistribution();
        foreach (var (foot, count) in footDist.OrderByDescending(x => x.Value))
        {
            var percentage = (count * 100.0 / seasonData.GetAllPlayers().Count);
            Console.WriteLine($"  {foot,-6}: {count,3} players ({percentage:F1}%)");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Analyze performance by height bracket.
    /// Shows which height ranges are most productive.
    /// </summary>
    public static void PerformanceByHeightBracket()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_worldcup.csv");
        seasonData.LoadSquadDataFromCsv("data/examples/squad_examples_worldcup.csv");

        Console.WriteLine("=== Performance by Height Bracket ===\n");
        Console.WriteLine("Height Range  | Players | Goals | Assists | Total");
        Console.WriteLine("--------------|---------|-------|---------|-------");

        var brackets = seasonData.GetPerformanceByHeightBracket();
        foreach (var (bracket, players, goals, assists) in brackets)
        {
            var total = goals + assists;
            Console.WriteLine($"{bracket,-13} | {players,7} | {goals,5} | {assists,7} | {total,5}");
        }
        Console.WriteLine();

        // Find most productive bracket
        var mostProductive = brackets
            .OrderByDescending(b => b.Goals + b.Assists)
            .First();

        Console.WriteLine($"🏆 Most productive height bracket: {mostProductive.HeightBracket}");
        Console.WriteLine($"   {mostProductive.Players} players, {mostProductive.Goals} goals, {mostProductive.Assists} assists");
        Console.WriteLine();
    }

    /// <summary>
    /// Compare physical attributes across teams.
    /// </summary>
    public static void TeamPhysicalComparison()
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_worldcup.csv");
        seasonData.LoadSquadDataFromCsv("data/examples/squad_examples_worldcup.csv");

        Console.WriteLine("=== Team Physical Comparison ===\n");
        Console.WriteLine("Team          | Avg Height | Avg Weight | Avg BMI | Avg Age");
        Console.WriteLine("--------------|------------|------------|---------|--------");

        var teamsWithStats = seasonData.Teams.Values
            .Select(t => new
            {
                Team = t.Name,
                AvgHeight = t.FullSquad.Where(p => p.Height.HasValue).Select(p => p.Height!.Value).DefaultIfEmpty().Average(),
                AvgWeight = t.FullSquad.Where(p => p.Weight.HasValue).Select(p => p.Weight!.Value).DefaultIfEmpty().Average(),
                AvgBMI = t.FullSquad.Where(p => p.BMI.HasValue).Select(p => p.BMI!.Value).DefaultIfEmpty().Average(),
                AvgAge = t.AverageSquadAge ?? 0
            })
            .OrderByDescending(t => t.AvgHeight)
            .ToList();

        foreach (var team in teamsWithStats)
        {
            Console.WriteLine($"{team.Team,-13} | {team.AvgHeight,10:F1} | {team.AvgWeight,10:F1} | {team.AvgBMI,7:F2} | {team.AvgAge,6:F1}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Analyze individual player physical profile with stats.
    /// </summary>
    public static void PlayerPhysicalProfile(string playerName)
    {
        var csvService = new CsvDataService();
        var seasonData = csvService.LoadSeasonDataFromFile("data/examples/tournament_examples_worldcup.csv");
        seasonData.LoadSquadDataFromCsv("data/examples/squad_examples_worldcup.csv");

        var player = seasonData.GetAllPlayers()
            .FirstOrDefault(p => p.Name.Contains(playerName, StringComparison.OrdinalIgnoreCase));

        if (player == null)
        {
            Console.WriteLine($"Player '{playerName}' not found.");
            return;
        }

        Console.WriteLine($"=== {player.Name} - Physical Profile ===\n");
        Console.WriteLine($"Position:      {player.Position ?? "Unknown"}");
        Console.WriteLine($"Nationality:   {player.Nationality ?? "Unknown"}");
        Console.WriteLine($"Age:           {player.Age ?? 0}");
        Console.WriteLine($"Height:        {player.Height} cm");
        Console.WriteLine($"Weight:        {player.Weight} kg");
        Console.WriteLine($"BMI:           {player.BMI:F2}");
        Console.WriteLine($"Preferred Foot: {player.PreferredFoot ?? "Unknown"}");

        if (player.Height.HasValue)
        {
            var avgHeightForPosition = seasonData.GetAverageHeightByPosition()
                .FirstOrDefault(x => x.Key == player.Position).Value;

            if (avgHeightForPosition > 0)
            {
                var diff = player.Height.Value - avgHeightForPosition;
                Console.WriteLine($"\nHeight vs position average: {(diff >= 0 ? "+" : "")}{diff:F1}cm");
            }
        }

        if (player.Weight.HasValue)
        {
            var avgWeightForPosition = seasonData.GetAverageWeightByPosition()
                .FirstOrDefault(x => x.Key == player.Position).Value;

            if (avgWeightForPosition > 0)
            {
                var diff = player.Weight.Value - avgWeightForPosition;
                Console.WriteLine($"Weight vs position average: {(diff >= 0 ? "+" : "")}{diff:F1}kg");
            }
        }

        Console.WriteLine();
    }
}
