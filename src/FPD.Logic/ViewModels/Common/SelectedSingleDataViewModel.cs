﻿using System;
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
    public class SelectedSingleDataViewModel : StyledClosableViewModelBase<IValueList>
    {
        private readonly IAccountStatisticsProvider _statisticsProvider;
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
            IAccountStatisticsProvider statisticsProvider,
            IValueList valueList,
            IUiStyles styles,
            UiGlobals globals,
            TwoName selectedName,
            Account accountDataType,
            IUpdater dataUpdater)
            : base(selectedName != null ? selectedName.ToString() : "No-Name", valueList, globals, styles,
                closable: true)
        {
            _statisticsProvider = statisticsProvider;
            SelectedName = selectedName;
            _dataType = accountDataType;
            UpdateRequest += (obj, args) => dataUpdater.PerformUpdate(ModelData, args);
            string currencySymbol = CurrencyCultureHelpers.CurrencySymbol(valueList.Names.Currency);
            TLVM = new TimeListViewModel(
                valueList.Values,
                $"Value({currencySymbol})",
                Styles,
                DeleteValue,
                ExecuteAddEditData);
            Stats = new AccountStatsViewModel(null, Styles);
            DeleteValuationCommand = new RelayCommand(ExecuteDeleteValuation);
            AddCsvData = new RelayCommand(ExecuteAddCsvData);
            ExportCsvData = new RelayCommand(ExecuteExportCsvData);
        }

        /// <inheritdoc/>
        public override void UpdateData(IValueList modelData, bool force)
        {
            base.UpdateData(modelData, force);
            if (SelectedName == null || modelData == null)
            {
                OnRequestClose(EventArgs.Empty);
                return;
            }

            TLVM.UpdateData(ModelData.Values, force);
            var stats = _statisticsProvider.GetStats(
                modelData,
                DateTime.Today,
                AccountStatisticsHelpers.AllStatistics());
            Stats.UpdateData(stats, force);
        }

        private void ExecuteAddEditData(DailyValuation oldValue, DailyValuation newValue)
            => OnUpdateRequest(new UpdateRequestArgs<IValueList>(true,
                valueList => valueList.TryEditData(oldValue.Day, newValue.Day, newValue.Value, ReportLogger)));

        /// <summary>
        /// Command to delete data from the value list.
        /// </summary>
        public ICommand DeleteValuationCommand { get; }

        private void ExecuteDeleteValuation() => DeleteValue(TLVM.SelectedValuation);

        private void DeleteValue(DailyValuation value)
        {
            if (value != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IValueList>(true,
                    valueList => valueList.TryDeleteData(value.Day, ReportLogger)));
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
        public ICommand AddCsvData { get; }

        private async void ExecuteAddCsvData()
        {
            if (_selectedName == null)
            {
                return;
            }

            FileInteractionResult result =
                await DisplayGlobals.FileInteractionService.OpenFile("csv", filter: "Csv Files|*.csv|All Files|*.*");
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
                    OnUpdateRequest(new UpdateRequestArgs<IValueList>(true,
                        valueList => valueList.TryEditData(view.Day, view.Day, view.Value, ReportLogger)));
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
        public ICommand ExportCsvData { get; }

        private async void ExecuteExportCsvData()
        {
            if (_selectedName == null)
            {
                return;
            }

            FileInteractionResult result = await DisplayGlobals.FileInteractionService.SaveFile(
                "csv",
                string.Empty,
                filter: "Csv Files|*.csv|All Files|*.*");
            if (!result.Success)
            {
                return;
            }

            CsvReaderWriter.WriteToCSVFile(ModelData, result.FilePath, ReportLogger);
        }
    }
}