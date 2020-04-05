﻿using FinancialStructures.DataStructures;
using FinancialStructures.FinanceInterfaces;
using FinancialStructures.NamingStructures;
using FinancialStructures.ReportLogging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialStructures.FinanceStructures
{
    /// <summary>
    /// General edit functions for a sector.
    /// </summary>
    public partial class SingleValueDataList
    {
        /// <summary>
        /// Compares another security and determines if has same name and company.
        /// </summary>
        public bool IsEqualTo(ISingleValueDataList otherAccount)
        {
            return Names.IsEqualTo(otherAccount.Names);
        }

        public int Count()
        {
            return fValues.Count();
        }

        public List<DayValue_ChangeLogged> GetDataForDisplay()
        {
            var output = new List<DayValue_ChangeLogged>();
            if (fValues.Any())
            {
                foreach (var datevalue in fValues.GetValuesBetween(fValues.FirstDate(), fValues.LatestDate()))
                {
                    fValues.TryGetValue(datevalue.Day, out double UnitPrice);
                    var thisday = new DayValue_ChangeLogged(datevalue.Day, UnitPrice, false);
                    output.Add(thisday);
                }
            }

            return output;
        }

        /// <summary>
        /// Edits the associated nameData to the account.
        /// </summary>
        /// <param name="newNames"></param>
        /// <returns></returns>
        public virtual bool EditNameData(NameData newNames)
        {
            Names = newNames;
            return true;
        }

        /// <summary>
        /// Adds <param name="value"/> to amounts on <param name="date"/> if data doesnt exist.
        /// </summary>
        public bool TryAddData(DateTime date, double value, LogReporter reportLogger)
        {
            if (fValues.ValueExists(date, out _))
            {
                reportLogger.Log("Error", "AddingData", "Data already exists.");
                return false;
            }

            return fValues.TryAddValue(date, value, reportLogger);
        }

        /// <summary>
        /// Edits value if value exists. Does nothing if it doesn't exist.
        /// </summary>
        public bool TryEditData(DateTime oldDate, DateTime date, double value, LogReporter reportLogger)
        {
            return fValues.TryEditData(oldDate, date, value, reportLogger);
        }

        /// <summary>
        /// Removes data on <paramref name="date"/> if it exists.
        /// </summary>
        public bool TryDeleteData(DateTime date, LogReporter reportLogger)
        {
            return fValues.TryDeleteValue(date, reportLogger);
        }

        /// <summary>
        /// Removes a sector associated to this OldCashAccount.
        /// </summary>
        /// <param name="sectorName"></param>
        /// <returns></returns>
        public bool TryRemoveSector(string sectorName)
        {
            if (IsSectorLinked(sectorName))
            {
                Names.Sectors.Remove(sectorName);
                return true;
            }

            return false;
        }

        internal bool IsSectorLinked(string sectorName)
        {
            if (Names.Sectors != null && Names.Sectors.Any())
            {
                foreach (var name in Names.Sectors)
                {
                    if (name == sectorName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}