using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;

namespace FinancialStructures.FinanceStructures
{
    public partial class Portfolio
    {
        public List<string> GetSecurityNames()
        {
            var names = new List<string>();
            foreach (var security in Funds)
            {
                names.Add(security.GetName());
            }

            return names;
        }

        /// <summary>
        /// Return alphabetically ordered list of all companies without repetition.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSecuritiesCompanyNames()
        {
            var companies = new List<string>();
            foreach (var security in Funds)
            {
                if (companies.IndexOf(security.GetCompany()) == -1)
                {
                    companies.Add(security.GetCompany());
                }
            }
            companies.Sort();

            return companies;
        }

        /// <summary>
        /// Return alphabetically ordered list of all companies without repetition.
        /// </summary>
        /// <returns></returns>
        public List<string> GetSecuritiesSectors()
        {
            var companies = new List<string>();
            foreach (var security in Funds)
            {
                var sectors = security.GetSectors();
                foreach (var sector in sectors)
                {
                    if (companies.IndexOf(sector) == -1)
                    {
                        companies.Add(sector);
                    }
                }
            }
            companies.Sort();

            return companies;
        }

        public List<NameCompDate> GetSecurityNamesAndCompanies()
        {
            var namesAndCompanies = new List<NameCompDate>();

            foreach (var security in Funds)
            {
                DateTime date = DateTime.MinValue;
                if (security.Any())
                {
                    date = security.LatestValue().Day;
                }

                namesAndCompanies.Add(new NameCompDate(security.GetName(), security.GetCompany(), security.GetCurrency(), security.GetUrl(), security.GetSectors(), date, false));
            }

            return namesAndCompanies;
        }

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
        /// This should only be called in combination with GetPortfolio() to avoid accidentally overwriting data.
        /// </summary>
        public List<Security> GetSecurities()
        {
            var listOfFunds = new List<Security>();
            foreach (Security sec in Funds)
            {
                listOfFunds.Add(sec.Copy());
            }
            return listOfFunds;
        }

        public List<CashAccount> GetBankAccounts()
        {
            return BankAccounts;
        }

        public List<Currency> GetCurrencies()
        {
            return Currencies;
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

        public bool TryGetSecurityData(string name, string company, out List<BasicDayDataView> data)
        {
            data = new List<BasicDayDataView>();
            foreach (Security sec in Funds)
            {
                if (sec.GetName() == name && sec.GetCompany() == company)
                {
                    data = sec.GetDataForDisplay();
                    return true;
                }
            }

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

        public bool TryAddSecurityFromName(string name, string company, string currency, string url, List<string> sectors, ErrorReports reports)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(company))
            {
                reports.AddGeneralReport(ReportType.Error, $"Company `{company}' or name `{name}' is not suitable.");
                return false;
            }

            if (DoesSecurityExistFromName(name, company))
            {
                reports.AddGeneralReport(ReportType.Error, $"Security `{company}'-`{name}' already exists.");
                return false;
            }

            var NewFund = new Security(name, company, currency, url);
            foreach (var sector in sectors)
            {
                NewFund.TryAddSector(sector);
            }
            Funds.Add(NewFund);
            reports.AddGeneralReport(ReportType.Report, $"Security `{company}'-`{name}' added to database.");
            return true;
        }

        public bool TryRemoveSecurity(string name, string company, ErrorReports reports)
        {
            foreach (Security sec in Funds)
            {
                if (sec.GetCompany() == company && sec.GetName() == name)
                {
                    Funds.Remove(sec);
                    reports.AddGeneralReport(ReportType.Report, $"Security `{company}'-`{name}' removed from the database.");
                    return true;
                }
            }
            reports.AddGeneralReport(ReportType.Error, $"Security `{company}'-`{name}' could not be found in the database.");
            return false;
        }

        public bool TryAddDataToSecurity(ErrorReports reports, string name, string company, DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < Funds.Count; fundIndex++)
            {
                if (Funds[fundIndex].GetCompany() == company && Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return Funds[fundIndex].TryAddData(reports, date, unitPrice, shares, Investment);
                }
            }
            reports.AddError($"Security `{company}'-`{name}' could not be found in the database.");
            return false;
        }

        public bool TryEditSecurity(ErrorReports reports, string name, string company, DateTime oldDate, DateTime newDate, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < Funds.Count; fundIndex++)
            {
                if (Funds[fundIndex].GetCompany() == company && Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return Funds[fundIndex].TryEditData(reports, oldDate, newDate, shares, unitPrice, Investment);
                }
            }

            return false;
        }

        public bool TryEditSecurityNameCompany(string name, string company, string newName, string newCompany, string currency, string url, List<string> sectors, ErrorReports reports)
        {
            for (int fundIndex = 0; fundIndex < Funds.Count; fundIndex++)
            {
                if (Funds[fundIndex].GetCompany() == company && Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return Funds[fundIndex].TryEditNameCompany(newName, newCompany, currency, url, sectors, reports);
                }
            }

            return false;
        }

        public bool TryRemoveSecurityData(ErrorReports reports, string name, string company, DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < Funds.Count; fundIndex++)
            {
                if (Funds[fundIndex].GetCompany() == company && Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return Funds[fundIndex].TryDeleteData(reports, date, shares, unitPrice, Investment);
                }
            }

            return false;
        }
    }
}
