﻿using System;
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
using FinancialStructures.Database;
using FinancialStructures.Database.Download;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Extensions.Statistics;
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
    public sealed class SelectedAssetViewModel : TabViewModelBase<IPortfolio>
    {
        /// <summary>
        /// The name data of the security this window details.
        /// </summary>
        internal readonly NameData fSelectedName;
        private readonly Account fAccountType = Account.Asset;
        private readonly Action<Action<IPortfolio>> UpdateDataCallback;
        private readonly IReportLogger fReportLogger;
        private readonly UiGlobals fUiGlobals;

        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

        internal SecurityTrade fOldSelectedTrade;
        internal SecurityTrade SelectedTrade;

        /// <inheritdoc/>
        public override bool Closable => true;

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
            set => SetAndNotify(ref fDebtTLVM, value, nameof(DebtTLVM));
        }

        private TimeListViewModel fPaymentsTLVM;

        /// <summary>
        /// View Model for the payments for the Asset.
        /// </summary>
        public TimeListViewModel PaymentsTLVM
        {
            get => fPaymentsTLVM;
            set => SetAndNotify(ref fPaymentsTLVM, value, nameof(PaymentsTLVM));
        }

        private AccountStatistics fSecurityStats;

        /// <summary>
        /// The statistics for the security.
        /// </summary>
        public AccountStatistics SecurityStats
        {
            get => fSecurityStats;
            set => SetAndNotify(ref fSecurityStats, value, nameof(SecurityStats));
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
        public SelectedAssetViewModel(IPortfolio portfolio, Action<Action<IPortfolio>> updateData, IReportLogger reportLogger, UiStyles styles, UiGlobals globals, NameData selectedName)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", portfolio)
        {
            fReportLogger = reportLogger;
            Styles = styles;
            fUiGlobals = globals;
            fSelectedName = selectedName;
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            UpdateDataCallback = updateData;

            if (portfolio.TryGetAccount(fAccountType, fSelectedName, out IValueList desired))
            {
                IAmortisableAsset security = desired as IAmortisableAsset;
                ValuesTLVM = new TimeListViewModel(security.Values, "Values", value => DeleteValue(fSelectedName, value), (old, newVal) => ExecuteAddEditValues(fSelectedName, old, newVal));
                DebtTLVM = new TimeListViewModel(security.Debt, "Debt", value => DeleteDebtValue(fSelectedName, value), (old, newVal) => ExecuteAddEditDebt(fSelectedName, old, newVal));
                PaymentsTLVM = new TimeListViewModel(security.Payments, "Payments", value => DeletePaymentValue(fSelectedName, value), (old, newVal) => ExecuteAddEditPayment(fSelectedName, old, newVal));
            }
            else
            {
                ValuesTLVM = new TimeListViewModel(null, "Values", value => DeleteValue(fSelectedName, value), (old, newVal) => ExecuteAddEditValues(fSelectedName, old, newVal));
                DebtTLVM = new TimeListViewModel(null, "Debt", value => DeleteDebtValue(fSelectedName, value), (old, newVal) => ExecuteAddEditDebt(fSelectedName, old, newVal));
                PaymentsTLVM = new TimeListViewModel(null, "Payments", value => DeletePaymentValue(fSelectedName, value), (old, newVal) => ExecuteAddEditPayment(fSelectedName, old, newVal));
            }

            UpdateData(portfolio, null);
        }

        private void DeleteValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                UpdateDataCallback(programPortfolio => programPortfolio.TryDeleteData(fAccountType, name.ToTwoName(), value.Day, fReportLogger));
            }
            else
            {
                _ = fReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "No Account was selected when trying to delete data.");
            }
        }

        private void DeleteDebtValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                UpdateDataCallback(programPortfolio => programPortfolio.TryDeleteAssetDebt(fAccountType, name.ToTwoName(), value.Day, fReportLogger));
            }
            else
            {
                _ = fReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "No Account was selected when trying to delete data.");
            }
        }

        private void DeletePaymentValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                UpdateDataCallback(programPortfolio => programPortfolio.TryDeleteAssetPayment(fAccountType, name.ToTwoName(), value.Day, fReportLogger));
            }
            else
            {
                _ = fReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "No Account was selected when trying to delete data.");
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
            _ = fReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Download selected for account {fSelectedName} - a {fAccountType}");
            if (fSelectedName != null)
            {
                NameData names = fSelectedName;
                UpdateDataCallback(async programPortfolio => await PortfolioDataUpdater.Download(fAccountType, programPortfolio, names, fReportLogger).ConfigureAwait(false));
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
            _ = fReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccountType} {fSelectedName} exporting data to csv.");
            if (fSelectedName != null)
            {
                FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile("csv", string.Empty, DataStore.Directory(fUiGlobals.CurrentFileSystem), "Csv Files|*.csv|All Files|*.*");
                if (result.Success)
                {
                    if (DataStore.TryGetAccount(fAccountType, fSelectedName, out IValueList account))
                    {
                        IAmortisableAsset security = account as IAmortisableAsset;
                        CsvReaderWriter.WriteToCSVFile(security, result.FilePath, fReportLogger);
                    }
                    else
                    {
                        _ = fReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Saving, $"Could not find {fAccountType}.");
                    }
                }
            }
        }

        private void ExecuteAddEditValues(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                bool edited = false;
                UpdateDataCallback(programPortfolio => edited = programPortfolio.TryAddOrEditData(fAccountType, name.ToTwoName(), oldValue, newValue, fReportLogger));
                if (!edited)
                {
                    _ = fReportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.EditingData, $"Was not able to add or edit {fAccountType} data.");
                }
            }
        }

        private void ExecuteAddEditDebt(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                bool edited = false;
                UpdateDataCallback(programPortfolio => edited = programPortfolio.TryAddOrEditAssetDebt(fAccountType, name.ToTwoName(), oldValue, newValue, fReportLogger));
                if (!edited)
                {
                    _ = fReportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.EditingData, $"Was not able to add or edit {fAccountType} data.");
                }
            }
        }

        private void ExecuteAddEditPayment(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                bool edited = false;
                UpdateDataCallback(programPortfolio => edited = programPortfolio.TryAddOrEditAssetPayment(fAccountType, name.ToTwoName(), oldValue, newValue, fReportLogger));
                if (!edited)
                {
                    _ = fReportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.EditingData, $"Was not able to add or edit {fAccountType} data.");
                }
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay, Action<object> removeTab)
        {
            _ = fReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccountType} {fSelectedName} updating data.");
            base.UpdateData(dataToDisplay);
            if (fSelectedName != null)
            {
                if (!dataToDisplay.TryGetAccount(fAccountType, fSelectedName, out IValueList desired))
                {
                    removeTab?.Invoke(this);
                    return;
                }

                IAmortisableAsset security = desired as IAmortisableAsset;
                ValuesTLVM?.UpdateData(security.Values);
                DebtTLVM?.UpdateData(security.Debt);
                SecurityStats = dataToDisplay.GetStats(DateTime.Today, fAccountType, fSelectedName, AccountStatisticsHelpers.DefaultAssetStats()).Single();
                Values = dataToDisplay.NumberData(fAccountType, fSelectedName, fReportLogger).ToList();
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay)
        {
            UpdateData(dataToDisplay, null);
        }
    }
}
