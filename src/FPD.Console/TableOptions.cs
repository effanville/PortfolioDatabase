using System.Collections.Generic;
using System.Text.Json.Serialization;

using Effanville.FinancialStructures.Database.Statistics;

namespace Effanville.FPD.Console;

public class TableOptions
{
    /// <summary>
    /// Should this table be displayed.
    /// </summary>
    public bool ShouldDisplay { get; set; }

    /// <summary>
    /// What field to sort the table by.
    /// </summary>
    public Statistic SortingField { get; set; }

    /// <summary>
    /// In which direction to sort the table.
    /// </summary>
    public SortDirection SortingDirection { get; set; }

    /// <summary>
    /// What fields to display in the table.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public List<Statistic> DisplayFields { get; set; }
}
