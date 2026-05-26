using FootballDataTool.Services;
using FootballDataTool.Visualisers;
using Spectre.Console;

AnsiConsole.Write(
    new FigletText("FootballDataTool")
        .Centered()
        .Color(Color.Green));

AnsiConsole.MarkupLine("[dim]CSV football season analyser · visualise form, points & results[/]\n");

string csvPath = GetCsvPath(args);
List<FootballDataTool.Models.Match> matches = LoadMatches(csvPath);
var analyzer = new MatchAnalyzer(matches);

AnsiConsole.MarkupLine(
    $"[green]✓[/] Loaded [bold]{matches.Count}[/] matches across " +
    $"[bold]{analyzer.GetGameweeks().Count}[/] gameweeks " +
    $"([bold]{analyzer.GetTeams().Count}[/] teams).\n");

bool running = true;
while (running)
{
    var choice = PromptMenu(
        "[bold cyan]What would you like to view?[/]",
        [
            "Home form by gameweek (per team)",
            "Points breakdown by gameweek",
            "Result matrix (aggregate / home / away)",
            "League standings",
            "Load a different CSV file",
            "Exit"
        ]);

    AnsiConsole.Clear();

    switch (choice)
    {
        case 1:
            HomeFormVisualiser.Render(analyzer);
            break;

        case 2:
            PointBreakdownVisualiser.Render(analyzer);
            break;

        case 3:
            ResultMatrixVisualiser.Render(analyzer);
            break;

        case 4:
            RenderStandings(analyzer);
            break;

        case 5:
            csvPath = PromptForPath();
            matches = LoadMatches(csvPath);
            analyzer = new MatchAnalyzer(matches);
            AnsiConsole.MarkupLine(
                $"[green]✓[/] Loaded [bold]{matches.Count}[/] matches across " +
                $"[bold]{analyzer.GetGameweeks().Count}[/] gameweeks " +
                $"([bold]{analyzer.GetTeams().Count}[/] teams).\n");
            break;

        case 6:
            running = false;
            break;
    }

    if (running)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to return to the main menu…[/]");
        if (!Console.IsInputRedirected)
            Console.ReadKey(intercept: true);
        else
            Console.ReadLine();
        AnsiConsole.Clear();
    }
}

AnsiConsole.MarkupLine("[bold green]Goodbye![/]");

// ── Helpers ───────────────────────────────────────────────────────────────────

/// <summary>
/// Shows a numbered menu and returns the 1-based selection index.
/// Falls back to a plain text numbered list when the terminal is not interactive.
/// </summary>
static int PromptMenu(string title, string[] options)
{
    if (!Console.IsInputRedirected)
    {
        // Interactive terminal — try Spectre.Console arrow-key selection
        try
        {
            var numbered = options.Select((o, i) => $"{i + 1}. {o}").ToArray();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .AddChoices(numbered));
            return int.Parse(selection.Split('.')[0]);
        }
        catch (NotSupportedException)
        {
            // Fall through to plain text menu below
        }
    }

    // Plain text numbered menu (non-interactive / fallback)
    AnsiConsole.MarkupLine(title);
    for (int i = 0; i < options.Length; i++)
        AnsiConsole.MarkupLine($"  [bold]{i + 1}.[/] {Markup.Escape(options[i])}");

    while (true)
    {
        AnsiConsole.Markup("[green]>[/] Enter number: ");
        var line = Console.ReadLine()?.Trim();
        if (int.TryParse(line, out int n) && n >= 1 && n <= options.Length)
            return n;
        AnsiConsole.MarkupLine($"[red]Please enter a number between 1 and {options.Length}.[/]");
    }
}

static string GetCsvPath(string[] args)
{
    // 1. Command-line argument
    if (args.Length > 0 && File.Exists(args[0]))
        return args[0];

    // 2. Check if a sample file exists next to the executable
    var samplePath = Path.Combine(AppContext.BaseDirectory, "data", "sample_season.csv");
    if (File.Exists(samplePath))
    {
        AnsiConsole.MarkupLine($"[dim]Using bundled sample data: {Markup.Escape(samplePath)}[/]\n");
        return samplePath;
    }

    // 3. Prompt the user
    return PromptForPath();
}

static string PromptForPath()
{
    if (!Console.IsInputRedirected)
    {
        try
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the [bold]path to your CSV file[/]:")
                    .Validate(path =>
                    {
                        if (!File.Exists(path))
                            return ValidationResult.Error($"[red]File not found:[/] {path}");
                        return ValidationResult.Success();
                    }));
        }
        catch (NotSupportedException) { }
    }

    while (true)
    {
        AnsiConsole.Markup("Enter the [bold]path to your CSV file[/]: ");
        var path = Console.ReadLine()?.Trim() ?? string.Empty;
        if (File.Exists(path)) return path;
        AnsiConsole.MarkupLine($"[red]File not found:[/] {Markup.Escape(path)}");
    }
}

static List<FootballDataTool.Models.Match> LoadMatches(string path)
{
    return AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .Start($"Loading [bold]{Markup.Escape(path)}[/]…", _ =>
        {
            try
            {
                var service = new CsvDataService();
                return service.LoadFromFile(path);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error loading CSV:[/] {Markup.Escape(ex.Message)}");
                Environment.Exit(1);
                return [];
            }
        });
}

static void RenderStandings(MatchAnalyzer analyzer)
{
    var gameweeks = analyzer.GetGameweeks();
    var maxGw = gameweeks.Max();

    int upTo = 0;
    bool prompted = false;

    if (!Console.IsInputRedirected)
    {
        try
        {
            upTo = AnsiConsole.Prompt(
                new TextPrompt<int>($"Show standings up to which gameweek? (1–{maxGw}, 0 = full season)")
                    .DefaultValue(0)
                    .Validate(v =>
                    {
                        if (v < 0 || v > maxGw)
                            return ValidationResult.Error($"[red]Enter a value between 0 and {maxGw}.[/]");
                        return ValidationResult.Success();
                    }));
            prompted = true;
        }
        catch (NotSupportedException) { }
    }

    if (!prompted)
    {
        AnsiConsole.Markup($"Show standings up to which gameweek? (1–{maxGw}, 0 = full season): ");
        var line = Console.ReadLine()?.Trim();
        upTo = int.TryParse(line, out int v) && v >= 0 && v <= maxGw ? v : 0;
    }

    var standings = analyzer.GetStandings(upTo);
    var title = upTo > 0 ? $"Standings after GW{upTo}" : "Final Standings";

    AnsiConsole.WriteLine();

    var table = new Table()
        .Border(TableBorder.Rounded)
        .Title($"[bold cyan]{title}[/]")
        .AddColumn(new TableColumn("[bold]#[/]").RightAligned())
        .AddColumn(new TableColumn("[bold]Team[/]").LeftAligned())
        .AddColumn(new TableColumn("[bold]P[/]").Centered())
        .AddColumn(new TableColumn("[bold]W[/]").Centered())
        .AddColumn(new TableColumn("[bold]D[/]").Centered())
        .AddColumn(new TableColumn("[bold]L[/]").Centered())
        .AddColumn(new TableColumn("[bold]GF[/]").Centered())
        .AddColumn(new TableColumn("[bold]GA[/]").Centered())
        .AddColumn(new TableColumn("[bold]GD[/]").Centered())
        .AddColumn(new TableColumn("[bold]Pts[/]").Centered());

    foreach (var r in standings)
    {
        var posColour = r.Position == 1 ? "gold1"
                      : r.Position <= 4 ? "green"
                      : r.Position > standings.Count - 3 ? "red"
                      : "white";

        table.AddRow(
            $"[{posColour}]{r.Position}[/]",
            $"[{posColour}]{Markup.Escape(r.TeamName)}[/]",
            r.Played.ToString(),
            r.Won.ToString(),
            r.Drawn.ToString(),
            r.Lost.ToString(),
            r.GoalsFor.ToString(),
            r.GoalsAgainst.ToString(),
            r.GoalDifference >= 0 ? $"[green]+{r.GoalDifference}[/]" : $"[red]{r.GoalDifference}[/]",
            $"[bold]{r.Points}[/]");
    }

    AnsiConsole.Write(table);
}
