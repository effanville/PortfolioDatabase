﻿using FinancialStructures.Database;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using PADGlobals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace FinanceWindowsViewModels.SecurityEdit
{
    public class UserButtonsViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private NameData fSelectedName;


        private DayDataView fSelectedValues;
        public ICommand DownloadCommand { get; }

        private async void ExecuteDownloadCommand(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null)
            {
                await DataUpdater.DownloadSecurity(Portfolio, fSelectedName.Company, fSelectedName.Name, UpdateReports, reports).ConfigureAwait(false);
            }
            UpdateMainWindow(true);
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }
        public ICommand DeleteSecurityCommand { get; }

        private void ExecuteDeleteSecurity(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null)
            {
                Portfolio.TryRemoveSecurity(reports, fSelectedName.Company, fSelectedName.Name);
            }
            else
            {
                reports.AddError("Something went wrong when trying to delete security.");
            }

            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateMainWindow(true);
        }

        public void UpdateButtonViewData(NameData newName, DayDataView newData)
        {
            fSelectedName = newName;
            fSelectedValues = newData;
        }

        public ICommand DeleteValuationCommand { get; }

        private void ExecuteDeleteValuation(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null && fSelectedValues != null)
            {
                Portfolio.TryRemoveSecurityData(reports, fSelectedName.Company, fSelectedName.Name, fSelectedValues.Date, fSelectedValues.ShareNo, fSelectedValues.UnitPrice, fSelectedValues.Investment);
            }

            if (reports.Any())
            {
                UpdateReports(reports);
            }

            UpdateMainWindow(true);
        }

        public ICommand AddCsvData { get; }

        private void ExecuteAddCsvData(Object obj)
        {
            if (fSelectedName != null)
            {
                var reports = new ErrorReports();
                OpenFileDialog openFile = new OpenFileDialog() { DefaultExt = "xml" };
                openFile.Filter = "Csv Files|*.csv|All Files|*.*";
                List<object> outputs = null;
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    outputs = CsvDataRead.ReadFromCsv(openFile.FileName, ElementType.Security, reports);
                }

                foreach (var objec in outputs)
                {
                    if (objec is DayDataView view)
                    {
                        Portfolio.TryAddDataToSecurity(reports, fSelectedName.Company, fSelectedName.Name, view.Date, view.ShareNo, view.UnitPrice, view.Investment);
                    }
                    else
                    {
                        reports.AddError("Have the wrong type of thing");
                    }
                }
                if (reports.Any())
                {
                    UpdateReports(reports);
                }

                UpdateMainWindow(false);
                //UpdateSelectedSecurityListBox();
            }
        }

        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;
        public UserButtonsViewModel(Portfolio portfolio, Action<bool> updateWindow, Action<ErrorReports> updateReports, NameData newName, DayDataView newData)
        {
            Portfolio = portfolio;
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            DownloadCommand = new BasicCommand(ExecuteDownloadCommand);
            DeleteSecurityCommand = new BasicCommand(ExecuteDeleteSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            AddCsvData = new BasicCommand(ExecuteAddCsvData);
            fSelectedName = newName;
            fSelectedValues = newData;
        }
    }
}
