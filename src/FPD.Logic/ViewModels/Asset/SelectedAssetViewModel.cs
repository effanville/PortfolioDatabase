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
using Effanville.FinancialStructures.Database.Download;
using Effanville.FinancialStructures.Database.Extensions.DataEdit;
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
    public sealed class SelectedAssetViewModel : StyledClosableViewModelBase<IAmortisableAsset, IPortfolio>
    {
        private readonly IPortfolio _portfolio;

        /// <summary>
        /// The name data of the security this window details.
        /// </summary>
        internal readonly NameData SelectedName;

        private readonly Account _dataType;

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
            IPortfolio portfolio,
            IAmortisableAsset asset,
            UiStyles styles,
            UiGlobals globals,
            NameData selectedName,
            Account dataType,
            IUpdater<IPortfolio> dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", asset, globals, styles, true)
        {
            _portfolio = portfolio;
            SelectedName = selectedName;
            _dataType = dataType;
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            UpdateRequest += dataUpdater.PerformUpdate;
            string currencySymbol =
                CurrencyCultureHelpers.CurrencySymbol(asset.Names.Currency);
            ValuesTLVM = new TimeListViewModel(asset.Values, $"Values({currencySymbol})", Styles,
                value => DeleteValue(SelectedName, value),
                (old, newVal) => ExecuteAddEditValues(SelectedName, old, newVal));
            DebtTLVM = new TimeListViewModel(asset.Debt, $"Debt({currencySymbol})", Styles,
                value => DeleteDebtValue(SelectedName, value),
                (old, newVal) => ExecuteAddEditDebt(SelectedName, old, newVal));
            PaymentsTLVM = new TimeListViewModel(asset.Payments, $"Payments({currencySymbol})", Styles,
                value => DeletePaymentValue(SelectedName, value),
                (old, newVal) => ExecuteAddEditPayment(SelectedName, old, newVal));
            ValuesTLVM.UpdateRequest += dataUpdater.PerformUpdate;
            DebtTLVM.UpdateRequest += dataUpdater.PerformUpdate;
            PaymentsTLVM.UpdateRequest += dataUpdater.PerformUpdate;
            Statistics = new AccountStatsViewModel(null, Styles);
        }

        private void DeleteValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryDeleteData(_dataType, name.ToTwoName(), value.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
            }
        }

        private void DeleteDebtValue(NameData name, DailyValuation value)
        {
            if (name == null || value == null)
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
                return;
            }

            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                programPortfolio =>
                    programPortfolio.TryDeleteAssetDebt(_dataType, name.ToTwoName(), value.Day, ReportLogger)));
        }

        private void DeletePaymentValue(NameData name, DailyValuation value)
        {
            if (name == null || value == null)
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
                return;
            }

            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                programPortfolio =>
                    programPortfolio.TryDeleteAssetPayment(_dataType, name.ToTwoName(), value.Day,
                        ReportLogger)));
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

            NameData names = SelectedName;
            OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                async programPortfolio => await PortfolioDataUpdater
                    .Download(_dataType, programPortfolio, names, ReportLogger).ConfigureAwait(false)));
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

        private void ExecuteAddEditValues(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditData(_dataType, name.ToTwoName(), oldValue,
                        newValue, ReportLogger)));
            }
        }

        private void ExecuteAddEditDebt(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditAssetDebt(_dataType, name.ToTwoName(),
                        oldValue, newValue, ReportLogger)));
            }
        }

        private void ExecuteAddEditPayment(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditAssetPayment(_dataType, name.ToTwoName(),
                        oldValue, newValue, ReportLogger)));
            }
        }

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
            var stats = new AccountStatistics(
                _portfolio,
                DateTime.Today,
                modelData,
                AccountStatisticsHelpers.DefaultAssetStats());
            Statistics.UpdateData(stats, force);
            Values = modelData.ListOfValues();
        }
    }
}