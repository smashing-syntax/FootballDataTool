namespace FootballDataTool.Models;

public class Match
{
    public int Gameweek { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }
    public DateTime? Date { get; set; }

    public string Result => HomeGoals > AwayGoals ? "H" : HomeGoals < AwayGoals ? "A" : "D";
    public int HomePoints => HomeGoals > AwayGoals ? 3 : HomeGoals == AwayGoals ? 1 : 0;
    public int AwayPoints => AwayGoals > HomeGoals ? 3 : HomeGoals == AwayGoals ? 1 : 0;

    public string ScoreString => $"{HomeGoals}-{AwayGoals}";
}
