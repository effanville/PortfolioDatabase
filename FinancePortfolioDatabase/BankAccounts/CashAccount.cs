using DataStructures;
using System;

namespace FinanceStructures
{
    /// <summary>
    /// Contains data to model a cash account.
    /// </summary>
    /// <remarks>
    /// Currently only suitable for a bank account.
    /// Eventually will also work for cash isa etc.
    /// </remarks>
    /// <!--The name and company are used to uniquely specify this.-->
    public partial class CashAccount
    {
        /// <summary>
        /// The name associated to the account
        /// </summary>
        private string fName;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public string Name
        {
            get { return fName; }
            set { fName = value; }
        }

        /// <summary>
        /// The company name associated to the account.
        /// </summary>
        private string fCompany;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public string Company
        {
            get { return fCompany; }
            set { fCompany = value; }
        }

        /// <summary>
        /// The time indexed data for the Cash Account.
        /// </summary>
        private TimeList fAmounts;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public TimeList Amounts
        {
            get { return fAmounts; }
            set { fAmounts = value; }
        }

        /// <summary>
        /// Default constructor where no data is known.
        /// </summary>
        internal CashAccount(string name, string company)
        {
            fName =name;
            fCompany =company;
            fAmounts = new TimeList();
        }

        /// <summary>
        /// Constructor used when data is known.
        /// </summary>
        private CashAccount(string name, string company, TimeList amounts)
        {
            fName = name;
            fCompany = company;
            fAmounts = amounts;
        }

        /// <summary>
        /// Parameterless constructor for serialisation.
        /// </summary>
        private CashAccount()
        {
            fAmounts = new TimeList();
        }

        /// <summary>
        /// Checks whether a non null non zero length data list.
        /// </summary>
        public bool Any()
        {
            return fAmounts != null || fAmounts.Any();
        }
    }
}
