using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GUIAccessorFunctions;
using SectorHelperFunctions;
using GuiSupport;
using GUIFinanceStructures;

namespace FinanceWindowsViewModels
{
    public class SectorEditWindowViewModel : INotifyPropertyChanged
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
                UpdateSelectedSecurityListBox();
            }
        }

        private List<BasicDayDataView> fSelectedSectorData;
        public List<BasicDayDataView> SelectedSectorData
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

        public ICommand AddValuationCommand { get; }

        public ICommand EditSectorCommand { get; }

        public ICommand DeleteSectorCommand { get; }

        private void UpdateFundListBox()
        {
            SectorNames = DatabaseAccessorHelper.GetSectorNames();

            ClearSelection();
        }

        private void UpdateSelectedSecurityListBox()
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
            SectorEditHelper.TryAddSector(selectedNameEdit);
            UpdateFundListBox();
            ClearSelection();

        }

        private void ExecuteAddValuationCommand(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) &&  Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditHelper.TryAddDataToSector(fSelectedName, date, value);
                    UpdateFundListBox();

                    ClearSelection();
                }
            }
        }

        private void ExecuteEditSector(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(ValuesEdit, out double value))
                {
                    SectorEditHelper.TryEditSector(fSelectedName, date, value);
                    UpdateFundListBox();

                    ClearSelection();
                }
            }
        }

        private void ExecuteDeleteSector(Object obj)
        {
            SectorEditHelper.TryDeleteSector(selectedNameEdit);
            UpdateFundListBox();
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public SectorEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;

            fSectorNames = new List<string>();
            fSelectedSectorData = new List<BasicDayDataView>();
            UpdateFundListBox();
            AddSectorCommand = new BasicCommand(ExecuteAddSector);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);

            EditSectorCommand = new BasicCommand(ExecuteEditSector);

            DeleteSectorCommand = new BasicCommand(ExecuteDeleteSector);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
