using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Effanville.Common.Structure.DisplayClasses;
using Effanville.Common.Structure.Extensions;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.Structure.ReportWriting;
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

        private const string ShowSecurities = "ShowSecurites";
        private const string ShowBankAccounts = "ShowBankAccounts";

        private const string ShowSectors = "ShowSectors";

        private const string ShowAssets = "ShowAssets";
        private const string ShowCurrencies = "ShowCurrencies";
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

        private Statistic _securitySortingField;

        /// <summary>
        /// The statistic to sort the security data by.
        /// </summary>
        public Statistic SecuritySortingField
        {
            get => _securitySortingField;
            set => SetAndNotify(ref _securitySortingField, value);
        }

        private SortDirection _securityDirection;

        /// <summary>
        /// The direction to sort the Security data in.
        /// </summary>
        public SortDirection SecurityDirection
        {
            get => _securityDirection;
            set => SetAndNotify(ref _securityDirection, value);
        }

        private List<Selectable<Statistic>> _securityColumnNames = new List<Selectable<Statistic>>();

        /// <summary>
        /// The possible columns for security export, and which ones are selected.
        /// </summary>
        public List<Selectable<Statistic>> SecurityColumnNames
        {
            get => _securityColumnNames;
            set => SetAndNotify(ref _securityColumnNames, value);
        }

        private Statistic _bankSortingField;

        /// <summary>
        /// The statistic to sort the bank account data by.
        /// </summary>
        public Statistic BankSortingField
        {
            get => _bankSortingField;
            set => SetAndNotify(ref _bankSortingField, value);
        }

        private SortDirection _bankDirection;

        /// <summary>
        /// The direction to sort the Bank Account data in.
        /// </summary>
        public SortDirection BankDirection
        {
            get => _bankDirection;
            set => SetAndNotify(ref _bankDirection, value);
        }

        private List<Selectable<Statistic>> _bankColumnNames = new List<Selectable<Statistic>>();

        /// <summary>
        /// The possible columns for bank account export, and which ones are selected.
        /// </summary>
        public List<Selectable<Statistic>> BankColumnNames
        {
            get => _bankColumnNames;
            set => SetAndNotify(ref _bankColumnNames, value);
        }

        private Statistic _sectorSortingField;

        /// <summary>
        /// The statistic to sort the sector data by.
        /// </summary>
        public Statistic SectorSortingField
        {
            get => _sectorSortingField;
            set => SetAndNotify(ref _sectorSortingField, value);
        }


        private SortDirection _sectorDirection;

        /// <summary>
        /// The direction to sort the Sector data in.
        /// </summary>
        public SortDirection SectorDirection
        {
            get => _sectorDirection;
            set => SetAndNotify(ref _sectorDirection, value);
        }

        private List<Selectable<Statistic>> _sectorColumnNames = new List<Selectable<Statistic>>();

        /// <summary>
        /// The possible columns for sector export, and which ones are selected.
        /// </summary>
        public List<Selectable<Statistic>> SectorColumnNames
        {
            get => _sectorColumnNames;
            set => SetAndNotify(ref _sectorColumnNames, value);
        }

        private Statistic _assetSortingField;

        /// <summary>
        /// The statistic to sort the Asset data by.
        /// </summary>
        public Statistic AssetSortingField
        {
            get => _assetSortingField;
            set => SetAndNotify(ref _assetSortingField, value);
        }

        private SortDirection _assetDirection;

        /// <summary>
        /// The direction to sort the Asset data in.
        /// </summary>
        public SortDirection AssetDirection
        {
            get => _assetDirection;
            set => SetAndNotify(ref _assetDirection, value);
        }

        private List<Selectable<Statistic>> _assetColumnNames = new List<Selectable<Statistic>>();

        /// <summary>
        /// The possible columns for Asset export, and which ones are selected.
        /// </summary>
        public List<Selectable<Statistic>> AssetColumnNames
        {
            get => _assetColumnNames;
            set => SetAndNotify(ref _assetColumnNames, value);
        }

        private Statistic _currencySortingField;

        /// <summary>
        /// The statistic to sort the Asset data by.
        /// </summary>
        public Statistic CurrencySortingField
        {
            get => _currencySortingField;
            set => SetAndNotify(ref _currencySortingField, value);
        }
        
        private SortDirection _currencyDirection;

        /// <summary>
        /// The direction to sort the Asset data in.
        /// </summary>
        public SortDirection CurrencyDirection
        {
            get => _currencyDirection;
            set => SetAndNotify(ref _currencyDirection, value);
        }

        private List<Selectable<Statistic>> _currencyColumnNames = new List<Selectable<Statistic>>();

        /// <summary>
        /// The possible columns for Asset export, and which ones are selected.
        /// </summary>
        public List<Selectable<Statistic>> CurrencyColumnNames
        {
            get => _currencyColumnNames;
            set => SetAndNotify(ref _currencyColumnNames, value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExportStatsViewModel(UiGlobals globals, IUiStyles styles, IConfiguration userConfiguration, IPortfolio portfolio, Action<object> closeWindow)
            : base(globals, styles, userConfiguration, portfolio, "", Account.All)
        {
            _closeWindowAction = closeWindow;
            ExportCommand = new RelayCommand(ExecuteExportCommand);

            foreach (Statistic stat in AccountStatisticsHelpers.DefaultSecurityStats())
            {
                SecurityColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            SecuritySortingField = Statistic.Company;

            foreach (Statistic stat in AccountStatisticsHelpers.DefaultSectorStats())
            {
                SectorColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            SectorSortingField = Statistic.Name;

            foreach (Statistic stat in AccountStatisticsHelpers.DefaultBankAccountStats())
            {
                BankColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            BankSortingField = Statistic.Company;

            foreach (Statistic stat in AccountStatisticsHelpers.DefaultAssetStats())
            {
                AssetColumnNames.Add(new Selectable<Statistic>(stat, true));
            }
            
            AssetSortingField = Statistic.Company;
            
            foreach (Statistic stat in AccountStatisticsHelpers.DefaultCurrencyStats())
            {
                CurrencyColumnNames.Add(new Selectable<Statistic>(stat, true));
            }
            
            CurrencySortingField = Statistic.Name;
            
            DisplayConditions.Add(new Selectable<string>(ValueFunds, true));
            DisplayConditions.Add(new Selectable<string>(Spacing, true));
            DisplayConditions.Add(new Selectable<string>(Colouring, true));
            DisplayConditions.Add(new Selectable<string>(ShowSecurities, true));
            DisplayConditions.Add(new Selectable<string>(ShowBankAccounts, true));
            DisplayConditions.Add(new Selectable<string>(ShowSectors, true));
            DisplayConditions.Add(new Selectable<string>(ShowBenchmarks, false));
            DisplayConditions.Add(new Selectable<string>(ShowAssets, false));
            DisplayConditions.Add(new Selectable<string>(ShowCurrencies, false));

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
        public ICommand ExportCommand
        {
            get;
        }

        private void ExecuteExportCommand()
        {
            UserConfiguration.StoreConfiguration(this);
            FileInteractionResult result = DisplayGlobals.FileInteractionService.SaveFile(DocumentType.Html.ToString().ToLower(), ModelData.Name, filter: "Html Files|*.html|CSV Files|*.csv|All Files|*.*");
            string path = null;

            if (result.Success)
            {
                path = result.FilePath;

                List<Statistic> securitySelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in SecurityColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        securitySelected.Add(column.Instance);
                    }
                }

                List<Statistic> BankSelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in BankColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        BankSelected.Add(column.Instance);
                    }
                }

                List<Statistic> sectorSelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in SectorColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        sectorSelected.Add(column.Instance);
                    }
                }

                List<Statistic> assetSelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in AssetColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        assetSelected.Add(column.Instance);
                    }
                }

                List<Statistic> currencySelected = new List<Statistic>();
                foreach (Selectable<Statistic> column in CurrencyColumnNames)
                {
                    if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                    {
                        currencySelected.Add(column.Instance);
                    }
                }

                PortfolioStatisticsSettings settings = new PortfolioStatisticsSettings(
                    DateTime.Today,
                    SelectableHelpers.GetData(DisplayConditions, ValueFunds),
                    SelectableHelpers.GetData(DisplayConditions, ShowBenchmarks),
                    SelectableHelpers.GetData(DisplayConditions, ShowSecurities),
                    securitySelected.Union(new List<Statistic>() { SecuritySortingField }).ToList(),
                    SelectableHelpers.GetData(DisplayConditions, ShowBankAccounts),
                    BankSelected.Union(new List<Statistic>() { BankSortingField }).ToList(),
                    SelectableHelpers.GetData(DisplayConditions, ShowSectors),
                    sectorSelected.Union(new List<Statistic>() { SectorSortingField }).ToList(),
                    SelectableHelpers.GetData(DisplayConditions, ShowAssets),
                    assetSelected.Union(new List<Statistic>() { AssetSortingField }).ToList(),
                    SelectableHelpers.GetData(DisplayConditions, ShowCurrencies),
                    currencySelected.Union(new List<Statistic>() { CurrencySortingField }).ToList());

                PortfolioStatistics stats = new PortfolioStatistics(ModelData, settings, DisplayGlobals.CurrentFileSystem);
                string extension = DisplayGlobals.CurrentFileSystem.Path.GetExtension(result.FilePath).Trim('.');
                DocumentType type = extension.ToEnum<DocumentType>();

                PortfolioStatisticsExportSettings exportSettings = new PortfolioStatisticsExportSettings(
                    SelectableHelpers.GetData(DisplayConditions, Spacing),
                    SelectableHelpers.GetData(DisplayConditions, Colouring),
                    SelectableHelpers.GetData(DisplayConditions, ShowSecurities),
                    SecuritySortingField,
                    SecurityDirection,
                    securitySelected,
                    SelectableHelpers.GetData(DisplayConditions, ShowBankAccounts),
                    BankSortingField,
                    BankDirection,
                    BankSelected,
                    SelectableHelpers.GetData(DisplayConditions, ShowSectors),
                    SectorSortingField,
                    SectorDirection,
                    sectorSelected,
                    SelectableHelpers.GetData(DisplayConditions, ShowAssets),
                    AssetSortingField,
                    AssetDirection,
                    assetSelected,
                    SelectableHelpers.GetData(DisplayConditions, ShowCurrencies),
                    CurrencySortingField,
                    CurrencyDirection,
                    currencySelected);

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
