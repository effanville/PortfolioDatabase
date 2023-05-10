using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

using Common.Structure.DataStructures;
using Common.Structure.NamingStructures;
using Common.UI;
using Common.UI.Commands;

using FinancialStructures;
using FinancialStructures.Database;
using FinancialStructures.Database.Extensions;
using FinancialStructures.Database.Extensions.Values;
using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.ViewModels.Common;

namespace FPD.Logic.ViewModels
{
    /// <summary>
    /// The homepage of the program, detailing main holdings in the portfolio.
    /// </summary>
    public class BasicDataViewModel : DataDisplayViewModelBase
    {
        private readonly Action<Action<IPortfolio>> DataUpdateCallback;

        private string fPortfolioNameText;

        /// <summary>
        /// The name of the portfolio to display.
        /// </summary>
        public string PortfolioNameText
        {
            get => fPortfolioNameText;
            set => SetAndNotify(ref fPortfolioNameText, value, nameof(PortfolioNameText));
        }

        private bool fHasValues;

        /// <summary>
        /// Are any values present in the portfolio.
        /// </summary>
        public bool HasValues
        {
            get => fHasValues;
            set
            {
                if (fHasValues != value)
                {
                    fHasValues = value;
                    OnPropertyChanged(nameof(HasValues));
                    OnPropertyChanged(nameof(NoValues));
                }
            }
        }

        /// <summary>
        /// The opposite of <see cref="HasValues"/>
        /// </summary>
        public bool NoValues => !fHasValues;

        private string fSecurityTotalText;

        /// <summary>
        /// A string detailing the total number of securities.
        /// </summary>
        public string SecurityTotalText
        {
            get => fSecurityTotalText;

            set => SetAndNotify(ref fSecurityTotalText, value, nameof(SecurityTotalText));

        }

        private string fSecurityAmountText;

        /// <summary>
        /// A string detailing the total value of securities.
        /// </summary>
        public string SecurityAmountText
        {
            get => fSecurityAmountText;
            set => SetAndNotify(ref fSecurityAmountText, value, nameof(SecurityAmountText));
        }

        private List<Labelled<TwoName, DailyValuation>> fTopSecurities;

        /// <summary>
        /// The major security latest values.
        /// </summary>
        public List<Labelled<TwoName, DailyValuation>> TopSecurities
        {
            get => fTopSecurities;
            set
            {
                if (fTopSecurities != value)
                {
                    fTopSecurities = value;
                    OnPropertyChanged(nameof(TopSecurities));
                }
            }
        }

        private string fBankAccountTotalText;

        /// <summary>
        /// The total number of bank accounts in the portfolio string.
        /// </summary>
        public string BankAccountTotalText
        {
            get => fBankAccountTotalText;
            set => SetAndNotify(ref fBankAccountTotalText, value, nameof(BankAccountTotalText));
        }

        private string fBankAccountAmountText;

        /// <summary>
        /// The total amount in bank accounts as a string.
        /// </summary>
        public string BankAccountAmountText
        {
            get => fBankAccountAmountText;
            set => SetAndNotify(ref fBankAccountAmountText, value, nameof(BankAccountAmountText));
        }

        private List<Labelled<TwoName, DailyValuation>> fTopBankAccounts;

        /// <summary>
        /// The major bank account values.
        /// </summary>
        public List<Labelled<TwoName, DailyValuation>> TopBankAccounts
        {
            get => fTopBankAccounts;
            set => SetAndNotify(ref fTopBankAccounts, value, nameof(TopBankAccounts));
        }

        private List<Note> fNotes;

        /// <summary>
        /// The notes held in the portfolio.
        /// </summary>
        public List<Note> Notes
        {
            get => fNotes;
            set => SetAndNotify(ref fNotes, value, nameof(Notes));
        }

        private Note fSelectedNote;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public BasicDataViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio, Action<Action<IPortfolio>> updateData)
            : base(globals, styles, portfolio, "Overview", Account.All)
        {
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            CreateCommand = new RelayCommand(ExecuteCreateEdit);
            DataUpdateCallback = updateData;
            UpdateData(portfolio);
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio portfolio)
        {
            DataStore = portfolio;
            PortfolioNameText = string.IsNullOrWhiteSpace(portfolio.Name) ? "Unsaved database" : $"{portfolio.Name}";
            HasValues = portfolio.NumberOf(Account.All) != 0;
            SecurityTotalText = $"Total Securities: {portfolio.NumberOf(Account.Security)}";
            SecurityAmountText = $"Total Value: {portfolio.TotalValue(Totals.Security).WithCurrencySymbol(portfolio.BaseCurrency)}";
            List<ISecurity> securities = portfolio.FundsThreadSafe.ToList();

            securities.Sort((fund, otherFund) => fund.ValueComparison(otherFund, DateTime.Today));
            TopSecurities = securities.Take(5).Select(name => new Labelled<TwoName, DailyValuation>(new TwoName(name.Names.Company, name.Names.Name), name.Value(DateTime.Today))).ToList();

            BankAccountTotalText = $"Total Bank Accounts: {portfolio.NumberOf(Account.BankAccount)}";
            BankAccountAmountText = $"Total Value: {portfolio.TotalValue(Totals.BankAccount).WithCurrencySymbol(portfolio.BaseCurrency)}";
            List<IExchangableValueList> bankAccounts = portfolio.BankAccountsThreadSafe.ToList();
            bankAccounts.Sort((bank, otherBank) => bank.ValueComparison(otherBank, DateTime.Today));
            TopBankAccounts = bankAccounts.Take(5).Select(name => new Labelled<TwoName, DailyValuation>(new TwoName(name.Names.Company, name.Names.Name), name.Value(DateTime.Today) ?? new DailyValuation(DateTime.Today, 0.0m))).ToList();
            Notes = portfolio.Notes.ToList();
        }

        /// <summary>
        /// Enacts the selected item in the datagrid has changed.
        /// </summary>
        public ICommand SelectionChangedCommand
        {
            get;
            set;
        }

        private void ExecuteSelectionChanged(object args)
        {
            if (Notes != null && args is Note note)
            {
                fSelectedNote = note;
            }
            else if (args == CollectionView.NewItemPlaceholder)
            {
                fSelectedNote = null;
            }
        }

        /// <summary>
        /// Adds a new entry if the view has more than the repository, or edits an entry if these are the same.
        /// </summary>
        public ICommand CreateCommand
        {
            get;
            set;
        }

        private void ExecuteCreateEdit()
        {
            if (fSelectedNote != null && !DataStore.Notes.Contains(fSelectedNote))
            {
                DataUpdateCallback(portfolio => portfolio.AddNote(fSelectedNote.TimeStamp, fSelectedNote.Text));
            }
        }

        /// <summary>
        /// Method to delete the selected note.
        /// </summary>
        public void DeleteSelectedNote()
        {
            if (fSelectedNote != null)
            {
                DataUpdateCallback(portfolio => portfolio.RemoveNote(fSelectedNote));
            }
        }
    }
}
