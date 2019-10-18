using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePortfolioDatabase
{
    public partial class Security
    {
        public bool IsEqualTo(Security otherSecurity)
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

        public Security Copy()
        {
            return new Security(fName, fCompany, fShares, fUnitPrice, fInvestments);
        }

        /// <summary>
        /// returns the name of the security.
        /// </summary>
        public string GetName()
        {
            return fName;
        }

        /// <summary>
        /// returns the company field of the security
        /// </summary>
        public string GetCompany()
        {
            return fCompany;
        }

        /// <summary>
        /// Checks if SharePrice data for the date specified exists. if so outputs index value
        /// </summary>
        public bool DoesDateSharesDataExist(DateTime date, out int index)
        {
            return fShares.ValueExists(date, out index);
        }

        /// <summary>
        /// Checks if UnitPrice data for the date specified exists. if so outputs index value
        /// </summary>
        public bool DoesDateUnitPriceDataExist(DateTime date, out int index)
        {
            return fUnitPrice.ValueExists(date, out index);
        }

        /// <summary>
        /// Checks if UnitPrice data for the date specified exists. if so outputs index value
        /// </summary>
        public bool DoesDateInvestmentDataExist(DateTime date, out int index)
        {
            return fInvestments.ValueExists(date, out index);
        }

        /// <summary>
        /// Attempts to add data for the date specified
        /// </summary>
        public bool TryAddData(DateTime date, double shares, double unitPrice = 0)
        {
            return true;
        }

        public bool TryAddInvestment(DateTime date, double value)
        {
            return true;
        }

        /// <summary>
        /// Try to edit data. If any dont have the relevant values, then do not edit
        /// </summary>
        /// <param name="date"></param>
        /// <param name="shares"></param>
        /// <param name="unitPrice"></param>
        /// <param name="Investment"></param>
        /// <returns></returns>
        public bool TryEditData(DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            if (!DoesDateSharesDataExist(date, out int index) || !DoesDateInvestmentDataExist(date, out int index2))
            {
                if (Investment == 0)
                {
                    return false;
                }

                if (!DoesDateUnitPriceDataExist(date, out int index3))
                {
                    return false;
                }
            }

            if (Investment != 0)
            {
                return fShares.TryEditData(date, shares) && fUnitPrice.TryEditData(date, unitPrice) && fInvestments.TryEditData(date, Investment);
            }

            return fShares.TryEditData(date, shares) && fUnitPrice.TryEditData(date, unitPrice);
        }

        public bool TryEditSharesData(DateTime date, double shares)
        {
            return fShares.TryEditData(date, shares);
        }

        public bool TryEditUnitPriceData(DateTime date, double investment)
        {
            return fUnitPrice.TryEditData(date, investment);
        }

        public bool TryEditInvestmentData(DateTime date, double unitPrice)
        {
            return fInvestments.TryEditData(date, unitPrice);
        }

        /// <summary>
        /// Edits name and company data of security.
        /// </summary>
        public bool TryEditNameCompany(string name, string company)
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
        /// returns data at index i.
        /// </summary>
        public bool TryGetData(int i, out DateTime date, out double price, out double units, out double investment)
        {
            // note using i here is really ambiguous as have many vectors of datetime.
            date = new DateTime();
            price = 0;
            units = 0;
            investment = 0;

            if (i < 0 || i > fUnitPrice.Count()-1)
            {
                return false;
            }


            return true;
        }

        /// <summary>
        /// Trys to get data on specific date. Only returns true if all data present.
        /// </summary>
        public bool TryGetData(DateTime date, out double price, out double units, out double investment)
        {
            price = 0;
            units = 0;
            investment = 0;

            return fUnitPrice.TryGetValue(date, out price) && fShares.TryGetValue(date, out units) && fInvestments.TryGetValue(date, out investment);
        }
    }
}
