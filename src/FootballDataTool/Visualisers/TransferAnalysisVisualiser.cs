using FootballDataTool.Models;
using FootballDataTool.Services;
using Spectre.Console;

namespace FootballDataTool.Visualisers;

/// <summary>
/// Visualizes transfer activity and spending patterns.
/// </summary>
public static class TransferAnalysisVisualiser
{
    public static void Render(Dictionary<string, TeamSeasonInfo> teamData)
    {
        if (!teamData.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No transfer data available.[/]");
            AnsiConsole.MarkupLine("[dim]Place team transfer JSON files in the data directory.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[bold cyan]Transfer Analysis - {teamData.Count} team(s)[/]\n");

        var stats = teamData
            .Select(kvp => new
            {
                Team = kvp.Key,
                Data = kvp.Value,
                Stats = TeamDataLoader.GetTransferStatistics(kvp.Value)
            })
            .OrderByDescending(x => Math.Abs(x.Stats.NetSpend))
            .ToList();

        // Overview table
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Transfer Spending Overview[/]")
            .AddColumn(new TableColumn("[bold]Team[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]In[/]").Centered())
            .AddColumn(new TableColumn("[bold]Out[/]").Centered())
            .AddColumn(new TableColumn("[bold]Spent[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Received[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Net[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Avg Age In[/]").Centered())
            .AddColumn(new TableColumn("[bold]Avg Age Out[/]").Centered());

        foreach (var item in stats)
        {
            var netColor = item.Stats.NetSpend > 0 ? "red" 
                         : item.Stats.NetSpend < 0 ? "green" 
                         : "white";

            table.AddRow(
                Markup.Escape(item.Team),
                item.Stats.TotalSignings.ToString(),
                item.Stats.TotalDepartures.ToString(),
                $"£{item.Stats.TotalSpent / 1_000_000:F1}m",
                $"£{item.Stats.TotalReceived / 1_000_000:F1}m",
                $"[{netColor}]£{item.Stats.NetSpend / 1_000_000:F1}m[/]",
                item.Stats.AverageAgeIn.HasValue ? $"{item.Stats.AverageAgeIn:F1}" : "-",
                item.Stats.AverageAgeOut.HasValue ? $"{item.Stats.AverageAgeOut:F1}" : "-");
        }

        AnsiConsole.Write(table);

        // Detailed breakdown for each team
        AnsiConsole.WriteLine();
        
        var teamChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Select a team for detailed analysis:[/]")
                .AddChoices(stats.Select(s => s.Team).Concat(new[] { "← Back to menu" })));

        if (teamChoice == "← Back to menu")
            return;

        var selectedTeam = stats.First(s => s.Team == teamChoice);
        RenderTeamDetail(selectedTeam.Team, selectedTeam.Data, selectedTeam.Stats);
    }

    private static void RenderTeamDetail(string teamName, TeamSeasonInfo data, TransferStatistics stats)
    {
        AnsiConsole.Clear();
        
        var panel = new Panel(
            Align.Center(new Markup($"[bold cyan]{Markup.Escape(teamName)}[/] - {Markup.Escape(data.Season)}"))
        ).Border(BoxBorder.Rounded);
        
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        // Manager info
        if (data.StartingManager != null)
        {
            var managerInfo = $"[bold]Manager:[/] {Markup.Escape(data.StartingManager.Name)}";
            if (data.StartingManager.Age.HasValue)
                managerInfo += $" ({data.StartingManager.Age})";
            if (data.StartingManager.YearsOfExperience.HasValue)
                managerInfo += $" [{data.StartingManager.YearsOfExperience}y exp]";
            
            AnsiConsole.MarkupLine(managerInfo);

            if (data.ManagerialChanges.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]⚠ {data.ManagerialChanges.Count} managerial change(s) this season[/]");
            }
            AnsiConsole.WriteLine();
        }

        // Transfer summary
        AnsiConsole.MarkupLine("[bold underline]Transfer Summary[/]");
        AnsiConsole.MarkupLine($"Total Spending: [red]£{stats.TotalSpent / 1_000_000:F1}m[/]");
        AnsiConsole.MarkupLine($"Total Income: [green]£{stats.TotalReceived / 1_000_000:F1}m[/]");
        
        var netColor = stats.NetSpend > 0 ? "red" : stats.NetSpend < 0 ? "green" : "white";
        AnsiConsole.MarkupLine($"Net Spend: [{netColor}]£{stats.NetSpend / 1_000_000:F1}m[/]");
        AnsiConsole.WriteLine();

        // Signings
        if (data.SummerSignings.Any() || data.WinterSignings.Any())
        {
            AnsiConsole.MarkupLine("[bold green]Signings[/]");
            RenderTransfers(data.SummerSignings.Concat(data.WinterSignings).ToList(), true);
            AnsiConsole.WriteLine();
        }

        // Departures
        if (data.SummerDepartures.Any() || data.WinterDepartures.Any())
        {
            AnsiConsole.MarkupLine("[bold yellow]Departures[/]");
            RenderTransfers(data.SummerDepartures.Concat(data.WinterDepartures).ToList(), false);
        }
    }

    private static void RenderTransfers(List<Transfer> transfers, bool isSignings)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Player")
            .AddColumn("Age")
            .AddColumn(isSignings ? "From" : "To")
            .AddColumn("Fee")
            .AddColumn("Date");

        foreach (var transfer in transfers.OrderByDescending(t => t.Fee ?? 0))
        {
            var age = transfer.PlayerAgeAtTransfer.HasValue 
                ? transfer.PlayerAgeAtTransfer.ToString() 
                : "-";

            var club = isSignings ? transfer.FromClub : transfer.ToClub;
            
            var fee = transfer.FormattedFee;
            if (transfer.Fee.HasValue)
            {
                var feeInM = transfer.Fee.Value / 1_000_000;
                fee = feeInM >= 1 
                    ? $"£{feeInM:F1}m" 
                    : $"£{transfer.Fee.Value / 1_000:F0}k";
            }

            table.AddRow(
                Markup.Escape(transfer.Player.Name),
                age,
                Markup.Escape(club),
                fee,
                transfer.TransferDate.ToString("dd/MM/yyyy"));
        }

        AnsiConsole.Write(table);
    }

    public static void RenderManagerComparison(Dictionary<string, TeamSeasonInfo> teamData, List<Match> matches)
    {
        var managersWithData = teamData
            .Where(kvp => kvp.Value.StartingManager != null)
            .Select(kvp => new
            {
                Team = kvp.Key,
                Manager = kvp.Value.StartingManager!,
                TeamData = kvp.Value
            })
            .ToList();

        if (!managersWithData.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No manager data available.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold cyan]Manager Profiles[/]")
            .AddColumn(new TableColumn("[bold]Manager[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Team[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Age[/]").Centered())
            .AddColumn(new TableColumn("[bold]Experience[/]").Centered())
            .AddColumn(new TableColumn("[bold]Nationality[/]").Centered())
            .AddColumn(new TableColumn("[bold]Formation[/]").LeftAligned());

        foreach (var item in managersWithData.OrderBy(x => x.Manager.Age))
        {
            var age = item.Manager.Age?.ToString() ?? "-";
            var exp = item.Manager.YearsOfExperience?.ToString() + "y" ?? "-";
            var formation = item.Manager.PreferredFormations.Any() 
                ? string.Join(", ", item.Manager.PreferredFormations.Take(2))
                : "-";

            table.AddRow(
                Markup.Escape(item.Manager.Name),
                Markup.Escape(item.Team),
                age,
                exp,
                Markup.Escape(item.Manager.Nationality ?? "-"),
                formation);
        }

        AnsiConsole.Write(table);

        // Age statistics
        var ages = managersWithData
            .Where(m => m.Manager.Age.HasValue)
            .Select(m => m.Manager.Age!.Value)
            .ToList();

        if (ages.Any())
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]Average manager age: {ages.Average():F1} years[/]");
            AnsiConsole.MarkupLine($"[dim]Youngest: {ages.Min()} | Oldest: {ages.Max()}[/]");
        }
    }
}
