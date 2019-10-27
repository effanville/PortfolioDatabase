using GUIAccessorFunctions;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using SecurityHelperFunctions;
using GuiSupport;
using FinanceStructures;
using GUIFinanceStructures;

namespace FinanceWindowsViewModels
{
    public class SecurityEditWindowViewModel : PropertyChangedBase
    {
        private List<NameComp> fFundNames;
        /// <summary>
        /// Name and Company data of Funds in database for view.
        /// </summary>
        public List<NameComp> FundNames
        {
            get { return fFundNames; }
            set { fFundNames = value; OnPropertyChanged(); }
        }

        private NameComp fSelectedName;
        
        /// <summary>
        /// Name and Company data of the selected security in the list <see cref="FundNames"/>
        /// </summary>
        public NameComp selectedName
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
        private List<BasicDayDataView> fSelectedSecurityData;
        public List<BasicDayDataView> SelectedSecurityData
        {
            get { return fSelectedSecurityData; }
            set { fSelectedSecurityData = value; OnPropertyChanged(); }
        }

        private BasicDayDataView fSelectedValues;

        public BasicDayDataView selectedValues
        {
            get { return fSelectedValues; }
            set { fSelectedValues = value; OnPropertyChanged(); UpdateValuationData(); }
        }

        private string fSelectedCompanyEdit;
        public string selectedCompanyEdit
        {
            get { return fSelectedCompanyEdit; }
            set { fSelectedCompanyEdit = value; OnPropertyChanged(); }
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

        private string fSharesEdit;

        public string SharesEdit
        {
            get { return fSharesEdit; }
            set { fSharesEdit = value; OnPropertyChanged(); }
        }

        private string fUnitPriceEdit;

        public string UnitPriceEdit
        {
            get { return fUnitPriceEdit; }
            set { fUnitPriceEdit = value; OnPropertyChanged(); }
        }

        private bool fNewInvestment;
        public bool NewInvestment
        {
            get { return fNewInvestment; }
            set { fNewInvestment = value; OnPropertyChanged(); }
        }


        public ICommand AddSecurityCommand { get; }

        public ICommand AddValuationCommand { get; }

        public ICommand EditSecurityCommand { get; }

        public ICommand EditSecurityNameCommand { get; }

        public ICommand DeleteSecurityCommand { get; }

        public ICommand DeleteValuationCommand { get; }

        public ICommand CloseCommand { get; }

        public ICommand ClearFundSelectionCommand { get; }

        public ICommand ClearDataSelectionCommand { get; }

        private void UpdateFundListBox()
        {
            FundNames = DatabaseAccessorHelper.GetSecurityNamesAndCompanies();

            selectedName = null;
            selectedNameEdit = null;
            selectedCompanyEdit = null;
            DateEdit = null;
            SharesEdit = null;
            UnitPriceEdit = null;
            NewInvestment = false;
        }

        private void UpdateSelectedSecurityListBox()
        {
            if (fSelectedName != null)
            {
                DatabaseAccessorHelper.GetPortfolio().TryGetSecurity(fSelectedName.Name, fSelectedName.Company, out Security wanted);
                selectedSecurity = wanted;
                selectedCompanyEdit = fSelectedName.Company;
                selectedNameEdit = fSelectedName.Name;

                if (SecurityEditHelper.TryGetSecurityData(fSelectedName.Name, fSelectedName.Company, out List<BasicDayDataView> values))
                {
                    SelectedSecurityData = values;
                }
            }
        }

        private void UpdateValuationData()
        {
            if (selectedValues != null)
            {
                DateEdit = selectedValues.Date.ToShortDateString();
                SharesEdit = selectedValues.ShareNo.ToString();
                UnitPriceEdit = selectedValues.UnitPrice.ToString();
                NewInvestment = selectedValues.Investment > 0 ? true : false;
            }
            else 
            {
                DateEdit = null;
                SharesEdit = null;
                UnitPriceEdit = null;
                NewInvestment = false;
            }
        }

        private void ExecuteClearSelection(Object obj)
        {
            ClearSelection();
        }

        /// <summary>
        /// Clears selected data in both Gridviews
        /// </summary>
        private void ClearSelection()
        {
            selectedSecurity = null;
            selectedNameEdit = null;
            selectedCompanyEdit = null;
            ClearDataSelection();
        }

        private void ExecuteClearDataSelection(Object obj)
        {
            ClearDataSelection();
        }

        private void ClearDataSelection()
        {
            SelectedSecurityData = null;
            DateEdit = null;
            SharesEdit = null;
            UnitPriceEdit = null;
            NewInvestment = false;
        }

        private void ExecuteAddSecurity(Object obj)
        {
            SecurityEditHelper.TryAddSecurity(selectedNameEdit, selectedCompanyEdit);
            UpdateFundListBox();
            ClearSelection();

        }

        private void ExecuteAddValuationCommand(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(SharesEdit, out double shares) && Double.TryParse(UnitPriceEdit, out double unitPrice))
                {
                    double investmentValue = 0;
                    if (NewInvestment)
                    {
                        //user specified added new money to fund, so all number share change is new money.  
                        //need to calculate this. Either have previous investment, so get numbershares
                        // or is new fund and all money added is new.
                        if (selectedSecurity.TryGetEarlierData(date, out DailyValuation earlierPrice, out DailyValuation earlierShareNo, out DailyValuation earlierInvestment))
                        {
                            investmentValue = (shares - earlierShareNo.Value) * unitPrice;
                        }
                        else
                        {
                            investmentValue = shares * unitPrice; 
                        }
                    }
                    SecurityEditHelper.TryAddDataToSecurity(fSelectedName.Name, fSelectedName.Company, date, shares, unitPrice, investmentValue);
                    UpdateFundListBox();
                   
                    ClearSelection();
                }
            }
        }

        private void ExecuteEditSecurityName(Object obj)
        {
            if (fSelectedName != null)
            {
                SecurityEditHelper.TryEditSecurityName(fSelectedName.Name, fSelectedName.Company, selectedNameEdit, selectedCompanyEdit);
                UpdateFundListBox();

                ClearSelection();
            }
        }

        private void ExecuteEditSecurity(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(SharesEdit, out double shares) && Double.TryParse(UnitPriceEdit, out double unitPrice))
                {
                    double investmentValue = 0;
                    if (NewInvestment)
                    {
                        investmentValue = shares * unitPrice;
                    }
                    SecurityEditHelper.TryEditSecurity(fSelectedName.Name, fSelectedName.Company, date, shares, unitPrice, investmentValue);
                    UpdateFundListBox();

                    ClearSelection();
                }
            }
        }

        private void ExecuteDeleteSecurity(Object obj)
        {
            SecurityEditHelper.TryDeleteSecurity(selectedNameEdit, selectedCompanyEdit);
            UpdateFundListBox(); 
        }

        private void ExecuteDeleteValuation(Object obj)
        {
            SecurityEditHelper.TryDeleteSecurityData(selectedName.Name, selectedName.Company, selectedValues.Date, selectedValues.ShareNo, selectedValues.UnitPrice, selectedValues.Investment);
            UpdateSelectedSecurityListBox();
        }

        private void ExecuteCloseCommand(Object obj)
        {
            windowToView("dataview");
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public SecurityEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            fFundNames = new List<NameComp>();
            fSelectedSecurityData = new List<BasicDayDataView>();
            UpdateFundListBox();
            AddSecurityCommand = new BasicCommand(ExecuteAddSecurity);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);
            EditSecurityCommand = new BasicCommand(ExecuteEditSecurity);
            EditSecurityNameCommand = new BasicCommand(ExecuteEditSecurityName);
            DeleteSecurityCommand = new BasicCommand(ExecuteDeleteSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);

            ClearFundSelectionCommand = new BasicCommand(ExecuteClearSelection);
            ClearDataSelectionCommand = new BasicCommand(ExecuteClearDataSelection);
        }
    }
}
