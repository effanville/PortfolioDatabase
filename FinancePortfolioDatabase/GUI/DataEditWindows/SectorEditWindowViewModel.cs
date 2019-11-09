using System;
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

        private List<string> fSectorNames;
        public List<string> SectorNames
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

        private string fSelectedName;
        public string selectedName
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

        public ICommand CreateSectorCommand { get; }

        public ICommand EditSectorNameCommand { get; }

        public ICommand AddValuationCommand { get; }

        public ICommand AddSectorDataCommand { get; }

        public ICommand EditSectorCommand { get; }

        public ICommand DeleteSectorCommand { get; }

        public ICommand DeleteDataCommand { get; }

        public ICommand EditSectorDataCommand { get; }

        public ICommand CloseCommand { get; }

        private void UpdateSectorListBox()
        {
            SectorNames = DatabaseAccessor.GetSectorNames();

            ClearSelection();
        }

        private void UpdateSelectedSectorListBox()
        {
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

        private void ExecuteCreateSector(Object obj)
        {
            if (selectedNameEdit != null)
            {
                SectorEditor.TryAddSector(selectedNameEdit);
                UpdateSectorListBox();
                ClearSelection();
            }
            else 
            {
                ErrorReports.AddError("No Name provided to create a sector.");
            }

            NotEditing = true;
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);

        }

        private void ExecuteEditSectorNameCommand(Object obj)
        {
            if (fSelectedNameEdit != null)
            {
                SectorEditor.TryEditSectorName(selectedName, selectedNameEdit);
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
                    SectorEditor.TryAddDataToSector(selectedName, date, value);
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

        private void ExecuteEditSectorData(Object obj)
        {
            if (selectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditor.TryEditSector(selectedName, date, value);
                    UpdateSectorListBox();

                    ClearSelection();
                }
            }

            UpdateMainWindow(true);
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
        }

        private void ExecuteDeleteSectorData(Object obj)
        {
            if (selectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditor.TryDeleteSectorData(selectedName, date, value);
                    UpdateSectorListBox();

                    ClearSelection();
                }
            }

            UpdateMainWindow(true);
        }

        private void ExecuteDeleteSector(Object obj)
        {
            SectorEditor.TryDeleteSector(selectedName);
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

            SectorNames = new List<string>();
            SelectedSectorData = new List<AccountDayDataView>();
            UpdateSectorListBox();

            AddSectorCommand = new BasicCommand(ExecuteAddSector);
            CreateSectorCommand = new BasicCommand(ExecuteCreateSector);
            EditSectorNameCommand = new BasicCommand(ExecuteEditSectorNameCommand);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);
            AddSectorDataCommand = new BasicCommand(ShowAddValues);

            EditSectorCommand = new BasicCommand(ExecuteEditSector);
            EditSectorDataCommand = new BasicCommand(ExecuteEditSectorData);
            DeleteDataCommand = new BasicCommand(ExecuteDeleteSectorData);
            DeleteSectorCommand = new BasicCommand(ExecuteDeleteSector);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
