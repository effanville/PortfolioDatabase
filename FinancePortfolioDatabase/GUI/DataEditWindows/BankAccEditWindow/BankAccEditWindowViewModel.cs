using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUIAccessorFunctions;
using GUISupport;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using GlobalHeldData;

namespace FinanceWindowsViewModels
{
    public class BankAccEditWindowViewModel : PropertyChangedBase
    {
        private List<NameData> fPreEditAccountNames;

        private List<NameData> fAccountNames;
        /// <summary>
        /// Name and Company data of Funds in database for view.
        /// </summary>
        public List<NameData> AccountNames
        {
            get { return fAccountNames; }
            set { fAccountNames = value; OnPropertyChanged(); }
        }

        private NameData fSelectedName;

        /// <summary>
        /// Name and Company data of the selected security in the list <see cref="AccountNames"/>
        /// </summary>
        public NameData selectedName
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

        public void UpdateAccountListBox()
        {
            var currentSelectionName = selectedName;
            AccountNames = GlobalData.Finances.GetBankAccountNamesAndCompanies();
            fPreEditAccountNames = GlobalData.Finances.GetBankAccountNamesAndCompanies();
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
                GlobalData.Finances.GetPortfolio().TryGetBankAccount(fSelectedName.Name, fSelectedName.Company, out CashAccount wanted);
                selectedAccount = wanted;

                if (GlobalData.Finances.TryGetAccountData(fSelectedName.Name, fSelectedName.Company, out List<AccountDayDataView> values))
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
            if (GlobalData.Finances.GetPortfolio().BankAccounts.Count != AccountNames.Count)
            {
                bool edited = false;
                foreach (var name in AccountNames)
                {
                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        GlobalData.Finances.TryAddBankAccount(name.Name, name.Company, name.Currency, name.Sectors, reports);
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
                        GlobalData.Finances.TryEditBankAccountName(fPreEditAccountNames[i].Name, fPreEditAccountNames[i].Company, name.Name, name.Company, name.Currency, name.Sectors, reports);
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
                if (GlobalData.Finances.GetBankAccountFromName(fSelectedName.Name, fSelectedName.Company).Count() != SelectedAccountData.Count)
                {
                    GlobalData.Finances.TryAddDataToBankAccount(selectedName.Name, selectedName.Company, selectedValues.Date, selectedValues.Amount);
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
                            GlobalData.Finances.TryEditBankAccount(selectedName.Name, selectedName.Company, fOldSelectedValue.Date, selectedValues.Date, selectedValues.Amount, reports);
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
                GlobalData.Finances.TryRemoveBankAccount(selectedName.Name, selectedName.Company, reports);
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
                GlobalData.Finances.TryDeleteBankAccountData(selectedName.Name, selectedName.Company, selectedValues.Date, reports);
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

        Action<bool> UpdateMainWindow;
        Action<ErrorReports> UpdateReports;

        public BankAccEditWindowViewModel(Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            UpdateMainWindow = updateWindow;
            UpdateReports = updateReports;
            fAccountNames = new List<NameData>();
            fPreEditAccountNames = new List<NameData>();
            fSelectedAccountData = new List<AccountDayDataView>();
            UpdateAccountListBox();

            CreateAccountCommand = new BasicCommand(ExecuteCreateBankAccount);
            EditAccountDataCommand = new BasicCommand(ExecuteEditDataCommand);
            DeleteAccountCommand = new BasicCommand(ExecuteDeleteBankAccount);
            DeleteValuationCommand = new BasicCommand(ExecuteDeleteValuation);
        }
    }
}
