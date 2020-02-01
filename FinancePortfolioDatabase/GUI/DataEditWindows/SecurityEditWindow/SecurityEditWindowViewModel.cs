using FinanceWindowsViewModels.SecurityEdit;
using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class SecurityEditWindowViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;
        private List<NameCompDate> fPreEditFundNames = new List<NameCompDate>();

        private List<NameCompDate> fFundNames = new List<NameCompDate>();
        /// <summary>
        /// Name and Company data of Funds in database for view.
        /// </summary>
        public List<NameCompDate> FundNames
        {
            get { return fFundNames; }
            set { fFundNames = value; OnPropertyChanged(); }
        }

        private NameData fSelectedName;

        /// <summary>
        /// Name and Company data of the selected security in the list <see cref="FundNames"/>
        /// </summary>
        public NameData selectedName
        {
            get { return fSelectedName; }
            set { fSelectedName = value; OnPropertyChanged(); UpdateSelectedSecurityListBox(); }
        }


        private Security fSelectedSecurity;
        /// <summary>
        /// The Complete data on the security selected
        /// </summary>
        public Security selectedSecurity
        {
            get { return fSelectedSecurity; }
            set { fSelectedSecurity = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// The pricing data of the selected security.
        /// </summary>
        private List<DayDataView> fSelectedSecurityData = new List<DayDataView>();
        public List<DayDataView> SelectedSecurityData
        {
            get { return fSelectedSecurityData; }
            set { fSelectedSecurityData = value; OnPropertyChanged(); }
        }

        private DayDataView fSelectedValues;
        private DayDataView fOldSelectedValues;
        private int selectedIndex;
        public DayDataView selectedValues
        {
            get
            {
                return fSelectedValues;
            }
            set
            {
                fSelectedValues = value;
                if (SelectedSecurityData != null)
                {
                    int index = SelectedSecurityData.IndexOf(value);
                    if (selectedIndex != index)
                    {
                        selectedIndex = index;
                        fOldSelectedValues = fSelectedValues?.Copy();
                    }
                }
                OnPropertyChanged();
                UpdateSubWindows();
            }
        }

        private UserButtonsViewModel fUserClickingVM;

        public UserButtonsViewModel UserClickingVM
        {
            get { return fUserClickingVM; }
            set { fUserClickingVM = value; OnPropertyChanged(); }
        }

        public ICommand CreateSecurityCommand { get; set; }

        public ICommand AddEditSecurityDataCommand { get; set; }

        public void UpdateFundListBox(Portfolio portfolio, List<Sector> sectors)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            var currentSelectedName = selectedName;
            FundNames = Portfolio.SecurityNamesAndCompanies();
            FundNames.Sort();
            fPreEditFundNames = Portfolio.SecurityNamesAndCompanies();
            fPreEditFundNames.Sort();

            for (int i = 0; i < FundNames.Count; i++)
            {
                if (FundNames[i].CompareTo(currentSelectedName) == 0)
                {
                    selectedName = FundNames[i];
                    return;
                }
            }

            selectedName = null;
        }

        private void UpdateSelectedSecurityListBox()
        {
            if (fSelectedName != null)
            {
                Portfolio.TryGetSecurity(fSelectedName.Company, fSelectedName.Name, out Security wanted);
                selectedSecurity = wanted;
                if (Portfolio.TryGetSecurityData(fSelectedName.Company, fSelectedName.Name, out List<DayDataView> values))
                {
                    SelectedSecurityData = values;
                }

                SelectLatestValue();
                UpdateSubWindows();
            }
            else 
            {
                SelectedSecurityData = null;
            }
        }

        private void UpdateSubWindows()
        {
            UserClickingVM.UpdateButtonViewData(selectedName, selectedValues);
        }

        private void SelectLatestValue()
        {
            if (SelectedSecurityData != null && SelectedSecurityData.Count > 0)
            {
                selectedValues = SelectedSecurityData[SelectedSecurityData.Count - 1];
            }
        }

        /// <summary>
        /// Clears selected data in both Gridviews
        /// </summary>
        private void ClearSelection()
        {
            selectedSecurity = null;
            UpdateSubWindows();
        }

        private void ExecuteCreateEditCommand(Object obj)
        {
            var reports = new ErrorReports();
            if (Portfolio.Funds.Count != FundNames.Count)
            {
                bool edited = false;
                foreach (var name in FundNames)
                {
                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        Portfolio.TryAddSecurity(reports, name.Company, name.Name, name.Currency, name.Url, name.Sectors);
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
                for (int i = 0; i < FundNames.Count; i++)
                {
                    var name = FundNames[i];

                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        Portfolio.TryEditSecurityName(reports, fPreEditFundNames[i].Company, fPreEditFundNames[i].Name, name.Company, name.Name, name.Currency, name.Url, name.Sectors);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                {
                    reports.AddError("Was not able to edit desired security.");
                }
            }

            if (reports.Any())
            {
                UpdateReports(reports);
            }

            ClearSelection();
            UpdateMainWindow(true);
        }

        private void ExecuteAddEditSecData(Object obj)
        {
            var reports = new ErrorReports();
            if (selectedName != null && selectedSecurity != null)
            {
                if (Portfolio.GetSecurityFromName(selectedName.Name, selectedName.Company).Count() != SelectedSecurityData.Count)
                {
                    Portfolio.TryAddDataToSecurity(reports, selectedName.Company, selectedName.Name, selectedValues.Date, selectedValues.ShareNo, selectedValues.UnitPrice, selectedValues.Investment);
                    selectedName.NewValue = false;
                }
                else
                {
                    bool edited = false;
                    for (int i = 0; i < SelectedSecurityData.Count; i++)
                    {
                        var name = SelectedSecurityData[i];

                        if (name.NewValue)
                        {
                            edited = true;
                            Portfolio.TryEditSecurityData(reports, selectedName.Company, selectedName.Name, fOldSelectedValues.Date, selectedValues.Date, selectedValues.ShareNo, selectedValues.UnitPrice, selectedValues.Investment);
                            name.NewValue = false;
                        }
                    }
                    if (!edited)
                    {
                        reports.AddError("Was not able to edit security data.");
                    }
                }
            }
            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateSelectedSecurityListBox();
        }

        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;

        public SecurityEditWindowViewModel(Portfolio portfolio, List<Sector> sectors, Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            UpdateFundListBox(portfolio, sectors);
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            UserClickingVM = new UserButtonsViewModel(Portfolio, updateWindow, updateReports, selectedName, selectedValues);

            

            CreateSecurityCommand = new BasicCommand(ExecuteCreateEditCommand);
            AddEditSecurityDataCommand = new BasicCommand(ExecuteAddEditSecData);
        }
    }
}
