using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using System;
using System.Collections.Generic;

namespace CurrencyHelperFunctions
{
    /// <summary>
    /// Helper class to edit the sectors held in the global database.
    /// </summary>
    public static class CurrencyEditor
    {
        /// <summary>
        /// Tries to add a sector to the underlying global database
        /// </summary>
        public static bool TryAddCurrency(string name, string url, ErrorReports reports)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    reports.AddError($"Sector with name {name} already exists.");
                    return false;
                }
            }
            Currency newSector = new Currency(name, url);
            reports.AddReport($"Added sector with name {name}.");
            GlobalData.Finances.Currencies.Add(newSector);
            return true;
        }

        /// <summary>
        /// Returns a sector from the database with specified name.
        /// </summary>
        public static bool TryGetCurrency(string name, out Currency Desired)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    Desired = sector.Copy();
                }
            }
            Desired = null;
            return false;
        }

        public static bool TryGetCurrencyData(string name, out List<AccountDayDataView> data)
        {
            data = new List<AccountDayDataView>();
            foreach (Currency sec in GlobalData.Finances.Currencies)
            {
                if (sec.GetName() == name)
                {
                    data = sec.GetDataForDisplay();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to add data to the sector. Fails if data already exists
        /// </summary>
        public static bool TryAddDataToCurrency(string name, DateTime date, double value)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    return sector.TryAddData(date, value);
                }
            }

            return false;
        }

        public static bool TryEditCurrency(string name, DateTime oldDate, DateTime date, double value, ErrorReports reports)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    return sector.TryEditData(oldDate, date, value, reports);
                }
            }

            return false;
        }

        public static bool TryDeleteCurrencyData(string name, DateTime date, double value, ErrorReports reports)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    return sector.TryDeleteData(date, value, reports);
                }
            }

            return false;
        }

        public static bool TryEditCurrencyName(string oldName, string newName, string url, ErrorReports reports)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (sector.GetName() == oldName)
                {
                    sector.TryEditNameUrl(newName, url);
                    reports.AddReport($"Renamed sector {oldName} with new name {newName}.");
                    return true;
                }
            }

            reports.AddError($"Could not rename sector {oldName} with new name {newName}.");
            return false;
        }

        /// <summary>
        /// Returns true if sector with given name exists.
        /// </summary>
        public static bool DoesCurrencyExist(string name)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes sector if sector exists. Does nothing otherwise.
        /// </summary>
        public static bool TryDeleteCurrency(string name)
        {
            foreach (var sector in GlobalData.Finances.Currencies)
            {
                if (name == sector.GetName())
                {
                    GlobalData.Finances.Currencies.Remove(sector);
                    return true;
                }
            }

            return false;
        }
    }
}
