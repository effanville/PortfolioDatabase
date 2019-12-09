using GlobalHeldData;
using FinancialStructures.FinanceStructures;
using GUIAccessorFunctions;
using FinancialStructures.ReportingStructures;

namespace PADGlobals
{
    public static class DataUpdater
    {
        public static void Downloader(ErrorReports reports)
        {
            Download.DownloadPortfolioLatest(GlobalData.Finances, reports);
            Download.DownloadBenchMarksLatest(GlobalData.BenchMarks, reports);
        }

        public static void DownloadSecurity(string company, string name, ErrorReports reports)
        {
            var sec = DatabaseAccessor.GetSecurityFromName(name, company);
            Download.DownloadSecurityLatest(sec, reports);
        }
    }
}
