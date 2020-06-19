﻿using System;
using FinancialStructures.DataStructures;
using NUnit.Framework;

namespace FinancialStructures_UnitTests.DataStructuresTests
{
    [TestFixture]
    public class SecurityDayDataTests
    {
        [TestCase("1/1/2019", "2/1/2019", -1)]
        [TestCase("1/1/2019", "1/1/2019", 0)]
        [TestCase("5/1/2019", "1/1/2019", 1)]
        public void ComparisonTests(DateTime first, DateTime second, int expected)
        {
            var one = new SecurityDayData(first, 0, 0, 0);
            var two = new SecurityDayData(second, 0, 0, 0);
            Assert.AreEqual(expected, one.CompareTo(two));
        }

        [TestCase("1/1/2019", 1, 1, 1, "01/01/2019, 1, 1, 1")]
        public void ToStringTests(DateTime first, double unit, double share, double inv, string expected)
        {
            var one = new SecurityDayData(first, unit, share, inv);
            Assert.AreEqual(expected, one.ToString());
        }
    }
}
