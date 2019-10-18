using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public class Sector
    {
        private string fName;
        private TimeList fValues;

        public Sector()
        { }

        public Sector(string name, TimeList values)
        {
            fName = name;
            fValues = values;
        }
    }
}
