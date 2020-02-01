using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GUISupport;
using PADGlobals;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FinanceWindowsViewModels
{
    public class BankAccEditWindowViewModel : PropertyChangedBase
    {
        private Portfolio Portfolio;
        private List<Sector> Sectors;
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
                if (SelectedAccountData != null)
                {
                    int index = SelectedAccountData.IndexOf(value);
                    if (selectedIndex != index)
                    {
                        selectedIndex = index;
                        fOldSelectedValue = fSelectedValues?.Copy();
                    }
                }
                OnPropertyChanged();
            }
        }

        public ICommand CreateAccountCommand { get; set; }

        public ICommand DeleteAccountCommand { get; }

        public ICommand EditAccountDataCommand { get; set; }

        public ICommand DeleteValuationCommand { get; }

        public ICommand DownloadCommand { get; }

        private async void ExecuteDownloadCommand(Object obj)
        {
            var reports = new ErrorReports();
            if (fSelectedName != null)
            {
                await DataUpdater.DownloadBankAccount(Portfolio, fSelectedName.Company, fSelectedName.Name, UpdateReports, reports).ConfigureAwait(false);
            }
            UpdateMainWindow(true);
            if (reports.Any())
            {
                UpdateReports(reports);
            }
        }

        public void UpdateAccountListBox(Portfolio portfolio, List<Sector> sectors)
        {
            Portfolio = portfolio;
            Sectors = sectors;
            var currentSelectionName = selectedName;
            AccountNames = Portfolio.GetBankAccountNamesAndCompanies();
            fPreEditAccountNames = Portfolio.GetBankAccountNamesAndCompanies();
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

        public void UpdateAccountListBox()
        {
            var currentSelectionName = selectedName;
            AccountNames = Portfolio.GetBankAccountNamesAndCompanies();
            fPreEditAccountNames = Portfolio.GetBankAccountNamesAndCompanies();
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
                Portfolio.GetPortfolio().TryGetBankAccount(fSelectedName.Name, fSelectedName.Company, out CashAccount wanted);
                selectedAccount = wanted;

                if (Portfolio.TryGetAccountData(fSelectedName.Name, fSelectedName.Company, out List<AccountDayDataView> values))
                {
                    SelectedAccountData = values;
                }

                SelectLatestValue();
            }
            else
            {
                SelectedAccountData = null;
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
            if (Portfolio.GetPortfolio().BankAccounts.Count != AccountNames.Count)
            {
                bool edited = false;
                foreach (var name in AccountNames)
                {
                    if (name.NewValue && (!string.IsNullOrEmpty(name.Name) || !string.IsNullOrEmpty(name.Company)))
                    {
                        edited = true;
                        Portfolio.TryAddBankAccount(name.Name, name.Company, name.Currency, name.Sectors, reports);
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
                        Portfolio.TryEditBankAccountName(fPreEditAccountNames[i].Name, fPreEditAccountNames[i].Company, name.Name, name.Company, name.Currency, name.Sectors, reports);
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
                if (Portfolio.GetBankAccountFromName(fSelectedName.Name, fSelectedName.Company).Count() != SelectedAccountData.Count)
                {
                    Portfolio.TryAddDataToBankAccount(selectedName.Name, selectedName.Company, selectedValues.Date, selectedValues.Amount);
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
                            Portfolio.TryEditBankAccount(selectedName.Name, selectedName.Company, fOldSelectedValue.Date, selectedValues.Date, selectedValues.Amount, reports);
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
                Portfolio.TryRemoveBankAccount(selectedName.Name, selectedName.Company, reports);
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
                Portfolio.TryDeleteBankAccountData(selectedName.Name, selectedName.Company, selectedValues.Date, reports);
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

        public BankAccEditWindowViewModel(Portfolio portfolio, List<Sector> sectors, Action<bool> updateWindow, Action<ErrorReports> updateReports)
        {
            Portfolio = portfolio;
            Sectors = sectors;
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
            DownloadCommand = new BasicCommand(ExecuteDownloadCommand);
        }
    }
}
