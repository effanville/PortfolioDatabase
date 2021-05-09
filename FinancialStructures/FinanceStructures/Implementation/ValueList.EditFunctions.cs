﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinancialStructures.NamingStructures;
using StructureCommon.DataStructures;
using StructureCommon.Reporting;

namespace FinancialStructures.FinanceStructures.Implementation
{
    /// <summary>
    /// General edit functions for a sector.
    /// </summary>
    public partial class ValueList
    {


        /// <inheritdoc/>
        public virtual bool EditNameData(NameData newNames)
        {
            Names = newNames;
            OnDataEdit(this, new EventArgs());
            return true;
        }

        /// <inheritdoc/>
        public virtual bool TryAddOrEditData(DateTime oldDate, DateTime date, double value, IReportLogger reportLogger = null)
        {
            if (Values.ValueExists(oldDate, out _))
            {
                return Values.TryEditData(oldDate, date, value, reportLogger);
            }

            return Values.TryAddValue(date, value, reportLogger);
        }

        /// <summary>
        /// Adds data input already read from a csv file to the
        /// </summary>
        /// <param name="valuationsToRead">A list or array values of the data from the file.</param>
        /// <param name="reportLogger">A logger to record outcomes.</param>
        public virtual List<object> CreateDataFromCsv(List<string[]> valuationsToRead, IReportLogger reportLogger = null)
        {
            List<object> dailyValuations = new List<object>();
            foreach (string[] dayValuation in valuationsToRead)
            {
                if (dayValuation.Length != 2)
                {
                    _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.Loading, "Line in Csv file has incomplete data.");
                    break;
                }

                DailyValuation line = new DailyValuation(DateTime.Parse(dayValuation[0]), double.Parse(dayValuation[1]));
                dailyValuations.Add(line);
            }

            return dailyValuations;
        }

        /// <summary>
        /// Writes the data held in the account to a csv file.
        /// </summary>
        /// <param name="writer">The writer holding the location of where to write.</param>
        /// <param name="reportLogger">A logger to record outcomes.</param>
        public virtual void WriteDataToCsv(TextWriter writer, IReportLogger reportLogger = null)
        {
            foreach (DailyValuation value in GetDataForDisplay())
            {
                writer.WriteLine(value.ToString());
            }
        }

        /// <inheritdoc/>
        public virtual bool TryDeleteData(DateTime date, IReportLogger reportLogger = null)
        {
            return Values.TryDeleteValue(date, reportLogger);
        }

        /// <inheritdoc/>
        public bool TryRemoveSector(string sectorName)
        {
            if (IsSectorLinked(sectorName))
            {
                _ = Names.Sectors.Remove(sectorName);
                OnDataEdit(this, new EventArgs());
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool IsSectorLinked(string sectorName)
        {
            if (Names.Sectors != null && Names.Sectors.Any())
            {
                foreach (string name in Names.Sectors)
                {
                    if (name == sectorName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public int NumberSectors()
        {
            return Names.Sectors.Count;
        }
    }
}