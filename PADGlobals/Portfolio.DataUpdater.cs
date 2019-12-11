using GlobalHeldData;
using FinancialStructures.FinanceStructures;
using GUIAccessorFunctions;
using FinancialStructures.ReportingStructures;
using System.Threading.Tasks;

namespace PADGlobals
{
    public static class DataUpdater
    {
        public async static Task Downloader(ErrorReports reports)
        {
            await Download.DownloadPortfolioLatest(GlobalData.Finances, reports).ConfigureAwait(false);
            await Download.DownloadBenchMarksLatest(GlobalData.BenchMarks, reports).ConfigureAwait(false);
        }

        public async static Task DownloadSecurity(string company, string name, ErrorReports reports)
        {
            var sec = DatabaseAccessor.GetSecurityFromName(name, company);
            await Download.DownloadSecurityLatest(sec, reports).ConfigureAwait(false);
        }
    }
}
