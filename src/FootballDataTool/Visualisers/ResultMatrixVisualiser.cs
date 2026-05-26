using FootballDataTool.Services;
using Spectre.Console;

namespace FootballDataTool.Visualisers;

public static class ResultMatrixVisualiser
{
    public static void Render(MatchAnalyzer analyzer)
    {
        AnsiConsole.WriteLine();

        string mode = "Home (row = home team)";
        bool modeSelected = false;

        if (!Console.IsInputRedirected)
        {
            try
            {
                mode = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select matrix [bold]view mode[/]:")
                        .AddChoices("Home (row = home team)", "Away (row = away team)", "Aggregate (combined)"));
                modeSelected = true;
            }
            catch (NotSupportedException) { }
        }

        if (!modeSelected)
        {
            AnsiConsole.MarkupLine("Select matrix [bold]view mode[/]:");
            AnsiConsole.MarkupLine("  [bold]1.[/] Home (row = home team)");
            AnsiConsole.MarkupLine("  [bold]2.[/] Away (row = away team)");
            AnsiConsole.MarkupLine("  [bold]3.[/] Aggregate (combined)");
            AnsiConsole.Markup("[green]>[/] Enter number (default 1): ");
            var line = Console.ReadLine()?.Trim();
            mode = line switch
            {
                "2" => "Away (row = away team)",
                "3" => "Aggregate (combined)",
                _   => "Home (row = home team)"
            };
        }

        var matrixMode = mode switch
        {
            "Home (row = home team)"  => MatrixMode.Home,
            "Away (row = away team)"  => MatrixMode.Away,
            _                         => MatrixMode.Aggregate
        };

        var matrix = analyzer.GetResultMatrix(matrixMode);
        var teams = analyzer.GetTeams();

        var modeLabel = matrixMode switch
        {
            MatrixMode.Home      => "Home Results",
            MatrixMode.Away      => "Away Results",
            MatrixMode.Aggregate => "Aggregate Results",
            _                    => "Results"
        };

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[bold cyan]{modeLabel} Matrix[/]");

        // Header row: first column is team name, rest are opponent names
        table.AddColumn(new TableColumn(
            matrixMode == MatrixMode.Away ? "[bold]Away Team ↓ / Home Team →[/]" : "[bold]Team ↓ / Opponent →[/]"
        ).LeftAligned());

        foreach (var opp in teams)
            table.AddColumn(new TableColumn($"[bold]{Markup.Escape(ShortName(opp))}[/]").Centered());

        foreach (var team in teams)
        {
            var row = new List<string> { $"[white bold]{Markup.Escape(team)}[/]" };

            foreach (var opp in teams)
            {
                if (string.Equals(team, opp, StringComparison.OrdinalIgnoreCase))
                {
                    row.Add("[dim]—[/]");
                    continue;
                }

                var cell = matrix[team][opp];
                row.Add(FormatCell(cell));
            }

            table.AddRow(row.ToArray());
        }

        AnsiConsole.Write(table);

        RenderLegend(matrixMode);
        AnsiConsole.WriteLine();

        // Also show a head-to-head summary between two selected teams
        bool showH2H = false;
        try
        {
            showH2H = !Console.IsInputRedirected &&
                      AnsiConsole.Confirm("View head-to-head detail between two teams?", defaultValue: false);
        }
        catch (NotSupportedException) { }

        if (!showH2H && Console.IsInputRedirected)
        {
            AnsiConsole.Markup("View head-to-head detail between two teams? (y/N): ");
            var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
            showH2H = ans == "y" || ans == "yes";
        }
        if (showH2H)
            RenderHeadToHead(analyzer, teams);
    }

    // ── Head-to-head detail ───────────────────────────────────────────────────

    private static void RenderHeadToHead(MatchAnalyzer analyzer, List<string> teams)
    {
        var (team1, team2) = PickTwoTeams(teams);
        ShowHeadToHeadTable(analyzer, team1, team2);
    }

    private static (string Team1, string Team2) PickTwoTeams(List<string> teams)
    {
        if (!Console.IsInputRedirected)
        {
            try
            {
                var t1 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [bold]first team[/]:")
                        .AddChoices(teams));

                var rem = teams.Where(t => !string.Equals(t, t1, StringComparison.OrdinalIgnoreCase)).ToList();
                var t2 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [bold]second team[/]:")
                        .AddChoices(rem));

                return (t1, t2);
            }
            catch (NotSupportedException) { }
        }

        // Plain text fallback
        AnsiConsole.MarkupLine("Select [bold]first team[/]:");
        for (int i = 0; i < teams.Count; i++)
            AnsiConsole.MarkupLine($"  [bold]{i + 1}.[/] {Markup.Escape(teams[i])}");
        AnsiConsole.Markup("[green]>[/] ");
        int t1idx = 0;
        if (int.TryParse(Console.ReadLine()?.Trim(), out int t1n) && t1n >= 1 && t1n <= teams.Count)
            t1idx = t1n - 1;
        string team1 = teams[t1idx];

        var remaining = teams.Where(t => !string.Equals(t, team1, StringComparison.OrdinalIgnoreCase)).ToList();
        AnsiConsole.MarkupLine("Select [bold]second team[/]:");
        for (int i = 0; i < remaining.Count; i++)
            AnsiConsole.MarkupLine($"  [bold]{i + 1}.[/] {Markup.Escape(remaining[i])}");
        AnsiConsole.Markup("[green]>[/] ");
        string team2 = remaining[0];
        if (int.TryParse(Console.ReadLine()?.Trim(), out int t2n) && t2n >= 1 && t2n <= remaining.Count)
            team2 = remaining[t2n - 1];

        return (team1, team2);
    }

    private static void ShowHeadToHeadTable(MatchAnalyzer analyzer, string team1, string team2)
    {
        var homeMatrix = analyzer.GetResultMatrix(MatrixMode.Home);
        var awayMatrix = analyzer.GetResultMatrix(MatrixMode.Away);

        AnsiConsole.WriteLine();

        var h2hTable = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[bold cyan]Head-to-Head: {Markup.Escape(team1)} vs {Markup.Escape(team2)}[/]")
            .AddColumn(new TableColumn("[bold]Fixture[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Score[/]").Centered())
            .AddColumn(new TableColumn("[bold]Result[/]").Centered());

        // Home fixture: team1 at home
        var t1Home = homeMatrix[team1][team2];
        if (t1Home.HasValue)
        {
            var (scored, conceded) = t1Home.Value;
            h2hTable.AddRow(
                $"{Markup.Escape(team1)} [dim](H)[/] vs {Markup.Escape(team2)}",
                $"{scored}-{conceded}",
                FormatResultLabel(scored, conceded, team1));
        }

        // Away fixture: team1 away
        var t1Away = awayMatrix[team1][team2];
        if (t1Away.HasValue)
        {
            var (scored, conceded) = t1Away.Value;
            h2hTable.AddRow(
                $"{Markup.Escape(team1)} [dim](A)[/] vs {Markup.Escape(team2)}",
                $"{scored}-{conceded}",
                FormatResultLabel(scored, conceded, team1));
        }

        AnsiConsole.Write(h2hTable);
        AnsiConsole.WriteLine();
    }

    // ── Formatting helpers ────────────────────────────────────────────────────

    private static string FormatCell((int Scored, int Conceded)? cell)
    {
        if (!cell.HasValue)
            return "[dim]-[/]";

        var (scored, conceded) = cell.Value;
        var score = $"{scored}-{conceded}";

        if (scored > conceded) return $"[bold green]{Markup.Escape(score)}[/]";
        if (scored == conceded) return $"[bold yellow]{Markup.Escape(score)}[/]";
        return $"[bold red]{Markup.Escape(score)}[/]";
    }

    private static string FormatResultLabel(int scored, int conceded, string teamLabel)
    {
        if (scored > conceded) return $"[green]Win[/]";
        if (scored == conceded) return $"[yellow]Draw[/]";
        return $"[red]Loss[/]";
    }

    private static void RenderLegend(MatrixMode mode)
    {
        AnsiConsole.MarkupLine("[dim]Score shown from the [bold]row team's[/] perspective.[/]");
        AnsiConsole.MarkupLine("[dim][green]Green[/]=Win  [yellow]Yellow[/]=Draw  [red]Red[/]=Loss[/]");

        if (mode == MatrixMode.Home)
            AnsiConsole.MarkupLine("[dim]Row = home team, Column = away team[/]");
        else if (mode == MatrixMode.Away)
            AnsiConsole.MarkupLine("[dim]Row = away team, Column = home team[/]");
        else
            AnsiConsole.MarkupLine("[dim]Aggregate: goals scored across all meetings (home + away)[/]");
    }

    /// <summary>Returns a shortened name that fits better in column headers.</summary>
    private static string ShortName(string name)
    {
        if (name.Length <= 10) return name;

        // Abbreviate multi-word names: e.g. "Manchester City" → "ManCit"
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
            return string.Concat(parts.Select(Abbreviate));

        // Single long word: truncate with ellipsis
        return name[..8] + "..";

        static string Abbreviate(string word)
        {
            int length = Math.Min(word.Length, 4);
            return char.ToUpper(word[0]) + word[1..length];
        }
    }
}
