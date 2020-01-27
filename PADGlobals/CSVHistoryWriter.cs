using FinancialStructures.Database;
using System;
using System.Linq;
using System.IO;
using FinancialStructures.ReportingStructures;

namespace PortfolioStatsCreatorHelper
{
    public static class CSVHistoryWriter
    {
        public async static void WriteHistoryToCSV(Portfolio portfolio, Action<ErrorReports> reportCallback, string filePath, int daysGap)
        {
            var reports = new ErrorReports();
            try
            {
                StreamWriter historyWriter = new StreamWriter(filePath);
                var historyStatistics = await portfolio.GenerateHistoryStats(daysGap).ConfigureAwait(false);
                if (!historyStatistics.Any())
                {
                    reports.AddError("Not enough history points to export.");
                    return;
                }
                historyWriter.WriteLine(historyStatistics[0].Headers());
                foreach (var statistic in historyStatistics)
                {
                    historyWriter.WriteLine(statistic.ToString());
                }
                historyWriter.Close();
                reports.AddReport($"Successfully exported history to {filePath}.");
            }
            catch (Exception exception)
            {
                reports.AddError(exception.Message);
                
            }
            finally
            {
                reportCallback(reports);
            }
        }
    }
}
