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

        private List<NameComp> fPreEditAccountNames;

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
            set { fSelectedName = value; OnPropertyChanged(); UpdateSelectedAccountListBox(); }
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

        public ICommand CreateAccountButtonCommand { get; }

        public ICommand CreateAccountCommand { get; set; }

        public ICommand AddValuationCommand { get; }

        public ICommand EditAccountCommand { get; }

        public ICommand EditAccountNameCommand { get; }

        public ICommand AddDataCommand { get; }

        public ICommand DeleteAccountCommand { get; }

        public ICommand EditAccountDataButtonCommand { get; }

        public ICommand EditAccountDataCommand { get; set; }

        public ICommand DeleteValuationCommand { get; }

        public ICommand CloseCommand { get; }

        public ICommand ClearAccountSelectionCommand { get; }

        public ICommand ClearDataSelectionCommand { get; }

        public void UpdateAccountListBox()
        {
            var currentSelectionName = selectedName;
            AccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            fPreEditAccountNames = DatabaseAccessor.GetBankAccountNamesAndCompanies();
            AccountNames.Sort();
            fPreEditAccountNames.Sort();

            for (int i = 0; i < AccountNames.Count; i++)
            {
                if (AccountNames[i].CompareTo(currentSelectionName) == 0)
                {
                    selectedName = AccountNames[i];
                }
            }

            UpdateSelectedAccountListBox();
        }

        private void UpdateSelectedAccountListBox()
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

        private void ExecuteCreateBankAccountButton(Object obj)
        {
            BankAccountEditor.TryAddBankAccount(selectedNameEdit, selectedCompanyEdit);
            UpdateAccountListBox();
            ClearSelection();
            DataAddEditVisibility = false;
            NameAddEditVisibility = false;
            UpdateMainWindow(true);
        }

        private void ExecuteCreateBankAccount(Object obj)
        {
            if (DatabaseAccessor.GetPortfolio().BankAccounts.Count != AccountNames.Count)
            {
                bool edited = false;
                foreach (var name in AccountNames)
                {
                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        BankAccountEditor.TryAddBankAccount(name.Name, name.Company);
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
                for (int i = 0; i < AccountNames.Count; i++)
                {
                    var name = AccountNames[i];

                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        BankAccountEditor.TryEditBankAccountName(fPreEditAccountNames[i].Name, fPreEditAccountNames[i].Company, name.Name, name.Company);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                {
                    ErrorReports.AddError("Was not able to edit desired sector.");
                }
            }

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

        private void ExecuteEditDataButtonCommand(Object obj)
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

        private void ExecuteEditDataCommand(Object obj)
        {
            if (selectedName != null && selectedAccount != null)
            {
                if (DatabaseAccessor.GetSectorFromName(selectedName.Name).Count() != SelectedAccountData.Count)
                {
                    BankAccountEditor.TryAddDataToBankAccount(selectedName.Name, selectedName.Company, selectedValues.Date, selectedValues.Amount);
                    selectedName.NewValue = false;
                }
                else
                {
                    bool edited = false;
                    for (int i = 0; i < SelectedAccountData.Count; i++)
                    {
                        var name = SelectedAccountData[i];

                        if (name.NewValue)
                        {
                            edited = true;
                            BankAccountEditor.TryEditBankAccount(selectedName.Name, selectedName.Company, selectedValues.Date, selectedValues.Amount);
                            name.NewValue = false;
                        }
                    }
                    if (!edited)
                    {
                        ErrorReports.AddError("Was not able to edit sector data.");
                    }
                }
            }
            UpdateSelectedAccountListBox();
        }

        private void ExecuteDeleteBankAccount(Object obj)
        {
            if (selectedName.Name != null)
            {
                BankAccountEditor.TryDeleteBankAccount(selectedName.Name, selectedName.Company);
            }
            else 
            {
                ErrorReports.AddError("No Bank Account was selected when trying to delete.");
            }

            UpdateAccountListBox();
            UpdateMainWindow(true);
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

            UpdateSelectedAccountListBox();
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
            fPreEditAccountNames = new List<NameComp>();
            fSelectedAccountData = new List<AccountDayDataView>();
            UpdateAccountListBox();
            AddAccountCommand = new BasicCommand(ExecuteAddSecurity);
            CreateAccountButtonCommand = new BasicCommand(ExecuteCreateBankAccountButton);
            CreateAccountCommand = new BasicCommand(ExecuteCreateBankAccount);
            AddDataCommand = new BasicCommand(ShowDataAdding);
            AddValuationCommand = new BasicCommand(ExecuteAddValuationCommand);

            EditAccountCommand = new BasicCommand(ExecuteEditBankAccount);

            EditAccountDataButtonCommand = new BasicCommand(ExecuteEditDataButtonCommand);
            EditAccountDataCommand = new BasicCommand(ExecuteEditDataCommand);
            EditAccountNameCommand = new BasicCommand(ExecuteEditSecurityName);
            DeleteAccountCommand = new BasicCommand(ExecuteDeleteBankAccount);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);

            ClearAccountSelectionCommand = new BasicCommand(ExecuteClearSelection);
            ClearDataSelectionCommand = new BasicCommand(ExecuteClearDataSelection);
        }
    }
}
