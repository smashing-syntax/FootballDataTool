using FootballDataTool.Services;

namespace FootballDataTool.Tests;

public class CsvDataServiceTests
{
    private static string WriteTempCsv(string content)
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void LoadFromFile_SimpleFormat_ParsesCorrectly()
    {
        var csv = """
                  GW,HomeTeam,AwayTeam,HomeGoals,AwayGoals
                  1,Arsenal,Chelsea,2,1
                  1,Liverpool,Spurs,3,0
                  2,Arsenal,Liverpool,1,1
                  """;

        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var matches = service.LoadFromFile(path);

            Assert.Equal(3, matches.Count);
            Assert.Equal("Arsenal", matches[0].HomeTeam);
            Assert.Equal("Chelsea", matches[0].AwayTeam);
            Assert.Equal(2, matches[0].HomeGoals);
            Assert.Equal(1, matches[0].AwayGoals);
            Assert.Equal(1, matches[0].Gameweek);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_FootballDataFormat_ParsesCorrectly()
    {
        var csv = """
                  Div,Date,HomeTeam,AwayTeam,FTHG,FTAG,FTR
                  E0,12/08/2023,Arsenal,Chelsea,2,1,H
                  E0,12/08/2023,Liverpool,Spurs,3,0,H
                  """;

        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var matches = service.LoadFromFile(path);

            Assert.Equal(2, matches.Count);
            Assert.Equal(2, matches[0].HomeGoals);
            Assert.Equal(1, matches[0].AwayGoals);
            // Dates are present so gameweeks are assigned by date grouping
            Assert.True(matches[0].Gameweek > 0);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_MissingFile_ThrowsFileNotFoundException()
    {
        var service = new CsvDataService();
        Assert.Throws<FileNotFoundException>(() => service.LoadFromFile("/nonexistent/file.csv"));
    }

    [Fact]
    public void LoadFromFile_MissingTeamColumns_ThrowsInvalidDataException()
    {
        var csv = "Score,Date\n2-1,12/08/2023\n";
        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var ex = Assert.Throws<InvalidDataException>(() => service.LoadFromFile(path));
            Assert.Contains("home team", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_MissingGoalColumns_ThrowsInvalidDataException()
    {
        var csv = "HomeTeam,AwayTeam,Date\nArsenal,Chelsea,12/08/2023\n";
        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var ex = Assert.Throws<InvalidDataException>(() => service.LoadFromFile(path));
            Assert.Contains("goals", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_SkipsRowsWithMissingTeamNames()
    {
        var csv = """
                  GW,HomeTeam,AwayTeam,HomeGoals,AwayGoals
                  1,Arsenal,Chelsea,2,1
                  2,,Spurs,1,0
                  3,Liverpool,,1,1
                  """;

        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var matches = service.LoadFromFile(path);

            // Only the first valid row should be loaded
            Assert.Single(matches);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_SkipsRowsWithNonNumericGoals()
    {
        var csv = """
                  GW,HomeTeam,AwayTeam,HomeGoals,AwayGoals
                  1,Arsenal,Chelsea,2,1
                  1,Liverpool,Spurs,three,0
                  """;

        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var matches = service.LoadFromFile(path);

            Assert.Single(matches);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromFile_WhitespaceTrimmedFromHeaders()
    {
        var csv = " GW , HomeTeam , AwayTeam , HomeGoals , AwayGoals \n1,Arsenal,Chelsea,2,1\n";
        var path = WriteTempCsv(csv);
        try
        {
            var service = new CsvDataService();
            var matches = service.LoadFromFile(path);
            Assert.Single(matches);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void AssignGameweeks_GreedyRoundRobin_AssignsCorrectly()
    {
        // Arrange: 4 matches that should form 2 gameweeks of 2 matches each
        var matches = new List<FootballDataTool.Models.Match>
        {
            new() { HomeTeam = "Arsenal",   AwayTeam = "Chelsea" },
            new() { HomeTeam = "Liverpool", AwayTeam = "Spurs" },
            new() { HomeTeam = "Chelsea",   AwayTeam = "Arsenal" },
            new() { HomeTeam = "Spurs",     AwayTeam = "Liverpool" },
        };

        CsvDataService.AssignGameweeks(matches);

        Assert.Equal(1, matches[0].Gameweek);
        Assert.Equal(1, matches[1].Gameweek);
        Assert.Equal(2, matches[2].Gameweek);
        Assert.Equal(2, matches[3].Gameweek);
    }

    [Fact]
    public void AssignGameweeks_IncreasesGameweekOnConflict()
    {
        // If the same team appears twice in a row (conflict), gameweek should increment
        var matches = new List<FootballDataTool.Models.Match>
        {
            new() { HomeTeam = "Arsenal", AwayTeam = "Chelsea" },
            new() { HomeTeam = "Arsenal", AwayTeam = "Spurs" }, // Arsenal conflict → new GW
        };

        CsvDataService.AssignGameweeks(matches);

        Assert.Equal(1, matches[0].Gameweek);
        Assert.Equal(2, matches[1].Gameweek);
    }
}
