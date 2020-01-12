using BankAccountHelperFunctions;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUIAccessorFunctions;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Windows.Input;

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
        private AccountDayDataView fOldSelectedValue;
        private int selectedIndex;
        public AccountDayDataView selectedValues
        {
            get 
            {
                return fSelectedValues; 
            }
            set 
            { 
                fSelectedValues = value;
                int index = SelectedAccountData.IndexOf(value);
                if (selectedIndex != index)
                {
                    selectedIndex = index;
                    fOldSelectedValue = fSelectedValues?.Copy();
                }
                OnPropertyChanged(); 
            }
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
            var reports = new ErrorReports();
            if (DatabaseAccessor.GetPortfolio().BankAccounts.Count != AccountNames.Count)
            {
                bool edited = false;
                foreach (var name in AccountNames)
                {
                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        BankAccountEditor.TryAddBankAccount(name.Name, name.Company, name.Currency, name.Sectors, reports);
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
                for (int i = 0; i < AccountNames.Count; i++)
                {
                    var name = AccountNames[i];

                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        BankAccountEditor.TryEditBankAccountName(fPreEditAccountNames[i].Name, fPreEditAccountNames[i].Company, name.Name, name.Company, name.Currency, name.Sectors, reports);
                        name.NewValue = false;
                    }
                }
                if (!edited)
                {
                    reports.AddError("Was not able to edit desired sector.");
                }
            }
            if (reports.Any())
            {
                UpdateReports(reports);
            }

            UpdateMainWindow(true);
        }

        private void ExecuteEditDataCommand(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null && selectedAccount != null)
            {
                if (DatabaseAccessor.GetBankAccountFromName(fSelectedName.Name, fSelectedName.Company).Count() != SelectedAccountData.Count)
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
                            BankAccountEditor.TryEditBankAccount(selectedName.Name, selectedName.Company, fOldSelectedValue.Date, selectedValues.Date, selectedValues.Amount, reports);
                            name.NewValue = false;
                        }
                    }
                    if (!edited)
                    {
                        reports.AddError("Was not able to edit sector data.");
                    }
                }
            }
            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateSelectedAccountListBox();
        }

        private void ExecuteDeleteBankAccount(Object obj)
        {
            var reports = new ErrorReports();
            if (selectedName.Name != null)
            {
                BankAccountEditor.TryDeleteBankAccount(selectedName.Name, selectedName.Company, reports);
            }
            else
            {
                reports.AddError("No Bank Account was selected when trying to delete.");
            }
            if (reports.Any())
            {
                UpdateReports(reports);
            }
            UpdateAccountListBox();
            UpdateMainWindow(true);
        }

        private void ExecuteDeleteValuation(Object obj)
        {
            var reports = new ErrorReports();
            if (selectedName != null)
            {
                BankAccountEditor.TryDeleteBankAccountData(selectedName.Name, selectedName.Company, selectedValues.Date, reports);
            }
            else
            {
                reports.AddError("No Bank Account was selected when trying to delete data.");
            }
            if (reports.Any())
            {
                UpdateReports(reports);
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
        Action<ErrorReports> UpdateReports;

        public BankAccEditWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<ErrorReports> updateReports)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            UpdateReports = updateReports;
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
