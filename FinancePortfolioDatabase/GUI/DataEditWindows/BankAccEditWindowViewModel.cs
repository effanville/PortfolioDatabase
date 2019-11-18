using System;
using GUISupport;
using FinanceStructures;
using ReportingStructures;
using GUIFinanceStructures;
using BankAccountHelperFunctions;
using System.Collections.Generic;
using System.Windows.Input;
using GUIAccessorFunctions;

namespace FinanceWindowsViewModels
{
    public class BankAccEditWindowViewModel : PropertyChangedBase
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
        private List<NameComp> fAccountNames;
        /// <summary>
        /// Name and Company data of Funds in database for view.
        /// </summary>
        public List<NameComp> AccountNames
        {
            get { return fAccountNames; }
            set { fAccountNames = value; OnPropertyChanged(); }
        }

        private NameComp fSelectedName;

        /// <summary>
        /// Name and Company data of the selected security in the list <see cref="AccountNames"/>
        /// </summary>
        public NameComp selectedName
        {
            get { return fSelectedName; }
            set { fSelectedName = value; OnPropertyChanged(); UpdateSelectedSecurityListBox(); }
        }

        private CashAccount fSelectedAccount;
        /// <summary>
        /// The Complete data on the security selected
        /// </summary>
        public CashAccount selectedAccount
        {
            get { return fSelectedAccount; }
            set { fSelectedAccount = value; OnPropertyChanged(); }
        }

        private List<AccountDayDataView> fSelectedAccountData;
        public List<AccountDayDataView> SelectedAccountData
        {
            get { return fSelectedAccountData; }
            set { fSelectedAccountData = value; OnPropertyChanged(); }
        }

        private AccountDayDataView fSelectedValues;
        public AccountDayDataView selectedValues
        {
            get { return fSelectedValues; }
            set { fSelectedValues = value; OnPropertyChanged(); }
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

        private string fAmountsEdit;

        public string AmountsEdit
        {
            get { return fAmountsEdit; }
            set { fAmountsEdit = value; OnPropertyChanged(); }
        }


        public ICommand AddAccountCommand { get; }

        public ICommand CreateAccountCommand { get; }

        public ICommand AddValuationCommand { get; }

        public ICommand EditAccountCommand { get; }

        public ICommand EditAccountNameCommand { get; }

        public ICommand AddDataCommand { get; }

        public ICommand DeleteAccountCommand { get; }

        public ICommand EditAccountDataCommand { get; }

        public ICommand DeleteValuationCommand { get; }

        public ICommand CloseCommand { get; }

        public ICommand ClearAccountSelectionCommand { get; }

        public ICommand ClearDataSelectionCommand { get; }

        private void UpdateAccountListBox()
        {
            var currentSelectionName = selectedName;
            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            for (int i = 0; i < AccountNames.Count; i++)
            {
                if (AccountNames[i].CompareTo(currentSelectionName) == 0)
                {
                    selectedName = AccountNames[i];
                }
            }

            UpdateSelectedSecurityListBox();
        }

        private void UpdateSelectedSecurityListBox()
        {
            if (fSelectedName != null)
            {
                DatabaseAccessor.GetPortfolio().TryGetBankAccount(fSelectedName.Name, fSelectedName.Company, out CashAccount wanted);
                selectedAccount = wanted;
                selectedCompanyEdit = fSelectedName.Company;
                selectedNameEdit = fSelectedName.Name;

                if (BankAccountEditor.TryGetBankAccountData(fSelectedName.Name, fSelectedName.Company, out List<AccountDayDataView> values))
                {
                    SelectedAccountData = values;
                }

                SelectLatestValue();
            }
        }

        private void SelectLatestValue()
        {
            if (SelectedAccountData != null && SelectedAccountData.Count > 0)
            {
                selectedValues = SelectedAccountData[SelectedAccountData.Count - 1];
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
            selectedAccount = null;
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
            SelectedAccountData = null;
            DateEdit = null;
            AmountsEdit = null;
        }

        private void ExecuteAddSecurity(Object obj)
        {
            NameAddEditVisibility = true;
            NotEditing = true;
            DataAddEditVisibility = false;
        }

        private void ShowDataAdding(Object obj)
        {
            NameAddEditVisibility = true;
            NotEditing = true;
            DataAddEditVisibility = true;
        }

        private void ExecuteCreateSecurity(Object obj)
        {
            BankAccountEditor.TryAddBankAccount(selectedNameEdit, selectedCompanyEdit);
            UpdateAccountListBox();
            ClearSelection();
            DataAddEditVisibility = false;
            NameAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteAddValuationCommand(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(AmountsEdit, out double shares))
                {
                    BankAccountEditor.TryAddDataToBankAccount(fSelectedName.Name, fSelectedName.Company, date, shares);
                    UpdateAccountListBox();

                    ClearSelection();
                }
            }

            DataAddEditVisibility = false;
            NameAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteEditSecurityName(Object obj)
        {
            if (fSelectedName != null)
            {
                BankAccountEditor.TryEditBankAccountName(fSelectedName.Name, fSelectedName.Company, selectedNameEdit, selectedCompanyEdit);
                UpdateAccountListBox();

                ClearSelection();
            }

            DataAddEditVisibility = false;
            NameAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteEditBankAccount(Object obj)
        {
            Editing = true;
            DataAddEditVisibility = true;
            NameAddEditVisibility = true;
        }

        private void ExecuteEditDataCommand(Object obj)
        {
            if (fSelectedName != null)
            {
                if (DateTime.TryParse(DateEdit, out DateTime date) && Double.TryParse(AmountsEdit, out double shares))
                {
                    BankAccountEditor.TryEditBankAccount(fSelectedName.Name, fSelectedName.Company, date, shares);
                    UpdateAccountListBox();

                    ClearSelection();
                }
                else 
                {
                    ErrorReports.AddError($"EditingData: {DateEdit} or {AmountsEdit} was not in a suitable format.");
                }
            }
            else
            {
                ErrorReports.AddError("No Bank Account was selected when trying to delete data.");
            }

            DataAddEditVisibility = false;
            NameAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteDeleteSecurity(Object obj)
        {
            if (selectedNameEdit != null || selectedCompanyEdit != null)
            {
                BankAccountEditor.TryDeleteBankAccount(selectedNameEdit, selectedCompanyEdit);
                UpdateAccountListBox();
                UpdateMainWindow(true);
            }
            else 
            {
                ErrorReports.AddError("No Bank Account was selected when trying to delete.");
            }
        }

        private void ExecuteDeleteValuation(Object obj)
        {
            if (selectedName != null)
            {
                BankAccountEditor.TryDeleteBankAccountData(selectedName.Name, selectedName.Company, selectedValues.Date);
            }
            else 
            {
                ErrorReports.AddError("No Bank Account was selected when trying to delete data.");
            }

            UpdateSelectedSecurityListBox();
            UpdateMainWindow(true);
        }

        private void ExecuteCloseCommand(Object obj)
        {
            UpdateMainWindow(false);
            windowToView("dataview");
        }
        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public BankAccEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            fAccountNames = new List<NameComp>();
            fSelectedAccountData = new List<AccountDayDataView>();
            UpdateAccountListBox();
            AddAccountCommand = new BasicCommand(ExecuteAddSecurity);
            CreateAccountCommand = new BasicCommand(ExecuteCreateSecurity);
            AddDataCommand = new BasicCommand(ShowDataAdding);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);

            EditAccountCommand = new BasicCommand(ExecuteEditBankAccount);

            EditAccountDataCommand = new BasicCommand(ExecuteEditDataCommand);
            EditAccountNameCommand = new BasicCommand(ExecuteEditSecurityName);
            DeleteAccountCommand = new BasicCommand(ExecuteDeleteSecurity);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);

            ClearAccountSelectionCommand = new BasicCommand(ExecuteClearSelection);
            ClearDataSelectionCommand = new BasicCommand(ExecuteClearDataSelection);
        }
    }
}
