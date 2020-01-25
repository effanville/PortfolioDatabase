using FinancialStructures.Database;
using FinancialStructures.ReportingStructures;
using GlobalHeldData;
using GUIAccessorFunctions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PADGlobals
{
    public static class DataUpdater
    {
        public async static Task Downloader(Action<ErrorReports> updateReports, ErrorReports reports)
        {
            await Download.DownloadPortfolioLatest(GlobalData.Finances, updateReports, reports).ConfigureAwait(false);
            await Download.DownloadBenchMarksLatest(GlobalData.BenchMarks, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadSecurity(string company, string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = GlobalData.Finances.GetSecurityFromName(name, company);
            await Download.DownloadSecurityLatest(sec, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadCurrency( string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = GlobalData.Finances.GetCurrencyFromName(name);
            await Download.DownloadCurrencyLatest(sec, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadBankAccount(string company, string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = GlobalData.Finances.GetBankAccountFromName(name, company);
            await Download.DownloadBankAccountLatest(sec, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadSector(string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = DatabaseAccessor.GetSectorFromName(name);
            await Download.DownloadSectorLatest(sec, updateReports, reports).ConfigureAwait(false);
        }
    }
}
