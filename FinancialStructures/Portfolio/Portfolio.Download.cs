using FinancialStructures.DataStructures;
using FinancialStructures.FinanceStructures;
using FinancialStructures.ReportingStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinancialStructures.Database
{
    public static class Download
    {
        private static string Pence = "GBX";
        private static string Pounds = "GBP";
        private static HttpClient client = new HttpClient();
        private static bool IsValidWebAddress(string address)
        {
            if (!Uri.TryCreate(address, UriKind.Absolute, out Uri uri) || null == uri)
            {
                return false;
            }

            return Uri.IsWellFormedUriString(address, UriKind.Absolute);
        }

        private static bool IsMorningstarAddress(string address)
        {
            return address.Contains("morningstar");
        }

        private static bool IsYahooAddress(string address)
        {
            return address.Contains("yahoo");
        }

        private static bool IsGoogleAddress(string address)
        {
            return address.Contains("google");
        }

        private static bool IsTrustNetAddress(string address)
        {
            return address.Contains("trustnet");
        }

        private static bool IsBloomBergAddress(string address)
        {
            return address.Contains("bloomberg.com");
        }

        private static async Task<string> DownloadFromURL(string url, ErrorReports reports)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            string newUrl = Uri.EscapeUriString(url);
            string output = string.Empty;
            if (IsValidWebAddress(newUrl))
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(newUrl).ConfigureAwait(false))
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync().ConfigureAwait(false);

                        // ... Display the result.
                        if (result != null && result.Length >= 50)
                        {
                            output = result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    reports.AddError($"Failed to download from url {url}. Reason : {ex.Message}");
                    return output;
                }
            }

            return output;
        }

        private static bool ProcessDownloadString(string url, string data, ErrorReports reports, out double value)
        {
            value = double.NaN;

            if (IsMorningstarAddress(url))
            {
                value = ProcessFromMorningstar(data);
                if (double.IsNaN(value))
                {
                    reports.AddError($"Could not download data from morningstar url: {url}");
                    return false;
                }
                return true;
            }
            if (IsYahooAddress(url))
            {
                value = ProcessFromYahoo(data);
                if (double.IsNaN(value))
                {
                    reports.AddError($"Could not download data from Yahoo url: {url}");
                    return false;
                }
                return true;
            }
            if (IsGoogleAddress(url))
            {
                value = ProcessFromGoogle(data);
                if (double.IsNaN(value))
                {
                    reports.AddError($"Could not download data from google url: {url}");
                    return false;
                }
                return true;
            }
            if (IsTrustNetAddress(url))
            {
                value = ProcessFromTrustNet(data);
                if (double.IsNaN(value))
                {
                    reports.AddError($"Could not download data from Trustnet url: {url}");
                    return false;
                }
                return true;
            }
            if (IsBloomBergAddress(url))
            {
                value = ProcessFromBloomBerg(data);
                if (double.IsNaN(value))
                {
                    reports.AddError($"Could not download data from BloomBerg url: {url}");
                    return false;
                }
                return true;
            }

            reports.AddError($"Url not of a currently implemented downloadable type: {url}");
            return false;
        }

        private static double ProcessFromMorningstar(string data)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    return true;
                }

                return false;
            };

            int penceValue = data.IndexOf(Pence);
            if (penceValue != -1)
            {
                string containsNewValue = data.Substring(Pence.Length + penceValue, 20);

                var digits = containsNewValue.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

                var str = new string(digits);
                if (string.IsNullOrEmpty(str))
                {
                    return double.NaN;
                }
                double i = double.Parse(str);
                i = i / 100;
                return i;
            }
            string searchName = "<td class=\"line text\">" + Pounds;
            int poundsValue = data.IndexOf(searchName);
            if (poundsValue != -1)
            {
                string containsNewValue = data.Substring(searchName.Length + poundsValue, 20);
                var digits = containsNewValue.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

                var str = new string(digits);
                if (string.IsNullOrEmpty(str))
                {
                    return double.NaN;
                }
                double i = double.Parse(str);

                return i;
            }

            return double.NaN;
        }

        private static double ProcessFromYahoo(string data)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.' || c == ',')
                {
                    return true;
                }

                return false;
            };
            string searchString = "data-reactid=\"33\"><span class=\"Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)\" data-reactid=\"34\">";
            int poundsValue = data.IndexOf(searchString);
            if (poundsValue != -1)
            {
                string containsNewValue = data.Substring(poundsValue + searchString.Length, 20);

                var digits = containsNewValue.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

                var str = new string(digits);
                if (string.IsNullOrEmpty(str))
                {
                    return double.NaN;
                }
                double i = double.Parse(str);
                if (data.Contains("GBp"))
                {
                    i = i / 100.0;
                }
                return i;
            }

            return double.NaN;
        }

        private static double ProcessFromGoogle(string data)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    return true;
                }

                return false;
            };
            string searchName = "regularMarketPrice";
            int penceValue = data.IndexOf(searchName);
            if (penceValue != -1)
            {
                string containsNewValue = data.Substring(searchName.Length + penceValue, 20);

                var digits = containsNewValue.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

                var str = new string(digits);
                if (string.IsNullOrEmpty(str))
                {
                    return double.NaN;
                }
                double i = double.Parse(str);
                return i;
            }

            return double.NaN;
        }

        private static double ProcessFromTrustNet(string data)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    return true;
                }

                return false;
            };
            string searchName = "regularMarketPrice";
            int penceValue = data.IndexOf(searchName);
            if (penceValue != -1)
            {
                string containsNewValue = data.Substring(searchName.Length + penceValue, 20);

                var digits = containsNewValue.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

                var str = new string(digits);
                if (string.IsNullOrEmpty(str))
                {
                    return double.NaN;
                }
                double i = double.Parse(str);
                return i;
            }

            return double.NaN;
        }



        private static double ProcessFromBloomBerg(string data)
        {
            bool continuer(char c)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    return true;
                }

                return false;
            };
            string searchName = "<span class=\"priceText__1853e8a5\">";
            int penceValue = data.IndexOf(searchName);
            if (penceValue != -1)
            {
                string containsNewValue = data.Substring(searchName.Length + penceValue, 20);

                var digits = containsNewValue.SkipWhile(c => !char.IsDigit(c)).TakeWhile(continuer).ToArray();

                var str = new string(digits);
                if (string.IsNullOrEmpty(str))
                {
                    return double.NaN;
                }
                double i = double.Parse(str);
                return i;
            }

            return double.NaN;
        }

        public async static Task DownloadPortfolioLatest(Portfolio portfo, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            foreach (var sec in portfo.GetSecurities())
            {
                await DownloadSecurityLatest(sec, updateReports, reports).ConfigureAwait(false);
            }
            foreach (var acc in portfo.GetBankAccounts())
            {
                await DownloadBankAccountLatest(acc, updateReports, reports).ConfigureAwait(false);
            }
            foreach (var currency in portfo.GetCurrencies())
            {
                await DownloadCurrencyLatest(currency, updateReports, reports).ConfigureAwait(false);
            }
        }

        public async static Task DownloadSecurityLatest(Security sec, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            string data = await DownloadFromURL(sec.GetUrl(), reports).ConfigureAwait(false);
            if (string.IsNullOrEmpty(data))
            {
                reports.AddError($"{sec.GetCompany()}-{sec.GetName()}: could not download data from {sec.GetUrl()}");
                return;
            }
            if (!ProcessDownloadString(sec.GetUrl(), data, reports, out double value))
            {
                return;
            }

            // best approximation for number of units is last known number of units.
            sec.TryGetEarlierData(DateTime.Today, out DailyValuation _, out DailyValuation units, out DailyValuation _);
            if (units == null)
            {
                units = new DailyValuation(DateTime.Today, 0);
            }

            sec.TryAddData(reports, DateTime.Today, value, units.Value);
            updateReports(reports);
        }

        public async static Task DownloadBankAccountLatest(CashAccount acc, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            string data = await DownloadFromURL(acc.GetUrl(), reports).ConfigureAwait(false);
            if (string.IsNullOrEmpty(data))
            {
                reports.AddError($"{acc.GetCompany()}-{acc.GetName()}: could not download data from {acc.GetUrl()}");
                return;
            }
            if (!ProcessDownloadString(acc.GetUrl(), data, reports, out double value))
            {
                return;
            }

            acc.TryAddValue(DateTime.Today, value);
            updateReports(reports);
        }

        public async static Task DownloadCurrencyLatest(Currency currency, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            string data = await DownloadFromURL(currency.GetUrl(), reports).ConfigureAwait(false);
            if (string.IsNullOrEmpty(data))
            {
                reports.AddError($"{currency.GetName()}: could not download data from {currency.GetUrl()}");
                return;
            }
            if (!ProcessDownloadString(currency.GetUrl(), data, reports, out double value))
            {
                return;
            }

            currency.TryAddData(DateTime.Today, value);
            updateReports(reports);
        }

        public async static Task DownloadBenchMarksLatest(List<Sector> sectors, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            foreach (var sector in sectors)
            {
                await DownloadSectorLatest(sector, updateReports, reports).ConfigureAwait(false);
            }
        }

        public async static Task DownloadSectorLatest(Sector sector, Action<ErrorReports> updateReports, ErrorReports reports)
        {
            string data = await DownloadFromURL(sector.GetUrl(), reports).ConfigureAwait(false);
            if (string.IsNullOrEmpty(data))
            {
                reports.AddError($"{sector.GetName()}: could not download data from {sector.GetUrl()}");
                return;
            }
            if (!ProcessDownloadString(sector.GetUrl(), data, reports, out double value))
            {
                return;
            }
            sector.TryAddData(DateTime.Today, value);
            updateReports(reports);
        }
    }
}
