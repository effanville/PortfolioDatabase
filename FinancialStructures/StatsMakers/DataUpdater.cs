using FinancialStructures.Database;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PADGlobals
{
    public static class DataUpdater
    {
        public async static Task Downloader(Portfolio portfolio, List<Sector> sectors, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            await Download.DownloadPortfolioLatest(portfolio, updateReports, reports).ConfigureAwait(false);
            await Download.DownloadBenchMarksLatest(sectors, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadSecurity(Portfolio portfolio, string company, string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = portfolio.GetSecurityFromName(name, company);
            await Download.DownloadSecurityLatest(sec, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadCurrency(Portfolio portfolio, NameData name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            await DownloadCurrency(portfolio, name.Name, updateReports, reports);
        }

        public async static Task DownloadCurrency(Portfolio portfolio, string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = portfolio.GetCurrencyFromName(name);
            await Download.DownloadCurrencyLatest(sec, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadBankAccount(Portfolio portfolio, NameData name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
          await DownloadBankAccount(portfolio, name.Company, name.Name, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadBankAccount(Portfolio portfolio, string company, string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            var sec = portfolio.GetBankAccountFromName(name, company);
            await Download.DownloadBankAccountLatest(sec, updateReports, reports).ConfigureAwait(false);
        }

        public async static Task DownloadSector(List<Sector> sectors, NameData name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            await DownloadSector(sectors, name.Name,  updateReports, reports);
        }

        public async static Task DownloadSector(List<Sector> sectors, string name, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            Sector sec = sectors.Find(sector => sector.GetName() == name);
            await Download.DownloadSectorLatest(sec, updateReports, reports).ConfigureAwait(false);
        }
    }
}
