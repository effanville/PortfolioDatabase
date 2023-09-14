using System;
using System.Collections.Generic;
using System.Windows.Input;

using Common.Structure.DataEdit;
using Common.Structure.DataStructures;
using Common.Structure.FileAccess;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;

using FinancialStructures;
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Statistics;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels.Asset
{
    /// <summary>
    /// View model for the display of a security data.
    /// </summary>
    public sealed class SelectedAssetViewModel : StyledClosableViewModelBase<IAmortisableAsset, IPortfolio>
    {
        private IPortfolio _portfolio;
        /// <summary>
        /// The name data of the security this window details.
        /// </summary>
        internal readonly NameData SelectedName;

        private readonly Account fAccountType;

        internal SecurityTrade fOldSelectedTrade;
        internal SecurityTrade SelectedTrade;

        private TimeListViewModel fValuesTLVM;

        /// <summary>
        /// View Model for the unit price data of the Security.
        /// </summary>
        public TimeListViewModel ValuesTLVM
        {
            get => fValuesTLVM;
            set => SetAndNotify(ref fValuesTLVM, value, nameof(ValuesTLVM));
        }

        private TimeListViewModel fDebtTLVM;

        /// <summary>
        /// View Model for the unit price data of the Security.
        /// </summary>
        public TimeListViewModel DebtTLVM
        {
            get => fDebtTLVM;
            set => SetAndNotify(ref fDebtTLVM, value);
        }

        private TimeListViewModel fPaymentsTLVM;

        /// <summary>
        /// View Model for the payments for the Asset.
        /// </summary>
        public TimeListViewModel PaymentsTLVM
        {
            get => fPaymentsTLVM;
            set => SetAndNotify(ref fPaymentsTLVM, value);
        }

        private AccountStatistics fSecurityStats;

        /// <summary>
        /// The statistics for the security.
        /// </summary>
        public AccountStatistics SecurityStats
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
            set => SetAndNotify(ref fValues, value);
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
            fAccountType = dataType;
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
        }

        private void DeleteValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryDeleteData(fAccountType, name.ToTwoName(), value.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
            }
        }

        private void DeleteDebtValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryDeleteAssetDebt(fAccountType, name.ToTwoName(), value.Day, ReportLogger)));
            }
            else
            {
                _ = ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData,
                    "No Account was selected when trying to delete data.");
            }
        }

        private void DeletePaymentValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryDeleteAssetPayment(fAccountType, name.ToTwoName(), value.Day,
                            ReportLogger)));
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
                $"Download selected for account {SelectedName} - a {fAccountType}");
            if (SelectedName != null)
            {
                NameData names = SelectedName;
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    async programPortfolio => await PortfolioDataUpdater
                        .Download(fAccountType, programPortfolio, names, ReportLogger).ConfigureAwait(false)));
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
                $"Selected {fAccountType} {SelectedName} exporting data to csv.");
            if (SelectedName != null)
            {
                FileInteractionResult result =
                    DisplayGlobals.FileInteractionService.SaveFile("csv", string.Empty,
                        filter: "Csv Files|*.csv|All Files|*.*");
                if (result.Success)
                {
                    CsvReaderWriter.WriteToCSVFile(ModelData, result.FilePath, ReportLogger);
                }
            }
        }

        private void ExecuteAddEditValues(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditData(fAccountType, name.ToTwoName(), oldValue,
                        newValue, ReportLogger)));
            }
        }

        private void ExecuteAddEditDebt(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditAssetDebt(fAccountType, name.ToTwoName(),
                        oldValue, newValue, ReportLogger)));
            }
        }

        private void ExecuteAddEditPayment(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => _ = programPortfolio.TryAddOrEditAssetPayment(fAccountType, name.ToTwoName(),
                        oldValue, newValue, ReportLogger)));
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IAmortisableAsset modelData)
        {
            _ = ReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess,
                $"Selected {fAccountType} {SelectedName} updating data.");
            base.UpdateData(modelData);
            if (SelectedName == null)
            {
                return;
            }

            if (modelData == null)
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }

            ValuesTLVM?.UpdateData(modelData.Values);
            DebtTLVM?.UpdateData(modelData.Debt);
            SecurityStats = new AccountStatistics(
                _portfolio, 
                DateTime.Today, 
                fAccountType,
                SelectedName,
                AccountStatisticsHelpers.DefaultAssetStats());
            Values = modelData.ListOfValues();
        }
    }
}