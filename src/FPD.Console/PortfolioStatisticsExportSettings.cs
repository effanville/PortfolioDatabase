using Effanville.FinancialStructures.Database.Export.Statistics;
using Effanville.FinancialStructures.Database.Statistics;

namespace Effanville.FPD.Console;

/// <summary>
/// A class containing settings for the export to file of a <see cref="PortfolioStatistics"/> object.
/// </summary>
public sealed class CommandStatisticsExportSettings
{
    /// <summary>
    /// Display with spacing in tables.
    /// </summary>
    public bool Spacing { get; set; } = false;

    /// <summary>
    /// Display with colours.
    /// </summary>
    public bool Colours { get; set; } = true;

    /// <summary>
    /// Options on displaying Securities.
    /// </summary>
    public TableOptions SecurityDisplayOptions { get; set; }

    /// <summary>
    /// Options on displaying bank accounts.
    /// </summary>
    public TableOptions BankAccountDisplayOptions { get; set; }

    /// <summary>
    /// Options on displaying sectors.
    /// </summary>
    public TableOptions SectorDisplayOptions { get; set; }

    /// <summary>
    /// Options on displaying assets.
    /// </summary>
    public TableOptions AssetDisplayOptions { get; set; }

    /// <summary>
    /// Options on displaying currencies.
    /// </summary>
    public TableOptions CurrencyDisplayOptions { get; set; }

    public CommandStatisticsExportSettings()
    {
        SecurityDisplayOptions = new TableOptions()
        {
            ShouldDisplay = true,
            SortingDirection = SortDirection.Ascending,
            SortingField = Statistic.Company
        };

        BankAccountDisplayOptions = new TableOptions()
        {
            ShouldDisplay = true,
            SortingDirection = SortDirection.Ascending,
            SortingField = Statistic.Company
        };
        SectorDisplayOptions = new TableOptions()
        {
            ShouldDisplay = true,
            SortingDirection = SortDirection.Ascending,
            SortingField = Statistic.Company
        };

        AssetDisplayOptions = new TableOptions()
        {
            ShouldDisplay = true,
            SortingDirection = SortDirection.Ascending,
            SortingField = Statistic.Company
        };
        CurrencyDisplayOptions = new TableOptions()
        {
            ShouldDisplay = true,
            SortingDirection = SortDirection.Ascending,
            SortingField = Statistic.Company
        };
    }

    public PortfolioStatisticsExportSettings Create()
    {
        return new PortfolioStatisticsExportSettings(
            Spacing,
            Colours,
            SecurityDisplayOptions.ShouldDisplay,
            SecurityDisplayOptions.SortingField,
            SecurityDisplayOptions.SortingDirection,
            SecurityDisplayOptions.DisplayFields ?? AccountStatisticsHelpers.DefaultSecurityStats(),

            BankAccountDisplayOptions.ShouldDisplay,
            BankAccountDisplayOptions.SortingField,
            BankAccountDisplayOptions.SortingDirection,
            BankAccountDisplayOptions.DisplayFields ?? AccountStatisticsHelpers.DefaultBankAccountStats(),

            SectorDisplayOptions.ShouldDisplay,
            SectorDisplayOptions.SortingField,
            SectorDisplayOptions.SortingDirection,
            SectorDisplayOptions.DisplayFields ?? AccountStatisticsHelpers.DefaultSectorStats(),

            AssetDisplayOptions.ShouldDisplay,
            AssetDisplayOptions.SortingField,
            AssetDisplayOptions.SortingDirection,
            AssetDisplayOptions.DisplayFields ?? AccountStatisticsHelpers.DefaultAssetStats(),

            CurrencyDisplayOptions.ShouldDisplay,
            CurrencyDisplayOptions.SortingField,
            CurrencyDisplayOptions.SortingDirection,
            CurrencyDisplayOptions.DisplayFields ?? AccountStatisticsHelpers.DefaultCurrencyStats());
    }
}
