using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Stats;

using NUnit.Framework;

namespace Effanville.FPD.Logic.Tests
{
    [TestFixture]
    public sealed class ConfigurationTests
    {
        private readonly string _defaultSerializedConfiguration =
@"<UserConfiguration xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Effanville.FPD.Logic.Configuration"">
  <ProgramVersion xmlns:d2p1=""http://schemas.datacontract.org/2004/07/System"">
    <d2p1:_Build>3</d2p1:_Build>
    <d2p1:_Major>1</d2p1:_Major>
    <d2p1:_Minor>2</d2p1:_Minor>
    <d2p1:_Revision>4</d2p1:_Revision>
  </ProgramVersion>
  <ChildConfigurations xmlns:d2p1=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">
    <d2p1:KeyValueOfstringanyType>
      <d2p1:Key>StatsViewModel</d2p1:Key>
      <d2p1:Value i:type=""StatsDisplayConfiguration"">
        <ChildConfigurations />
        <DisplayValueFunds>false</DisplayValueFunds>
        <HasLoaded>false</HasLoaded>
      </d2p1:Value>
    </d2p1:KeyValueOfstringanyType>
    <d2p1:KeyValueOfstringanyType>
      <d2p1:Key>StatsCreatorWindowViewModel</d2p1:Key>
      <d2p1:Value i:type=""StatsCreatorConfiguration"">
        <ChildConfigurations>
          <d2p1:KeyValueOfstringanyType>
            <d2p1:Key>ExportStatsViewModel</d2p1:Key>
            <d2p1:Value i:type=""ExportStatsConfiguration"">
              <AssetColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"" />
              <AssetDirection>Ascending</AssetDirection>
              <AssetSortingField>AccountType</AssetSortingField>
              <BankColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"" />
              <BankDirection>Ascending</BankDirection>
              <BankSortingField>AccountType</BankSortingField>
              <CurrencyColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"" />
              <CurrencyDirection>Ascending</CurrencyDirection>
              <CurrencySortingField>AccountType</CurrencySortingField>
              <DisplayConditions xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"" />
              <HasLoaded>false</HasLoaded>
              <SectorColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"" />
              <SectorDirection>Ascending</SectorDirection>
              <SectorSortingField>AccountType</SectorSortingField>
              <SecurityColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"" />
              <SecurityDirection>Ascending</SecurityDirection>
              <SecuritySortingField>AccountType</SecuritySortingField>
            </d2p1:Value>
          </d2p1:KeyValueOfstringanyType>
          <d2p1:KeyValueOfstringanyType>
            <d2p1:Key>ExportHistoryViewModel</d2p1:Key>
            <d2p1:Value i:type=""ExportHistoryConfiguration"">
              <ChildConfigurations />
              <GenerateBankAccountValues>false</GenerateBankAccountValues>
              <GenerateSectorValues>false</GenerateSectorValues>
              <GenerateSecurityValues>false</GenerateSecurityValues>
              <HasLoaded>false</HasLoaded>
              <HistoryGapDays>0</HistoryGapDays>
            </d2p1:Value>
          </d2p1:KeyValueOfstringanyType>
          <d2p1:KeyValueOfstringanyType>
            <d2p1:Key>ExportReportViewModel</d2p1:Key>
            <d2p1:Value i:type=""ExportReportConfiguration"">
              <ChildConfigurations />
              <DisplayValueFunds>false</DisplayValueFunds>
              <HasLoaded>false</HasLoaded>
            </d2p1:Value>
          </d2p1:KeyValueOfstringanyType>
        </ChildConfigurations>
        <HasLoaded>false</HasLoaded>
      </d2p1:Value>
    </d2p1:KeyValueOfstringanyType>
  </ChildConfigurations>
</UserConfiguration>";

        private readonly string _exampleSerializedConfiguration =
@"<UserConfiguration xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.datacontract.org/2004/07/Effanville.FPD.Logic.Configuration"">
  <ProgramVersion xmlns:d2p1=""http://schemas.datacontract.org/2004/07/System"">
    <d2p1:_Build>3</d2p1:_Build>
    <d2p1:_Major>1</d2p1:_Major>
    <d2p1:_Minor>2</d2p1:_Minor>
    <d2p1:_Revision>4</d2p1:_Revision>
  </ProgramVersion>
  <ChildConfigurations xmlns:d2p1=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">
    <d2p1:KeyValueOfstringanyType>
      <d2p1:Key>StatsViewModel</d2p1:Key>
      <d2p1:Value i:type=""StatsDisplayConfiguration"">
        <ChildConfigurations />
        <DisplayValueFunds>true</DisplayValueFunds>
        <HasLoaded>true</HasLoaded>
        <StatisticNames xmlns:d5p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>AccountType</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Company</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Name</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Currency</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>LatestValue</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>UnitPrice</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>NumberUnits</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>MeanSharePrice</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>RecentChange</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>FundFraction</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>FundCompanyFraction</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Investment</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Profit</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Debt</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>IRR3M</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>IRR6M</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>IRR1Y</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>IRR5Y</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>IRRTotal</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>DrawDown</d5p1:Instance>
            <d5p1:Selected>false</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>MDD</d5p1:Instance>
            <d5p1:Selected>false</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Sectors</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>NumberOfAccounts</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>FirstDate</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>LastInvestmentDate</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>LastPurchaseDate</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>LatestDate</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>NumberEntries</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>EntryYearDensity</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
          <d5p1:SelectableOfStatisticAUSmO6kd>
            <d5p1:Instance>Notes</d5p1:Instance>
            <d5p1:Selected>true</d5p1:Selected>
          </d5p1:SelectableOfStatisticAUSmO6kd>
        </StatisticNames>
      </d2p1:Value>
    </d2p1:KeyValueOfstringanyType>
    <d2p1:KeyValueOfstringanyType>
      <d2p1:Key>StatsCreatorWindowViewModel</d2p1:Key>
      <d2p1:Value i:type=""StatsCreatorConfiguration"">
        <ChildConfigurations>
          <d2p1:KeyValueOfstringanyType>
            <d2p1:Key>ExportStatsViewModel</d2p1:Key>
            <d2p1:Value i:type=""ExportStatsConfiguration"">
              <AssetColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Company</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Name</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestValue</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>RecentChange</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Investment</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Profit</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Debt</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR3M</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR6M</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR1Y</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR5Y</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRRTotal</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FirstDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Sectors</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberEntries</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>EntryYearDensity</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Notes</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
              </AssetColumnNames>
              <AssetDirection>Ascending</AssetDirection>
              <AssetSortingField>Company</AssetSortingField>
              <BankColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Company</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Name</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Currency</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestValue</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>RecentChange</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FundFraction</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FundCompanyFraction</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FirstDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Sectors</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberEntries</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>EntryYearDensity</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Notes</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
              </BankColumnNames>
              <BankDirection>Ascending</BankDirection>
              <BankSortingField>Company</BankSortingField>
              <CurrencyColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Company</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Name</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestValue</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>RecentChange</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Investment</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Profit</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FundFraction</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberOfAccounts</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FirstDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Sectors</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberEntries</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>EntryYearDensity</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Notes</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
              </CurrencyColumnNames>
              <CurrencyDirection>Ascending</CurrencyDirection>
              <CurrencySortingField>Name</CurrencySortingField>
              <DisplayConditions xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>DisplayValueFunds</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>Spacing</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>Colours</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>ShowSecurites</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>ShowBankAccounts</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>ShowSectors</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>ShowBenchmarks</d8p1:Instance>
                  <d8p1:Selected>false</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>ShowAssets</d8p1:Instance>
                  <d8p1:Selected>false</d8p1:Selected>
                </d8p1:SelectableOfstring>
                <d8p1:SelectableOfstring>
                  <d8p1:Instance>ShowCurrencies</d8p1:Instance>
                  <d8p1:Selected>false</d8p1:Selected>
                </d8p1:SelectableOfstring>
              </DisplayConditions>
              <HasLoaded>true</HasLoaded>
              <SectorColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Company</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Name</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestValue</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>RecentChange</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Profit</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR3M</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR6M</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR1Y</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR5Y</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRRTotal</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberOfAccounts</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FirstDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberEntries</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>EntryYearDensity</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Notes</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
              </SectorColumnNames>
              <SectorDirection>Ascending</SectorDirection>
              <SectorSortingField>Name</SectorSortingField>
              <SecurityColumnNames xmlns:d8p1=""http://schemas.datacontract.org/2004/07/Effanville.Common.Structure.DisplayClasses"">
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Company</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Name</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Currency</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestValue</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>UnitPrice</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberUnits</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>MeanSharePrice</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>RecentChange</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FundFraction</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FundCompanyFraction</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Investment</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Profit</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR3M</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR6M</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR1Y</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRR5Y</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>IRRTotal</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>DrawDown</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>MDD</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>FirstDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LastInvestmentDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LastPurchaseDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>LatestDate</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Sectors</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>NumberEntries</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>EntryYearDensity</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
                <d8p1:SelectableOfStatisticAUSmO6kd>
                  <d8p1:Instance>Notes</d8p1:Instance>
                  <d8p1:Selected>true</d8p1:Selected>
                </d8p1:SelectableOfStatisticAUSmO6kd>
              </SecurityColumnNames>
              <SecurityDirection>Ascending</SecurityDirection>
              <SecuritySortingField>Company</SecuritySortingField>
            </d2p1:Value>
          </d2p1:KeyValueOfstringanyType>
          <d2p1:KeyValueOfstringanyType>
            <d2p1:Key>ExportHistoryViewModel</d2p1:Key>
            <d2p1:Value i:type=""ExportHistoryConfiguration"">
              <ChildConfigurations />
              <GenerateBankAccountValues>false</GenerateBankAccountValues>
              <GenerateSectorValues>false</GenerateSectorValues>
              <GenerateSecurityValues>false</GenerateSecurityValues>
              <HasLoaded>true</HasLoaded>
              <HistoryGapDays>20</HistoryGapDays>
            </d2p1:Value>
          </d2p1:KeyValueOfstringanyType>
          <d2p1:KeyValueOfstringanyType>
            <d2p1:Key>ExportReportViewModel</d2p1:Key>
            <d2p1:Value i:type=""ExportReportConfiguration"">
              <ChildConfigurations />
              <DisplayValueFunds>true</DisplayValueFunds>
              <HasLoaded>true</HasLoaded>
            </d2p1:Value>
          </d2p1:KeyValueOfstringanyType>
        </ChildConfigurations>
        <HasLoaded>true</HasLoaded>
      </d2p1:Value>
    </d2p1:KeyValueOfstringanyType>
  </ChildConfigurations>
</UserConfiguration>";

        [Test]
        public void CanLoadConfig()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string testPath = "c:/temp/user.config";
            string file = File.ReadAllText(TestConstants.ExampleDatabaseLocation + "\\user.config");
            tempFileSystem.AddFile(testPath, new MockFileData(file));
            UserConfiguration config = UserConfiguration.LoadFromUserConfigFile(testPath, tempFileSystem);

            Assert.That(config.ChildConfigurations.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                StatsDisplayConfiguration statsVM = config.ChildConfigurations[nameof(StatsViewModel)] as StatsDisplayConfiguration;
                if (statsVM == null)
                {
                    Assert.Fail("Stats config should exist");
                }

                Assert.That(statsVM.ChildConfigurations.Count, Is.EqualTo(0));
                Assert.That(statsVM.HasLoaded, Is.EqualTo(true));
                Assert.That(statsVM.DisplayValueFunds, Is.EqualTo(true));
                Assert.That(statsVM.StatisticNames.Count, Is.EqualTo(29));
                Assert.That(statsVM.StatisticNames.First(name => name.Instance == Statistic.AccountType).Selected, Is.EqualTo(false));
                Assert.That(statsVM.StatisticNames.First(name => name.Instance == Statistic.FundFraction).Selected, Is.EqualTo(false));
            });

            Assert.Multiple(() =>
            {
                StatsCreatorConfiguration statsCreatorVM = config.ChildConfigurations[UserConfiguration.StatsCreator] as StatsCreatorConfiguration;
                Assert.That(statsCreatorVM.ChildConfigurations.Count, Is.EqualTo(2));

                ExportStatsConfiguration exportStats = statsCreatorVM.ChildConfigurations[UserConfiguration.StatsOptions] as ExportStatsConfiguration;
                Assert.That(exportStats.HasLoaded, Is.EqualTo(true));

                ExportHistoryConfiguration exportHistory = statsCreatorVM.ChildConfigurations[UserConfiguration.HistoryOptions] as ExportHistoryConfiguration;
                Assert.That(exportHistory.HistoryGapDays, Is.EqualTo(20));
                Assert.That(exportHistory.HasLoaded, Is.EqualTo(true));
            });
        }

        [Test]
        public void CanLoadWithoutConfigFile()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string testPath = "c:/temp/user.config";
            UserConfiguration config = UserConfiguration.LoadFromUserConfigFile(testPath, tempFileSystem);

            Assert.That(config.ChildConfigurations.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                StatsDisplayConfiguration statsVM = config.ChildConfigurations[nameof(StatsViewModel)] as StatsDisplayConfiguration;
                Assert.That(statsVM.ChildConfigurations.Count, Is.EqualTo(0));
                Assert.That(statsVM.HasLoaded, Is.EqualTo(false));
                Assert.That(statsVM.DisplayValueFunds, Is.EqualTo(false));
            });

            Assert.Multiple(() =>
            {
                StatsCreatorConfiguration statsCreatorVM = config.ChildConfigurations[UserConfiguration.StatsCreator] as StatsCreatorConfiguration;
                Assert.That(statsCreatorVM.ChildConfigurations.Count, Is.EqualTo(3));

                ExportStatsConfiguration exportStats = statsCreatorVM.ChildConfigurations[UserConfiguration.StatsOptions] as ExportStatsConfiguration;
                Assert.That(exportStats.HasLoaded, Is.EqualTo(false));

                ExportHistoryConfiguration exportHistory = statsCreatorVM.ChildConfigurations[UserConfiguration.HistoryOptions] as ExportHistoryConfiguration;
                Assert.That(exportHistory.HistoryGapDays, Is.EqualTo(0));
                Assert.That(exportHistory.HasLoaded, Is.EqualTo(false));
            });
        }

        [Test]
        public void CanSaveDefaultConfig()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            UserConfiguration config = new UserConfiguration();

            config.ProgramVersion = new Version(1, 2, 3, 4);
            string testPath = "c:/temp/saved/user.config";
            config.SetConfigLocation(testPath, tempFileSystem);
            config.SaveConfiguration();

            string file = tempFileSystem.File.ReadAllText(testPath);
            Assert.That(file, Is.EqualTo(_defaultSerializedConfiguration));
        }

        [Test]
        public void CanSaveConfig()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            string testPath = "c:/temp/database.xml";

            UiGlobals globals = TestSetupHelper.SetupGlobalsMock(tempFileSystem, TestSetupHelper.CreateFileMock(testPath).Object, TestSetupHelper.CreateDialogMock().Object);

            SynchronousUpdater<IPortfolio> dataUpdater = new SynchronousUpdater<IPortfolio>();
            Common.Structure.DataEdit.SynchronousUpdater updater = new Common.Structure.DataEdit.SynchronousUpdater();
            string testConfigPath = "c:/temp/saved/user.config";
            UserConfiguration config = UserConfiguration.LoadFromUserConfigFile(
                testConfigPath,
                globals.CurrentFileSystem,
                globals.ReportLogger);

            var downloader = TestSetupHelper.SetupDownloader();
            config.ProgramVersion = new Version(1, 2, 3, 4);
            MainWindowViewModel vm = new MainWindowViewModel(globals,
                null,
                PortfolioFactory.GenerateEmpty(),
                dataUpdater,
                downloader,
                new ViewModelFactory(null, globals, dataUpdater, updater, downloader, config, null),
                config,
                null,
                null,
                null,
                null);
            vm.SaveConfig();

            string file = tempFileSystem.File.ReadAllText(testConfigPath);
            Assert.That(file, Is.EqualTo(_exampleSerializedConfiguration));
        }

        [Test]
        public void RoundTripTest()
        {
            MockFileSystem tempFileSystem = new MockFileSystem();
            UserConfiguration config = new UserConfiguration();
            config.ProgramVersion = new Version(1, 2, 3, 4);
            string testPath = "c:/temp/saved/user.config";
            config.SetConfigLocation(testPath, tempFileSystem);
            config.SaveConfiguration();

            string file = tempFileSystem.File.ReadAllText(testPath);
            Assert.That(file, Is.EqualTo(_defaultSerializedConfiguration));

            UserConfiguration newConfig = UserConfiguration.LoadFromUserConfigFile(testPath, tempFileSystem);

            Assert.That(newConfig.ChildConfigurations.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                StatsDisplayConfiguration statsVM = newConfig.ChildConfigurations[nameof(StatsViewModel)] as StatsDisplayConfiguration;
                Assert.That(statsVM.ChildConfigurations.Count, Is.EqualTo(0));
                Assert.That(statsVM.HasLoaded, Is.EqualTo(false));
                Assert.That(statsVM.DisplayValueFunds, Is.EqualTo(false));
            });

            Assert.Multiple(() =>
            {
                StatsCreatorConfiguration statsCreatorVM = newConfig.ChildConfigurations[UserConfiguration.StatsCreator] as StatsCreatorConfiguration;
                Assert.That(statsCreatorVM.ChildConfigurations.Count, Is.EqualTo(3));

                ExportStatsConfiguration exportStats = statsCreatorVM.ChildConfigurations[UserConfiguration.StatsOptions] as ExportStatsConfiguration;
                Assert.That(exportStats.HasLoaded, Is.EqualTo(false));

                ExportHistoryConfiguration exportHistory = statsCreatorVM.ChildConfigurations[UserConfiguration.HistoryOptions] as ExportHistoryConfiguration;
                Assert.That(exportHistory.HistoryGapDays, Is.EqualTo(0));
                Assert.That(exportHistory.HasLoaded, Is.EqualTo(false));
            });
        }
    }
}
