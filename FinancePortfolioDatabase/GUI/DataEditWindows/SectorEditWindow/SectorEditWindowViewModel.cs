using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUIAccessorFunctions;
using GUISupport;
using SectorHelperFunctions;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class SectorEditWindowViewModel : PropertyChangedBase
    {
        private List<NameComp> fPreEditSectorNames;

        private List<NameComp> fSectorNames;
        public List<NameComp> SectorNames
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

        private NameComp fSelectedName;
        public NameComp SelectedName
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
        public AccountDayDataView SelectedDataPoint
        {
            get { return fSelectedDataPoint; }
            set { fSelectedDataPoint = value; OnPropertyChanged(); }
        }

        public ICommand CreateSectorCommand { get; set; }

        public ICommand DeleteSectorCommand { get; set; }

        public ICommand DeleteSectorDataCommand { get; }

        public ICommand EditSectorDataCommand { get; set; }

        public ICommand CloseCommand { get; }

        public void UpdateSectorListBox()
        {
            var currentSelectedName = SelectedName;
            SectorNames = DatabaseAccessor.GetSectorNames();
            SectorNames.Sort();
            fPreEditSectorNames = DatabaseAccessor.GetSectorNames();
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
                if (SectorEditor.TryGetSectorData(fSelectedName.Name, out List<AccountDayDataView> values))
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
            if (DatabaseAccessor.GetBenchMarks().Count != SectorNames.Count)
            {
                bool edited = false;
                foreach (var name in SectorNames)
                {
                    if (name.NewValue && !string.IsNullOrEmpty(name.Name))
                    {
                        edited = true;
                        SectorEditor.TryAddSector(name.Name, name.Url, reports);
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
                        SectorEditor.TryEditSectorName(fPreEditSectorNames[i].Name, name.Name, name.Url, reports);
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
            UpdateSectorListBox();
            UpdateMainWindow(true);
        }


        private void ExecuteEditSectorData(Object obj)
        {
            var reports = new ErrorReports();
            if (SelectedName != null && SelectedDataPoint != null)
            {
                if (DatabaseAccessor.GetSectorFromName(SelectedName.Name).Count() != SelectedSectorData.Count)
                {
                    SectorEditor.TryAddDataToSector(SelectedName.Name, SelectedDataPoint.Date, SelectedDataPoint.Amount);
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
                            SectorEditor.TryEditSector(SelectedName.Name, SelectedDataPoint.Date, SelectedDataPoint.Amount, reports);
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
                if (SectorEditor.TryDeleteSectorData(SelectedName.Name, SelectedDataPoint.Date, SelectedDataPoint.Amount, reports))
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
                SectorEditor.TryDeleteSector(SelectedName.Name);
            }
            else if (DatabaseAccessor.GetBenchMarks().Count != SectorNames.Count)
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

        private void ExecuteCloseCommand(Object obj)
        {
            UpdateMainWindow(true);
            windowToView("dataview");
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;
        Action<ErrorReports> UpdateReports;
        public SectorEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<ErrorReports> updateReports)
        {
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            SectorNames = new List<NameComp>();
            fPreEditSectorNames = new List<NameComp>();
            SelectedSectorData = new List<AccountDayDataView>();
            UpdateSectorListBox();

            CreateSectorCommand = new BasicCommand(ExecuteCreateSector);
            EditSectorDataCommand = new BasicCommand(ExecuteEditSectorData);
            DeleteSectorCommand = new BasicCommand(ExecuteDeleteSector);
            DeleteSectorDataCommand = new BasicCommand(ExecuteDeleteSectorData);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
