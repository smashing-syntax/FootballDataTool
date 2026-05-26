using FootballDataTool.Models;
using FootballDataTool.Services;
using Spectre.Console;

namespace FootballDataTool.Visualisers;

public static class HomeFormVisualiser
{
    public static void Render(MatchAnalyzer analyzer)
    {
        var teams = analyzer.GetTeams();
        var gameweeks = analyzer.GetGameweeks();
        var homeForm = analyzer.GetHomeFormByGameweek();

        AnsiConsole.WriteLine();

        if (gameweeks.Count > 20)
        {
            AnsiConsole.MarkupLine("[yellow]Season has more than 20 gameweeks — displaying GW 1–20 first.[/]");
            gameweeks = gameweeks.Take(20).ToList();
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold cyan]Home Form by Gameweek[/]")
            .AddColumn(new TableColumn("[bold]Team[/]").LeftAligned());

        foreach (var gw in gameweeks)
            table.AddColumn(new TableColumn($"[bold]GW{gw}[/]").Centered());

        foreach (var team in teams)
        {
            var row = new List<string> { $"[white]{Markup.Escape(team)}[/]" };

            foreach (var gw in gameweeks)
            {
                var match = homeForm[team][gw];
                row.Add(FormatCell(match, team));
            }

            table.AddRow(row.ToArray());
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("[dim]W=[green]Win[/]  D=[yellow]Draw[/]  L=[red]Loss[/]  -=No home game[/]");
        AnsiConsole.WriteLine();
    }

    private static string FormatCell(Match? match, string team)
    {
        if (match == null)
            return "[dim]-[/]";

        var score = match.ScoreString;

        return match.Result switch
        {
            "H" => $"[bold green]W {Markup.Escape(score)}[/]",
            "D" => $"[bold yellow]D {Markup.Escape(score)}[/]",
            "A" => $"[bold red]L {Markup.Escape(score)}[/]",
            _   => $"[dim]{Markup.Escape(score)}[/]"
        };
    }
}
