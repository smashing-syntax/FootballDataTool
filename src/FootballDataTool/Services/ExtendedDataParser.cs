using FootballDataTool.Models;
using System.Text.RegularExpressions;
using Match = FootballDataTool.Models.Match;

namespace FootballDataTool.Services;

/// <summary>
/// Service for parsing extended match data from CSV records into rich structured models.
/// Handles lineups, goalscorers, substitutions, cards, etc.
/// </summary>
public class ExtendedDataParser
{
    /// <summary>
    /// Parses extended data from a CSV record if available.
    /// Returns null if no extended data is present.
    /// </summary>
    public static MatchExtendedData? ParseExtendedData(CsvMatchRecord record, Match match)
    {
        // Check if we have any extended data
        bool hasExtendedData = HasAnyExtendedData(record);
        
        if (!hasExtendedData)
            return null;

        var extendedData = new MatchExtendedData();

        // Parse managers
        extendedData.HomeManager = record.HomeManager;
        extendedData.AwayManager = record.AwayManager;

        // Parse formations
        extendedData.HomeFormation = record.HomeFormation;
        extendedData.AwayFormation = record.AwayFormation;

        // Parse venue
        if (!string.IsNullOrWhiteSpace(record.Stadium))
        {
            extendedData.Stadium = new Stadium
            {
                Name = record.Stadium
            };

            if (int.TryParse(record.StadiumCapacity, out int capacity))
                extendedData.Stadium.Capacity = capacity;
        }

        // Parse attendance
        if (int.TryParse(record.Attendance, out int attendance))
            extendedData.Attendance = attendance;

        // Parse lineups
        extendedData.HomeStartingLineup = ParsePlayerList(record.HomeLineup);
        extendedData.AwayStartingLineup = ParsePlayerList(record.AwayLineup);
        extendedData.HomeSubstitutes = ParsePlayerList(record.HomeSubstitutes);
        extendedData.AwaySubstitutes = ParsePlayerList(record.AwaySubstitutes);

        // Parse goalscorers
        extendedData.Goals.AddRange(ParseGoals(record.HomeGoalscorers, match.HomeTeam, true));
        extendedData.Goals.AddRange(ParseGoals(record.AwayGoalscorers, match.AwayTeam, false));
        extendedData.Goals = extendedData.Goals.OrderBy(g => g.Minute).ThenBy(g => g.AddedTimeMinute ?? 0).ToList();

        // Parse substitutions
        extendedData.Substitutions.AddRange(ParseSubstitutions(record.HomeSubstitutions, match.HomeTeam));
        extendedData.Substitutions.AddRange(ParseSubstitutions(record.AwaySubstitutions, match.AwayTeam));
        extendedData.Substitutions = extendedData.Substitutions.OrderBy(s => s.Minute).ThenBy(s => s.AddedTimeMinute ?? 0).ToList();

        // Parse cards
        extendedData.Cards.AddRange(ParseCards(record.HomeYellowCards, match.HomeTeam, CardType.Yellow));
        extendedData.Cards.AddRange(ParseCards(record.AwayYellowCards, match.AwayTeam, CardType.Yellow));
        extendedData.Cards.AddRange(ParseCards(record.HomeRedCards, match.HomeTeam, CardType.Red));
        extendedData.Cards.AddRange(ParseCards(record.AwayRedCards, match.AwayTeam, CardType.Red));
        extendedData.Cards = extendedData.Cards.OrderBy(c => c.Minute).ThenBy(c => c.AddedTimeMinute ?? 0).ToList();

        // Parse match officials
        extendedData.Referee = record.Referee;
        if (!string.IsNullOrWhiteSpace(record.AssistantReferee1))
            extendedData.AssistantReferees.Add(record.AssistantReferee1);
        if (!string.IsNullOrWhiteSpace(record.AssistantReferee2))
            extendedData.AssistantReferees.Add(record.AssistantReferee2);
        extendedData.FourthOfficial = record.FourthOfficial;
        extendedData.VarReferee = record.VarReferee;

        // Parse weather
        if (!string.IsNullOrWhiteSpace(record.Temperature) || !string.IsNullOrWhiteSpace(record.WeatherConditions))
        {
            extendedData.Weather = new WeatherConditions
            {
                Conditions = record.WeatherConditions
            };

            if (decimal.TryParse(record.Temperature, out decimal temp))
                extendedData.Weather.Temperature = temp;
        }

        return extendedData;
    }

    private static bool HasAnyExtendedData(CsvMatchRecord record)
    {
        return !string.IsNullOrWhiteSpace(record.HomeManager)
            || !string.IsNullOrWhiteSpace(record.AwayManager)
            || !string.IsNullOrWhiteSpace(record.HomeFormation)
            || !string.IsNullOrWhiteSpace(record.Stadium)
            || !string.IsNullOrWhiteSpace(record.Attendance)
            || !string.IsNullOrWhiteSpace(record.HomeGoalscorers)
            || !string.IsNullOrWhiteSpace(record.AwayGoalscorers)
            || !string.IsNullOrWhiteSpace(record.HomeLineup)
            || !string.IsNullOrWhiteSpace(record.AwayLineup)
            || !string.IsNullOrWhiteSpace(record.HomeSubstitutions)
            || !string.IsNullOrWhiteSpace(record.AssistantReferee1)
            || !string.IsNullOrWhiteSpace(record.Temperature);
    }

    /// <summary>
    /// Parses a delimited list of players (semi-colon or comma separated).
    /// Format: "1. Player Name; 2. Another Player" or "Player Name, Another Player"
    /// </summary>
    private static List<Player> ParsePlayerList(string? playerList)
    {
        if (string.IsNullOrWhiteSpace(playerList))
            return new List<Player>();

        var delimiter = playerList.Contains(';') ? ';' : ',';
        var players = new List<Player>();

        foreach (var playerStr in playerList.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = playerStr.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            // Try to parse "1. Player Name" format
            var match = PlayerNumberRegex.Match(trimmed);
            if (match.Success)
            {
                players.Add(new Player
                {
                    ShirtNumber = int.Parse(match.Groups[1].Value),
                    Name = match.Groups[2].Value.Trim()
                });
            }
            else
            {
                players.Add(new Player { Name = trimmed });
            }
        }

        return players;
    }

    /// <summary>
    /// Parses goalscorers string.
    /// Formats supported:
    /// - "Player1 45', Player2 67'"
    /// - "Player1 (45), Player2 (67)"
    /// - "Player1 45' (pen), Player2 67' (assist: Assister)"
    /// </summary>
    private static List<GoalEvent> ParseGoals(string? goalscorers, string team, bool isHome)
    {
        if (string.IsNullOrWhiteSpace(goalscorers))
            return new List<GoalEvent>();

        var goals = new List<GoalEvent>();
        
        // Split by common delimiters
        var entries = goalscorers.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);

        foreach (var entry in entries)
        {
            var goal = ParseGoalEntry(entry.Trim(), team);
            if (goal != null)
                goals.Add(goal);
        }

        return goals;
    }

    private static GoalEvent? ParseGoalEntry(string entry, string team)
    {
        // Match patterns like: "Player Name 45'" or "Player Name (45)" or "Player Name 45+2'"
        var match = GoalRegex.Match(entry);
        if (!match.Success)
            return null;

        var goal = new GoalEvent
        {
            Scorer = new Player { Name = match.Groups[1].Value.Trim() },
            Minute = int.Parse(match.Groups[2].Value),
            TeamScoring = team
        };

        // Check for added time
        if (match.Groups[3].Success)
            goal.AddedTimeMinute = int.Parse(match.Groups[3].Value);

        // Check for goal type indicators
        var remainder = entry[(match.Index + match.Length)..].ToLower();
        
        if (remainder.Contains("pen") || remainder.Contains("(p)"))
            goal.Type = GoalType.Penalty;
        else if (remainder.Contains("og") || remainder.Contains("own"))
            goal.Type = GoalType.OwnGoal;
        else if (remainder.Contains("fk") || remainder.Contains("free kick"))
            goal.Type = GoalType.DirectFreeKick;
        else
            goal.Type = GoalType.Open;

        // Check for assister
        var assistMatch = AssistRegex.Match(remainder);
        if (assistMatch.Success)
        {
            goal.Assister = new Player { Name = assistMatch.Groups[1].Value.Trim() };
        }

        return goal;
    }

    /// <summary>
    /// Parses substitutions.
    /// Format: "Player On ← Player Off 60'" or "Player On -> Player Off (60)"
    /// </summary>
    private static List<SubstitutionEvent> ParseSubstitutions(string? substitutions, string team)
    {
        if (string.IsNullOrWhiteSpace(substitutions))
            return new List<SubstitutionEvent>();

        var subs = new List<SubstitutionEvent>();
        var entries = substitutions.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);

        foreach (var entry in entries)
        {
            var sub = ParseSubstitutionEntry(entry.Trim(), team);
            if (sub != null)
                subs.Add(sub);
        }

        return subs;
    }

    private static SubstitutionEvent? ParseSubstitutionEntry(string entry, string team)
    {
        // Match patterns with various arrow styles
        var match = SubstitutionRegex.Match(entry);
        if (!match.Success)
            return null;

        var sub = new SubstitutionEvent
        {
            PlayerOn = new Player { Name = match.Groups[1].Value.Trim() },
            PlayerOff = new Player { Name = match.Groups[2].Value.Trim() },
            Minute = int.Parse(match.Groups[3].Value),
            Team = team
        };

        // Check for added time
        if (match.Groups[4].Success)
            sub.AddedTimeMinute = int.Parse(match.Groups[4].Value);

        return sub;
    }

    /// <summary>
    /// Parses cards.
    /// Format: "Player1 45', Player2 67'"
    /// </summary>
    private static List<CardEvent> ParseCards(string? cards, string team, CardType type)
    {
        if (string.IsNullOrWhiteSpace(cards))
            return new List<CardEvent>();

        var cardEvents = new List<CardEvent>();
        var entries = cards.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);

        foreach (var entry in entries)
        {
            var card = ParseCardEntry(entry.Trim(), team, type);
            if (card != null)
                cardEvents.Add(card);
        }

        return cardEvents;
    }

    private static CardEvent? ParseCardEntry(string entry, string team, CardType type)
    {
        var match = CardRegex.Match(entry);
        if (!match.Success)
            return null;

        var card = new CardEvent
        {
            Player = new Player { Name = match.Groups[1].Value.Trim() },
            Minute = int.Parse(match.Groups[2].Value),
            Team = team,
            Type = type
        };

        // Check for added time
        if (match.Groups[3].Success)
            card.AddedTimeMinute = int.Parse(match.Groups[3].Value);

        return card;
    }

    // Regex patterns
    private static readonly Regex PlayerNumberRegex = new(@"^(\d+)\.\s*(.+)$", RegexOptions.Compiled);
    private static readonly Regex GoalRegex = new(@"^(.+?)\s*[(\[]?(\d+)(?:\+(\d+))?[)\]]?['\s]*", RegexOptions.Compiled);
    private static readonly Regex AssistRegex = new(@"assist:?\s*(.+?)(?:\s|$|\)|\])", RegexOptions.Compiled);
    private static readonly Regex SubstitutionRegex = new(@"^(.+?)\s*(?:←|<-|->|→)\s*(.+?)\s*[(\[]?(\d+)(?:\+(\d+))?[)\]]?", RegexOptions.Compiled);
    private static readonly Regex CardRegex = new(@"^(.+?)\s*[(\[]?(\d+)(?:\+(\d+))?[)\]]?", RegexOptions.Compiled);
}
