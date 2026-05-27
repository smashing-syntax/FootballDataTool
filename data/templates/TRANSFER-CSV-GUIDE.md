# Transfer CSV Guide

## Overview

Transfers can now be managed in **CSV format** (easier than JSON!) or continue using JSON.

**Benefits of CSV:**
- ✅ Easy to fill in Excel/Google Sheets
- ✅ Multiple teams in one file
- ✅ Auto-calculates ages from dates of birth
- ✅ Flexible date formats
- ✅ Supports both signings and departures

---

## CSV Template Location

📁 `data/templates/transfers_template.csv`

---

## CSV Format

### Required Columns

| Column | Description | Example |
|--------|-------------|---------|
| `Team` | Team name (must match CSV match data) | `Arsenal` |
| `Season` | Season identifier | `2023/24` |
| `PlayerName` | Full player name | `Declan Rice` |
| `FromClub` | Selling club | `West Ham United` |
| `ToClub` | Buying club | `Arsenal` |
| `TransferDate` | Date of transfer | `2023-07-15` |
| `Window` | Transfer window | `Summer` or `Winter` |
| `Type` | Transfer type | `Permanent`, `Loan`, or `Free` |

### Optional Columns

| Column | Description | Example | Default |
|--------|-------------|---------|---------|
| `ShirtNumber` | Squad number | `41` | - |
| `Position` | Player position | `Midfielder` | - |
| `Nationality` | Player nationality | `England` | - |
| `DateOfBirth` | Birth date (YYYY-MM-DD) | `1999-01-14` | - |
| `Fee` | Transfer fee (numeric) | `105000000` | - |
| `FeeCurrency` | Currency code | `GBP`, `EUR`, `USD` | `GBP` |
| `IsLoan` | Is it a loan? | `Yes` or `No` | `No` |
| `PlayerAge` | Age at transfer | `24` | Auto-calculated |
| `ContractYears` | Contract length | `5` | - |
| `ContractExpiry` | Contract end date | `2028-06-30` | - |
| `Notes` | Additional notes | `Club record signing` | - |
| `PreviousClub` | Previous club before this one | `West Ham United` | - |

---

## Date Formats Supported

The parser accepts multiple date formats:
- `YYYY-MM-DD` (recommended) - `2023-07-15`
- `DD/MM/YYYY` - `15/07/2023`
- `MM/DD/YYYY` - `07/15/2023`
- `DD-MM-YYYY` - `15-07-2023`

---

## Transfer Types

### Transfer Type Values
- **`Permanent`** - Permanent transfer with fee
- **`Loan`** - Temporary loan (set `IsLoan=Yes`)
- **`Free`** - Free transfer (no fee)

### Transfer Windows
- **`Summer`** - Pre-season window (May-August)
- **`Winter`** - Mid-season window (January)

---

## How It Works

### Signings vs Departures

The system automatically determines if a transfer is a signing or departure based on the `Team` column:

**Signing Example:**
```csv
Team,FromClub,ToClub,Window
Arsenal,West Ham United,Arsenal,Summer
```
→ Arsenal **signed** player from West Ham (ToClub = Arsenal)

**Departure Example:**
```csv
Team,FromClub,ToClub,Window
Arsenal,Arsenal,Bayer Leverkusen,Summer
```
→ Arsenal player **departed** to Leverkusen (FromClub = Arsenal)

### Multiple Teams in One File

You can track transfers for **multiple teams** in a single CSV:

```csv
Team,Season,PlayerName,FromClub,ToClub,...
Arsenal,2023/24,Declan Rice,West Ham United,Arsenal,...
Arsenal,2023/24,Granit Xhaka,Arsenal,Bayer Leverkusen,...
Chelsea,2023/24,Moises Caicedo,Brighton,Chelsea,...
Chelsea,2023/24,Mason Mount,Chelsea,Manchester United,...
```

---

## Example: Complete Transfer Entry

```csv
Arsenal,2023/24,Declan Rice,41,Midfielder,England,1999-01-14,West Ham United,Arsenal,2023-07-15,Summer,Permanent,105000000,GBP,No,24,5,2028-06-30,Club record signing,West Ham United
```

**Breakdown:**
- Team: `Arsenal`
- Season: `2023/24`
- Player: `Declan Rice`, #41, Midfielder, English
- Born: `1999-01-14` (Age 24 at transfer)
- Transfer: West Ham → Arsenal
- Date: `2023-07-15` (Summer window)
- Fee: £105m (permanent)
- Contract: 5 years until 2028-06-30
- Notes: "Club record signing"

---

## Usage in Code

### Loading Transfers from CSV

```csharp
var seasonData = csvDataService.LoadSeasonDataFromFile("data/premier-league/2023-24.csv");

// Load transfers from CSV (easier than JSON!)
seasonData.LoadTransferDataFromCsv("data/premier-league/transfers_2023-24.csv");

// Now access transfer data
var arsenal = seasonData.GetTeam("Arsenal");
var summerSignings = arsenal?.SeasonInfo?.SummerSignings;

foreach (var transfer in summerSignings ?? [])
{
    Console.WriteLine($"{transfer.Player.Name} from {transfer.FromClub} for {transfer.Fee:C}");
}
```

### Loading Transfers from JSON (still supported)

```csharp
// Load individual team JSON files
seasonData.LoadTransferData(
    "data/premier-league/arsenal_2023-24_transfers.json",
    "data/premier-league/chelsea_2023-24_transfers.json"
);
```

---

## Advantages Over JSON

| Feature | CSV | JSON |
|---------|-----|------|
| Multiple teams in one file | ✅ Yes | ❌ One file per team |
| Edit in Excel/Google Sheets | ✅ Easy | ❌ Difficult |
| Auto-calculate ages | ✅ Yes | ❌ Manual |
| Flexible date formats | ✅ Yes | ❌ Strict format |
| Visual structure | ✅ Table view | ❌ Nested objects |
| Bulk data entry | ✅ Fast | ❌ Repetitive |

---

## Tips for Filling Out Transfer CSV

1. **Start Simple**: Begin with required columns only (Team, Season, PlayerName, FromClub, ToClub, TransferDate, Window, Type)

2. **Age Auto-Calculation**: If you provide `DateOfBirth`, the system automatically calculates `PlayerAge` - no need to fill both!

3. **Fee Format**: Enter fees as numbers without commas: `105000000` not `105,000,000`

4. **Loan Transfers**: 
   - Set `Type=Loan` AND `IsLoan=Yes`
   - Fee can be the loan fee (if any)
   - ContractYears = duration of loan

5. **Free Transfers**: 
   - Set `Type=Free`
   - Leave `Fee` empty or set to `0`

6. **Same Player, Multiple Teams**: 
   - If tracking a player's journey across multiple clubs, add separate rows
   - Use `PreviousClub` to show career progression

7. **Testing**: Load and test after every 5-10 transfers to catch errors early

---

## Common Pitfalls

❌ **Team name mismatch**: CSV uses "Man United", JSON uses "Manchester United"
→ Use **exact same names** as in your match CSV

❌ **Date format confusion**: `07/15/2023` (US) vs `15/07/2023` (UK)
→ Use `YYYY-MM-DD` to avoid ambiguity

❌ **Fee with currency symbol**: `£105m` 
→ Use numeric only: `105000000`, set currency in `FeeCurrency` column

❌ **Signing recorded twice**: Both as Arsenal signing AND West Ham departure
→ Record once per team perspective if tracking both

---

## File Organization

Recommended structure:

```
data/
├── templates/
│   └── transfers_template.csv          ← Template with examples
├── premier-league/
│   ├── 2023-24_matches.csv             ← Match data
│   ├── 2023-24_transfers.csv           ← All PL transfers (CSV) ✨ NEW
│   ├── arsenal_2023-24_transfers.json  ← Or individual JSON files
│   └── chelsea_2023-24_transfers.json
└── la-liga/
    ├── 2022-23_matches.csv
    └── 2022-23_transfers.csv           ← All La Liga transfers (CSV)
```

---

## Migration from JSON to CSV

If you have existing JSON files, you can:

1. **Keep using JSON** - still fully supported
2. **Convert to CSV** - copy data into CSV format
3. **Use both** - Load JSON for some teams, CSV for others

The system handles both seamlessly!

---

## Summary

**For manual data entry**: Use CSV (much easier!)
**For programmatic data**: Use JSON (more structured)

CSV template: `data/templates/transfers_template.csv`

Happy transfer tracking! 🚀⚽
