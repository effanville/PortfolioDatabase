using FinanceStructures;
using System;

namespace SectorHelperFunctions
{
    /// <summary>
    /// Helper class to edit the securities held in the global database.
    /// </summary>
    public static class SectorEditHelper
    {
        /// <summary>
        /// Tries to add a sector to the underlying global database
        /// </summary>
        public static bool TryAddSector(string name)
        {
            return true;
            //return GlobalData.BenchMark.TryAddSector(name);
        }

        /// <summary>
        /// Returns a sector from the database with specified name.
        /// </summary>
        public static bool TryGetSector(string name, out Sector Desired)
        {
            Desired = new Sector();
            return true;
            //return GlobalData.Benchmark.TryGetSector(name, out Desired);
        }

        /// <summary>
        /// Attempts to add data to the sector. Fails if data already exists
        /// </summary>
        public static bool TryAddDataToSector(string name, DateTime date, double Value)
        {
            return true;
        }

        public static bool TryEditSector(string name, DateTime date, double value)
        {
            return true;
        }

        /// <summary>
        /// Returns true if sector with given name exists.
        /// </summary>
        public static bool DoesSectorExist(string name)
        {
            return true;
            //return GlobalData.Finances.DoesSectorExistFromName(name);
        }

        /// <summary>
        /// Deletes sector if security exists. Does nothing otherwise.
        /// </summary>
        public static bool TryDeleteSector(string name)
        {
            return true;
            //return GlobalData.BenchMark.TryRemoveSector(name);
        }
    }
}
