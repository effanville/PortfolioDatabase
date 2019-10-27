using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanceStructures
{
    public partial class Portfolio
    {
        public bool DoesCompanyExist(string company)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetCompany() == company)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Security> CompanySecurities(string company)
        {
            var securities = new List<Security>();
            foreach (var sec in Funds)
            {
                if (sec.GetCompany() == company)
                {
                    securities.Add(sec);
                }
            }

            return securities;
        }

        public double CompanyValue(string company, DateTime date)
        {
            var securities = CompanySecurities(company);
            if (securities.Count() == 0)
            {
                return double.NaN;
            }
            double value = 0;
            foreach (var security in securities)
            {
                value += security.GetNearestEarlierValuation(date).Value;
            }

            return value;
        }
    }
}
