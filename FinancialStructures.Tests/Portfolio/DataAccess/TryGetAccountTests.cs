﻿using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using FinancialStructures.Tests.TestDatabaseConstructor;
using NUnit.Framework;

namespace FinancialStructures.Tests.Database.DataAccess
{
    [TestFixture]
    public sealed class TryGetAccountTests
    {
        [Test]
        public void TryGetSecurity()
        {
            var constructor = new DatabaseConstructor();
            _ = constructor.WithSecurityFromName("Company", "name");

            var portfolio = constructor.database;

            bool result = portfolio.TryGetAccount(Account.Security, new TwoName("Company", "name"), out var desired);

            Assert.AreEqual(true, result);
            Assert.IsNotNull(desired);
            Assert.AreEqual("Company", desired.Names.Company);
            Assert.AreEqual("name", desired.Names.Name);
        }

        [Test]
        public void TryGetNoSecurity()
        {
            var constructor = new DatabaseConstructor();
            _ = constructor.WithSecurityFromName("Company", "name");

            var portfolio = constructor.database;

            bool result = portfolio.TryGetAccount(Account.Security, new TwoName("Company", "NewName"), out var desired);

            Assert.AreEqual(false, result);
            Assert.IsNull(desired);
        }

        [Test]
        public void TryGetSector()
        {
            var constructor = new DatabaseConstructor();
            _ = constructor.WithSectorFromName("Company", "name");

            var portfolio = constructor.database;

            bool result = portfolio.TryGetAccount(Account.Benchmark, new TwoName("Company", "name"), out var desired);

            Assert.AreEqual(true, result);
            Assert.IsNotNull(desired);
            Assert.AreEqual("Company", desired.Names.Company);
            Assert.AreEqual("name", desired.Names.Name);
        }

        [Test]
        public void TryGetNoSector()
        {
            var constructor = new DatabaseConstructor();
            _ = constructor.WithSectorFromName("Company", "name");

            var portfolio = constructor.database;

            bool result = portfolio.TryGetAccount(Account.Benchmark, new TwoName("NewCompany", "NewName"), out var desired);

            Assert.AreEqual(false, result);
            Assert.IsNull(desired);
        }
    }
}