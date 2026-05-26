using FootballDataTool.Models;
using FootballDataTool.Services;

namespace FootballDataTool.Tests;

public class MatchAnalyzerTests
{
    private static List<Match> BuildSampleMatches()
    {
        // 4-team league, 6 gameweeks (home + away round)
        return
        [
            // GW1
            new Match { Gameweek = 1, HomeTeam = "Arsenal",   AwayTeam = "Chelsea",  HomeGoals = 2, AwayGoals = 1 },
            new Match { Gameweek = 1, HomeTeam = "Liverpool", AwayTeam = "Spurs",    HomeGoals = 3, AwayGoals = 0 },
            // GW2
            new Match { Gameweek = 2, HomeTeam = "Arsenal",   AwayTeam = "Liverpool",HomeGoals = 1, AwayGoals = 1 },
            new Match { Gameweek = 2, HomeTeam = "Chelsea",   AwayTeam = "Spurs",    HomeGoals = 0, AwayGoals = 2 },
            // GW3
            new Match { Gameweek = 3, HomeTeam = "Arsenal",   AwayTeam = "Spurs",    HomeGoals = 4, AwayGoals = 0 },
            new Match { Gameweek = 3, HomeTeam = "Chelsea",   AwayTeam = "Liverpool",HomeGoals = 1, AwayGoals = 2 },
            // GW4 (reverse fixtures begin)
            new Match { Gameweek = 4, HomeTeam = "Chelsea",   AwayTeam = "Arsenal",  HomeGoals = 0, AwayGoals = 2 },
            new Match { Gameweek = 4, HomeTeam = "Spurs",     AwayTeam = "Liverpool",HomeGoals = 1, AwayGoals = 3 },
            // GW5
            new Match { Gameweek = 5, HomeTeam = "Liverpool", AwayTeam = "Arsenal",  HomeGoals = 0, AwayGoals = 1 },
            new Match { Gameweek = 5, HomeTeam = "Spurs",     AwayTeam = "Chelsea",  HomeGoals = 1, AwayGoals = 1 },
            // GW6
            new Match { Gameweek = 6, HomeTeam = "Spurs",     AwayTeam = "Arsenal",  HomeGoals = 0, AwayGoals = 3 },
            new Match { Gameweek = 6, HomeTeam = "Liverpool", AwayTeam = "Chelsea",  HomeGoals = 2, AwayGoals = 0 },
        ];
    }

    [Fact]
    public void GetTeams_ReturnsAllTeamsAlphabetically()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var teams = analyzer.GetTeams();

        Assert.Equal(["Arsenal", "Chelsea", "Liverpool", "Spurs"], teams);
    }

    [Fact]
    public void GetGameweeks_ReturnsOrderedGameweeks()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var gws = analyzer.GetGameweeks();

        Assert.Equal([1, 2, 3, 4, 5, 6], gws);
    }

    [Fact]
    public void GetStandings_FullSeason_CorrectPoints()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var standings = analyzer.GetStandings();

        // Arsenal: W5 D1 L0 → 16 points
        var arsenal = standings.First(r => r.TeamName == "Arsenal");
        Assert.Equal(5, arsenal.Won);
        Assert.Equal(1, arsenal.Drawn);
        Assert.Equal(0, arsenal.Lost);
        Assert.Equal(16, arsenal.Points);
        Assert.Equal(1, arsenal.Position);
    }

    [Fact]
    public void GetStandings_AtGameweek_OnlyCountsMatchesUpToThatGW()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());

        // After GW1: Arsenal 3 pts (W 2-1), Liverpool 3 pts (W 3-0), Chelsea 0, Spurs 0
        var standingsAfterGw1 = analyzer.GetStandings(1);
        var arsenalGw1 = standingsAfterGw1.First(r => r.TeamName == "Arsenal");
        Assert.Equal(3, arsenalGw1.Points);
        Assert.Equal(1, arsenalGw1.Played);
    }

    [Fact]
    public void GetStandings_ZeroGameweek_ReturnsFinalStandings()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var full = analyzer.GetStandings(0);
        var same = analyzer.GetStandings();

        Assert.Equal(full.Select(r => r.Points), same.Select(r => r.Points));
    }

    [Fact]
    public void GetCumulativePoints_PointsGrowMonotonically()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var cumulative = analyzer.GetCumulativePoints();

        foreach (var (_, points) in cumulative)
        {
            for (int i = 1; i < points.Count; i++)
                Assert.True(points[i].CumulativePoints >= points[i - 1].CumulativePoints);
        }
    }

    [Fact]
    public void GetCumulativePoints_FinalValueMatchesStandingsPoints()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var cumulative = analyzer.GetCumulativePoints();
        var standings = analyzer.GetStandings();

        foreach (var record in standings)
        {
            var finalPts = cumulative[record.TeamName].Last().CumulativePoints;
            Assert.Equal(record.Points, finalPts);
        }
    }

    [Fact]
    public void GetHomeFormByGameweek_ReturnsNullForGameweeksWithNoHomeGame()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var form = analyzer.GetHomeFormByGameweek();

        // Arsenal plays at home in GW1, 2, 3 but NOT in GW4, 5, 6
        Assert.NotNull(form["Arsenal"][1]);
        Assert.NotNull(form["Arsenal"][2]);
        Assert.NotNull(form["Arsenal"][3]);
        Assert.Null(form["Arsenal"][4]);
        Assert.Null(form["Arsenal"][5]);
        Assert.Null(form["Arsenal"][6]);
    }

    [Fact]
    public void GetResultMatrix_HomeMode_CorrectScores()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var matrix = analyzer.GetResultMatrix(MatrixMode.Home);

        // Arsenal 2-1 Chelsea at home in GW1
        var cell = matrix["Arsenal"]["Chelsea"];
        Assert.NotNull(cell);
        Assert.Equal(2, cell!.Value.Scored);
        Assert.Equal(1, cell.Value.Conceded);
    }

    [Fact]
    public void GetResultMatrix_AwayMode_ReflectsAwayPerspective()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var matrix = analyzer.GetResultMatrix(MatrixMode.Away);

        // Chelsea was away against Arsenal (2-1 to Arsenal), so Chelsea (away) vs Arsenal: 1-2
        var cell = matrix["Chelsea"]["Arsenal"];
        Assert.NotNull(cell);
        Assert.Equal(1, cell!.Value.Scored);   // Chelsea scored 1 away
        Assert.Equal(2, cell.Value.Conceded);  // Chelsea conceded 2 away
    }

    [Fact]
    public void GetResultMatrix_AggregateMode_SumsHomeAndAway()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var matrix = analyzer.GetResultMatrix(MatrixMode.Aggregate);

        // Arsenal home vs Chelsea: 2-1; Chelsea away vs Arsenal (i.e. Arsenal at home): counted above.
        // Arsenal away at Chelsea: 2-0 (GW4 Chelsea 0-2 Arsenal)
        // Arsenal aggregate vs Chelsea: (2+2) scored, (1+0) conceded = 4-1
        var cell = matrix["Arsenal"]["Chelsea"];
        Assert.NotNull(cell);
        Assert.Equal(4, cell!.Value.Scored);
        Assert.Equal(1, cell.Value.Conceded);
    }

    [Fact]
    public void GetCrossGroupPoints_CorrectlyCalculatesPoints()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());

        // Top 2 (Arsenal, Liverpool) vs Bottom 2 (Chelsea, Spurs)
        var result = analyzer.GetCrossGroupPoints(
            ["Arsenal", "Liverpool"],
            ["Chelsea", "Spurs"]);

        // Cross-matches involving (Arsenal or Liverpool) vs (Chelsea or Spurs):
        // GW1: Arsenal 2-1 Chelsea → Arsenal +3, Chelsea +0
        // GW1: Liverpool 3-0 Spurs → Liverpool +3, Spurs +0
        // GW2: Chelsea 0-2 Spurs → NOT cross (both bottom)
        // GW3: Arsenal 4-0 Spurs → Arsenal +3, Spurs +0
        // GW3: Chelsea 1-2 Liverpool → Liverpool +3, Chelsea +0
        // GW4: Chelsea 0-2 Arsenal → Arsenal +3, Chelsea +0
        // GW4: Spurs 1-3 Liverpool → Liverpool +3, Spurs +0
        // GW5: Spurs 1-1 Chelsea → NOT cross (both bottom)
        // GW6: Liverpool 2-0 Chelsea → Liverpool +3, Chelsea +0
        // GW6: Spurs 0-3 Arsenal → Arsenal +3, Spurs +0
        // Group1 total: 3+3+3+3+3+3+3+3 = 18... wait let me recount

        // From the matches:
        // GW1: Arsenal 2-1 Chelsea => G1 gets 3 pts
        // GW1: Liverpool 3-0 Spurs => G1 gets 3 pts
        // GW2: Arsenal 1-1 Liverpool => NOT cross (both group 1)
        // GW2: Chelsea 0-2 Spurs => NOT cross (both group 2)
        // GW3: Arsenal 4-0 Spurs => G1 gets 3 pts
        // GW3: Chelsea 1-2 Liverpool => G1 gets 3 pts
        // GW4: Chelsea 0-2 Arsenal => G1 gets 3 pts
        // GW4: Spurs 1-3 Liverpool => G1 gets 3 pts
        // GW5: Liverpool 0-1 Arsenal => NOT cross (both group 1)
        // GW5: Spurs 1-1 Chelsea => NOT cross (both group 2)
        // GW6: Spurs 0-3 Arsenal => G1 gets 3 pts
        // GW6: Liverpool 2-0 Chelsea => G1 gets 3 pts
        // Total G1: 24 pts, G2: 0 pts

        Assert.Equal(24, result.Group1Points);
        Assert.Equal(0, result.Group2Points);
    }

    [Fact]
    public void GetTopTeams_ReturnsCorrectCount()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var top2 = analyzer.GetTopTeams(2);
        Assert.Equal(2, top2.Count);
        Assert.Equal("Arsenal", top2[0]);
    }

    [Fact]
    public void GetBottomTeams_ReturnsCorrectCount()
    {
        var analyzer = new MatchAnalyzer(BuildSampleMatches());
        var bottom2 = analyzer.GetBottomTeams(2);
        Assert.Equal(2, bottom2.Count);
    }
}
