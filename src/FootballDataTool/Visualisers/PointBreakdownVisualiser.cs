using FootballDataTool.Services;
using Spectre.Console;

namespace FootballDataTool.Visualisers;

public static class PointBreakdownVisualiser
{
    public static void Render(MatchAnalyzer analyzer)
    {
        AnsiConsole.WriteLine();

        var gameweeks = analyzer.GetGameweeks();
        var cumulativePoints = analyzer.GetCumulativePoints();
        var teams = analyzer.GetTeams();

        // ── Cumulative points table ────────────────────────────────────────────
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold cyan]Cumulative Points by Gameweek[/]")
            .AddColumn(new TableColumn("[bold]Team[/]").LeftAligned());

        foreach (var gw in gameweeks)
            table.AddColumn(new TableColumn($"[bold]GW{gw}[/]").Centered());

        // Resolve final-standings order so the table reads top→bottom
        var finalStandings = analyzer.GetStandings();
        var orderedTeams = finalStandings.Select(r => r.TeamName).ToList();

        var palette = BuildPalette(orderedTeams);

        foreach (var team in orderedTeams)
        {
            var row = new List<string> { $"[{palette[team]}]{Markup.Escape(team)}[/]" };

            foreach (var gw in gameweeks)
            {
                var entry = cumulativePoints[team].FirstOrDefault(p => p.Gameweek == gw);
                row.Add(entry.CumulativePoints > 0
                    ? $"[{palette[team]}]{entry.CumulativePoints}[/]"
                    : "[dim]0[/]");
            }

            table.AddRow(row.ToArray());
        }

        AnsiConsole.Write(table);

        // ── Bar chart for points at the last completed gameweek ────────────────
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold cyan]Final Points Tally[/]");

        var chart = new BarChart()
            .Width(Console.WindowWidth - 4)
            .Label("[grey]Points[/]");

        foreach (var team in orderedTeams)
        {
            var finalPts = cumulativePoints[team].LastOrDefault().CumulativePoints;
            chart.AddItem(team, finalPts, GetBarColor(palette[team]));
        }

        AnsiConsole.Write(chart);

        // ── Cross-group analysis ───────────────────────────────────────────────
        RenderCrossGroupAnalysis(analyzer, orderedTeams);
    }

    private static void RenderCrossGroupAnalysis(MatchAnalyzer analyzer, List<string> orderedTeams)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold cyan]Cross-Group Points Analysis[/]");
        AnsiConsole.MarkupLine("[dim]How many points did each group take against the other?[/]");
        AnsiConsole.WriteLine();

        int teamCount = orderedTeams.Count;
        if (teamCount < 4)
        {
            AnsiConsole.MarkupLine("[yellow]Not enough teams for cross-group analysis (need at least 4).[/]");
            return;
        }

        // Offer predefined splits based on league size, e.g. Top 6 vs Bottom 3
        // Or allow custom selection
        var groupSize = Math.Max(2, teamCount / 4);
        var topGroup = orderedTeams.Take(groupSize).ToList();
        var bottomGroup = orderedTeams.TakeLast(groupSize).ToList();

        var result = analyzer.GetCrossGroupPoints(topGroup, bottomGroup);

        var xTable = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[bold]Top {groupSize} vs Bottom {groupSize}[/]")
            .AddColumn(new TableColumn("[bold]Metric[/]").LeftAligned())
            .AddColumn(new TableColumn($"[bold green]Top {groupSize}[/]").Centered())
            .AddColumn(new TableColumn($"[bold red]Bottom {groupSize}[/]").Centered());

        xTable.AddRow("Teams",
            string.Join(", ", result.Group1Teams.Select(t => Markup.Escape(t))),
            string.Join(", ", result.Group2Teams.Select(t => Markup.Escape(t))));

        xTable.AddRow("Points from cross-fixtures",
            $"[green]{result.Group1Points}[/]",
            $"[red]{result.Group2Points}[/]");

        xTable.AddRow("Goals scored in cross-fixtures",
            $"[green]{result.Group1Goals}[/]",
            $"[red]{result.Group2Goals}[/]");

        AnsiConsole.Write(xTable);

        // ── Custom cross-group prompt ──────────────────────────────────────────
        AnsiConsole.WriteLine();

        bool custom = false;
        try
        {
            custom = !Console.IsInputRedirected &&
                     AnsiConsole.Confirm("Run a custom cross-group comparison?", defaultValue: false);
        }
        catch (NotSupportedException) { }

        if (!custom && Console.IsInputRedirected)
        {
            AnsiConsole.Markup("Run a custom cross-group comparison? (y/N): ");
            var answer = Console.ReadLine()?.Trim().ToLowerInvariant();
            custom = answer == "y" || answer == "yes";
        }
        if (!custom) return;

        var allTeams = orderedTeams;

        var g1 = PickTeams("Select [green]Group 1[/] teams:", allTeams);

        var remaining = allTeams.Except(g1).ToList();
        if (remaining.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]All teams are in Group 1 — cannot create Group 2.[/]");
            return;
        }

        var g2 = PickTeams("Select [red]Group 2[/] teams:", remaining);

        var customResult = analyzer.GetCrossGroupPoints(g1, g2);

        var customTable = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Custom Cross-Group Analysis[/]")
            .AddColumn(new TableColumn("[bold]Metric[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold green]Group 1[/]").Centered())
            .AddColumn(new TableColumn("[bold red]Group 2[/]").Centered());

        customTable.AddRow("Teams",
            string.Join(", ", customResult.Group1Teams.Select(t => Markup.Escape(t))),
            string.Join(", ", customResult.Group2Teams.Select(t => Markup.Escape(t))));

        customTable.AddRow("Points from cross-fixtures",
            $"[green]{customResult.Group1Points}[/]",
            $"[red]{customResult.Group2Points}[/]");

        customTable.AddRow("Goals scored in cross-fixtures",
            $"[green]{customResult.Group1Goals}[/]",
            $"[red]{customResult.Group2Goals}[/]");

        AnsiConsole.Write(customTable);
        AnsiConsole.WriteLine();
    }

    private static List<string> PickTeams(string prompt, List<string> choices)
    {
        if (!Console.IsInputRedirected)
        {
            try
            {
                return AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title(prompt)
                        .AddChoices(choices));
            }
            catch (NotSupportedException) { }
        }

        AnsiConsole.MarkupLine(prompt);
        for (int i = 0; i < choices.Count; i++)
            AnsiConsole.MarkupLine($"  [bold]{i + 1}.[/] {Markup.Escape(choices[i])}");

        AnsiConsole.MarkupLine("Enter comma-separated numbers (e.g. 1,3,4):");
        AnsiConsole.Markup("[green]>[/] ");
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        var selected = new List<string>();
        foreach (var token in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (int.TryParse(token.Trim(), out int idx) && idx >= 1 && idx <= choices.Count)
                selected.Add(choices[idx - 1]);
        }
        return selected.Count > 0 ? selected : [choices[0]];
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Dictionary<string, string> BuildPalette(List<string> teams)
    {
        string[] colours =
        [
            "cyan1", "green", "yellow", "blue", "magenta", "red",
            "aqua", "lime", "gold1", "dodgerblue1", "orchid", "tomato",
            "white", "grey", "purple", "orange1"
        ];

        var palette = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < teams.Count; i++)
            palette[teams[i]] = colours[i % colours.Length];

        return palette;
    }

    private static Color GetBarColor(string spectreColour) => spectreColour switch
    {
        "cyan1"       => Color.Cyan1,
        "green"       => Color.Green,
        "yellow"      => Color.Yellow,
        "blue"        => Color.Blue,
        "magenta"     => Color.Magenta1,
        "red"         => Color.Red,
        "aqua"        => Color.Aqua,
        "lime"        => Color.Lime,
        "gold1"       => Color.Gold1,
        "dodgerblue1" => Color.DodgerBlue1,
        "orchid"      => Color.Orchid,
        "tomato"      => Color.OrangeRed1,
        "white"       => Color.White,
        "grey"        => Color.Grey,
        "purple"      => Color.Purple,
        "orange1"     => Color.Orange1,
        _             => Color.White
    };
}
