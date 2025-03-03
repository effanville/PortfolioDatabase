using System;
using System.Collections.Generic;
using System.Windows.Input;

using Effanville.Common.ReportWriting.Documents;
using Effanville.Common.Structure.DisplayClasses;
using Effanville.Common.Structure.Extensions;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Export.Statistics;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    /// <summary>
    /// View model for the stats options page.
    /// </summary>
    public sealed class ExportStatsViewModel : DataDisplayViewModelBase
    {
        private readonly Action<object> _closeWindowAction;

        private List<Selectable<string>> _displayConditions = new List<Selectable<string>>();

        private const string ShowBenchmarks = "ShowBenchmarks";
        private const string ValueFunds = "DisplayValueFunds";
        private const string Colouring = "Colours";
        private const string Spacing = "Spacing";

        /// <summary>
        /// Miscellaneous selections for how the exported file should look.
        /// </summary>
        public List<Selectable<string>> DisplayConditions
        {
            get => _displayConditions;
            set => SetAndNotify(ref _displayConditions, value);
        }

        private ExportDataViewModel _securitySortingData;

        public ExportDataViewModel SecuritySortingData
        {
            get => _securitySortingData;
            set => SetAndNotify(ref _securitySortingData, value);
        }

        private ExportDataViewModel _bankAccountSortingData;

        public ExportDataViewModel BankAccountSortingData
        {
            get => _bankAccountSortingData;
            set => SetAndNotify(ref _bankAccountSortingData, value);
        }

        private ExportDataViewModel _assetSortingData;

        public ExportDataViewModel AssetSortingData
        {
            get => _assetSortingData;
            set => SetAndNotify(ref _assetSortingData, value);
        }

        private ExportDataViewModel _sectorSortingData;

        public ExportDataViewModel SectorSortingData
        {
            get => _sectorSortingData;
            set => SetAndNotify(ref _sectorSortingData, value);
        }

        private ExportDataViewModel _currencySortingData;

        public ExportDataViewModel CurrencySortingData
        {
            get => _currencySortingData;
            set => SetAndNotify(ref _currencySortingData, value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExportStatsViewModel(UiGlobals globals, IUiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> closeWindow)
            : base(globals, styles, userConfiguration, portfolio, null, "", Account.All)
        {
            _closeWindowAction = closeWindow;
            ExportCommand = new RelayCommand(ExecuteExportCommand);

            TableOptions<Statistic> securityData = new TableOptions<Statistic>(true, Statistic.Company, SortDirection.Ascending, null);
            SecuritySortingData = new ExportDataViewModel("Securities", securityData, DisplayGlobals, Styles, AccountStatisticsHelpers.DefaultSecurityStats());

            TableOptions<Statistic> bankAccountData = new TableOptions<Statistic>(true, Statistic.Company, SortDirection.Ascending, null);
            BankAccountSortingData = new ExportDataViewModel("BankAccounts", bankAccountData, DisplayGlobals, Styles, AccountStatisticsHelpers.DefaultBankAccountStats());

            TableOptions<Statistic> sectorData = new TableOptions<Statistic>(true, Statistic.Name, SortDirection.Ascending, null);
            SectorSortingData = new ExportDataViewModel("Sectors", sectorData, DisplayGlobals, Styles, AccountStatisticsHelpers.DefaultSectorStats());

            TableOptions<Statistic> assetData = new TableOptions<Statistic>(false, Statistic.Company, SortDirection.Ascending, null);
            AssetSortingData = new ExportDataViewModel("Assets", assetData, DisplayGlobals, Styles, AccountStatisticsHelpers.DefaultAssetStats());

            TableOptions<Statistic> currencyData = new TableOptions<Statistic>(false, Statistic.Name, SortDirection.Ascending, null);
            CurrencySortingData = new ExportDataViewModel("Currencies", currencyData, DisplayGlobals, Styles, AccountStatisticsHelpers.DefaultCurrencyStats());


            DisplayConditions.Add(new Selectable<string>(ValueFunds, true));
            DisplayConditions.Add(new Selectable<string>(Spacing, true));
            DisplayConditions.Add(new Selectable<string>(Colouring, true));
            DisplayConditions.Add(new Selectable<string>(ShowBenchmarks, false));

            if (UserConfiguration.HasLoaded)
            {
                UserConfiguration.RestoreFromConfiguration(this);
            }
            else
            {
                UserConfiguration.HasLoaded = true;
                UserConfiguration.StoreConfiguration(this);
            }
        }

        /// <summary>
        /// Command to instantiate the export of statistics.
        /// </summary>
        public ICommand ExportCommand { get; }

        private async void ExecuteExportCommand()
        {
            UserConfiguration.StoreConfiguration(this);
            FileInteractionResult result = await DisplayGlobals.FileInteractionService.SaveFile(
                DocumentType.Html.ToString().ToLower(),
                ModelData.Name,
                filter: "Html Files|*.html|CSV Files|*.csv|All Files|*.*");
            string path = null;

            if (result.Success)
            {
                path = result.FilePath;

                PortfolioStatisticsSettings settings = new PortfolioStatisticsSettings(
                    DateTime.Today,
                    SelectableHelpers.GetData(DisplayConditions, ValueFunds),
                    SelectableHelpers.GetData(DisplayConditions, ShowBenchmarks),
                    SecuritySortingData.CreateOptions(),
                    BankAccountSortingData.CreateOptions(),
                    SectorSortingData.CreateOptions(),
                    AssetSortingData.CreateOptions(),
                    CurrencySortingData.CreateOptions());

                PortfolioStatistics stats = new PortfolioStatistics(ModelData, settings, DisplayGlobals.CurrentFileSystem);
                string extension = DisplayGlobals.CurrentFileSystem.Path.GetExtension(result.FilePath).Trim('.');
                DocumentType type = extension.ToEnum<DocumentType>();

                PortfolioStatisticsExportSettings exportSettings = new PortfolioStatisticsExportSettings(
                    SelectableHelpers.GetData(DisplayConditions, Spacing),
                    SelectableHelpers.GetData(DisplayConditions, Colouring),
                    SecuritySortingData.CreateTableOptions(),
                    BankAccountSortingData.CreateTableOptions(),
                    SectorSortingData.CreateTableOptions(),
                    AssetSortingData.CreateTableOptions(),
                    CurrencySortingData.CreateTableOptions());
                stats.ExportToFile(DisplayGlobals.CurrentFileSystem, result.FilePath, type, exportSettings, ReportLogger);

                ReportLogger.Log(ReportType.Information, ReportLocation.StatisticsPage.ToString(), "Created statistics page");
            }
            else
            {
                ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.StatisticsPage.ToString(), "Was not able to create page in place specified.");
            }

            _closeWindowAction(new HtmlViewerViewModel(Styles, DisplayGlobals, "Exported Stats", path));
        }
    }
}
