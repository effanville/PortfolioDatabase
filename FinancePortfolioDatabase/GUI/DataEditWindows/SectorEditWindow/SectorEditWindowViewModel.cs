﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using GUIAccessorFunctions;
using SectorHelperFunctions;
using GUISupport;
using GUIFinanceStructures;
using ReportingStructures;

namespace FinanceWindowsViewModels
{
    public class SectorEditWindowViewModel : PropertyChangedBase
    {
        private bool fDataAddEditVisibility;
        public bool DataAddEditVisibility
        {
            get { return fDataAddEditVisibility; }
            set { fDataAddEditVisibility = value; OnPropertyChanged(); }
        }

        private bool fNameAddEditVisibility;
        public bool NameAddEditVisibility
        {
            get { return fNameAddEditVisibility; }
            set { fNameAddEditVisibility = value; OnPropertyChanged(); }
        }

        private bool fEditing;

        public bool Editing
        {
            get { return fEditing; }
            set { fEditing = value; OnPropertyChanged(); }
        }
        public bool NotEditing
        {
            get { return !fEditing; }
            set { fEditing = !value; OnPropertyChanged(); }
        }

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
        public NameComp selectedName
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

        private AccountDayDataView fSelectedSector;
        public AccountDayDataView selectedSector
        {
            get { return fSelectedSector; }
            set { fSelectedSector = value; OnPropertyChanged(); }
        }

        private string fSelectedNameEdit;
        public string selectedNameEdit
        {
            get { return fSelectedNameEdit; }
            set { fSelectedNameEdit = value; OnPropertyChanged(); }
        }

        private string fDateEdit;

        public string DateEdit
        {
            get { return fDateEdit; }
            set { fDateEdit = value; OnPropertyChanged(); }
        }

        private string fValuesEdit;

        public string ValuesEdit
        {
            get { return fValuesEdit; }
            set { fValuesEdit = value; OnPropertyChanged(); }
        }

        public ICommand AddSectorCommand { get; }

        public ICommand CreateSectorCommand { get; set; }

        public ICommand CreateSectorButtonCommand { get; }

        public ICommand EditSectorNameCommand { get; }

        public ICommand AddValuationCommand { get; }

        public ICommand AddSectorDataCommand { get; }

        public ICommand EditSectorCommand { get; }

        public ICommand DeleteSectorCommand { get; set; }

        public ICommand DeleteDataCommand { get; }

        public ICommand EditSectorDataButtonCommand { get; }

        public ICommand EditSectorDataCommand { get; set; }

        public ICommand CloseCommand { get; }

        public void UpdateSectorListBox()
        {
            SectorNames = DatabaseAccessor.GetSectorNames();
            fPreEditSectorNames= DatabaseAccessor.GetSectorNames();
        }

        private void UpdateSelectedSectorListBox()
        {
            if (fSelectedName != null)
            {
                selectedNameEdit = fSelectedName.Name;

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
                selectedSector = SelectedSectorData[SelectedSectorData.Count - 1];
            }
        }

        private void ClearSelection()
        {
            SelectedSectorData = null;
            selectedNameEdit = null;
            DateEdit = null;
            ValuesEdit = null;
            UpdateMainWindow(true);
        }

        private void ExecuteAddSector(Object obj)
        {
            NotEditing = true;
            NameAddEditVisibility = true;
            DataAddEditVisibility = false;
        }

        private void ShowAddValues(Object obj)
        {
            NotEditing = true;
            NameAddEditVisibility = true;
            DataAddEditVisibility = true;
        }
        private void ExecuteCreateSectorButton(Object obj)
        {
            if (selectedNameEdit != null)
            {
                SectorEditor.TryAddSector(selectedNameEdit);
            }
        }
        private void ExecuteCreateSector(Object obj)
        {
            if (DatabaseAccessor.GetBenchMarks().Count != SectorNames.Count)
            {
                bool edited = false;
                foreach (var name in SectorNames)
                {
                    if (name.NewValue && !string.IsNullOrEmpty(name.Name))
                    {
                        edited = true;
                        SectorEditor.TryAddSector(name.Name);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                { 
                    ErrorReports.AddError("No Name provided to create a sector.");
                }
            }
            else
            {
                // maybe fired from editing stuff. Try that
                bool edited = false;
                for(int i = 0;i < SectorNames.Count; i++)
                {
                    var name = SectorNames[i];

                    if (name.NewValue && !string.IsNullOrEmpty(name.Name))
                    {
                        edited = true;
                        SectorEditor.TryEditSectorName(fPreEditSectorNames[i].Name, name.Name);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                {
                    ErrorReports.AddError("Was not able to edit desired sector.");
                }
            }

            UpdateSectorListBox();
            ClearSelection();
            NotEditing = true;
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteEditSectorNameCommand(Object obj)
        {
            if (fSelectedNameEdit != null)
            {
                SectorEditor.TryEditSectorName(selectedName.Name, selectedNameEdit);
            }

            NotEditing = true;
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteAddValuationCommand(Object obj)
        {
            if (selectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) &&  Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditor.TryAddDataToSector(selectedName.Name, date, value);
                    UpdateSectorListBox();

                    ClearSelection();
                }
            }

            NotEditing = true;
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }
        private void ExecuteEditSector(Object obj)
        {
            Editing = true;
            NameAddEditVisibility = true;
            DataAddEditVisibility = true;
        }

        private void ExecuteEditSectorButtonData(Object obj)
        {
            if (selectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditor.TryEditSector(selectedName.Name, date, value);
                    UpdateSectorListBox();

                    ClearSelection();
                }
            }

            UpdateMainWindow(true);
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
        }

        private void ExecuteEditSectorData(Object obj)
        {
            if (selectedName != null && selectedSector != null)
            {
                if (DatabaseAccessor.GetSectorFromName(selectedName.Name).Count() != SelectedSectorData.Count)
                {
                    SectorEditor.TryAddDataToSector(selectedName.Name, selectedSector.Date, selectedSector.Amount);
                    selectedSector.NewValue = false;
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
                            SectorEditor.TryEditSector(selectedName.Name, selectedSector.Date, selectedSector.Amount);
                            name.NewValue = false;
                        }
                    }
                    if (!edited)
                    {
                        ErrorReports.AddError("Was not able to edit sector data.");
                    }
                }
            }
            UpdateSelectedSectorListBox();
        }

        private void ExecuteDeleteSectorData(Object obj)
        {
            if (selectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditor.TryDeleteSectorData(selectedName.Name, date, value);
                    UpdateSectorListBox();

                    ClearSelection();
                }
            }

            UpdateMainWindow(true);
        }

        private void ExecuteDeleteSector(Object obj)
        {
            if (selectedName != null)
            {
                SectorEditor.TryDeleteSector(selectedName.Name);
            }
            else if (DatabaseAccessor.GetBenchMarks().Count != SectorNames.Count)
            {

            }
            else 
            {
                ErrorReports.AddError("Something went wrong when trying to delete sector");
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

        public SectorEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;

            SectorNames = new List<NameComp>();
            fPreEditSectorNames = new List<NameComp>();
            SelectedSectorData = new List<AccountDayDataView>();
            UpdateSectorListBox();

            AddSectorCommand = new BasicCommand(ExecuteAddSector);
            CreateSectorCommand = new BasicCommand(ExecuteCreateSector);
            CreateSectorButtonCommand = new BasicCommand(ExecuteCreateSectorButton);
            EditSectorNameCommand = new BasicCommand(ExecuteEditSectorNameCommand);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);
            AddSectorDataCommand = new BasicCommand(ShowAddValues);

            EditSectorCommand = new BasicCommand(ExecuteEditSector);
            EditSectorDataButtonCommand = new BasicCommand(ExecuteEditSectorButtonData);
            EditSectorDataCommand = new BasicCommand(ExecuteEditSectorData);
            DeleteDataCommand = new BasicCommand(ExecuteDeleteSectorData);
            DeleteSectorCommand = new BasicCommand(ExecuteDeleteSector);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}