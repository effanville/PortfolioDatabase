using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using PADGlobals;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class CurrencyEditWindowViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;
        private List<NameData> fPreEditSectorNames;

        private List<NameData> fSectorNames;
        public List<NameData> SectorNames
        {
            get
            {
                return fSectorNames;
            }
            set
            {
                fSectorNames = value;
                OnPropertyChanged();
            }
        }

        private NameData fSelectedName;
        public NameData SelectedName
        {
            get
            {
                return fSelectedName;
            }
            set
            {
                fSelectedName = value;
                OnPropertyChanged();
                UpdateSelectedSectorListBox();
            }
        }

        private List<AccountDayDataView> fSelectedSectorData;
        public List<AccountDayDataView> SelectedSectorData
        {
            get
            {
                return fSelectedSectorData;
            }
            set
            {
                fSelectedSectorData = value;
                OnPropertyChanged();
            }
        }

        private AccountDayDataView fSelectedDataPoint;
        private AccountDayDataView fOldSelectedData;
        private int selectedIndex;
        public AccountDayDataView SelectedDataPoint
        {
            get
            {
                return fSelectedDataPoint;
            }
            set
            {
                fSelectedDataPoint = value;
                int index = SelectedSectorData.IndexOf(value);
                if (selectedIndex != index)
                {
                    selectedIndex = index;
                    fOldSelectedData = fSelectedDataPoint?.Copy();
                }
                OnPropertyChanged();
            }
        }

        public ICommand CreateSectorCommand { get; set; }

        public ICommand DeleteSectorCommand { get; set; }

        public ICommand DeleteSectorDataCommand { get; }

        public ICommand EditSectorDataCommand { get; set; }

        public ICommand DownloadCommand { get; }

        private async void ExecuteDownloadCommand(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null)
            {
                await DataUpdater.DownloadCurrency(Portfolio, fSelectedName.Name, UpdateReports, reports).ConfigureAwait(false);
            }
            UpdateMainWindow(true);
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        public void UpdateSectorListBox()
        {
            var currentSelectedName = SelectedName;
            SectorNames = Portfolio.GetCurrencyNames();
            SectorNames.Sort();
            fPreEditSectorNames = Portfolio.GetCurrencyNames();
            fPreEditSectorNames.Sort();

            for (int i = 0; i < SectorNames.Count; i++)
            {
                if (SectorNames[i].CompareTo(currentSelectedName) == 0)
                {
                    SelectedName = SectorNames[i];
                }
            }
        }

        private void UpdateSelectedSectorListBox()
        {
            if (fSelectedName != null)
            {
                if (Portfolio.TryGetCurrencyData(fSelectedName.Name, out List<AccountDayDataView> values))
                {
                    SelectedSectorData = values;
                }

                SelectLatestValue();
            }
        }

        private void SelectLatestValue()
        {
            if (SelectedSectorData != null && SelectedSectorData.Count > 0)
            {
                SelectedDataPoint = SelectedSectorData[SelectedSectorData.Count - 1];
            }
        }

        private void ExecuteCreateSector(Object obj)
        {
            var reports = new ErrorReports();
            if (Portfolio.GetCurrencies().Count != SectorNames.Count)
            {
                bool edited = false;
                foreach (var name in SectorNames)
                {
                    if (name.NewValue && !string.IsNullOrEmpty(name.Name))
                    {
                        edited = true;
                        Portfolio.TryAddCurrency(name.Name, name.Url, reports);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                {
                    reports.AddError("No Name provided to create a sector.");
                }
            }
            else
            {
                // maybe fired from editing stuff. Try that
                bool edited = false;
                for (int i = 0; i < SectorNames.Count; i++)
                {
                    var name = SectorNames[i];

                    if (name.NewValue && !string.IsNullOrEmpty(name.Name))
                    {
                        edited = true;
                        Portfolio.TryEditCurrencyName(fPreEditSectorNames[i].Name, name.Name, name.Url, reports);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                {
                    reports.AddError("Was not able to edit desired sector.");
                }
            }
            if (reports.Any())
            {
                UpdateReports(reports);
            }
            // UpdateSectorListBox();
            UpdateMainWindow(true);
        }


        private void ExecuteEditSectorData(Object obj)
        {
            var reports = new ErrorReports();
            if (SelectedName != null && SelectedDataPoint != null)
            {
                if (Portfolio.GetCurrencyFromName(SelectedName.Name).Count() != SelectedSectorData.Count)
                {
                    Portfolio.TryAddDataToCurrency(SelectedName.Name, SelectedDataPoint.Date, SelectedDataPoint.Amount);
                    SelectedDataPoint.NewValue = false;
                }
                else
                {
                    bool edited = false;
                    for (int i = 0; i < SelectedSectorData.Count; i++)
                    {
                        var name = SelectedSectorData[i];

                        if (name.NewValue)
                        {
                            edited = true;
                            Portfolio.TryEditCurrency(SelectedName.Name, fOldSelectedData.Date, SelectedDataPoint.Date, SelectedDataPoint.Amount, reports);
                            name.NewValue = false;
                        }
                    }
                    if (!edited)
                    {
                        reports.AddError("Was not able to edit sector data.");
                    }
                }
            }

            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateSelectedSectorListBox();
        }

        private void ExecuteDeleteSectorData(Object obj)
        {
            var reports = new ErrorReports();
            if (SelectedName != null && SelectedDataPoint != null)
            {
                if (Portfolio.TryDeleteCurrencyData(SelectedName.Name, SelectedDataPoint.Date, SelectedDataPoint.Amount, reports))
                {
                    UpdateSectorListBox();
                }
            }

            if (reports.Any())
            {
                UpdateReports(reports);
            }

            UpdateMainWindow(true);
        }

        private void ExecuteDeleteSector(Object obj)
        {
            var reports = new ErrorReports();
            if (SelectedName != null)
            {
                Portfolio.TryDeleteCurrency(SelectedName.Name);
            }
            else if (Portfolio.GetCurrencies().Count != SectorNames.Count)
            {

            }
            else
            {
                reports.AddError("Something went wrong when trying to delete sector");
            }

            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateSectorListBox();
            UpdateMainWindow(true);
        }


        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;
        public CurrencyEditWindowViewModel(Portfolio portfolio, List<Sector> sectors, Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            SectorNames = new List<NameData>();
            fPreEditSectorNames = new List<NameData>();
            SelectedSectorData = new List<AccountDayDataView>();
            UpdateSectorListBox();

            CreateSectorCommand = new BasicCommand(ExecuteCreateSector);
            EditSectorDataCommand = new BasicCommand(ExecuteEditSectorData);
            DeleteSectorCommand = new BasicCommand(ExecuteDeleteSector);
            DeleteSectorDataCommand = new BasicCommand(ExecuteDeleteSectorData);
            DownloadCommand = new BasicCommand(ExecuteDownloadCommand);
        }
    }
}
