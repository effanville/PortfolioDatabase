using System;
using System.Linq;

using Effanville.FPD.Logic.Tests.TestHelpers;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

public static class PortfolioGeneratorHelper
{
    public static IPortfolio CreateFromTable(Table table)
    {
        var portfolio = TestSetupHelper.CreateEmptyDataBase();
        if (table != null)
        {
            foreach (TableRow row in table.Rows)
            {
                string accountAsString = row["Account"];
                var eff = Enum.Parse<Account>(accountAsString);
                var nameData = NameDataFromRow(row);
                portfolio.TryAdd(eff, nameData);
            }
        }

        return portfolio;
    }    
    public static void UpdateModelData(IPortfolio portfolio, Table table)
    {
        if (table != null)
        {
            foreach (TableRow row in table.Rows)
            {
                string accountAsString = row["Account"];
                var eff = Enum.Parse<Account>(accountAsString);
                var nameData = NameDataFromRow(row);
                portfolio.TryAdd(eff, nameData);
            }
        }
    }
    
    public static void RemoveModelData(IPortfolio portfolio, Table table)
    {
        if (table != null)
        {
            foreach (TableRow row in table.Rows)
            {
                string accountAsString = row["Account"];
                var eff = Enum.Parse<Account>(accountAsString);
                var nameData = NameDataFromRow(row);
                portfolio.TryRemove(eff, nameData);
            }
        }
    }

    public static NameData NameDataFromRow(TableRow row)
    { 
        row.TryGetValue("Currency", out var currency);
        row.TryGetValue("Url", out var url);
        row.TryGetValue("Sectors", out var sectors);
        var sectorsSet = !string.IsNullOrEmpty(sectors)
            ? sectors?.Split(',').ToHashSet()
            : null;
        return new NameData(
            row["Company"],
            row["Name"],
            currency,
            url,
            sectorsSet);
    }
     
}