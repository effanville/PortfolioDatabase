using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.NamingStructures;

using Reqnroll;

namespace Effanville.FPD.Logic.Tests.TestHelpers;

public static class TableParsers
{
    public static NameData NameFromRow(DataTableRow row)
    {
        row.TryGetValue("Currency", out string currency);
        row.TryGetValue("Url", out string url);
        row.TryGetValue("Sectors", out string sectors);
        row.TryGetValue("Broker", out string broker);

        HashSet<string> sectorsSet = !string.IsNullOrEmpty(sectors)
            ? sectors?.Split(',').ToHashSet()
            : null;
        return new NameData(
            row["Company"],
            row["Name"],
            currency,
            url,
            sectorsSet)
        { Broker = broker };
    }
}