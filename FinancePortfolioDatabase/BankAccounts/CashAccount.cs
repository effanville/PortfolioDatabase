
namespace FinanceStructures
{
    public partial class CashAccount
    {
        private string fName;

        /// <summary>
        /// For Serialisation only
        /// </summary>
        public string Name
        {
            get { return fName; }
            set { fName = value; }
        }

        private string fCompany;
        /// <summary>
        /// For Serialisation only
        /// </summary>
        public string Company
        {
            get { return fCompany; }
            set { fCompany = value; }
        }

        private TimeList fAmounts;

        /// <summary>
        /// Here for serialisation and viewing in GUI
        /// </summary>
        public TimeList Amounts
        {
            get { return fAmounts; }
            set { fAmounts = value; }
        }

        public CashAccount(string name, string company)
        {
            fName =name;
            fCompany =company;
            fAmounts = new TimeList();
        }

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
    }
}
