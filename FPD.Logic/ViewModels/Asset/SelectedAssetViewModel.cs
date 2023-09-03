using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Common.Structure.DataEdit;
using Common.Structure.DataStructures;
using Common.Structure.FileAccess;
using Common.Structure.Reporting;
using Common.UI;
using Common.UI.Commands;
using Common.UI.Services;
using Common.UI.ViewModelBases;

using FinancialStructures;
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
        internal readonly NameData SelectedName;
        private readonly Account fAccountType;
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
        public SelectedAssetViewModel(
            IPortfolio portfolio,
            UiStyles styles, 
            UiGlobals globals, 
            NameData selectedName,
            Account dataType,
            IUpdater<IPortfolio> dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", portfolio)
        {
            fReportLogger = globals.ReportLogger;
            Styles = styles;
            fUiGlobals = globals;
            SelectedName = selectedName;
            fAccountType = Account.Asset;
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            UpdateRequest += dataUpdater.PerformUpdate;
            if (portfolio.TryGetAccount(fAccountType, SelectedName, out IValueList desired))
            {
                IAmortisableAsset asset = desired as IAmortisableAsset;
                string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(asset.Names.Currency ?? portfolio.BaseCurrency);
                ValuesTLVM = new TimeListViewModel(asset.Values, $"Values({currencySymbol})", Styles, value => DeleteValue(SelectedName, value), (old, newVal) => ExecuteAddEditValues(SelectedName, old, newVal));
                DebtTLVM = new TimeListViewModel(asset.Debt, $"Debt({currencySymbol})", Styles, value => DeleteDebtValue(SelectedName, value), (old, newVal) => ExecuteAddEditDebt(SelectedName, old, newVal));
                PaymentsTLVM = new TimeListViewModel(asset.Payments, $"Payments({currencySymbol})", Styles, value => DeletePaymentValue(SelectedName, value), (old, newVal) => ExecuteAddEditPayment(SelectedName, old, newVal));
                ValuesTLVM.UpdateRequest += dataUpdater.PerformUpdate;
                DebtTLVM.UpdateRequest += dataUpdater.PerformUpdate;
                PaymentsTLVM.UpdateRequest += dataUpdater.PerformUpdate;
            }
            else
            {
                string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(portfolio.BaseCurrency);
                ValuesTLVM = new TimeListViewModel(null, $"Values({currencySymbol})", Styles, value => DeleteValue(SelectedName, value), (old, newVal) => ExecuteAddEditValues(SelectedName, old, newVal));
                DebtTLVM = new TimeListViewModel(null, $"Debt({currencySymbol})", Styles, value => DeleteDebtValue(SelectedName, value), (old, newVal) => ExecuteAddEditDebt(SelectedName, old, newVal));
                PaymentsTLVM = new TimeListViewModel(null, $"Payments({currencySymbol})", Styles, value => DeletePaymentValue(SelectedName, value), (old, newVal) => ExecuteAddEditPayment(SelectedName, old, newVal));
                ValuesTLVM.UpdateRequest += dataUpdater.PerformUpdate;
                DebtTLVM.UpdateRequest += dataUpdater.PerformUpdate;
                PaymentsTLVM.UpdateRequest += dataUpdater.PerformUpdate;
            }

            UpdateData(portfolio, null);
        }

        private void DeleteValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryDeleteData(fAccountType, name.ToTwoName(), value.Day, fReportLogger)));
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
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryDeleteAssetDebt(fAccountType, name.ToTwoName(), value.Day, fReportLogger)));
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
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => programPortfolio.TryDeleteAssetPayment(fAccountType, name.ToTwoName(), value.Day, fReportLogger)));
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
            _ = fReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Download selected for account {SelectedName} - a {fAccountType}");
            if (SelectedName != null)
            {
                NameData names = SelectedName;
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, async programPortfolio => await PortfolioDataUpdater.Download(fAccountType, programPortfolio, names, fReportLogger).ConfigureAwait(false)));
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
            _ = fReportLogger.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccountType} {SelectedName} exporting data to csv.");
            if (SelectedName != null)
            {
                FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile("csv", string.Empty, filter: "Csv Files|*.csv|All Files|*.*");
                if (result.Success)
                {
                    if (ModelData.TryGetAccount(fAccountType, SelectedName, out IValueList account))
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
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => _ = programPortfolio.TryAddOrEditData(fAccountType, name.ToTwoName(), oldValue, newValue, fReportLogger)));
            }
        }

        private void ExecuteAddEditDebt(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => _ = programPortfolio.TryAddOrEditAssetDebt(fAccountType, name.ToTwoName(), oldValue, newValue, fReportLogger)));
            }
        }

        private void ExecuteAddEditPayment(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (newValue != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, programPortfolio => _ = programPortfolio.TryAddOrEditAssetPayment(fAccountType, name.ToTwoName(), oldValue, newValue, fReportLogger)));
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay, Action<object> removeTab)
        {
            _ = fReportLogger?.Log(ReportSeverity.Detailed, ReportType.Information, ReportLocation.DatabaseAccess, $"Selected {fAccountType} {SelectedName} updating data.");
            base.UpdateData(dataToDisplay);
            if (SelectedName != null)
            {
                if (!dataToDisplay.TryGetAccount(fAccountType, SelectedName, out IValueList desired))
                {
                    removeTab?.Invoke(this);
                    return;
                }

                IAmortisableAsset asset = desired as IAmortisableAsset;
                ValuesTLVM?.UpdateData(asset.Values);
                DebtTLVM?.UpdateData(asset.Debt);
                SecurityStats = dataToDisplay.GetStats(DateTime.Today, fAccountType, SelectedName, AccountStatisticsHelpers.DefaultAssetStats()).Single();
                Values = dataToDisplay.NumberData(fAccountType, SelectedName, fReportLogger).ToList();
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay)
        {
            UpdateData(dataToDisplay, null);
        }
    }
}
