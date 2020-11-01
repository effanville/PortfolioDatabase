﻿using System;

namespace FinancialStructures.FinanceInterfaces
{
    /// <summary>
    /// Contains <see cref="IPortfolio"/> specific event arguments.
    /// </summary>
    public class PortfolioEventArgs : EventArgs
    {
        /// <summary>
        /// Is the change pertaining to Securities.
        /// </summary>
        public bool IsSecurity
        {
            get
            {
                return ChangedAccount == Account.Security || ChangedAccount == Account.All;
            }
        }

        /// <summary>
        /// Is the change pertaining to Benchmarks.
        /// </summary>
        public bool IsBenchmark
        {
            get
            {
                return ChangedAccount == Account.Benchmark || ChangedAccount == Account.All;
            }
        }

        /// <summary>
        /// Is the change pertaining to BankAccounts.
        /// </summary>
        public bool IsBankAccount
        {
            get
            {
                return ChangedAccount == Account.BankAccount || ChangedAccount == Account.All;
            }
        }

        /// <summary>
        /// Is the change pertaining to Currencies.
        /// </summary>
        public bool IsCurrency
        {
            get
            {
                return ChangedAccount == Account.Currency || ChangedAccount == Account.All;
            }
        }

        /// <summary>
        /// The type of account this refers to.
        /// </summary>
        public Account ChangedAccount
        {
            get;
        }

        /// <summary>
        /// Details how the account type has changed.
        /// </summary>
        public EventAction HowChanged
        {
            get;
        }

        /// <summary>
        /// Constructor taking an account type, and how the data was edited.
        /// </summary>
        public PortfolioEventArgs(Account type, EventAction eventAction)
            : base()
        {
            HowChanged = eventAction;
            ChangedAccount = type;
        }

        /// <summary>
        /// Constructor taking an account type.
        /// </summary>
        public PortfolioEventArgs(Account type)
            : base()
        {
            ChangedAccount = type;
        }

        /// <summary>
        /// Default Constructor that alerts to all changed.
        /// </summary>
        public PortfolioEventArgs()
            : base()
        {
            ChangedAccount = Account.All;
        }

        /// <summary>
        /// Should the type of account be updated or not.
        /// </summary>
        public bool ShouldUpdate(Account dataType)
        {
            return ChangedAccount == Account.All || dataType.Equals(ChangedAccount);
        }
    }
}
