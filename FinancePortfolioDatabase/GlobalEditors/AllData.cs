using FinanceStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingDummyClasses
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
