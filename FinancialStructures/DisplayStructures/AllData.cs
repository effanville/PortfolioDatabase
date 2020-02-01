using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using System.Collections.Generic;

namespace SavingClasses
{
    public class AllData
    {
        public Portfolio MyFunds
        { get; set; }

        public List<Sector> myBenchMarks
        { get; set; }

        public AllData()
        { }

        public AllData(Portfolio portfo, List<Sector> sectors)
        {
            MyFunds = portfo;
            myBenchMarks = sectors;
        }
    }
}
