using System.Collections.Generic;
using System.Linq;

namespace FinancialStructures.ReportingStructures
{
    /// <summary>
    /// Structure of reports to tell the user what is happening
    /// </summary>
    public class ErrorReports
    {
        public ErrorReports()
        {
            fReports = new List<ErrorReport>();
        }
        public int Count()
        {
            return fReports.Count();
        }

        public bool Any()
        {
            return fReports.Any();
        }
        /// <summary>
        /// Instantiates the 
        /// </summary>
        public bool Configure()
        {
            fReports = new List<ErrorReport>();
            return true;
        }

        private List<ErrorReport> fReports;

        public void AddReports(ErrorReports reports)
        {
            fReports.AddRange(reports.GetReports());
            fReports.Sort();
        }

        /// <summary>
        /// Adds a report of any type to the existing list 
        /// </summary>
        public void AddGeneralReport(ReportType type, string newReport)
        {
            fReports.Add(new ErrorReport(type, newReport));
        }

        /// <summary>
        /// Adds a report to the existing list 
        /// </summary>
        public void AddReport(string newReport)
        {
            AddGeneralReport(ReportType.Report, newReport);
        }

        /// <summary>
        /// Adds an Error report to the existing list 
        /// </summary>
        public void AddError(string newReport)
        {
            AddGeneralReport(ReportType.Error, newReport);
        }

        /// <summary>
        /// Adds a Warning report to the existing list 
        /// </summary>
        public void AddWarning(string newReport)
        {
            AddGeneralReport(ReportType.Warning, newReport);
        }

        /// <summary>
        /// Function to obtain 
        /// </summary>
        /// <returns>Currently held reports</returns>
        public List<ErrorReport> GetReports()
        {
            return fReports;
        }

        /// <summary>
        /// Removes element at index <param name="i"/>
        /// </summary>
        public void RemoveReport(int i)
        {
            if (i >= 0 && i < fReports.Count())
            {
                fReports.RemoveAt(i);
            }
        }

        public void Clear()
        {
            fReports.Clear();
        }
    }
}
