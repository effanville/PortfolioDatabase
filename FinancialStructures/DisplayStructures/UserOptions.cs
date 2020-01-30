using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioStatsCreatorHelper
{
    public class UserOptions
    {
        public List<string> SecurityDataToExport { get; set; }

        public List<string> BankAccDataToExport { get; set; }

        public List<string> SectorDataToExport { get; set; }
        public bool DisplayValueFunds { get; set; }
        public bool Spacing { get; set; }

        public bool Colours { get; set; }
    }
}
