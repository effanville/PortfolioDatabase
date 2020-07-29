﻿using System;
using FinancialStructures.DataStructures;
using NUnit.Framework;

namespace FinancialStructures_UnitTests.DataStructuresTests
{
    [TestFixture]
    public class DayValue_NamedTests
    {
        [TestCase("name", "company", "name", "company", 0)]
        [TestCase("name", "company", "name", "ompany", -1)]
        [TestCase("name", "ompany", "name", "company", 1)]
        [TestCase("ame", "company", "name", "company", -1)]
        [TestCase("name", "company", "ame", "company", 1)]
        public void ComparisonCorrect(string name1, string company1, string name2, string company2, int expected)
        {
            var one = new DayValue_Named(company1, name1, new DateTime(), 0);
            var two = new DayValue_Named(company2, name2, new DateTime(), 0);
            Assert.AreEqual(expected, one.CompareTo(two));
        }

        [TestCase("name", "company", "12/5/2019", 5, "company-name-05/12/2019, 5")]
        [TestCase(null, "company", "12/5/2019", 5, "company-05/12/2019, 5")]
        [TestCase("name", null, "12/5/2019", 5, "name-05/12/2019, 5")]
        [TestCase("name", "company", "12/5/2019", 0, "company-name-05/12/2019, 0")]
        [TestCase("", "company", "12/5/2019", 5, "company-05/12/2019, 5")]
        public void ToStringTests(string name1, string company1, DateTime date, double value, string expected)
        {
            var one = new DayValue_Named(company1, name1, date, value);
            Assert.AreEqual(expected, one.ToString());
        }
    }
}