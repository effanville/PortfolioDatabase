﻿using System;
using System.Linq;
using FinancialStructures.PortfolioAPI;
using FinancialStructures.Tests.TestDatabaseConstructor;
using NUnit.Framework;

namespace FinancialStructures.Tests.PortfolioAPI.DataAccess
{
    [TestFixture]
    public sealed class DataListsTests
    {
        [Test]
        public void CanRetrieveSecurityData()
        {
            var generator = new DatabaseConstructor();
            string secCompany = "company1";
            generator.WithSecurityFromNameAndDataPoint(secCompany, "name1", date: new DateTime(2000, 1, 1), sharePrice: 101, numberUnits: 12);
            generator.WithSecurityFromNameAndDataPoint("otherCompany", "name1", date: new DateTime(2000, 1, 1), sharePrice: 101, numberUnits: 12);
            var database = generator.database;

            var data = database.CompanySecurities(secCompany);

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("name1", data.Single().Name);
        }

        [Test]
        public void RetrievesNewListForNoSecurityData()
        {
            var generator = new DatabaseConstructor();
            string secCompany = "company1";
            generator.WithSecurityFromNameAndDataPoint(secCompany, "name1", date: new DateTime(2000, 1, 1), sharePrice: 101, numberUnits: 12);

            var database = generator.database;

            var data = database.CompanySecurities("other");

            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public void CanDisplayBankAccountData()
        {
            var generator = new DatabaseConstructor();

            string bankCompany = "Bank";
            generator.WithBankAccountFromNameAndDataPoint(bankCompany, "AccountName", date: new DateTime(2000, 1, 1), value: 53);
            var database = generator.database;

            var data = database.CompanyBankAccounts(bankCompany);

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(bankCompany, data.Single().Company);
        }

        [Test]
        public void RetrievesNewListForNoBankAccountData()
        {
            var generator = new DatabaseConstructor();

            string bankCompany = "Bank";
            generator.WithBankAccountFromNameAndDataPoint(bankCompany, "AccountName", date: new DateTime(2000, 1, 1), value: 53);
            var database = generator.database;

            var data = database.CompanyBankAccounts("name");

            Assert.AreEqual(0, data.Count);
        }
    }
}