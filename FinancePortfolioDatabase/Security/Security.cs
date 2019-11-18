using System;
using System.Collections;
using DataStructures;

namespace FinanceStructures
{
    /// <summary>
    /// Class to model a stock, or a unit trust.
    /// </summary>
    public partial class Security : IComparable
    {
        /// <summary>
        /// The name of the security in question.
        /// </summary>
        private string fName;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public string Name
        {
            get { return fName; }
            set { fName= value; }
        }

        /// <summary>
        /// The company the security belongs with.
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
        /// The number of shares held in this security.
        /// </summary>
        private TimeList fShares;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public TimeList Shares
        {
            get { return fShares; }
            set { fShares = value; }
        }

        /// <summary>
        /// The data of the price per unit/share of this security.
        /// </summary>
        private TimeList fUnitPrice;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public TimeList UnitPrice
        {
            get { return fUnitPrice; }
            set { fUnitPrice = value; }
        }

        /// <summary>
        /// A list of investments made in this security.
        /// </summary>
        private TimeList fInvestments;

        /// <summary>
        /// This should only be used for serialisation.
        /// </summary>
        public TimeList Investments
        {
            get { return fInvestments; }
            set { fInvestments = value; }
        }

        /// <summary>
        /// An empty constructor.
        /// </summary>
        private Security()
        {
            fShares = new TimeList();
            fUnitPrice = new TimeList();
            fInvestments = new TimeList();
        }

        /// <summary>
        /// Constructor creating a new security.
        /// </summary>
        public Security(string name, string company)
        {
            fName = name;
            fCompany = company;
            fShares = new TimeList();
            fUnitPrice = new TimeList();
            fInvestments = new TimeList();
        }

        /// <summary>
        /// Constructor to make a new security from known data.
        /// </summary>
        private Security(string name, string company, TimeList shares, TimeList prices, TimeList investments)
        {
            fName = name;
            fCompany = company;
            fShares = shares;
            fUnitPrice = prices;
            fInvestments = investments;
        }

        /// <summary>
        /// Method of comparison
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is Security value)
            {
                if (Company == value.Company)
                {
                    return Name.CompareTo(value.Name);
                }

                return Company.CompareTo(value.Company);
            }

            return 0;
        }
    }
}
