using System;
using System.Collections.Generic;
using GUIFinanceStructures;
using DataStructures;

namespace FinanceStructures
{
    public partial class Security
    {
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
        /// Returns true if shares or unit prices have an item
        /// </summary>
        internal bool Any()
        {
            if (fUnitPrice.Any() || fShares.Any())
            {
                return true;
            }

            return false;
        }

        internal Security Copy()
        {
            return new Security(fName, fCompany, fShares, fUnitPrice, fInvestments);
        }

        /// <summary>
        /// returns the name of the security.
        /// </summary>
        internal string GetName()
        {
            return fName;
        }

        /// <summary>
        /// returns the company field of the security
        /// </summary>
        internal string GetCompany()
        {
            return fCompany;
        }

        internal List<BasicDayDataView> GetDataForDisplay()
        {
            var output = new List<BasicDayDataView>();
            foreach (var datevalue in fUnitPrice.GetValuesBetween(fUnitPrice.GetFirstDate(),fUnitPrice.GetLatestDate()))
            {
                fUnitPrice.TryGetValue(datevalue.Day, out double UnitPrice);
                fShares.TryGetValue(datevalue.Day, out double shares);
                fInvestments.TryGetValue(datevalue.Day, out double invest);
                var thisday = new BasicDayDataView(datevalue.Day, UnitPrice, shares, invest);
                output.Add(thisday);
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
                if (DoesDateSharesDataExist(date, out int index)  || DoesDateUnitPriceDataExist(date, out int index2))
                {
                    return false;
                }

                return fShares.TryAddValue(date, shares) & fUnitPrice.TryAddValue(date, unitPrice);
            }

            // here we dont care about shares or investments
            if (shares == 0)
            {
                if (DoesDateUnitPriceDataExist(date, out int index3))
                {
                    return false;
                }

                return fUnitPrice.TryAddValue(date, unitPrice) ;
            }

            if (DoesDateSharesDataExist(date, out int index4) || DoesDateInvestmentDataExist(date, out int index5) || DoesDateUnitPriceDataExist(date, out int index6))
            {
                return false;
            }

            return fShares.TryAddValue(date, shares) & fUnitPrice.TryAddValue(date, unitPrice) & fInvestments.TryAddValue(date, investment);
        }

        internal bool TryAddInvestment(DateTime date, double value)
        {
            if (!DoesDateInvestmentDataExist(date, out int index2))
            { }
            return true;
        }

        /// <summary>
        /// Try to edit data. If any dont have any relevant values, then do not edit
        /// If do have relevant values, then edit that value
        /// If value doesnt exist, then add
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
            return editShares & editUnitPrice;
        }

        internal bool TryEditSharesData(DateTime date, double shares)
        {
            return fShares.TryEditData(date, shares);
        }

        internal bool TryEditUnitPriceData(DateTime date, double investment)
        {
            return fUnitPrice.TryEditData(date, investment);
        }

        internal bool TryEditInvestmentData(DateTime date, double unitPrice)
        {
            return fInvestments.TryEditData(date, unitPrice);
        }

        /// <summary>
        /// Edits name and company data of security.
        /// </summary>
        internal bool TryEditNameCompany(string name, string company)
        {
            if (name != fName)
            {
                fName = name;
            }
            if (company != fCompany)
            {
                fCompany = company;
            }

            return true;
        }

        /// <summary>
        /// Trys to get data on specific date. Only returns true if all data present.
        /// </summary>
        internal bool TryGetData(DateTime date, out double price, out double units, out double investment)
        {
            price = 0;
            units = 0;
            investment = 0;

            return fUnitPrice.TryGetValue(date, out price) & fShares.TryGetValue(date, out units) & fInvestments.TryGetValue(date, out investment);
        }

        /// <summary>
        /// Trys to get latest data earlier than date requested. Only returns true if all data present.
        /// </summary>
        internal bool TryGetEarlierData(DateTime date, out DailyValuation price, out DailyValuation units, out DailyValuation investment)
        {
            price = null;
            units = null;
            investment = null;

            return fUnitPrice.TryGetNearestEarlierValue(date, out price) & fShares.TryGetNearestEarlierValue(date, out units) & fInvestments.TryGetNearestEarlierValue(date, out investment);
        }

        internal bool TryDeleteData(DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            bool units = false;
            bool invs = false;
            bool sharetrue = false;
            if (shares > 0  )
            {
                sharetrue = fShares.TryDeleteValue(date);
            }
            if (unitPrice > 0)
            {
                units = fUnitPrice.TryDeleteValue(date);
            }
            if (Investment > 0)
            {
                invs= fInvestments.TryDeleteValue(date); 
            }

            return units & invs & sharetrue;
        }
    }
}
