﻿namespace FinancialStructures.Database
{
    /// <summary>
    /// Provides conversions between <see cref="Account"/> and <see cref="Totals"/> enums.
    /// </summary>
    public static class EnumConvert
    {
        /// <summary>
        /// Converts a Account enum to a Totals enum.
        /// </summary>
        public static Totals ToTotals(this Account element)
        {
            switch (element)
            {
                default:
                case Account.All:
                {
                    return Totals.All;
                }
                case Account.Security:
                {
                    return Totals.Security;
                }
                case Account.BankAccount:
                {
                    return Totals.BankAccount;
                }
                case Account.Benchmark:
                {
                    return Totals.Benchmark;
                }
                case Account.Currency:
                {
                    return Totals.Currency;
                }
            }
        }

        /// <summary>
        /// Converts a Account enum to a Totals enum.
        /// </summary>
        public static Account ToAccount(this Totals element)
        {
            switch (element)
            {
                case Totals.All:
                case Totals.Sector:
                case Totals.Company:
                case Totals.CurrencySector:
                default:
                {
                    return Account.All;
                }
                case Totals.Security:
                case Totals.SecurityCompany:
                case Totals.SecuritySector:
                case Totals.SecurityCurrency:
                {
                    return Account.Security;
                }
                case Totals.BankAccount:
                case Totals.BankAccountCompany:
                case Totals.BankAccountSector:
                case Totals.BankAccountCurrency:
                {
                    return Account.BankAccount;
                }
                case Totals.Benchmark:
                {
                    return Account.Benchmark;
                }
                case Totals.Currency:
                {
                    return Account.Currency;
                }
            }
        }


        /// <summary>
        /// Converts a Account enum to a Totals enum.
        /// </summary>
        public static Totals ToCompanyTotal(this Totals element)
        {
            switch (element)
            {
                case Totals.All:
                case Totals.Sector:
                case Totals.Company:
                case Totals.CurrencySector:
                default:
                {
                    return Totals.All;
                }
                case Totals.Security:
                case Totals.SecurityCompany:
                case Totals.SecuritySector:
                case Totals.SecurityCurrency:
                {
                    return Totals.SecurityCompany;
                }
                case Totals.BankAccount:
                case Totals.BankAccountCompany:
                case Totals.BankAccountSector:
                case Totals.BankAccountCurrency:
                {
                    return Totals.BankAccountCompany;
                }
                case Totals.Benchmark:
                {
                    return Totals.Benchmark;
                }
                case Totals.Currency:
                {
                    return Totals.Currency;
                }
            }
        }
    }
}
