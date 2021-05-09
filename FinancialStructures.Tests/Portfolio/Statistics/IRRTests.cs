﻿using System;
using FinancialStructures.Database;
using FinancialStructures.Database.Statistics;
using NUnit.Framework;

namespace FinancialStructures.Tests.Database.Statistics
{
    public sealed class IRRTests
    {
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.All, "2010 /1/1", "2019/1/1", 0.07373046875)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.All, "2011/1/1", "2019/1/1", 0.07373046875)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.All, "2012/1/1", "2019/1/1", 0.07080078125)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.All, "2013/1/1", "2019/1/1", 0.095018679818698937)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.All, "2014/1/1", "2019/1/1", 0.13186306269750059)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.Security, "2010 /1/1", "2019/1/1", 0.07373046875)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.Security, "2011/1/1", "2019/1/1", 0.07373046875)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.Security, "2012/1/1", "2019/1/1", 0.07080078125)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.Security, "2013/1/1", "2019/1/1", 0.095018679818698937)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.Security, "2014/1/1", "2019/1/1", 0.13186306269750059)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.BankAccount, "2010 /1/1", "2019/1/1", -0.025712539370376319)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.BankAccount, "2011/1/1", "2019/1/1", -0.09969157728161171)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.BankAccount, "2012/1/1", "2019/1/1", -0.062068923361617845)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.BankAccount, "2013/1/1", "2019/1/1", -0.037103019820752703)]
        [TestCase(TestDatabaseName.TwoSecTwoBank, Totals.BankAccount, "2014/1/1", "2019/1/1", -0.029457843771104164)]
        [TestCase(TestDatabaseName.TwoSec, Totals.BankAccount, "2010 /1/1", "2019/1/1", 0.0)]
        [TestCase(TestDatabaseName.TwoSec, Totals.BankAccount, "2011/1/1", "2019/1/1", 0.0)]
        public void IRRPortfolioTests(TestDatabaseName databaseName, Totals totals, DateTime earlier, DateTime later, double expected)
        {
            var portfolio = TestDatabase.Databases[databaseName];
            Assert.AreEqual(expected, portfolio.IRRTotal(totals, earlier, later));
        }
    }
}