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

using FPD.Logic.TemplatesAndStyles;

using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Extensions.Statistics;
using FinancialStructures.Database.Statistics;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;
using FinancialStructures;

using FPD.Logic.ViewModels.Stats;

using Common.Structure.DataEdit;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// View model to display a list with one value.
    /// </summary>
    public class SelectedSingleDataViewModel : StyledClosableViewModelBase<IValueList, IPortfolio>
    {
        private IPortfolio _portfolio;
        private readonly Account TypeOfAccount;

        private NameData _selectedName;

        /// <summary>
        /// Name and Company data of the selected security in the list
        /// </summary>
        public NameData SelectedName
        {
            get => _selectedName;
            set => SetAndNotify(ref _selectedName, value);
        }

        private TimeListViewModel _TLVM;

        /// <summary>
        /// View Model for the values.
        /// </summary>
        public TimeListViewModel TLVM
        {
            get => _TLVM;
            set => SetAndNotify(ref _TLVM, value);
        }

        private AccountStatsViewModel _stats;

        /// <summary>
        /// The statistics for the account.
        /// </summary>
        public AccountStatsViewModel Stats
        {
            get => _stats;
            set => SetAndNotify(ref _stats, value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectedSingleDataViewModel(
            IPortfolio portfolio,
            IValueList valueList,
            UiStyles styles,
            UiGlobals globals,
            NameData selectedName,
            Account accountType,
            IUpdater<IPortfolio> dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", valueList, globals, styles,
                closable: true)
        {
            _portfolio = portfolio;
            SelectedName = selectedName;
            TypeOfAccount = accountType;
            UpdateRequest += dataUpdater.PerformUpdate;
            string currencySymbol =
                CurrencyCultureHelpers.CurrencySymbol(valueList.Names.Currency ?? portfolio.BaseCurrency);
            TLVM = new TimeListViewModel(valueList.Values, $"Value({currencySymbol})", Styles,
                value => DeleteValue(SelectedName, value),
                (old, newVal) => ExecuteAddEditData(SelectedName, old, newVal));
            TLVM.UpdateRequest += dataUpdater.PerformUpdate;
            
            var stats = new AccountStatistics(
                _portfolio,
                DateTime.Today, 
                TypeOfAccount, SelectedName,
                AccountStatisticsHelpers.AllStatistics());
            Stats = new AccountStatsViewModel(stats, Styles);
            DeleteValuationCommand = new RelayCommand(ExecuteDeleteValuation);
            AddCsvData = new RelayCommand(ExecuteAddCsvData);
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
        }

        /// <inheritdoc/>
        public override void UpdateData(IValueList modelData)
        {
            base.UpdateData(modelData);
            if (SelectedName == null || modelData == null)
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }

            TLVM?.UpdateData(ModelData.Values);
            var stats = new AccountStatistics(
                _portfolio,
                DateTime.Today, 
                TypeOfAccount, SelectedName,
                AccountStatisticsHelpers.AllStatistics());
            Stats.UpdateData(stats);
        }

        private void ExecuteAddEditData(NameData name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (name != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        _ = programPortfolio.TryAddOrEditData(TypeOfAccount, name, oldValue, newValue, ReportLogger)));
            }
        }

        /// <summary>
        /// Command to delete data from the value list.
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
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => programPortfolio.TryDeleteData(TypeOfAccount, name, value.Day, ReportLogger)));
            }
            else
            {
                ReportLogger.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData.ToString(),
                    "No Account was selected when trying to delete data.");
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
            if (_selectedName != null)
            {
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
                    if (obj is DailyValuation view)
                    {
                        OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                            programPortfolio =>
                                programPortfolio.TryAddOrEditData(TypeOfAccount, SelectedName, view, view,
                                    ReportLogger)));
                    }
                    else
                    {
                        ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(),
                            "Have the wrong type of thing");
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
            if (_selectedName == null)
            {
                return;
            }

            FileInteractionResult result =
                DisplayGlobals.FileInteractionService.SaveFile("csv", string.Empty,
                    filter: "Csv Files|*.csv|All Files|*.*");
            if (!result.Success)
            {
                return;
            }

            CsvReaderWriter.WriteToCSVFile(ModelData, result.FilePath, ReportLogger);
        }
    }
}