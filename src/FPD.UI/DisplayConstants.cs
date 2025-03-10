﻿using System;
using System.Collections.Generic;
using System.Linq;

using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FinancialStructures.DataStructures;

namespace Effanville.FPD.UI
{
    /// <summary>
    /// Container class for various constant values to be used in the display.
    /// </summary>
    public sealed class DisplayConstants
    {
        public const string StyleBridgeName = "Bridge";
            
        /// <summary>
        /// A list of all possible <see cref="TradeType"/> values.
        /// </summary>
        public static IReadOnlyList<TradeType> TradeTypes => Enum.GetValues(typeof(TradeType)).Cast<TradeType>().ToList();

        /// <summary>
        /// List of all applicable statistics for Securities.
        /// </summary>
        public static IReadOnlyList<Statistic> SecurityFieldNames => AccountStatisticsHelpers.DefaultSecurityStats();

        /// <summary>
        /// List of all applicable statistics for Bank accounts.
        /// </summary>
        public static IReadOnlyList<Statistic> BankFieldNames => AccountStatisticsHelpers.DefaultBankAccountStats();

        /// <summary>
        /// List of all applicable statistics for sectors.
        /// </summary>
        public static IReadOnlyList<Statistic> SectorFieldNames => AccountStatisticsHelpers.DefaultSectorStats();

        /// <summary>
        /// List of all applicable statistics for sectors.
        /// </summary>
        public static IReadOnlyList<Statistic> AssetFieldNames => AccountStatisticsHelpers.DefaultAssetStats();

        /// <summary>
        /// List of all possible sorting directions.
        /// </summary>
        public static IReadOnlyList<SortDirection> SortDirections => Enum.GetValues(typeof(SortDirection)).Cast<SortDirection>().ToList();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DisplayConstants()
        {
        }
    }
}
