using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.FileAccess;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.Services;
using Effanville.FinancialStructures;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Download;
using Effanville.FinancialStructures.Database.Extensions.DataEdit;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels.Security
{
    /// <summary>
    /// View model for the display of a security data.
    /// </summary>
    public class SelectedSecurityViewModel : StyledClosableViewModelBase<ISecurity, IPortfolio>
    {
        private readonly IPortfolio _portfolio;
        private readonly Account _dataType;

        private SecurityTrade _oldSelectedTrade;
        private SecurityTrade _selectedTrade;

        /// <summary>
        /// The name data of the security this window details.
        /// </summary>
        public TwoName SelectedName
        {
            get;
        }

        private TimeListViewModel _TLVM;

        /// <summary>
        /// View Model for the unit price data of the Security.
        /// </summary>
        public TimeListViewModel TLVM
        {
            get => _TLVM;
            set => SetAndNotify(ref _TLVM, value);
        }

        private List<SecurityTrade> _trades = new List<SecurityTrade>();

        /// <summary>
        /// The list of trades to display.
        /// </summary>
        public List<SecurityTrade> Trades
        {
            get => _trades;
            set => SetAndNotify(ref _trades, value);
        }

        private string _tradePriceHeader;

        /// <summary>
        /// The header for the Trade Price column.
        /// </summary>
        public string TradePriceHeader
        {
            get => _tradePriceHeader;
            set => SetAndNotify(ref _tradePriceHeader, value);
        }

        private string _tradeTotalCostHeader;

        /// <summary>
        /// The header for the Trade Price column.
        /// </summary>
        public string TradeTotalCostHeader
        {
            get => _tradeTotalCostHeader;
            set => SetAndNotify(ref _tradeTotalCostHeader, value);
        }

        private AccountStatsViewModel _securityStats;

        /// <summary>
        /// The statistics for the security.
        /// </summary>
        public AccountStatsViewModel SecurityStats
        {
            get => _securityStats;
            set => SetAndNotify(ref _securityStats, value);
        }

        private List<DailyValuation> _values;

        /// <summary>
        /// List of total held amount of the security.
        /// </summary>
        public List<DailyValuation> Values
        {
            get => _values;
            set => SetAndNotify(ref _values, value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectedSecurityViewModel(
            IPortfolio portfolio,
            ISecurity security,
            UiStyles styles,
            UiGlobals globals,
            TwoName selectedName,
            Account account,
            IUpdater<IPortfolio> dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", security, globals, styles, true)
        {
            _portfolio = portfolio;
            SelectedName = selectedName;
            DeleteValuationCommand = new RelayCommand(ExecuteDeleteValuation);
            AddCsvData = new RelayCommand(ExecuteAddCsvData);
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            AddEditDataCommand = new RelayCommand(ExecuteAddEditData);
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            _dataType = account;
            UpdateRequest += dataUpdater.PerformUpdate;

            string currencySymbol =
                CurrencyCultureHelpers.CurrencySymbol(security.Names.Currency ?? portfolio.BaseCurrency);
            TradePriceHeader = $"Price({currencySymbol})";
            TradeTotalCostHeader = $"Total Cost({currencySymbol})";
            TLVM = new TimeListViewModel(security.UnitPrice, $"UnitPrice({currencySymbol})", Styles,
                value => DeleteValue(SelectedName, value),
                (old, newVal) => ExecuteAddEditUnitPriceData(SelectedName, old, newVal));
            TLVM.UpdateRequest += dataUpdater.PerformUpdate;
            SecurityStats = new AccountStatsViewModel(null, Styles);
        }

        /// <summary>
        /// Command to delete a selected Trade.
        /// </summary>
        public ICommand DeleteValuationCommand
        {
            get;
        }

        private void ExecuteDeleteValuation() => DeleteValue(SelectedName, TLVM.SelectedValuation);

        private void DeleteValue(TwoName name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(false,
                    programPortfolio =>
                        programPortfolio.TryDeleteData(_dataType, name, value.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
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
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Download selected for account {SelectedName} - a {_dataType}");
            if (SelectedName == null)
            {
                return;
            }

            TwoName names = SelectedName;
            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                async programPortfolio => await PortfolioDataUpdater
                    .Download(_dataType, programPortfolio, names, ReportLogger).ConfigureAwait(false)));
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
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Selected {_dataType} {SelectedName} adding data from csv.");
            if (SelectedName == null)
            {
                return;
            }

            FileInteractionResult result =
                DisplayGlobals.FileInteractionService.OpenFile("csv", filter: "Csv Files|*.csv|All Files|*.*");
            List<object> outputs = null;
            if (result.Success)
            {
                outputs = CsvReaderWriter.ReadFromCsv(ModelData, result.FilePath, ReportLogger);
            }

            if (outputs == null)
            {
                return;
            }

            foreach (object obj in outputs)
            {
                if (obj is SecurityDayData view)
                {
                    var value = new DailyValuation(view.Date, view.UnitPrice);
                    OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                        programPortfolio =>
                            programPortfolio.TryAddOrEditData(_dataType, SelectedName, value, value,
                                ReportLogger)));
                    OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                        programPortfolio => programPortfolio.TryAddOrEditTradeData(_dataType, SelectedName,
                            view.Trade, view.Trade, ReportLogger)));
                }
                else
                {
                    ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(),
                        "Have the wrong type of thing");
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
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Selected {_dataType} {SelectedName} exporting data to csv.");
            if (SelectedName == null)
            {
                return;
            }

            FileInteractionResult result =
                DisplayGlobals.FileInteractionService.SaveFile("csv", string.Empty,
                    filter: "Csv Files|*.csv|All Files|*.*");
            if (result.Success)
            {
                CsvReaderWriter.WriteToCSVFile(ModelData, result.FilePath, ReportLogger);
            }
        }

        private void ExecuteAddEditUnitPriceData(TwoName name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        _ = programPortfolio.TryAddOrEditData(_dataType, name, oldValue, newValue,
                            ReportLogger)));
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(ISecurity modelData)
        {
            _ = ReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Selected {_dataType} {SelectedName} updating data.");
            base.UpdateData(modelData);
            if (SelectedName == null || modelData == null)
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }

            TLVM?.UpdateData(ModelData.UnitPrice);
            Trades = ModelData.Trades.ToList();
            var securityStats = new AccountStatistics(
                _portfolio,
                DateTime.Today,
                _dataType,
                ModelData.Names,
                AccountStatisticsHelpers.AllStatistics());
            SecurityStats.UpdateData(securityStats);
            Values = modelData.ListOfValues().ToList();
        }

        private RelayCommand _preEditCommand;

        /// <summary>
        /// Called prior to an edit occurring in a row. This is used
        /// to record the state of the row before editing.
        /// </summary>
        public ICommand PreEditCommand
        {
            get
            {
                _preEditCommand ??= new RelayCommand(PreEdit);
                return _preEditCommand;
            }
        }

        private void PreEdit() => _oldSelectedTrade = _selectedTrade?.Copy();

        /// <summary>
        /// Retrieve the default value for a new trade.
        /// </summary>
        /// <returns></returns>
        public SecurityTrade DefaultTradeValue() =>
            new SecurityTrade()
            {
                TradeType = TradeType.Buy, Names = SelectedName, Day = DateTime.Today
            };

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
                _selectedTrade = trade;
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
            if (_selectedTrade != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditTradeData(_dataType, SelectedName,
                        _oldSelectedTrade, _selectedTrade, ReportLogger)));
            }
        }

        /// <summary>
        /// Deletes the pre selected trade.
        /// </summary>
        public void DeleteTrade()
        {
            if (SelectedName != null && _selectedTrade != null)
            {
                Trades.Remove(_selectedTrade);
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryDeleteTradeData(_dataType, SelectedName, _selectedTrade.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
            }
        }
    }
}