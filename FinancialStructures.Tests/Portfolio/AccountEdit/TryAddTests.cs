﻿using System.Linq;
using Common.Structure.Reporting;
using FinancialStructures.Database;
using FinancialStructures.Database.Implementation;
using FinancialStructures.NamingStructures;
using FinancialStructures.Tests.TestDatabaseConstructor;
using NUnit.Framework;

namespace FinancialStructures.Tests.Database.AccountEdit
{
    [TestFixture]
    public sealed class TryAddTests
    {
        private readonly string BaseCompanyName = "someCompany";
        private readonly string BaseName = "someName";

        [Test]
        public void CanAddSecurity()
        {
            Portfolio database = new DatabaseConstructor().GetInstance();

            _ = database.TryAdd(Account.Security, new NameData(BaseCompanyName, BaseName));

            Assert.AreEqual(1, database.Funds.Count);
            NameData accountNames = database.Funds.First().Names;
            Assert.AreEqual(BaseName, accountNames.Name);
            Assert.AreEqual(BaseCompanyName, accountNames.Company);
        }

        [Test]
        public void CanAddSector()
        {
            Portfolio database = new DatabaseConstructor().GetInstance();

            _ = database.TryAdd(Account.Benchmark, new NameData(BaseCompanyName, BaseName));

            Assert.AreEqual(1, database.BenchMarks.Count);
            NameData accountNames = database.BenchMarks.First().Names;
            Assert.AreEqual(BaseName, accountNames.Name);
            Assert.AreEqual(BaseCompanyName, accountNames.Company);
        }

        [Test]
        public void CanAddBankAccount()
        {
            Portfolio database = new DatabaseConstructor().GetInstance();

            _ = database.TryAdd(Account.BankAccount, new NameData(BaseCompanyName, BaseName));

            Assert.AreEqual(1, database.BankAccounts.Count);
            NameData accountNames = database.BankAccounts.First().Names;
            Assert.AreEqual(BaseName, accountNames.Name);
            Assert.AreEqual(BaseCompanyName, accountNames.Company);
        }

        [Test]
        public void CanAddCurrency()
        {
            Portfolio database = new DatabaseConstructor().GetInstance();

            _ = database.TryAdd(Account.Currency, new NameData(BaseCompanyName, BaseName));

            Assert.AreEqual(1, database.Currencies.Count);
            NameData accountNames = database.Currencies.First().Names;
            Assert.AreEqual(BaseName, accountNames.Name);
            Assert.AreEqual(BaseCompanyName, accountNames.Company);
        }

        [Test]
        public void ReportsSecurityCorrect()
        {
            Portfolio database = new DatabaseConstructor().GetInstance();
            IReportLogger logging = new LogReporter(null, saveInternally: true);
            _ = database.TryAdd(Account.Security, new NameData(BaseCompanyName, BaseName), logging);

            ErrorReports reports = logging.Reports;
            Assert.AreEqual(1, reports.Count());
            ErrorReport report = reports.First();
            Assert.AreEqual(ReportType.Information, report.ErrorType);
            Assert.AreEqual("AddingData", report.ErrorLocation);
            Assert.AreEqual(ReportSeverity.Detailed, report.ErrorSeverity);
            Assert.AreEqual($"Security-{BaseCompanyName}-{BaseName} added to database.", report.Message);
        }

        [Test]
        public void AddingSecurityFailReports()
        {
            Portfolio database = 
                new DatabaseConstructor()
                .WithSecurity(BaseCompanyName, BaseName)
                .GetInstance();
            IReportLogger logging = new LogReporter(null, saveInternally: true);
            _ = database.TryAdd(Account.Security, new NameData(BaseCompanyName, BaseName), logging);

            ErrorReports reports = logging.Reports;
            Assert.AreEqual(1, reports.Count());
            ErrorReport report = reports.First();
            Assert.AreEqual(ReportType.Error, report.ErrorType);
            Assert.AreEqual("AddingData", report.ErrorLocation);
            Assert.AreEqual(ReportSeverity.Critical, report.ErrorSeverity);
            Assert.AreEqual($"Security-{BaseCompanyName}-{BaseName} already exists.", report.Message);
        }

        [Test]
        public void ReportSectorCorrect()
        {
            Portfolio database = new DatabaseConstructor().GetInstance();
            IReportLogger logging = new LogReporter(null, saveInternally: true);
            _ = database.TryAdd(Account.Benchmark, new NameData(BaseCompanyName, BaseName), logging);

            ErrorReports reports = logging.Reports;
            Assert.AreEqual(1, reports.Count());

            ErrorReport report = reports.First();
            Assert.AreEqual(ReportType.Information, report.ErrorType);
            Assert.AreEqual("AddingData", report.ErrorLocation);
            Assert.AreEqual(ReportSeverity.Detailed, report.ErrorSeverity);
            Assert.AreEqual($"Benchmark-{BaseCompanyName}-{BaseName} added to database.", report.Message);
        }

        [Test]
        public void AddingSectorFailReports()
        {
            Portfolio database = 
                new DatabaseConstructor()
                .WithSectorFromName(BaseCompanyName, BaseName)
                .GetInstance();
            IReportLogger logging = new LogReporter(null, saveInternally: true);
            _ = database.TryAdd(Account.Benchmark, new NameData(BaseCompanyName, BaseName), logging);

            ErrorReports reports = logging.Reports;
            Assert.AreEqual(1, reports.Count());
            ErrorReport report = reports.First();
            Assert.AreEqual(ReportType.Error, report.ErrorType);
            Assert.AreEqual("AddingData", report.ErrorLocation);
            Assert.AreEqual(ReportSeverity.Critical, report.ErrorSeverity);
            Assert.AreEqual($"Benchmark-{BaseCompanyName}-{BaseName} already exists.", report.Message);
        }
    }
}
