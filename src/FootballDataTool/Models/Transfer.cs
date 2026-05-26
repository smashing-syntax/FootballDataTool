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

    /// <summary>
    /// Transfer fee in the specified currency.
    /// </summary>
    public decimal? Fee { get; set; }

    /// <summary>
    /// Currency code (GBP, EUR, USD, etc.).
    /// </summary>
    public string? FeeCurrency { get; set; }

    /// <summary>
    /// Whether this is a loan transfer.
    /// </summary>
    public bool IsLoan { get; set; }

    /// <summary>
    /// Player's age at the time of transfer.
    /// </summary>
    public int? PlayerAgeAtTransfer { get; set; }

    /// <summary>
    /// Contract length in years (if known).
    /// </summary>
    public int? ContractYears { get; set; }

    /// <summary>
    /// Contract expiry date (if known).
    /// </summary>
    public DateTime? ContractExpiry { get; set; }

    /// <summary>
    /// Additional notes about the transfer.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Calculates fee per year of contract (for value analysis).
    /// </summary>
    public decimal? FeePerYear => 
        Fee.HasValue && ContractYears.HasValue && ContractYears > 0
            ? Fee.Value / ContractYears.Value
            : null;

    /// <summary>
    /// Formatted fee string with currency.
    /// </summary>
    public string FormattedFee => Fee.HasValue
        ? $"{FeeCurrency ?? "GBP"} {Fee:N0}"
        : Type == TransferType.FreeTransfer ? "Free" 
        : IsLoan ? "Loan" 
        : "Undisclosed";
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
