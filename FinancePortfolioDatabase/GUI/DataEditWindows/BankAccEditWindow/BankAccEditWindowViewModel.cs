using System;
using GUISupport;
using FinancialStructures.FinanceStructures;
using FinancialStructures.ReportingStructures;
using FinancialStructures.GUIFinanceStructures;
using BankAccountHelperFunctions;
using System.Collections.Generic;
using System.Windows.Input;
using GUIAccessorFunctions;

namespace FinanceWindowsViewModels
{
    public class BankAccEditWindowViewModel : PropertyChangedBase
    {
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

        public ICommand CreateAccountCommand { get; set; }

        public ICommand DeleteAccountCommand { get; }

        public ICommand EditAccountDataCommand { get; set; }

        public ICommand DeleteValuationCommand { get; }

        public ICommand CloseCommand { get; }

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
        }

        private void UpdateSelectedAccountListBox()
        {
            if (fSelectedName != null)
            {
                DatabaseAccessor.GetPortfolio().TryGetBankAccount(fSelectedName.Name, fSelectedName.Company, out CashAccount wanted);
                selectedAccount = wanted;

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
            UpdateMainWindow(true);
        }

        private void ExecuteEditDataCommand(Object obj)
        {
            if (fSelectedName != null && selectedAccount != null)
            {
                if (DatabaseAccessor.GetBankAccountFromName(fSelectedName.Name, fSelectedName.Company).Count() !=  SelectedAccountData.Count)
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

            CreateAccountCommand = new BasicCommand(ExecuteCreateBankAccount);
            EditAccountDataCommand = new BasicCommand(ExecuteEditDataCommand);
            DeleteAccountCommand = new BasicCommand(ExecuteDeleteBankAccount);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
            CloseCommand = new BasicCommand(ExecuteCloseCommand);
        }
    }
}
