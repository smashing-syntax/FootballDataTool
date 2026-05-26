namespace FootballDataTool.Models;

/// <summary>
/// Represents a player transfer (signing or departure).
/// </summary>
public class Transfer
{
    public Player Player { get; set; } = new();
    public string FromClub { get; set; } = string.Empty;
    public string ToClub { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public TransferWindow Window { get; set; }
    public TransferType Type { get; set; }
    public decimal? Fee { get; set; }
    public string? FeeCurrency { get; set; }
    public bool IsLoan { get; set; }
    public string? Notes { get; set; }
}

public enum TransferWindow
{
    Summer,
    Winter,
    OutOfWindow  // Free agents, emergency loans, etc.
}

public enum TransferType
{
    Permanent,
    Loan,
    LoanReturn,
    FreeTransfer,
    ContractExpiry,
    Retirement
}
