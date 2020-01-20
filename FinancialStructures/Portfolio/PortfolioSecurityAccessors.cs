using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;

namespace FinancialStructures.Database
{
    public static partial class PortfolioSecurity
    {
        public static List<string> GetSecurityNames(this Portfolio portfolio)
        {
            var names = new List<string>();
            foreach (var security in portfolio.Funds)
            {
                names.Add(security.GetName());
            }

            return names;
        }

        /// <summary>
        /// Return alphabetically ordered list of all companies without repetition.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSecuritiesCompanyNames(this Portfolio portfolio)
        {
            var companies = new List<string>();
            foreach (var security in portfolio.Funds)
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
        public static List<string> GetSecuritiesSectors(this Portfolio portfolio)
        {
            var companies = new List<string>();
            foreach (var security in portfolio.Funds)
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

        public static List<NameCompDate> GetSecurityNamesAndCompanies(this Portfolio portfolio)
        {
            var namesAndCompanies = new List<NameCompDate>();

            foreach (var security in portfolio.Funds)
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

        public static bool DoesSecurityExist(this Portfolio portfolio, Security fund)
        {
            foreach (Security sec in portfolio.Funds)
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
        public static List<Security> GetSecurities(this Portfolio portfolio)
        {
            var listOfFunds = new List<Security>();
            foreach (Security sec in portfolio.Funds)
            {
                listOfFunds.Add(sec.Copy());
            }
            return listOfFunds;
        }

        /// <summary>
        /// Outputs a copy of the security if it exists.
        /// </summary>
        public static bool TryGetSecurity(this Portfolio portfolio, string name, string company, out Security desired)
        {
            foreach (Security sec in portfolio.Funds)
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

        public static bool TryGetSecurityData(this Portfolio portfolio, string name, string company, out List<DayDataView> data)
        {
            data = new List<DayDataView>();
            foreach (Security sec in portfolio.Funds)
            {
                if (sec.GetName() == name && sec.GetCompany() == company)
                {
                    data = sec.GetDataForDisplay();
                    return true;
                }
            }

            return false;
        }

        public static bool DoesSecurityExistFromName(this Portfolio portfolio, string name, string company)
        {
            foreach (Security sec in portfolio.Funds)
            {
                if (sec.GetCompany() == company && sec.GetName() == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryAddSecurityFromName(this Portfolio portfolio, string name, string company, string currency, string url, List<string> sectors, ErrorReports reports)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(company))
            {
                reports.AddGeneralReport(ReportType.Error, $"Company `{company}' or name `{name}' is not suitable.");
                return false;
            }

            if (portfolio.DoesSecurityExistFromName(name, company))
            {
                reports.AddGeneralReport(ReportType.Error, $"Security `{company}'-`{name}' already exists.");
                return false;
            }

            var NewFund = new Security(name, company, currency, url);
            foreach (var sector in sectors)
            {
                NewFund.TryAddSector(sector);
            }
            portfolio.Funds.Add(NewFund);
            reports.AddGeneralReport(ReportType.Report, $"Security `{company}'-`{name}' added to database.");
            return true;
        }

        public static bool TryRemoveSecurity(this Portfolio portfolio, string name, string company, ErrorReports reports)
        {
            foreach (Security sec in portfolio.Funds)
            {
                if (sec.GetCompany() == company && sec.GetName() == name)
                {
                    portfolio.Funds.Remove(sec);
                    reports.AddGeneralReport(ReportType.Report, $"Security `{company}'-`{name}' removed from the database.");
                    return true;
                }
            }
            reports.AddGeneralReport(ReportType.Error, $"Security `{company}'-`{name}' could not be found in the database.");
            return false;
        }

        public static bool TryAddDataToSecurity(this Portfolio portfolio, ErrorReports reports, string name, string company, DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < portfolio.Funds.Count; fundIndex++)
            {
                if (portfolio.Funds[fundIndex].GetCompany() == company && portfolio.Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return portfolio.Funds[fundIndex].TryAddData(reports, date, unitPrice, shares, Investment);
                }
            }
            reports.AddError($"Security `{company}'-`{name}' could not be found in the database.");
            return false;
        }

        /// <summary>
        /// Tries to add a security to the underlying global database
        /// </summary>
        public static bool TryAddSecurity(this Portfolio portfolio, string name, string company, string currency, string url, string sectors, ErrorReports reports)
        {
            List<string> sectorList = new List<string>();
            if (!string.IsNullOrEmpty(sectors))
            {
                var sectorsSplit = sectors.Split(',');

                sectorList.AddRange(sectorsSplit);
                for (int i = 0; i < sectorList.Count; i++)
                {
                    sectorList[i] = sectorList[i].Trim(' ');
                }
            }

            return portfolio.TryAddSecurityFromName(name, company, currency, url, sectorList, reports);
        }

        /// <summary>
        /// Renames the security if this exists.
        /// </summary>
        public static bool TryEditSecurityName(this Portfolio portfolio, string name, string company, string newName, string newCompany, string currency, string url, string sectors, ErrorReports reports)
        {
            List<string> sectorList = new List<string>();
            if (!string.IsNullOrEmpty(sectors))
            {
                var sectorsSplit = sectors.Split(',');
                sectorList.AddRange(sectorsSplit);
                for (int i = 0; i < sectorList.Count; i++)
                {
                    sectorList[i] = sectorList[i].Trim(' ');
                }
            }
            return portfolio.TryEditSecurityNameCompany(name, company, newName, newCompany, currency, url, sectorList, reports);
        }


        public static bool TryEditSecurity(this Portfolio portfolio, ErrorReports reports, string name, string company, DateTime oldDate, DateTime newDate, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < portfolio.Funds.Count; fundIndex++)
            {
                if (portfolio.Funds[fundIndex].GetCompany() == company && portfolio.Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return portfolio.Funds[fundIndex].TryEditData(reports, oldDate, newDate, shares, unitPrice, Investment);
                }
            }

            return false;
        }

        public static bool TryEditSecurityNameCompany(this Portfolio portfolio, string name, string company, string newName, string newCompany, string currency, string url, List<string> sectors, ErrorReports reports)
        {
            for (int fundIndex = 0; fundIndex < portfolio.Funds.Count; fundIndex++)
            {
                if (portfolio.Funds[fundIndex].GetCompany() == company && portfolio.Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return portfolio.Funds[fundIndex].TryEditNameCompany(newName, newCompany, currency, url, sectors, reports);
                }
            }

            return false;
        }

        public static bool TryRemoveSecurityData(this Portfolio portfolio, ErrorReports reports, string name, string company, DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            for (int fundIndex = 0; fundIndex < portfolio.Funds.Count; fundIndex++)
            {
                if (portfolio.Funds[fundIndex].GetCompany() == company && portfolio.Funds[fundIndex].GetName() == name)
                {
                    // now edit data
                    return portfolio.Funds[fundIndex].TryDeleteData(reports, date, shares, unitPrice, Investment);
                }
            }

            return false;
        }
    }
}
