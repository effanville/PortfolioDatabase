using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Common.Structure.DataStructures;
using Common.Structure.FileAccess;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;
using Common.UI.ViewModelBases;
using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Extensions.Statistics;
using FinancialStructures.Database.Statistics;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures;
using FPD.Logic.ViewModels.Stats;
using Common.Structure.DataEdit;

namespace FPD.Logic.ViewModels.Security
{
    /// <summary>
    /// View model for the display of a security data.
    /// </summary>
    public class SelectedSecurityViewModel : StyledClosableViewModelBase<IPortfolio>
    {
        private readonly Account fAccount;

        internal SecurityTrade fOldSelectedTrade;
        internal SecurityTrade SelectedTrade;

        /// <summary>
        /// The name data of the security this window details.
        /// </summary>
        public NameData SelectedName
        {
            get;
        }

        private TimeListViewModel fTLVM;

        /// <summary>
        /// View Model for the unit price data of the Security.
        /// </summary>
        public TimeListViewModel TLVM
        {
            get => fTLVM;
            set => SetAndNotify(ref fTLVM, value);
        }

        private List<SecurityTrade> fTrades = new List<SecurityTrade>();

        /// <summary>
        /// The list of trades to display.
        /// </summary>
        public List<SecurityTrade> Trades
        {
            get => fTrades;
            set => SetAndNotify(ref fTrades, value);
        }

        private string fTradePriceHeader;

        /// <summary>
        /// The header for the Trade Price column.
        /// </summary>
        public string TradePriceHeader
        {
            get => fTradePriceHeader;
            set => SetAndNotify(ref fTradePriceHeader, value);
        }

        private string fTradeTotalCostHeader;

        /// <summary>
        /// The header for the Trade Price column.
        /// </summary>
        public string TradeTotalCostHeader
        {
            get => fTradeTotalCostHeader;
            set => SetAndNotify(ref fTradeTotalCostHeader, value);
        }

        private AccountStatsViewModel fSecurityStats;

        /// <summary>
        /// The statistics for the security.
        /// </summary>
        public AccountStatsViewModel SecurityStats
        {
            get => fSecurityStats;
            set => SetAndNotify(ref fSecurityStats, value);
        }

        private List<DailyValuation> fValues;

        /// <summary>
        /// List of total held amount of the security.
        /// </summary>
        public List<DailyValuation> Values
        {
            get => fValues;
            set => SetAndNotify(ref fValues, value, nameof(Values));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectedSecurityViewModel(
            IPortfolio portfolio, 
            UiStyles styles, 
            UiGlobals globals, 
            NameData selectedName, 
            Account account,
            IUpdater<IPortfolio> dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", portfolio, globals, styles, true)
        {
            SelectedName = selectedName;
            DeleteValuationCommand = new RelayCommand(ExecuteDeleteValuation);
            AddCsvData = new RelayCommand(ExecuteAddCsvData);
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            AddEditDataCommand = new RelayCommand(ExecuteAddEditData);
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            fAccount = account;
            UpdateRequest += dataUpdater.PerformUpdate;

            if (portfolio.TryGetAccount(fAccount, SelectedName, out IValueList desired))
            {
                ISecurity security = desired as ISecurity;
                string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(security.Names.Currency ?? portfolio.BaseCurrency);
                TradePriceHeader = $"Price({currencySymbol})";
                TradeTotalCostHeader = $"Total Cost({currencySymbol})";
                TLVM = new TimeListViewModel(security.UnitPrice, $"UnitPrice({currencySymbol})", Styles, value => DeleteValue(SelectedName, value), (old, newVal) => ExecuteAddEditUnitPriceData(SelectedName, old, newVal));
                TLVM.UpdateRequest += dataUpdater.PerformUpdate;
                var securityStats = portfolio.GetStats(DateTime.Today, fAccount, SelectedName, AccountStatisticsHelpers.AllStatistics()).Single();
                SecurityStats = new AccountStatsViewModel(securityStats, Styles);
            }
            else
            {
                string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(portfolio.BaseCurrency);
                TradePriceHeader = $"Price({currencySymbol})";
                TradeTotalCostHeader = $"Total Cost({currencySymbol})";
                TLVM = new TimeListViewModel(null, $"UnitPrice({currencySymbol})", Styles, value => DeleteValue(SelectedName, value), (old, newVal) => ExecuteAddEditUnitPriceData(SelectedName, old, newVal));
                TLVM.UpdateRequest += dataUpdater.PerformUpdate;
                SecurityStats = new AccountStatsViewModel(null, Styles);
            }

            UpdateData(portfolio);
        }

        /// <summary>
        /// Command to delete a selected Trade.
        /// </summary>
        public ICommand DeleteValuationCommand
        {
            get;
        }

        private void ExecuteDeleteValuation() => DeleteValue(SelectedName, TLVM.SelectedValuation);

        private void DeleteValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(false, programPortfolio => programPortfolio.TryDeleteData(fAccount, name.ToTwoName(), value.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "No Account was selected when trying to delete data.");
            }
        }

        /// <summary>
        /// Downloads the latest data for the selected entry.
        /// </summary>
        public ICommand DownloadCommand
        {
            get;
        }

        private void ExecuteDownloadCommand()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Download selected for account {SelectedName} - a {fAccount}");
            if (SelectedName != null)
            {
                NameData names = SelectedName;
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, async programPortfolio => await PortfolioDataUpdater.Download(fAccount, programPortfolio, names, ReportLogger).ConfigureAwait(false)));
            }
        }

        /// <summary>
        /// Command to add data to the security from a csv file.
        /// </summary>
        public ICommand AddCsvData
        {
            get;
        }

        private void ExecuteAddCsvData()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccount} {SelectedName} adding data from csv.");
            if (SelectedName != null)
            {
                FileInteractionResult result = DisplayGlobals.FileInteractionService.OpenFile("csv", filter: "Csv Files|*.csv|All Files|*.*");
                List<object> outputs = null;
                bool exists = ModelData.TryGetAccount(fAccount, SelectedName, out IValueList account);
                if (result.Success && exists)
                {
                    ISecurity security = account as ISecurity;
                    outputs = CsvReaderWriter.ReadFromCsv(security, result.FilePath, ReportLogger);
                }
                if (outputs != null)
                {
                    foreach (object objec in outputs)
                    {
                        if (objec is SecurityDayData view)
                        {
                            var value = new DailyValuation(view.Date, view.UnitPrice);
                            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryAddOrEditData(fAccount, SelectedName, value, value, ReportLogger)));
                            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryAddOrEditTradeData(fAccount, SelectedName, view.Trade, view.Trade, ReportLogger)));
                        }
                        else
                        {
                            ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(), "Have the wrong type of thing");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Command to export the data to a csv file.
        /// </summary>
        public ICommand ExportCsvData
        {
            get;
        }

        private void ExecuteExportCsvData()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccount} {SelectedName} exporting data to csv.");
            if (SelectedName != null)
            {
                FileInteractionResult result = DisplayGlobals.FileInteractionService.SaveFile("csv", string.Empty, filter: "Csv Files|*.csv|All Files|*.*");
                if (result.Success)
                {
                    if (ModelData.TryGetAccount(fAccount, SelectedName, out IValueList account))
                    {
                        ISecurity security = account as ISecurity;
                        CsvReaderWriter.WriteToCSVFile(security, result.FilePath, ReportLogger);
                    }
                    else
                    {
                        _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Saving, $"Could not find {fAccount}.");
                    }
                }
            }
        }

        private void ExecuteAddEditUnitPriceData(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => _ = programPortfolio.TryAddOrEditData(fAccount, name.ToTwoName(), oldValue, newValue, ReportLogger)));
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData)
        {
            _ = ReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccount} {SelectedName} updating data.");
            base.UpdateData(modelData);
            if (SelectedName != null)
            {
                if (!modelData.Exists(fAccount, SelectedName))
                {
                    OnRequestClose(EventArgs.Empty);
                    return;
                }

                _ = modelData.TryGetAccount(fAccount, SelectedName, out IValueList desired);

                ISecurity security = desired as ISecurity;
                TLVM?.UpdateData(security.UnitPrice);
                Trades = security.Trades.ToList();
                var securityStats = modelData.GetStats(DateTime.Today, fAccount, SelectedName, AccountStatisticsHelpers.AllStatistics()).Single();
                SecurityStats.UpdateData(securityStats);
                Values = modelData.NumberData(fAccount, SelectedName, ReportLogger).ToList();
            }
        }

        private RelayCommand fPreEditCommand;

        /// <summary>
        /// Called prior to an edit occurring in a row. This is used
        /// to record the state of the row before editing.
        /// </summary>
        public ICommand PreEditCommand
        {
            get
            {
                fPreEditCommand ??= new RelayCommand(PreEdit);

                return fPreEditCommand;
            }
        }

        private void PreEdit() => fOldSelectedTrade = SelectedTrade?.Copy();

        /// <summary>
        /// Retrieve the default value for a new trade.
        /// </summary>
        /// <returns></returns>
        public SecurityTrade DefaultTradeValue()
        {
            return new SecurityTrade()
            {
                TradeType = TradeType.Buy,
                Names = SelectedName.ToTwoName(),
                Day = DateTime.Today
            };
        }

        /// <summary>
        /// Command to update the selected trade.
        /// </summary>
        public ICommand SelectionChangedCommand
        {
            get;
            set;
        }

        private void ExecuteSelectionChanged(object obj)
        {
            if (Trades != null && obj is SecurityTrade trade)
            {
                SelectedTrade = trade;
            }
        }

        /// <summary>
        /// Command to add or edit data in the Trade list.
        /// </summary>
        public ICommand AddEditDataCommand
        {
            get;
            set;
        }

        private void ExecuteAddEditData()
        {
            if (SelectedTrade != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => _ = programPortfolio.TryAddOrEditTradeData(fAccount, SelectedName.ToTwoName(), fOldSelectedTrade, SelectedTrade, ReportLogger)));
            }
        }

        /// <summary>
        /// Deletes the pre selected trade.
        /// </summary>
        public void DeleteTrade()
        {
            if (SelectedName != null && SelectedTrade != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryDeleteTradeData(fAccount, SelectedName, SelectedTrade.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "No Account was selected when trying to delete data.");
            }
        }
    }
}
