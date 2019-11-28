using System;
using System.Collections.Generic;
using System.Linq;


namespace FinancialStructures.ReportingStructures
{
    /// <summary>
    /// Structure of reports to tell the user what is happening
    /// </summary>
    public static class ErrorReports
    {
        /// <summary>
        /// Instantiates the 
        /// </summary>
        /// <returns></returns>
        public static bool Configure()
        {
            fReports = new List<ErrorReport>();
            return true;
        }

        private static List<ErrorReport> fReports;

        /// <summary>
        /// Adds a report of any type to the existing list 
        /// </summary>
        public static void AddGeneralReport(ReportType type, string newReport)
        {
            fReports.Add(new ErrorReport(type,newReport));
        }

        /// <summary>
        /// Adds a report to the existing list 
        /// </summary>
        public static void AddReport(string newReport)
        {
            AddGeneralReport(ReportType.Report, newReport);
        }

        /// <summary>
        /// Adds an Error report to the existing list 
        /// </summary>
        public static void AddError(string newReport)
        {
            AddGeneralReport(ReportType.Error, newReport);
        }

        /// <summary>
        /// Adds a Warning report to the existing list 
        /// </summary>
        public static void AddWarning(string newReport)
        {
            AddGeneralReport(ReportType.Warning, newReport);
        }

        /// <summary>
        /// Function to obtain 
        /// </summary>
        /// <returns>Currently held reports</returns>
        public static List<ErrorReport> GetReports()
        {
            return fReports;
        }

        /// <summary>
        /// Removes element at index <param name="i"/>
        /// </summary>
        public static void RemoveReport(int i)
        {
            if (i >= 0 && i < fReports.Count())
            {
                fReports.RemoveAt(i);
            }
        }

        public static void Clear()
        {
            fReports.Clear();
        }
    }
}
