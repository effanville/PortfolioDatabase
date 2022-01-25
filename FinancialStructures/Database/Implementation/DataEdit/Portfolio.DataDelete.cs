﻿using System;
using System.Collections.Generic;
using Common.Structure.Reporting;
using FinancialStructures.FinanceStructures;
using FinancialStructures.NamingStructures;

namespace FinancialStructures.Database.Implementation
{
    public partial class Portfolio
    {
        /// <inheritdoc/>
        public bool TryDeleteTradeData(Account elementType, TwoName name, DateTime date, IReportLogger reportLogger = null)
        {
            switch (elementType)
            {
                case Account.Security:
                {
                    foreach (ISecurity account in FundsThreadSafe)
                    {
                        if (name.IsEqualTo(account.Names))
                        {
                            return account.TryDeleteTradeData(date, reportLogger);
                        }
                    }

                    _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, $"Could not find {elementType} - {name}.");
                    return false;
                }

                case Account.All:
                case Account.Currency:
                case Account.BankAccount:
                case Account.Benchmark:
                case Account.Asset:
                default:
                    _ = reportLogger?.LogUseful(ReportType.Error, ReportLocation.DeletingData, $"Editing an Unknown type.");
                    return false;
            }
        }

        /// <inheritdoc/>
        public bool TryDeleteAssetDebt(Account elementType, TwoName name, DateTime date, IReportLogger reportLogger = null)
        {
            if (elementType != Account.Asset)
            {
                _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.AddingData, $"Cannot edit debt for account of type {elementType}.");
                return false;
            }

            IReadOnlyList<IAmortisableAsset> assets = Assets;
            for (int fundIndex = 0; fundIndex < NumberOf(Account.Security); fundIndex++)
            {
                if (name.IsEqualTo(assets[fundIndex].Names))
                {
                    return assets[fundIndex].TryDeleteDebt(date, reportLogger);
                }
            }

            _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.AddingData, $"Asset `{name.Company}'-`{name.Name}' could not be found in the database.");
            return false;
        }

        /// <inheritdoc/>
        public bool TryDeleteAssetPayment(Account elementType, TwoName name, DateTime date, IReportLogger reportLogger = null)
        {
            if (elementType != Account.Asset)
            {
                _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.AddingData, $"Cannot edit debt for account of type {elementType}.");
                return false;
            }

            IReadOnlyList<IAmortisableAsset> assets = Assets;
            for (int fundIndex = 0; fundIndex < NumberOf(Account.Security); fundIndex++)
            {
                if (name.IsEqualTo(assets[fundIndex].Names))
                {
                    return assets[fundIndex].TryDeletePayment(date, reportLogger);
                }
            }

            _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.AddingData, $"Asset `{name.Company}'-`{name.Name}' could not be found in the database.");
            return false;
        }

        /// <inheritdoc/>
        public bool TryDeleteData(Account elementType, TwoName name, DateTime date, IReportLogger reportLogger = null)
        {
            switch (elementType)
            {
                case Account.Security:
                {
                    return TryDeleteSingleListData(FundsThreadSafe, elementType, name, date, reportLogger);
                }
                case Account.Currency:
                {
                    return TryDeleteSingleListData(CurrenciesThreadSafe, elementType, name, date, reportLogger);
                }
                case Account.BankAccount:
                {
                    return TryDeleteSingleListData(BankAccountsThreadSafe, elementType, name, date, reportLogger);
                }
                case Account.Benchmark:
                {
                    return TryDeleteSingleListData(BenchMarksThreadSafe, elementType, name, date, reportLogger);
                }
                case Account.Asset:
                {
                    return TryDeleteSingleListData(Assets, elementType, name, date, reportLogger);
                }
                case Account.All:
                default:
                    _ = reportLogger?.LogUseful(ReportType.Error, ReportLocation.DeletingData, $"Editing an Unknown type.");
                    return false;
            }
        }

        private static bool TryDeleteSingleListData<T>(IReadOnlyList<T> values, Account elementType, TwoName name, DateTime date, IReportLogger reportLogger = null) where T : IValueList
        {
            foreach (T account in values)
            {
                if (name.IsEqualTo(account.Names))
                {
                    return account.TryDeleteData(date, reportLogger);
                }
            }

            _ = reportLogger?.Log(ReportSeverity.Critical, ReportType.Error, ReportLocation.DeletingData, $"Could not find {elementType} - {name}.");
            return false;
        }
    }
}
