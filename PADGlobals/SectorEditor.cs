using FinancialStructures.FinanceStructures;
using GlobalHeldData;
using System;
using System.Collections.Generic;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;

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
        public static bool TryAddSector(string name, string url)
        {
            foreach (var sector in GlobalData.BenchMarks)
            {
                if (name == sector.GetName())
                {
                    ErrorReports.AddError($"Sector with name {name} already exists.");
                    return false;
                }
            }
            Sector newSector = new Sector(name, url);
            ErrorReports.AddReport($"Added sector with name {name}.");
            GlobalData.BenchMarks.Add(newSector);
            return true;
        }

        /// <summary>
        /// Returns a sector from the database with specified name.
        /// </summary>
        public static bool TryGetSector(string name, out Sector Desired)
        {
            foreach (var sector in GlobalData.BenchMarks)
            { 
                if (name == sector.GetName())
                {
                    Desired = sector.Copy();
                } 
            }
            Desired = null;
            return false;
        }

        public static bool TryGetSectorData(string name, out List<AccountDayDataView> data)
        {
            data = new List<AccountDayDataView>();
            foreach (Sector sec in GlobalData.BenchMarks)
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
        public static bool TryAddDataToSector(string name, DateTime date, double value)
        {
            foreach (var sector in GlobalData.BenchMarks)
            {
                if (name == sector.GetName())
                {
                    return sector.TryAddData(date, value);
                }
            }

            return false;
        }

        public static bool TryEditSector(string name, DateTime date, double value)
        {
            foreach (var sector in GlobalData.BenchMarks)
            {
                if (name == sector.GetName())
                {
                    return sector.TryEditData(date, value);
                }
            }

            return false;
        }

        public static bool TryDeleteSectorData(string name, DateTime date, double value)
        {
            foreach (var sector in GlobalData.BenchMarks)
            {
                if (name == sector.GetName())
                {
                    return sector.TryDeleteData(date, value);
                }
            }

            return false;
        }

        public static bool TryEditSectorName(string oldName, string newName, string url)
        {
            foreach (var sector in GlobalData.BenchMarks)
            {
                if (sector.GetName() == oldName)
                {
                    sector.TryEditNameUrl(newName, url);
                    ErrorReports.AddReport($"Renamed sector {oldName} with new name {newName}.");
                    return true;
                }
            }

            ErrorReports.AddError($"Could not rename sector {oldName} with new name {newName}.");
            return false;
        }

        /// <summary>
        /// Returns true if sector with given name exists.
        /// </summary>
        public static bool DoesSectorExist(string name)
        {
            foreach (var sector in GlobalData.BenchMarks)
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
        public static bool TryDeleteSector(string name)
        {
            foreach (var sector in GlobalData.BenchMarks)
            {
                if (name == sector.GetName())
                {
                    GlobalData.BenchMarks.Remove(sector);
                    return true;
                }
            }
            
            return false;
        }
    }
}
