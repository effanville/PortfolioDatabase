using GUIAccessorFunctions;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using SecurityHelperFunctions;
using GUISupport;
using FinanceStructures;
using DataStructures;
using GUIFinanceStructures;
using ReportingStructures;

namespace FinanceWindowsViewModels
{
    public class SecurityEditWindowViewModel : PropertyChangedBase
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

        public ICommand CreateSecurityCommand { get; }

        public ICommand AddValuationCommand { get; }

        public ICommand EditSecurityCommand { get; }

        public ICommand ShowEditSecurityCommand { get; }

        public ICommand EditSecurityNameCommand { get; }

        public ICommand AddDataCommand { get; }

        public ICommand DeleteSecurityCommand { get; }

        public ICommand DeleteValuationCommand { get; }

        public ICommand CloseCommand { get; }

        public ICommand ClearFundSelectionCommand { get; }

        public ICommand ClearDataSelectionCommand { get; }

        private void UpdateFundListBox()
        {
            FundNames = DatabaseAccessor.GetSecurityNamesAndCompanies();
            FundNames.Sort();
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
                DatabaseAccessor.GetPortfolio().TryGetSecurity(fSelectedName.Name, fSelectedName.Company, out Security wanted);
                selectedSecurity = wanted;
                selectedCompanyEdit = fSelectedName.Company;
                selectedNameEdit = fSelectedName.Name;

                if (SecurityEditor.TryGetSecurityData(fSelectedName.Name, fSelectedName.Company, out List<BasicDayDataView> values))
                {
                    SelectedSecurityData = values;
                }

                SelectLatestValue();
            }
        }

        private void SelectLatestValue()
        {
            if (SelectedSecurityData != null  && SelectedSecurityData.Count > 0)
            {
                selectedValues = SelectedSecurityData[SelectedSecurityData.Count - 1];
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
            DataAddEditVisibility = false;
            NameAddEditVisibility = true;
            NotEditing = true;
        }

        private void ExecuteCreateSecurity(Object obj)
        {
            if (selectedNameEdit != null && selectedCompanyEdit != null)
            {
                SecurityEditor.TryAddSecurity(selectedNameEdit, selectedCompanyEdit);
                UpdateFundListBox();
                ClearSelection();
                
            }
            else 
            {
                ErrorReports.AddError("Name or company given were null");
            }
            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteAddValuationCommand(Object obj)
        {
            NameAddEditVisibility = true;
            DataAddEditVisibility = true;
            NotEditing = true;
        }
        /// <summary>
        /// Adds data to selected security.
        /// </summary>
        /// <remarks>
        /// This method not sufficient
        /// Must rethink how I deal with new investments.</remarks>
        private void ExecuteAddDataCommand(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date))
                {
                    if (Double.TryParse(SharesEdit, out double shares) && Double.TryParse(UnitPriceEdit, out double unitPrice))
                    {
                        double investmentValue = 0;
                        if (NewInvestment)
                        {
                            //user specified added new money to fund, so difference between last number of shares and numer shares now is considered new money.  
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

                        //Current 
                        SecurityEditor.TryAddDataToSecurity(fSelectedName.Name, fSelectedName.Company, date, shares, unitPrice, investmentValue);
                        UpdateFundListBox();
                    }
                    else
                    {
                        ErrorReports.AddError($"Number of Shares of {SharesEdit} or unit price {UnitPriceEdit} was not parsable as a number.");
                    }
                }
                else 
                { 
                    ErrorReports.AddError($"Date of {DateEdit} was not parsable as a Date.");
                }
            }

            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteEditSecurityName(Object obj)
        {
            if (fSelectedName != null)
            {
                SecurityEditor.TryEditSecurityName(fSelectedName.Name, fSelectedName.Company, selectedNameEdit, selectedCompanyEdit);
                UpdateFundListBox();

                ClearSelection();
            }

            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteShowEditSecurity(Object obj)
        {
            NameAddEditVisibility = true;
            Editing = true;
            DataAddEditVisibility = true;
        }

        private void ExecuteEditSecurity(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date))
                {
                    if (Double.TryParse(SharesEdit, out double shares) && Double.TryParse(UnitPriceEdit, out double unitPrice))
                    {
                        double investmentValue = 0;
                        if (NewInvestment)
                        {
                            investmentValue = shares * unitPrice;
                        }
                        SecurityEditor.TryEditSecurity(fSelectedName.Name, fSelectedName.Company, date, shares, unitPrice, investmentValue);
                        UpdateFundListBox();

                        ClearDataSelection();
                    }
                }
                else 
                {
                    ErrorReports.AddError($"Date format of {DateEdit} was not suitable.");
                }
            }
            else 
            {
                ErrorReports.AddReport("No security or data selected to edit");
            }

            NameAddEditVisibility = false;
            DataAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteDeleteSecurity(Object obj)
        {
            SecurityEditor.TryDeleteSecurity(selectedNameEdit, selectedCompanyEdit);
            UpdateFundListBox();
            ClearSelection();
            UpdateMainWindow(true);
        }

        private void ExecuteDeleteValuation(Object obj)
        {
            if (selectedName != null && selectedValues != null)
            {
                SecurityEditor.TryDeleteSecurityData(selectedName.Name, selectedName.Company, selectedValues.Date, selectedValues.ShareNo, selectedValues.UnitPrice, selectedValues.Investment);
                UpdateSelectedSecurityListBox();
                ClearDataSelection();
            }

            UpdateMainWindow(true);
        }

        private void ExecuteCloseCommand(Object obj)
        {
            UpdateMainWindow(false);
            windowToView("dataview");
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public SecurityEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            DataAddEditVisibility = false;
            NameAddEditVisibility = false;
            Editing = false;
            fFundNames = new List<NameComp>();
            fSelectedSecurityData = new List<BasicDayDataView>();
            UpdateFundListBox();
            AddSecurityCommand = new BasicCommand(ExecuteAddSecurity);
            CreateSecurityCommand = new BasicCommand(ExecuteCreateSecurity);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);
            AddDataCommand = new BasicCommand(ExecuteAddDataCommand);
            EditSecurityCommand = new BasicCommand(ExecuteEditSecurity);
            ShowEditSecurityCommand = new BasicCommand(ExecuteShowEditSecurity);
            EditSecurityNameCommand = new BasicCommand(ExecuteEditSecurityName);
            DeleteSecurityCommand = new BasicCommand(ExecuteDeleteSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);

            ClearFundSelectionCommand = new BasicCommand(ExecuteClearSelection);
            ClearDataSelectionCommand = new BasicCommand(ExecuteClearDataSelection);
        }
    }
}
