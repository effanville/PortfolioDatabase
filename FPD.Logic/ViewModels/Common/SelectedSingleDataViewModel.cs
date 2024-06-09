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
using Effanville.FinancialStructures.Database.Extensions.DataEdit;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Stats;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// View model to display a list with one value.
    /// </summary>
    public class SelectedSingleDataViewModel : StyledClosableViewModelBase<IValueList, IPortfolio>
    {
        private readonly IPortfolio _portfolio;
        private readonly Account _dataType;

        private TwoName _selectedName;

        /// <summary>
        /// Name and Company data of the selected security in the list
        /// </summary>
        public TwoName SelectedName
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
            TwoName selectedName,
            Account accountDataType,
            IUpdater<IPortfolio> dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", valueList, globals, styles,
                closable: true)
        {
            _portfolio = portfolio;
            SelectedName = selectedName;
            _dataType = accountDataType;
            UpdateRequest += dataUpdater.PerformUpdate;
            string currencySymbol =
                CurrencyCultureHelpers.CurrencySymbol(valueList.Names.Currency ?? portfolio.BaseCurrency);
            TLVM = new TimeListViewModel(valueList.Values, $"Value({currencySymbol})", Styles,
                value => DeleteValue(SelectedName, value),
                (old, newVal) => ExecuteAddEditData(SelectedName, old, newVal));
            TLVM.UpdateRequest += dataUpdater.PerformUpdate;
            Stats = new AccountStatsViewModel(null, Styles);
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

            TLVM.UpdateData(ModelData.Values);
            var stats = new AccountStatistics(
                _portfolio,
                DateTime.Today, 
                modelData,
                AccountStatisticsHelpers.AllStatistics());
            Stats.UpdateData(stats);
        }

        private void ExecuteAddEditData(TwoName name, DailyValuation oldValue, DailyValuation newValue)
        {
            if (name != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        _ = programPortfolio.TryAddOrEditData(_dataType, name, oldValue, newValue, ReportLogger)));
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

        private void DeleteValue(TwoName name, DailyValuation value)
        {
            if (name != null && value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio => programPortfolio.TryDeleteData(_dataType, name, value.Day, ReportLogger)));
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
            if (_selectedName == null)
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
                if (obj is DailyValuation view)
                {
                    OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true,
                        programPortfolio =>
                            programPortfolio.TryAddOrEditData(_dataType, SelectedName, view, view,
                                ReportLogger)));
                }
                else
                {
                    ReportLogger.Log(ReportType.Error, ReportLocation.StatisticsPage.ToString(),
                        "Have the wrong type of thing");
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