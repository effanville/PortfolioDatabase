using System;
using System.Collections.Generic;
using GUIFinanceStructures;
using DataStructures;
using ReportingStructures;

namespace FinanceStructures
{
    /// <summary>
    /// Contains data editing of a security class.
    /// </summary>
    public partial class Security
    {
        /// <summary>
        /// Compares another security and determines if has same name and company.
        /// </summary>
        internal bool IsEqualTo(Security otherSecurity)
        {
            if (otherSecurity.GetName() != fName)
            {
                return false;
            }

            if (otherSecurity.GetCompany() != fCompany)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if shares and unit prices have an item or are not null.
        /// </summary>
        internal bool Any()
        {
            if (fUnitPrice.Any() && fShares.Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Makes a copy of the security.
        /// </summary>
        internal Security Copy()
        {
            return new Security(fName, fCompany, fShares, fUnitPrice, fInvestments);
        }

        /// <summary>
        /// Returns the name of the security.
        /// </summary>
        internal string GetName()
        {
            return fName;
        }

        /// <summary>
        /// Returns the company field of the security
        /// </summary>
        internal string GetCompany()
        {
            return fCompany;
        }

        /// <summary>
        /// Produces a list of data for visual display purposes.
        /// </summary>
        internal List<BasicDayDataView> GetDataForDisplay()
        {
            var output = new List<BasicDayDataView>();
            if (fUnitPrice.Any())
            {
                foreach (var datevalue in fUnitPrice.GetValuesBetween(fUnitPrice.GetFirstDate(), fUnitPrice.GetLatestDate()))
                {
                    fUnitPrice.TryGetValue(datevalue.Day, out double UnitPrice);
                    fShares.TryGetValue(datevalue.Day, out double shares);
                    fInvestments.TryGetValue(datevalue.Day, out double invest);
                    var thisday = new BasicDayDataView(datevalue.Day, UnitPrice, shares, invest);
                    output.Add(thisday);
                }
            }

            return output;
        }

        /// <summary>
        /// Checks if SharePrice data for the date specified exists. if so outputs index value
        /// </summary>
        internal bool DoesDateSharesDataExist(DateTime date, out int index)
        {
            return fShares.ValueExists(date, out index);
        }

        /// <summary>
        /// Checks if UnitPrice data for the date specified exists. if so outputs index value
        /// </summary>
        internal bool DoesDateUnitPriceDataExist(DateTime date, out int index)
        {
            return fUnitPrice.ValueExists(date, out index);
        }

        /// <summary>
        /// Checks if UnitPrice data for the date specified exists. if so outputs index value
        /// </summary>
        internal bool DoesDateInvestmentDataExist(DateTime date, out int index)
        {
            return fInvestments.ValueExists(date, out index);
        }

        /// <summary>
        /// Attempts to add data for the date specified.
        /// If cannot add any value that one wants to, then doesn't add all the values chosen.
        /// </summary>
        internal bool TryAddData(DateTime date, double unitPrice, double shares = 0, double investment = 0)
        {
            // here we don't care about investments
            if (investment == 0)
            {
                if (DoesDateSharesDataExist(date, out int index)  || DoesDateUnitPriceDataExist(date, out int _))
                {
                    ErrorReports.AddGeneralReport(ReportType.Error, $"Security `{fCompany}'-`{fName}' already has NumShares or UnitPrice data on {date.ToShortDateString()}.");
                    return false;
                }

                return fShares.TryAddValue(date, shares) & fUnitPrice.TryAddValue(date, unitPrice) && ComputeInvestments();
            }

            // here we dont care about shares or investments
            if (shares == 0)
            {
                if (DoesDateUnitPriceDataExist(date, out int _))
                {
                    ErrorReports.AddGeneralReport(ReportType.Error, $"Security `{fCompany}'-`{fName}' already has UnitPrice data on {date.ToShortDateString()}.");
                    return false;
                }

                return fUnitPrice.TryAddValue(date, unitPrice) && ComputeInvestments();
            }

            if (DoesDateSharesDataExist(date, out int _) || DoesDateInvestmentDataExist(date, out int _) || DoesDateUnitPriceDataExist(date, out int _))
            {
                ErrorReports.AddGeneralReport(ReportType.Error, $"Security `{fCompany}'-`{fName}' already has NumShares or UnitPrice or Investment data on {date.ToShortDateString()}.");
                return false;
            }

            return fShares.TryAddValue(date, shares) & fUnitPrice.TryAddValue(date, unitPrice) & fInvestments.TryAddValue(date, investment) && ComputeInvestments();
        }


        /// <summary>
        /// Try to edit data. If any dont have any relevant values, then do not edit
        /// If do have relevant values, then edit that value
        /// If investment value doesnt exist, then add that value.
        /// </summary>
        internal bool TryEditData(DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            bool editShares = false;
            bool editUnitPrice = false;
            if (DoesDateSharesDataExist(date, out int index))
            {
                editShares = fShares.TryEditData(date, shares);
            }

            if (DoesDateUnitPriceDataExist(date, out int index3))
            {
                editUnitPrice = fUnitPrice.TryEditData(date, unitPrice);
            }
            
            fInvestments.TryEditDataOtherwiseAdd(date, Investment);

            return editShares & editUnitPrice && ComputeInvestments();
        }

        /// <summary>
        /// Edits shares data only.
        /// </summary>
        internal bool TryEditSharesData(DateTime date, double shares)
        {
            return fShares.TryEditData(date, shares) && ComputeInvestments();
        }

        /// <summary>
        /// Edits unit price data only.
        /// </summary>
        internal bool TryEditUnitPriceData(DateTime date, double investment)
        {
            return fUnitPrice.TryEditData(date, investment) && ComputeInvestments();
        }

        /// <summary>
        /// Edits investment data only.
        /// </summary>
        internal bool TryEditInvestmentData(DateTime date, double unitPrice)
        {
            return fInvestments.TryEditData(date, unitPrice) && ComputeInvestments();
        }

        /// <summary>
        /// Edits name and company data of security.
        /// </summary>
        internal bool TryEditNameCompany(string name, string company)
        {
            if (name != fName)
            {
                ErrorReports.AddGeneralReport(ReportType.Report, $"Security `{fCompany}'-`{fName}' has name `{fName}' edited to `{name}'.");
                fName = name;
            }
            if (company != fCompany)
            {
                ErrorReports.AddGeneralReport(ReportType.Report, $"Security `{fCompany}'-`{fName}' has company `{fCompany}' edited to `{company}'.");
                fCompany = company;
            }

            return true;
        }

        /// <summary>
        /// Trys to get data on specific date. Only returns true if all data present.
        /// </summary>
        internal bool TryGetData(DateTime date, out double price, out double units, out double investment)
        {
            return fUnitPrice.TryGetValue(date, out price) & fShares.TryGetValue(date, out units) & fInvestments.TryGetValue(date, out investment);
        }

        /// <summary>
        /// Trys to get latest data earlier than date requested. Only returns true if all data present.
        /// </summary>
        internal bool TryGetEarlierData(DateTime date, out DailyValuation price, out DailyValuation units, out DailyValuation investment)
        {
            return fUnitPrice.TryGetNearestEarlierValue(date, out price) & fShares.TryGetNearestEarlierValue(date, out units) & fInvestments.TryGetNearestEarlierValue(date, out investment);
        }

        /// <summary>
        /// Tries to delete the data. If it can, it deletes all data specified, then returns true only if all data has been successfully deleted.
        /// </summary>
        internal bool TryDeleteData(DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            bool units = false;
            bool sharetrue = false;
            if (shares > 0  )
            {
                sharetrue = fShares.TryDeleteValue(date);
            }
            if (unitPrice > 0)
            {
                units = fUnitPrice.TryDeleteValue(date);
            }

            return units & sharetrue & fInvestments.TryDeleteValue(date) && ComputeInvestments();
        }

        /// <summary>
        /// Upon a new/edit investment, one needs to recompute the values of the investments
        /// One should not change Inv = 0 or Inv > 0  to ensure that dividend reivestments are not accidentally included in a new investment.
        /// This though causes a problem if a value is deleted.
        /// </summary>
        /// <remarks>
        /// This should be called throughout, whenever one updates the data stored in the Security.
        /// </remarks>
        private bool ComputeInvestments()
        {
                // return true;
            for (int index = 0; index < fInvestments.Count(); index++)
            {
                var investmentValue = fInvestments[index];
                if (investmentValue.Value > 0)
                {
                    DailyValuation sharesCurrentValue = fShares.GetNearestEarlierValue(investmentValue.Day);
                    DailyValuation sharesPreviousValue = fShares.GetLastEarlierValue(investmentValue.Day) ?? new DailyValuation(DateTime.Today, 0);
                    if (sharesCurrentValue != null)
                    {
                        fInvestments.TryEditData(investmentValue.Day, (sharesCurrentValue.Value - sharesPreviousValue.Value) * fUnitPrice.GetNearestEarlierValue(investmentValue.Day).Value);
                    }
                }
            }

            return true;
        }
    }
}
