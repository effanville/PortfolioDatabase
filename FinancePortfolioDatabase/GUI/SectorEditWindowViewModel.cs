using System;
using System.Collections.Generic;
using System.Windows.Input;
using GUIAccessorFunctions;
using SectorHelperFunctions;
using GUISupport;
using GUIFinanceStructures;

namespace FinanceWindowsViewModels
{
    public class SectorEditWindowViewModel : PropertyChangedBase
    {
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

        public ICommand EditSectorNameCommand { get; }

        public ICommand AddValuationCommand { get; }

        public ICommand EditSectorCommand { get; }

        public ICommand DeleteSectorCommand { get; }

        public ICommand DeleteDataCommand { get; }

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
        }

        private void ExecuteAddSector(Object obj)
        {
            SectorEditor.TryAddSector(selectedNameEdit);
            UpdateSectorListBox();
            ClearSelection();

        }

        private void ExecuteEditSectorNameCommand(Object obj)
        {
            if (fSelectedNameEdit != null)
            {
                SectorEditor.TryEditSectorName(selectedName, selectedNameEdit);
            }
           
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
        }

        private void ExecuteEditSector(Object obj)
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
        }

        private void ExecuteDeleteSector(Object obj)
        {
            SectorEditor.TryDeleteSector(selectedName);
            UpdateSectorListBox();
        }

        private void ExecuteCloseCommand(Object obj)
        {
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
            EditSectorNameCommand = new BasicCommand(ExecuteEditSectorNameCommand);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);

            EditSectorCommand = new BasicCommand(ExecuteEditSector);
            DeleteDataCommand = new BasicCommand(ExecuteDeleteSectorData);
            DeleteSectorCommand = new BasicCommand(ExecuteDeleteSector);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
