using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.Tests.TestHelpers;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

public static class PortfolioGeneratorHelper
{
    public static IPortfolio CreateFromTable(Table table)
    {
        IPortfolio portfolio = TestSetupHelper.CreateEmptyDataBase();
        if (table != null)
        {
            foreach (TableRow row in table.Rows)
            {
                string accountAsString = row["Account"];
                Account eff = Enum.Parse<Account>(accountAsString);
                NameData nameData = TableParsers.NameFromRow(row);
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
                Account eff = Enum.Parse<Account>(accountAsString);
                NameData nameData = TableParsers.NameFromRow(row);
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
                Account eff = Enum.Parse<Account>(accountAsString);
                NameData nameData = TableParsers.NameFromRow(row);
                portfolio.TryRemove(eff, nameData);
            }
        }
    }
}