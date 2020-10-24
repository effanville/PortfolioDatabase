﻿using FinancialStructures.FinanceInterfaces;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Database
{
    public partial class Portfolio
    {
        /// <inheritdoc/>
        public bool TryGetSecurity(TwoName names, out ISecurity desired)
        {
            foreach (ISecurity sec in Funds)
            {
                if (names.IsEqualTo(sec.Names))
                {
                    desired = sec.Copy();
                    return true;
                }
            }
            desired = null;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetAccount(Account accountType, TwoName names, out ISingleValueDataList desired)
        {
            bool success = false;
            desired = null;
            switch (accountType)
            {
                case (Account.BankAccount):
                {
                    foreach (ICashAccount sec in BankAccounts)
                    {
                        if (names.IsEqualTo(sec.Names))
                        {
                            desired = sec;
                            success = true;
                        }
                    }
                    break;
                }
                case (Account.Currency):
                {
                    foreach (ICurrency currency in Currencies)
                    {
                        if (names.IsEqualTo(currency.Names))
                        {
                            desired = currency;
                            success = true;
                        }
                    }
                    break;
                }
                case (Account.Benchmark):
                {
                    foreach (ISector sector in BenchMarks)
                    {
                        if (sector.Name == names.Name)
                        {
                            desired = sector;
                            success = true;
                        }
                    }
                    break;
                }
            }

            return success;
        }
    }
}