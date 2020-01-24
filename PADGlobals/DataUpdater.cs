using FinancialStructures.Database;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
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
            var sec = GlobalData.Finances.GetSecurityFromName(name, company);
            await Download.DownloadSecurityLatest(sec, reports).ConfigureAwait(false);
        }
    }
}
