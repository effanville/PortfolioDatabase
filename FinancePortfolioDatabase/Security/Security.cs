namespace FinanceStructures
{
    public partial class Security
    {
        private string fName;

        /// <summary>
        /// For serialisation only.
        /// </summary>
        public string Name
        {
            get { return fName; }
            set { fName= value; }
        }
        private string fCompany;

        /// <summary>
        /// For serialisation only.
        /// </summary>
        public string Company
        { 
            get { return fCompany; }
            set { fCompany = value; }
        }

        private TimeList fShares;

        /// <summary>
        /// For serialisation only.
        /// </summary>
        public TimeList Shares
        {
            get { return fShares; }
            set { fShares = value; }
        }

        private TimeList fUnitPrice;

        /// <summary>
        /// For serialisation only.
        /// </summary>
        public TimeList UnitPrice
        {
            get { return fUnitPrice; }
            set { fUnitPrice = value; }
        }

        private TimeList fInvestments;

        /// <summary>
        /// For serialisation only.
        /// </summary>
        public TimeList Investments
        {
            get { return fInvestments; }
            set { fInvestments = value; }
        }

        public Security()
        {
            fShares = new TimeList();
            fUnitPrice = new TimeList();
            fInvestments = new TimeList();
        }

        public Security(string name, string company)
        {
            fName = name;
            fCompany = company;
            fShares = new TimeList();
            fUnitPrice = new TimeList();
            fInvestments = new TimeList();
        }

        private Security(string name, string company, TimeList shares, TimeList prices, TimeList investments)
        {
            fName = name;
            fCompany = company;
            fShares = shares;
            fUnitPrice = prices;
            fInvestments = investments;
        }
    }
}
