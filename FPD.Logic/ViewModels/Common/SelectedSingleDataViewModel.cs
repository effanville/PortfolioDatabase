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
using FPD.Logic.TemplatesAndStyles;
using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Extensions.Statistics;
using FinancialStructures.Database.Statistics;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures;
using FPD.Logic.ViewModels.Stats;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// View model to display a list with one value.
    /// </summary>
    public class SelectedSingleDataViewModel : TabViewModelBase<IPortfolio>
    {
        private readonly UiGlobals fUiGlobals;
        private readonly Account TypeOfAccount;
        private readonly Action<Action<IPortfolio>> UpdateDataCallback;
        private readonly IReportLogger fReportLogger;

        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => SetAndNotify(ref fStyles, value, nameof(Styles));
        }

        /// <inheritdoc/>
        public override bool Closable => true;

        private NameData fSelectedName;

        /// <summary>
        /// Name and Company data of the selected security in the list
        /// </summary>
        public NameData SelectedName
        {
            get => fSelectedName;
            set => SetAndNotify(ref fSelectedName, value, nameof(SelectedName));

        }

        private TimeListViewModel fTLVM;

        /// <summary>
        /// View Model for the values.
        /// </summary>
        public TimeListViewModel TLVM
        {
            get => fTLVM;
            set => SetAndNotify(ref fTLVM, value, nameof(TLVM));
        }

        private AccountStatsViewModel fStats;

        /// <summary>
        /// The statistics for the account.
        /// </summary>
        public AccountStatsViewModel Stats
        {
            get => fStats;
            set => SetAndNotify(ref fStats, value, nameof(Stats));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectedSingleDataViewModel(
            IPortfolio portfolio,
            Action<Action<IPortfolio>> updateDataCallback,
            UiStyles styles,
            UiGlobals globals,
            NameData selectedName,
            Account accountType)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", portfolio)
        {
            fUiGlobals = globals;
            Styles = styles;
            fReportLogger = fUiGlobals.ReportLogger;
            UpdateDataCallback = updateDataCallback;
            SelectedName = selectedName;
            TypeOfAccount = accountType;
            if (portfolio.TryGetAccount(accountType, SelectedName, out IValueList valueList))
            {
                string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(valueList.Names.Currency ?? portfolio.BaseCurrency);
                TLVM = new TimeListViewModel(valueList.Values, $"Value({currencySymbol})", Styles, value => DeleteValue(SelectedName, value), (old, newVal) => ExecuteAddEditData(SelectedName, old, newVal));
                var stats = portfolio.GetStats(DateTime.Today, TypeOfAccount, SelectedName, AccountStatisticsHelpers.AllStatistics()).Single();

                Stats = new AccountStatsViewModel(stats, Styles);
            }
            else
            {
                string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(portfolio.BaseCurrency);
                TLVM = new TimeListViewModel(null, $"Value({currencySymbol})", Styles, value => DeleteValue(SelectedName, value), (old, newVal) => ExecuteAddEditData(SelectedName, old, newVal));
                Stats = new AccountStatsViewModel(null, Styles);
            }

            UpdateData(portfolio);

            DeleteValuationCommand = new RelayCommand(ExecuteDeleteValuation);
            AddCsvData = new RelayCommand(ExecuteAddCsvData);
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);

        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay, Action<object> removeTab)
        {
            base.UpdateData(dataToDisplay);
            if (SelectedName != null)
            {
                if (!dataToDisplay.Exists(TypeOfAccount, SelectedName))
                {
                    removeTab?.Invoke(this);
                    return;
                }

                _ = dataToDisplay.TryGetAccount(TypeOfAccount, SelectedName, out IValueList desired);
                TLVM?.UpdateData(desired.Values);
                var stats = dataToDisplay.GetStats(DateTime.Today, TypeOfAccount, SelectedName, AccountStatisticsHelpers.AllStatistics()).Single();
                Stats.UpdateData(stats);
            }
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio dataToDisplay)
        {
            UpdateData(dataToDisplay, null);
        }

        private void ExecuteAddEditData(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (name != null)
            {
                UpdateDataCallback(programPortfolio => _ = programPortfolio.TryAddOrEditData(TypeOfAccount, name, oldValue, newValue, fReportLogger));
            }
        }

        /// <summary>
        /// Command to delete data from the value list.
        /// </summary>
        public ICommand DeleteValuationCommand
        {
            get;
        }

        private void ExecuteDeleteValuation()
        {
            DeleteValue(SelectedName, TLVM.SelectedValuation);
        }

        private void DeleteValue(NameData name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                UpdateDataCallback(programPortfolio => programPortfolio.TryDeleteData(TypeOfAccount, name, value.Day, fReportLogger));
            }
            else
            {
                _ = fReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, "No Account was selected when trying to delete data.");
            }
        }

        /// <summary>
        /// Command to add data from a csv file.
        /// </summary>
        public ICommand AddCsvData
        {
            get;
        }

        private void ExecuteAddCsvData()
        {
            if (fSelectedName != null)
            {
                FileInteractionResult result = fUiGlobals.FileInteractionService.OpenFile("csv", filter: "Csv Files|*.csv|All Files|*.*");
                List<object> outputs = null;
                bool exists = DataStore.TryGetAccount(TypeOfAccount, fSelectedName, out IValueList account);
                if (result.Success && exists)
                {
                    outputs = CsvReaderWriter.ReadFromCsv(account, result.FilePath, fReportLogger);
                }
                if (outputs != null)
                {
                    foreach (object objec in outputs)
                    {
                        if (objec is DailyValuation view)
                        {
                            UpdateDataCallback(programPortfolio => programPortfolio.TryAddOrEditData(TypeOfAccount, SelectedName, view, view, fReportLogger));
                        }
                        else
                        {
                            fReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(), "Have the wrong type of thing");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Command to export data to a csv file.
        /// </summary>
        public ICommand ExportCsvData
        {
            get;
        }

        private void ExecuteExportCsvData()
        {
            if (fSelectedName != null)
            {
                FileInteractionResult result = fUiGlobals.FileInteractionService.SaveFile("csv", string.Empty, filter: "Csv Files|*.csv|All Files|*.*");
                if (result.Success)
                {
                    if (DataStore.TryGetAccount(TypeOfAccount, fSelectedName, out IValueList account))
                    {
                        CsvReaderWriter.WriteToCSVFile(account, result.FilePath, fReportLogger);
                    }
                    else
                    {
                        _ = fReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Saving, $"Could not find {TypeOfAccount}.");
                    }
                }
            }
        }
    }
}
