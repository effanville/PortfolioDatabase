using System;

namespace FinancePortfolioDatabase
{
    public partial class Portfolio
    {
        public bool DoesSecurityExist(Security fund)
        {
            foreach (Security sec in Funds)
            {
                if (sec.IsEqualTo(fund))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Outputs a copy of the security if it exists.
        /// </summary>
        public bool TryGetSecurity(string name, string company, out Security desired)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetName() == name && sec.GetCompany() == company)
                {
                    desired = sec.Copy();
                    return true;
                }
            }
            desired = null;
            return false;
        }

        public bool DoesSecurityExistFromName(string name, string company)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetCompany() == company && sec.GetName() == name)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryAddSecurity(Security NewFund)
        {
            if (DoesSecurityExist(NewFund))
            {
                return false;
            }

            Funds.Add(NewFund);
            return true;
        }

        public bool TryAddSecurityFromName(string name, string company)
        {
            if (DoesSecurityExistFromName(name, company))
            {
                return false;
            }

            var NewFund = new Security(name, company);
            Funds.Add(NewFund);
            return true;
        }

        public bool TryRemoveSecurity(string name, string company)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetCompany() == company && sec.GetName() == name)
                {
                    Funds.Remove(sec);
                    return true;
                }
            }

            return false;
        }
        
        public bool TryEditSecurity(string name, string company, DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < Funds.Count; fundIndex++)
            {
                if (Funds[fundIndex].GetCompany() == company && Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return Funds[fundIndex].TryEditData(date, shares, unitPrice, Investment);
                }
            }

            return false;
        }

        public bool TryEditSecurityNameCompany(string name, string company, string newName, string newCompany)
        {
            for (int fundIndex = 0; fundIndex < Funds.Count; fundIndex++)
            {
                if (Funds[fundIndex].GetCompany() == company && Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return Funds[fundIndex].TryEditNameCompany(newName, newCompany);
                }
            }

            return false;
        }
    }
}
