﻿using System;
using System.Collections.Generic;
using System.Linq;
using FinancialStructures.Database;
using FinancialStructures.Database.Implementation;
using FinancialStructures.FinanceStructures;
using FinancialStructures.Tests.TestDatabaseConstructor;
using NUnit.Framework;

namespace FinancialStructures.Tests.Database.DataAccess
{
    [TestFixture]
    public sealed class DataListsTests
    {
        [Test]
        public void CanRetrieveSecurityData()
        {
            DatabaseConstructor generator = new DatabaseConstructor();
            string secCompany = "company1";
            _ = generator.WithSecurity(secCompany, "name1", dates: new[] { new DateTime(2000, 1, 1) }, sharePrice: new[] { 101.0 }, numberUnits: new[] { 12.0 });
            _ = generator.WithSecurity("otherCompany", "name1", dates: new[] { new DateTime(2000, 1, 1) }, sharePrice: new[] { 101.0 }, numberUnits: new[] { 12.0 });

            IReadOnlyList<IValueList> data = generator.Database.CompanyAccounts(Account.Security, secCompany);

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("name1", data.Single().Names.Name);
        }

        [Test]
        public void RetrievesNewListForNoSecurityData()
        {
            DatabaseConstructor generator = new DatabaseConstructor();
            string secCompany = "company1";
            _ = generator.WithSecurity(secCompany, "name1", dates: new[] { new DateTime(2000, 1, 1) }, sharePrice: new[] { 101.0 }, numberUnits: new[] { 12.0 });

            IReadOnlyList<IValueList> data = generator.Database.CompanyAccounts(Account.Security, "other");
            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public void CanDisplayBankAccountData()
        {
            DatabaseConstructor generator = new DatabaseConstructor();

            string bankCompany = "Bank";
            _ = generator.WithBankAccount(bankCompany, "AccountName", dates: new[] { new DateTime(2000, 1, 1) }, values: new[] { 53.0 });

            IReadOnlyList<IValueList> data = generator.Database.CompanyAccounts(Account.BankAccount, bankCompany);

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual(bankCompany, data.Single().Names.Company);
        }

        [Test]
        public void RetrievesNewListForNoBankAccountData()
        {
            DatabaseConstructor generator = new DatabaseConstructor();

            string bankCompany = "Bank";
            _ = generator.WithBankAccount(bankCompany, "AccountName", dates: new[] { new DateTime(2000, 1, 1) }, values: new[] { 53.0 });
            Portfolio database = generator.Database;

            IReadOnlyList<IValueList> data = database.CompanyAccounts(Account.BankAccount, "name");

            Assert.AreEqual(0, data.Count);
        }
    }
}
