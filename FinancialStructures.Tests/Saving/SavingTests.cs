﻿using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FinancialStructures.Database;
using FinancialStructures.Database.Implementation;
using FinancialStructures.NamingStructures;
using NUnit.Framework;

namespace FinancialStructures.Tests.Saving
{
    [TestFixture]
    public sealed class SavingTests
    {
        private static IEnumerable<(string name, IPortfolio testPortfolio, string XmlString)> OldStyleTestLists()
        {
            yield return ("empty", new Portfolio() { FilePath = "c:/temp/saved.xml" }, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <Funds />\r\n    <BankAccounts />\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
            var portfolio = new Portfolio() { FilePath = "c:/temp/saved.xml" };
            _ = portfolio.TryAdd(Account.Security, new NameData("company", "name"));
            _ = portfolio.TryAdd(Account.BankAccount, new NameData("bank", "account"));
            _ = portfolio.TryAdd(Account.Currency, new NameData("gbp", "hkd"));
            _ = portfolio.TryAdd(Account.Benchmark, new NameData("first", "last"));
            yield return ("AccountsNoData", portfolio, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>company</Company>\r\n          <Name>name</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values />\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values />\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values />\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>bank</Company>\r\n          <Name>account</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Amounts>\r\n          <Values />\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies>\r\n      <Currency>\r\n        <Names>\r\n          <Company>gbp</Company>\r\n          <Name>hkd</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Currency>\r\n    </Currencies>\r\n    <BenchMarks>\r\n      <Sector>\r\n        <Names>\r\n          <Company>first</Company>\r\n          <Name>last</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Sector>\r\n    </BenchMarks>\r\n  </MyFunds>\r\n</AllData>");
            var portfolioWithData = new Portfolio() { FilePath = "c:/temp/saved.xml" };
            _ = portfolioWithData.TryAdd(Account.Security, new NameData("company", "name", "GBP", "http://temp.com", new HashSet<string>() { "UK", "China" }, "some information"));
            _ = portfolioWithData.TryAddOrEditDataToSecurity(new TwoName("company", "name"), new System.DateTime(2015, 1, 1), new System.DateTime(2015, 1, 1), 5, 1.2, 6, null);
            _ = portfolioWithData.TryAdd(Account.BankAccount, new NameData("bank", "account"));
            _ = portfolioWithData.TryAdd(Account.Currency, new NameData("gbp", "hkd"));
            _ = portfolioWithData.TryAdd(Account.Benchmark, new NameData("first", "last"));
            yield return ("AccountsWithData", portfolioWithData, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>company</Company>\r\n          <Name>name</Name>\r\n          <Url>http://temp.com</Url>\r\n          <Currency>GBP</Currency>\r\n          <Sectors>\r\n            <string>UK</string>\r\n            <string>China</string>\r\n          </Sectors>\r\n          <Notes>some information</Notes>\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2015-01-01T00:00:00</Day>\r\n              <Value>5</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2015-01-01T00:00:00</Day>\r\n              <Value>1.2</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2015-01-01T00:00:00</Day>\r\n              <Value>6</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>bank</Company>\r\n          <Name>account</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Amounts>\r\n          <Values />\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies>\r\n      <Currency>\r\n        <Names>\r\n          <Company>gbp</Company>\r\n          <Name>hkd</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Currency>\r\n    </Currencies>\r\n    <BenchMarks>\r\n      <Sector>\r\n        <Names>\r\n          <Company>first</Company>\r\n          <Name>last</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Sector>\r\n    </BenchMarks>\r\n  </MyFunds>\r\n</AllData>");

            var testDatabases = TestDatabase.Databases;
            yield return (TestDatabaseName.OneBank.ToString(), testDatabases[TestDatabaseName.OneBank], "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <BaseCurrency>GBP</BaseCurrency>\r\n    <Funds />\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>Santander</Company>\r\n          <Name>Current</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>125.2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>90.6</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>77.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>101.1</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Values>\r\n        <Amounts>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>125.2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>90.6</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>77.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>101.1</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
            yield return (TestDatabaseName.OneSec.ToString(), testDatabases[TestDatabaseName.OneSec], "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <BaseCurrency>GBP</BaseCurrency>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>BlackRock</Company>\r\n          <Name>UK Stock</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>1.5</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>17.3</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>4</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>5.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>5.5</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>125.2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>90.6</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>77.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>101.1</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts />\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
            yield return (TestDatabaseName.OneSecOneBank.ToString(), testDatabases[TestDatabaseName.OneSecOneBank], "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <BaseCurrency>GBP</BaseCurrency>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>BlackRock</Company>\r\n          <Name>UK Stock</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>1.5</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>17.3</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>4</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>5.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>5.5</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>125.2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>90.6</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>77.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>101.1</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>Santander</Company>\r\n          <Name>Current</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>125.2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>90.6</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>77.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>101.1</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Values>\r\n        <Amounts>\r\n          <Values>\r\n            <DailyValuation>\r\n              <Day>2010-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2011-01-01T00:00:00</Day>\r\n              <Value>100</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2012-05-01T00:00:00</Day>\r\n              <Value>125.2</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2015-04-03T00:00:00</Day>\r\n              <Value>90.6</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2018-05-06T00:00:00</Day>\r\n              <Value>77.7</Value>\r\n            </DailyValuation>\r\n            <DailyValuation>\r\n              <Day>2020-01-01T00:00:00</Day>\r\n              <Value>101.1</Value>\r\n            </DailyValuation>\r\n          </Values>\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
        }


        private static IEnumerable<(string name, IPortfolio testPortfolio, string XmlString)> NewStyleTestLists()
        {
            yield return ("empty", new Portfolio() { FilePath = "c:/temp/saved.xml" }, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <Funds />\r\n    <BankAccounts />\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
            var portfolio = new Portfolio() { FilePath = "c:/temp/saved.xml" };
            _ = portfolio.TryAdd(Account.Security, new NameData("company", "name"));
            _ = portfolio.TryAdd(Account.BankAccount, new NameData("bank", "account"));
            _ = portfolio.TryAdd(Account.Currency, new NameData("gbp", "hkd"));
            _ = portfolio.TryAdd(Account.Benchmark, new NameData("first", "last"));
            yield return ("AccountsNoData", portfolio, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>company</Company>\r\n          <Name>name</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values />\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values />\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values />\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>bank</Company>\r\n          <Name>account</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Amounts>\r\n          <Values />\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies>\r\n      <Currency>\r\n        <Names>\r\n          <Company>gbp</Company>\r\n          <Name>hkd</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Currency>\r\n    </Currencies>\r\n    <BenchMarks>\r\n      <Sector>\r\n        <Names>\r\n          <Company>first</Company>\r\n          <Name>last</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Sector>\r\n    </BenchMarks>\r\n  </MyFunds>\r\n</AllData>");
            var portfolioWithData = new Portfolio() { FilePath = "c:/temp/saved.xml" };
            _ = portfolioWithData.TryAdd(Account.Security, new NameData("company", "name", "GBP", "http://temp.com", new HashSet<string>() { "UK", "China" }, "some information"));
            _ = portfolioWithData.TryAddOrEditDataToSecurity(new TwoName("company", "name"), new System.DateTime(2015, 1, 1), new System.DateTime(2015, 1, 1), 5, 1.2, 6, null);
            _ = portfolioWithData.TryAdd(Account.BankAccount, new NameData("bank", "account"));
            _ = portfolioWithData.TryAdd(Account.Currency, new NameData("gbp", "hkd"));
            _ = portfolioWithData.TryAdd(Account.Benchmark, new NameData("first", "last"));
            yield return ("AccountsWithData", portfolioWithData, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>company</Company>\r\n          <Name>name</Name>\r\n          <Url>http://temp.com</Url>\r\n          <Currency>GBP</Currency>\r\n          <Sectors>\r\n            <string>UK</string>\r\n            <string>China</string>\r\n          </Sectors>\r\n          <Notes>some information</Notes>\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values>\r\n            <DV D=\"2015-01-01T00:00:00\" V=\"5\" />\r\n          </Values>\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values>\r\n            <DV D=\"2015-01-01T00:00:00\" V=\"1.2\" />\r\n          </Values>\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values>\r\n            <DV D=\"2015-01-01T00:00:00\" V=\"6\" />\r\n          </Values>\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>bank</Company>\r\n          <Name>account</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Amounts>\r\n          <Values />\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies>\r\n      <Currency>\r\n        <Names>\r\n          <Company>gbp</Company>\r\n          <Name>hkd</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Currency>\r\n    </Currencies>\r\n    <BenchMarks>\r\n      <Sector>\r\n        <Names>\r\n          <Company>first</Company>\r\n          <Name>last</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n      </Sector>\r\n    </BenchMarks>\r\n  </MyFunds>\r\n</AllData>");

            var testDatabases = TestDatabase.Databases;
            yield return (TestDatabaseName.OneBank.ToString(), testDatabases[TestDatabaseName.OneBank], "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <BaseCurrency>GBP</BaseCurrency>\r\n    <Funds />\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>Santander</Company>\r\n          <Name>Current</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"125.2\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"90.6\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"77.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"101.1\" />\r\n          </Values>\r\n        </Values>\r\n        <Amounts>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"125.2\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"90.6\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"77.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"101.1\" />\r\n          </Values>\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
            yield return (TestDatabaseName.OneSec.ToString(), testDatabases[TestDatabaseName.OneSec], "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <BaseCurrency>GBP</BaseCurrency>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>BlackRock</Company>\r\n          <Name>UK Stock</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"2\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"1.5\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"17.3\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"4\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"5.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"5.5\" />\r\n          </Values>\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"125.2\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"90.6\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"77.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"101.1\" />\r\n          </Values>\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n          </Values>\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts />\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
            yield return (TestDatabaseName.OneSecOneBank.ToString(), testDatabases[TestDatabaseName.OneSecOneBank], "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AllData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <MyFunds>\r\n    <FilePath>c:/temp/saved.xml</FilePath>\r\n    <BaseCurrency>GBP</BaseCurrency>\r\n    <Funds>\r\n      <Security>\r\n        <Names>\r\n          <Company>BlackRock</Company>\r\n          <Name>UK Stock</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values />\r\n        </Values>\r\n        <Shares>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"2\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"1.5\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"17.3\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"4\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"5.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"5.5\" />\r\n          </Values>\r\n        </Shares>\r\n        <UnitPrice>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"125.2\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"90.6\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"77.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"101.1\" />\r\n          </Values>\r\n        </UnitPrice>\r\n        <Investments>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n          </Values>\r\n        </Investments>\r\n      </Security>\r\n    </Funds>\r\n    <BankAccounts>\r\n      <CashAccount>\r\n        <Names>\r\n          <Company>Santander</Company>\r\n          <Name>Current</Name>\r\n          <Sectors />\r\n        </Names>\r\n        <Values>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"125.2\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"90.6\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"77.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"101.1\" />\r\n          </Values>\r\n        </Values>\r\n        <Amounts>\r\n          <Values>\r\n            <DV D=\"2010-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2011-01-01T00:00:00\" V=\"100\" />\r\n            <DV D=\"2012-05-01T00:00:00\" V=\"125.2\" />\r\n            <DV D=\"2015-04-03T00:00:00\" V=\"90.6\" />\r\n            <DV D=\"2018-05-06T00:00:00\" V=\"77.7\" />\r\n            <DV D=\"2020-01-01T00:00:00\" V=\"101.1\" />\r\n          </Values>\r\n        </Amounts>\r\n      </CashAccount>\r\n    </BankAccounts>\r\n    <Currencies />\r\n    <BenchMarks />\r\n  </MyFunds>\r\n</AllData>");
        }

        private static IEnumerable<TestCaseData> WriteSerializationData(string testName)
        {
            var tests = NewStyleTestLists();
            foreach (var test in tests)
            {
                yield return new TestCaseData(test.XmlString, test.testPortfolio).SetName($"{testName}-{test.name}");
            }
        }

        [TestCaseSource(nameof(WriteSerializationData), new object[] { nameof(WriteXmlTests) })]
        public void WriteXmlTests(string expectedXml, IPortfolio times)
        {
            var tempFileSystem = new MockFileSystem();
            string savePath = "c:/temp/saved.xml";

            times.SavePortfolio(savePath, tempFileSystem, null);

            string file = tempFileSystem.File.ReadAllText(savePath);

            Assert.AreEqual(expectedXml, file);
        }

        private static IEnumerable<TestCaseData> ReadSerializationData(string testName)
        {
            var oldTests = OldStyleTestLists();
            foreach (var test in oldTests)
            {
                yield return new TestCaseData(test.XmlString, test.testPortfolio).SetName($"{testName}old-{test.name}");
            }

            var newTests = NewStyleTestLists();
            foreach (var test in newTests)
            {
                yield return new TestCaseData(test.XmlString, test.testPortfolio).SetName($"{testName}-{test.name}");
            }
        }

        [TestCaseSource(nameof(ReadSerializationData), new object[] { nameof(ReadXmlTests) })]
        public void ReadXmlTests(string expectedXml, IPortfolio times)
        {
            var loadedPortfolio = new Portfolio();
            var tempFileSystem = new MockFileSystem();
            string savePath = "c:/temp/saved.xml";
            tempFileSystem.AddFile(savePath, new MockFileData(expectedXml));
            loadedPortfolio.LoadPortfolio(savePath, tempFileSystem, null);

            AreEqual(times, loadedPortfolio);
        }


        [TestCaseSource(nameof(WriteSerializationData), new object[] { nameof(RoundTripSaveTests) })]
        public void RoundTripSaveTests(string expectedXml, IPortfolio database)
        {
            var tempFileSystem = new MockFileSystem();
            string savePath = "c:/temp/saved.xml";

            database.SavePortfolio(savePath, tempFileSystem, null);

            string file = tempFileSystem.File.ReadAllText(savePath);

            Assert.AreEqual(expectedXml, file);

            var loadedPortfolio = new Portfolio();
            loadedPortfolio.LoadPortfolio(savePath, tempFileSystem, null);

            AreEqual(database, loadedPortfolio);
        }

        private void AreEqual(IPortfolio expected, IPortfolio actual)
        {
            if (expected == null | actual == null)
            {
                Assert.IsTrue(expected == null && actual == null);
            }

            Assert.AreEqual(expected.FilePath, actual.FilePath);

            if (expected.FundsThreadSafe.Count == actual.FundsThreadSafe.Count)
            {
                for (int i = 0; i < expected.FundsThreadSafe.Count; i++)
                {
                    var expectedSec = expected.FundsThreadSafe[i];
                    var actualSec = actual.FundsThreadSafe[i];
                    Assert.AreEqual(expectedSec.Names, actualSec.Names);
                }
            }
            else
            {
                Assert.IsTrue(false, "Funds dont have the same number.");
            }

            if (expected.BankAccountsThreadSafe.Count == actual.BankAccountsThreadSafe.Count)
            {
                for (int i = 0; i < expected.BankAccountsThreadSafe.Count; i++)
                {
                    var expectedSec = expected.BankAccountsThreadSafe[i];
                    var actualSec = actual.BankAccountsThreadSafe[i];
                    Assert.AreEqual(expectedSec.Names, actualSec.Names);
                }
            }
            else
            {
                Assert.IsTrue(false, "Funds dont have the same number.");
            }

            if (expected.CurrenciesThreadSafe.Count == actual.CurrenciesThreadSafe.Count)
            {
                for (int i = 0; i < expected.CurrenciesThreadSafe.Count; i++)
                {
                    var expectedSec = expected.CurrenciesThreadSafe[i];
                    var actualSec = actual.CurrenciesThreadSafe[i];
                    Assert.AreEqual(expectedSec.Names, actualSec.Names);
                }
            }
            else
            {
                Assert.IsTrue(false, "Funds dont have the same number.");
            }

            if (expected.BenchMarksThreadSafe.Count == actual.BenchMarksThreadSafe.Count)
            {
                for (int i = 0; i < expected.BenchMarksThreadSafe.Count; i++)
                {
                    var expectedBenchMark = expected.BenchMarksThreadSafe[i];
                    var actualBenchMark = actual.BenchMarksThreadSafe[i];
                    Assert.AreEqual(expectedBenchMark.Names, actualBenchMark.Names);
                }
            }
            else
            {
                Assert.IsTrue(false, "Funds dont have the same number.");
            }

            CollectionAssert.AreEqual(expected.BaseCurrency, actual.BaseCurrency);
        }
    }
}