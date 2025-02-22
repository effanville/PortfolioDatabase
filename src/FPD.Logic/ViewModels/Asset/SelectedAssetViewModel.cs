using System;
using System.Collections.Generic;
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
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels.Asset
{
    /// <summary>
    /// View model for the display of a security data.
    /// </summary>
    public sealed class SelectedAssetViewModel : StyledClosableViewModelBase<IAmortisableAsset>
    {
        private readonly IAccountStatisticsProvider _statisticsProvider;
        private readonly IUpdater _updater;

        /// <summary>
        /// The name data of the security this window details.
        /// </summary>
        internal readonly NameData SelectedName;

        private readonly Account _dataType;
        private readonly IPortfolioDataDownloader _portfolioDataDownloader;
        private TimeListViewModel _valuesTLVM;

        /// <summary>
        /// View Model for the unit price data of the Security.
        /// </summary>
        public TimeListViewModel ValuesTLVM
        {
            get => _valuesTLVM;
            set => SetAndNotify(ref _valuesTLVM, value);
        }

        private TimeListViewModel _debtTLVM;

        /// <summary>
        /// View Model for the unit price data of the Security.
        /// </summary>
        public TimeListViewModel DebtTLVM
        {
            get => _debtTLVM;
            set => SetAndNotify(ref _debtTLVM, value);
        }

        private TimeListViewModel _paymentsTLVM;

        /// <summary>
        /// View Model for the payments for the Asset.
        /// </summary>
        public TimeListViewModel PaymentsTLVM
        {
            get => _paymentsTLVM;
            set => SetAndNotify(ref _paymentsTLVM, value);
        }

        private AccountStatsViewModel _statistics;

        /// <summary>
        /// The statistics for the security.
        /// </summary>
        public AccountStatsViewModel Statistics
        {
            get => _statistics;
            set => SetAndNotify(ref _statistics, value);
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
        public SelectedAssetViewModel(
            IAccountStatisticsProvider statisticsProvider,
            IAmortisableAsset asset,
            IUiStyles styles,
            UiGlobals globals,
            NameData selectedName,
            Account dataType,
            IUpdater dataUpdater,
            IPortfolioDataDownloader portfolioDataDownloader)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", asset, globals, styles, true)
        {
            _statisticsProvider = statisticsProvider;
            SelectedName = selectedName;
            _dataType = dataType;
            _updater = dataUpdater;
            _portfolioDataDownloader = portfolioDataDownloader;
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(asset.Names.Currency);
            ValuesTLVM = new TimeListViewModel(
                asset.Values,
                $"Values({currencySymbol})",
                Styles,
                DeleteValue,
                ExecuteAddEditValues);
            DebtTLVM = new TimeListViewModel(
                asset.Debt,
                $"Debt({currencySymbol})",
                Styles,
                DeleteDebtValue,
                ExecuteAddEditDebt);
            PaymentsTLVM = new TimeListViewModel(
                asset.Payments,
                $"Payments({currencySymbol})",
                Styles,
                DeletePaymentValue,
                ExecuteAddEditPayment);
            Statistics = new AccountStatsViewModel(null, Styles);
        }

        private void DeleteValue(DailyValuation value)
            => _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                asset => asset.TryDeleteData(value.Day)));

        private void DeleteDebtValue(DailyValuation value)
            => _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                asset => asset.TryDeleteDebt(value.Day)));

        private void DeletePaymentValue(DailyValuation value)
            => _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                asset => asset.TryDeletePayment(value.Day)));

        /// <summary>
        /// Downloads the latest data for the selected entry.
        /// </summary>
        public ICommand DownloadCommand { get; }

        private void ExecuteDownloadCommand()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Download selected for account {SelectedName} - a {_dataType}");
            if (SelectedName == null)
            {
                return;
            }

            NameData names = SelectedName;
            _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                async asset => await _portfolioDataDownloader.Download(ModelData, ReportLogger).ConfigureAwait(false)));
        }

        /// <summary>
        /// Command to export the data to a csv file.
        /// </summary>
        public ICommand ExportCsvData { get; }

        private async void ExecuteExportCsvData()
        {
            _ = ReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Selected {_dataType} {SelectedName} exporting data to csv.");
            if (SelectedName == null)
            {
                return;
            }

            FileInteractionResult result = await DisplayGlobals.FileInteractionService.SaveFile(
                "csv",
                string.Empty,
                filter: "Csv Files|*.csv|All Files|*.*");
            if (result.Success)
            {
                CsvReaderWriter.WriteToCSVFile(ModelData, result.FilePath, ReportLogger);
            }
        }

        private void ExecuteAddEditValues(DailyValuation oldValue, DailyValuation newValue)
            => _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                asset => _ = asset.TryEditData(oldValue.Day, newValue.Day, newValue.Value)));

        private void ExecuteAddEditDebt(DailyValuation oldValue, DailyValuation newValue)
            => _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                asset => _ = asset.TryEditDebt(oldValue.Day, newValue.Day, newValue.Value)));

        private void ExecuteAddEditPayment(DailyValuation oldValue, DailyValuation newValue)
            => _updater.PerformUpdate(ModelData, new UpdateRequestArgs<IAmortisableAsset>(true,
                asset => _ = asset.TryEditPayment(oldValue.Day, newValue.Day, newValue.Value)));

        /// <inheritdoc/>
        public override void UpdateData(IAmortisableAsset modelData, bool force)
        {
            _ = ReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Selected {_dataType} {SelectedName} updating data.");
            base.UpdateData(modelData, force);
            if (SelectedName == null)
            {
                return;
            }

            if (modelData == null)
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }

            ValuesTLVM?.UpdateData(modelData.Values, force);
            DebtTLVM?.UpdateData(modelData.Debt, force);
            PaymentsTLVM?.UpdateData(modelData.Payments, force);
            var stats = _statisticsProvider.GetStats(
                modelData,
                DateTime.Today,
                AccountStatisticsHelpers.DefaultAssetStats());
            Statistics.UpdateData(stats, force);
            Values = modelData.ListOfValues();
        }
    }
}