﻿using System;
using System.Collections.Generic;
using System.Linq;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.Tests.TestDatabaseConstructor;
using NUnit.Framework;
using Common.Structure.Reporting;

namespace FinancialStructures.Tests.Database.DataAccess
{
    [TestFixture]
    public sealed class DataTests
    {
        [Test]
        public void CanDisplaySecurityData()
        {
            var generator = new DatabaseConstructor();
            string secCompany = "company1";
            generator.WithSecurity(secCompany, "name1", dates: new[] { new DateTime(2000, 1, 1) }, sharePrice: new[] { 101.0 }, numberUnits: new[] { 12.0 });
            var database = generator.Database;

            var data = database.SecurityData(new TwoName(secCompany, "name1"));

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(12, data.Single().ShareNo);
            Assert.AreEqual(101, data.Single().UnitPrice);
        }

        [Test]
        public void RetrievesNewListForNoSecurityData()
        {
            var generator = new DatabaseConstructor();
            string secCompany = "company1";
            generator.WithSecurity(secCompany, "name1", dates: new[] { new DateTime(2000, 1, 1) }, sharePrice: new[] { 101.0 }, numberUnits: new[] { 12.0 });

            var database = generator.Database;

            var data = database.SecurityData(new TwoName(secCompany, "name"));

            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public void CanDisplayBankAccountData()
        {
            var generator = new DatabaseConstructor();

            string bankCompany = "Bank";
            generator.WithBankAccount(bankCompany, "AccountName", dates: new[] { new DateTime(2000, 1, 1) }, values: new[] { 53.0 });
            var database = generator.Database;

            var data = database.NumberData(Account.BankAccount, new NameData(bankCompany, "AccountName"));

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(53, data.Single().Value);
        }

        [Test]
        public void RetrievesNewListForNoBankAccountData()
        {
            var generator = new DatabaseConstructor();

            string bankCompany = "Bank";
            generator.WithBankAccount(bankCompany, "AccountName", dates: new[] { new DateTime(2000, 1, 1) }, values: new[] { 53.0 });
            var database = generator.Database;

            var data = database.NumberData(Account.BankAccount, new NameData(bankCompany, "name"));

            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public void ReturnsErrorMessageForNoBankAccountData()
        {
            var generator = new DatabaseConstructor();

            var reports = new List<ErrorReport>();
            IReportLogger logging = new LogReporter((a, b, c, d) => reports.Add(new ErrorReport(a, b, c, d)));
            string bankCompany = "Bank";
            generator.WithBankAccount(bankCompany, "AccountName", dates: new[] { new DateTime(2000, 1, 1) }, values: new[] { 53.0 });
            var database = generator.Database;

            var data = database.NumberData(Account.BankAccount, new NameData(bankCompany, "name"), logging);

            Assert.AreEqual(0, data.Count);
            Assert.AreEqual(1, reports.Count);
            var report = reports.Single();
            Assert.AreEqual(ReportSeverity.Useful, report.ErrorSeverity);
            Assert.AreEqual(ReportType.Error, report.ErrorType);
            Assert.AreEqual(ReportLocation.DatabaseAccess, report.ErrorLocation);
            Assert.AreEqual($"Could not find BankAccount - {bankCompany}-name", report.Message);
        }
    }
}
