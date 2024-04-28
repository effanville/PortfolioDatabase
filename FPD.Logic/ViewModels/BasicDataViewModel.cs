using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DataStructures;
using Effanville.Common.Structure.NamingStructures;
using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.FinancialStructures;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Extensions.Values;
using Effanville.FinancialStructures.DataStructures;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels
{
    /// <summary>
    /// The homepage of the program, detailing main holdings in the portfolio.
    /// </summary>
    public class BasicDataViewModel : DataDisplayViewModelBase
    {
        private string _portfolioNameText;

        /// <summary>
        /// The name of the portfolio to display.
        /// </summary>
        public string PortfolioNameText
        {
            get => _portfolioNameText;
            set => SetAndNotify(ref _portfolioNameText, value, nameof(PortfolioNameText));
        }

        private bool _hasValues;

        /// <summary>
        /// Are any values present in the portfolio.
        /// </summary>
        public bool HasValues
        {
            get => _hasValues;
            set
            {
                if (_hasValues != value)
                {
                    _hasValues = value;
                    OnPropertyChanged(nameof(HasValues));
                    OnPropertyChanged(nameof(NoValues));
                }
            }
        }

        /// <summary>
        /// The opposite of <see cref="HasValues"/>
        /// </summary>
        public bool NoValues => !_hasValues;

        private string _securityTotalText;

        /// <summary>
        /// A string detailing the total number of securities.
        /// </summary>
        public string SecurityTotalText
        {
            get => _securityTotalText;
            set => SetAndNotify(ref _securityTotalText, value);
        }

        private string _securityAmountText;

        /// <summary>
        /// A string detailing the total value of securities.
        /// </summary>
        public string SecurityAmountText
        {
            get => _securityAmountText;
            set => SetAndNotify(ref _securityAmountText, value);
        }

        private List<Labelled<TwoName, DailyValuation>> _topSecurities;

        /// <summary>
        /// The major security latest values.
        /// </summary>
        public List<Labelled<TwoName, DailyValuation>> TopSecurities
        {
            get => _topSecurities;
            set => SetAndNotify(ref _topSecurities, value);
        }

        private string _bankAccountTotalText;

        /// <summary>
        /// The total number of bank accounts in the portfolio string.
        /// </summary>
        public string BankAccountTotalText
        {
            get => _bankAccountTotalText;
            set => SetAndNotify(ref _bankAccountTotalText, value);
        }

        private string _bankAccountAmountText;

        /// <summary>
        /// The total amount in bank accounts as a string.
        /// </summary>
        public string BankAccountAmountText
        {
            get => _bankAccountAmountText;
            set => SetAndNotify(ref _bankAccountAmountText, value);
        }

        private List<Labelled<TwoName, DailyValuation>> _topBankAccounts;

        /// <summary>
        /// The major bank account values.
        /// </summary>
        public List<Labelled<TwoName, DailyValuation>> TopBankAccounts
        {
            get => _topBankAccounts;
            set => SetAndNotify(ref _topBankAccounts, value);
        }

        private List<Note> _notes;

        /// <summary>
        /// The notes held in the portfolio.
        /// </summary>
        public List<Note> Notes
        {
            get => _notes;
            set => SetAndNotify(ref _notes, value);
        }

        private Note _selectedNote;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public BasicDataViewModel(UiGlobals globals, UiStyles styles, IPortfolio portfolio)
            : base(globals, styles, portfolio, "Overview", Account.All)
        {
            SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChanged);
            CreateCommand = new RelayCommand(ExecuteCreateEdit);
        }

        /// <inheritdoc/>
        public override void UpdateData(IPortfolio modelData)
        {
            ModelData = modelData;
            
            PortfolioNameText = string.IsNullOrWhiteSpace(modelData.Name) ? "Unsaved database" : modelData.Name;
            HasValues = modelData.NumberOf(Account.All) != 0;
            SecurityTotalText = $"Total Securities: {modelData.NumberOf(Account.Security)}";
            SecurityAmountText = $"Total Value: {modelData.TotalValue(Totals.Security).WithCurrencySymbol(modelData.BaseCurrency)}";
            List<ISecurity> securities = modelData.Funds.ToList();

            securities.Sort((fund, otherFund) => fund.ValueComparison(otherFund, DateTime.Today));
            var securitiesTop = new List<Labelled<TwoName, DailyValuation>>(5);
            foreach (var sec in securities.Take(5))
            {
                ICurrency currency = modelData.Currency(sec);
                Labelled<TwoName, DailyValuation> temp = new Labelled<TwoName, DailyValuation>(
                    new TwoName(sec.Names.Company, sec.Names.Name),
                    sec.Value(DateTime.Today, currency));
                securitiesTop.Add(temp);
            }

            TopSecurities = securitiesTop;

            BankAccountTotalText = $"Total Bank Accounts: {modelData.NumberOf(Account.BankAccount)}";
            BankAccountAmountText = $"Total Value: {modelData.TotalValue(Totals.BankAccount).WithCurrencySymbol(modelData.BaseCurrency)}";
            List<IExchangableValueList> bankAccounts = modelData.BankAccounts.ToList();
            bankAccounts.Sort((bank, otherBank) => bank.ValueComparison(otherBank, DateTime.Today));

            var bankAccountsTop = new List<Labelled<TwoName, DailyValuation>>(5);
            foreach (var bank in bankAccounts.Take(5))
            {
                ICurrency currency = modelData.Currency(bank);
                var temp = new Labelled<TwoName, DailyValuation>(
                    new TwoName(bank.Names.Company, bank.Names.Name),
                    bank.Value(DateTime.Today, currency));
                bankAccountsTop.Add(temp);
            }

            TopBankAccounts = bankAccountsTop;
            Notes = modelData.Notes.ToList();
        }

        /// <summary>
        /// Enacts the selected item in the datagrid has changed.
        /// </summary>
        public ICommand SelectionChangedCommand { get; set; }

        private void ExecuteSelectionChanged(object args)
        {
            if (Notes != null && args is Note note)
            {
                _selectedNote = note;
            }
            else if (args == CollectionView.NewItemPlaceholder)
            {
                _selectedNote = null;
            }
        }

        /// <summary>
        /// Adds a new entry if the view has more than the repository, or edits an entry if these are the same.
        /// </summary>
        public ICommand CreateCommand { get; set; }

        private void ExecuteCreateEdit()
        {
            if (_selectedNote != null && !ModelData.Notes.Contains(_selectedNote))
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, portfolio => portfolio.AddNote(_selectedNote.TimeStamp, _selectedNote.Text)));
            }
        }

        /// <summary>
        /// Method to delete the selected note.
        /// </summary>
        public void DeleteSelectedNote()
        {
            if (_selectedNote != null)
            {
                OnUpdateRequest(new UpdateRequestArgs<IPortfolio>(true, portfolio => portfolio.RemoveNote(_selectedNote)));
            }
        }
    }
}
