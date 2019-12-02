using GlobalHeldData;
using FinancialStructures.FinanceStructures;
using GUIAccessorFunctions;

namespace PADGlobals
{
    public static class DataUpdater
    {
        public static void Downloader()
        {
            Download.DownloadPortfolioLatest(GlobalData.Finances);
            Download.DownloadBenchMarksLatest(GlobalData.BenchMarks);
        }

        public static void DownloadSecurity(string company, string name)
        {
            var sec = DatabaseAccessor.GetSecurityFromName(name, company);
            Download.DownloadSecurityLatest(sec);
        }
    }
}
