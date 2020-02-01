using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;

namespace SectorHelperFunctions
{
    /// <summary>
    /// Helper class to edit the sectors held in the global database.
    /// </summary>
    public static class SectorEditor
    {
        /// <summary>
        /// Tries to add a sector to the underlying global database
        /// </summary>
        public static bool TryAddSector(List<Sector> sectors, string name, string url, ErrorReports reports)
        {
            foreach (var sector in sectors)
            {
                if (name == sector.GetName())
                {
                    reports.AddError($"Sector with name {name} already exists.");
                    return false;
                }
            }
            Sector newSector = new Sector(name, url);
            reports.AddReport($"Added sector with name {name}.");
            sectors.Add(newSector);
            return true;
        }

        /// <summary>
        /// Returns a sector from the database with specified name.
        /// </summary>
        public static bool TryGetSector(List<Sector> sectors, string name, out Sector Desired)
        {
            foreach (var sector in sectors)
            {
                if (name == sector.GetName())
                {
                    Desired = sector.Copy();
                }
            }
            Desired = null;
            return false;
        }

        public static bool TryGetSectorData(List<Sector> sectors, string name, out List<AccountDayDataView> data)
        {
            data = new List<AccountDayDataView>();
            foreach (Sector sec in sectors)
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
        public static bool TryAddDataToSector(List<Sector> sectors, string name, DateTime date, double value)
        {
            foreach (var sector in sectors)
            {
                if (name == sector.GetName())
                {
                    return sector.TryAddData(date, value);
                }
            }

            return false;
        }

        public static bool TryEditSector(List<Sector> sectors, string name, DateTime oldDate, DateTime date, double value, ErrorReports reports)
        {
            foreach (var sector in sectors)
            {
                if (name == sector.GetName())
                {
                    return sector.TryEditData(oldDate, date, value, reports);
                }
            }

            return false;
        }

        public static bool TryDeleteSectorData(List<Sector> sectors, string name, DateTime date, double value, ErrorReports reports)
        {
            foreach (var sector in sectors)
            {
                if (name == sector.GetName())
                {
                    return sector.TryDeleteData(date, value, reports);
                }
            }

            return false;
        }

        public static bool TryEditSectorName(List<Sector> sectors, string oldName, string newName, string url, ErrorReports reports)
        {
            foreach (var sector in sectors)
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
        public static bool DoesSectorExist(List<Sector> sectors, string name)
        {
            foreach (var sector in sectors)
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
        public static bool TryDeleteSector(List<Sector> sectors, string name)
        {
            foreach (var sector in sectors)
            {
                if (name == sector.GetName())
                {
                    sectors.Remove(sector);
                    return true;
                }
            }

            return false;
        }
    }
}
