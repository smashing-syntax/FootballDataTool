using FootballDataTool.Models;
using FootballDataTool.Services;
using Spectre.Console;

namespace FootballDataTool.Visualisers;

/// <summary>
/// Visualizes team age profiles and statistics.
/// </summary>
public static class AgeProfileVisualiser
{
    public static void Render(List<Match> matches)
    {
        var matchesWithData = matches
            .Where(m => m.ExtendedData != null)
            .ToList();

        if (!matchesWithData.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No matches with lineup data available.[/]");
            return;
        }

        // Calculate average ages per team
        var teamAges = new Dictionary<string, List<double>>();

        foreach (var match in matchesWithData)
        {
            if (match.ExtendedData?.HomeAverageAge.HasValue == true)
            {
                if (!teamAges.ContainsKey(match.HomeTeam))
                    teamAges[match.HomeTeam] = new List<double>();
                teamAges[match.HomeTeam].Add(match.ExtendedData.HomeAverageAge.Value);
            }

            if (match.ExtendedData?.AwayAverageAge.HasValue == true)
            {
                if (!teamAges.ContainsKey(match.AwayTeam))
                    teamAges[match.AwayTeam] = new List<double>();
                teamAges[match.AwayTeam].Add(match.ExtendedData.AwayAverageAge.Value);
            }
        }

        if (!teamAges.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No age data available in lineups.[/]");
            return;
        }

        // Calculate season averages
        var seasonAverages = teamAges
            .Select(kvp => new
            {
                Team = kvp.Key,
                AvgAge = kvp.Value.Average(),
                MinAge = kvp.Value.Min(),
                MaxAge = kvp.Value.Max(),
                Matches = kvp.Value.Count
            })
            .OrderBy(x => x.AvgAge)
            .ToList();

        // Create table
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold cyan]Team Age Profiles[/]")
            .AddColumn(new TableColumn("[bold]Team[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Avg Age[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Min[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Max[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Range[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Profile[/]").LeftAligned());

        foreach (var team in seasonAverages)
        {
            var range = team.MaxAge - team.MinAge;
            var profile = team.AvgAge < 24.5 ? "[green]Youth[/]"
                        : team.AvgAge > 27.5 ? "[yellow]Experience[/]"
                        : "[cyan]Balanced[/]";

            // Create age bar
            var normalizedAge = (team.AvgAge - 20) / 15.0; // Scale 20-35 to 0-1
            var barLength = (int)(normalizedAge * 20);
            var bar = new string('█', Math.Max(1, Math.Min(20, barLength)));

            table.AddRow(
                Markup.Escape(team.Team),
                $"{team.AvgAge:F1}",
                $"{team.MinAge:F1}",
                $"{team.MaxAge:F1}",
                $"{range:F1}",
                profile);
        }

        AnsiConsole.Write(table);

        // Show interesting stats
        AnsiConsole.WriteLine();
        var youngest = seasonAverages.First();
        var oldest = seasonAverages.Last();

        AnsiConsole.MarkupLine($"[green]Youngest team:[/] [bold]{Markup.Escape(youngest.Team)}[/] ({youngest.AvgAge:F1} years)");
        AnsiConsole.MarkupLine($"[yellow]Oldest team:[/] [bold]{Markup.Escape(oldest.Team)}[/] ({oldest.AvgAge:F1} years)");
        AnsiConsole.MarkupLine($"[cyan]Age spread:[/] {oldest.AvgAge - youngest.AvgAge:F1} years");

        // Find youngest and oldest players across all matches
        var allPlayers = matches
            .SelectMany(m => new[]
            {
                m.ExtendedData?.HomeYoungestPlayer,
                m.ExtendedData?.AwayYoungestPlayer,
                m.ExtendedData?.HomeOldestPlayer,
                m.ExtendedData?.AwayOldestPlayer
            })
            .Where(p => p?.Age.HasValue == true)
            .ToList();

        if (allPlayers.Any())
        {
            var youngestPlayer = allPlayers.OrderBy(p => p!.Age).First();
            var oldestPlayer = allPlayers.OrderByDescending(p => p!.Age).First();

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Youngest player:[/] {Markup.Escape(youngestPlayer!.ToString())} ({youngestPlayer.Age} years)");
            AnsiConsole.MarkupLine($"[yellow]Oldest player:[/] {Markup.Escape(oldestPlayer!.ToString())} ({oldestPlayer.Age} years)");
        }

        // Age distribution
        AnsiConsole.WriteLine();
        ShowAgeDistribution(matchesWithData);
    }

    private static void ShowAgeDistribution(List<Match> matches)
    {
        var allAges = matches
            .Where(m => m.ExtendedData != null)
            .SelectMany(m => m.ExtendedData!.HomeStartingLineup.Concat(m.ExtendedData.AwayStartingLineup))
            .Where(p => p.Age.HasValue)
            .Select(p => p.Age!.Value)
            .ToList();

        if (!allAges.Any())
            return;

        var ageBrackets = new[]
        {
            ("18-21", allAges.Count(a => a >= 18 && a <= 21)),
            ("22-25", allAges.Count(a => a >= 22 && a <= 25)),
            ("26-29", allAges.Count(a => a >= 26 && a <= 29)),
            ("30+", allAges.Count(a => a >= 30))
        };

        var chart = new BarChart()
            .Width(60)
            .Label("[bold]Age Distribution Across All Lineups[/]");

        foreach (var (bracket, count) in ageBrackets)
        {
            if (count > 0)
                chart.AddItem(bracket, count, GetAgeColor(bracket));
        }

        AnsiConsole.Write(chart);
    }

    private static Color GetAgeColor(string bracket) => bracket switch
    {
        "18-21" => Color.Green,
        "22-25" => Color.Blue,
        "26-29" => Color.Yellow,
        "30+" => Color.Orange1,
        _ => Color.White
    };
}
