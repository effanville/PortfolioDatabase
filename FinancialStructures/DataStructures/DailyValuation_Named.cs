using System;

namespace FinancialStructures.DataStructures
{
    public class DailyValuation_Named : DailyValuation
    {
        /// <summary>
        /// Added name of the daily valuation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Added company of the Daily valuation
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public DailyValuation_Named() : base()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DailyValuation_Named(string name, string company, DateTime day, double value) : base(day, value)
        {
            Name = name;
            Company = company;
        }

        /// <summary>
        /// Constructor to create an instance from a base class instance.
        /// </summary>
        public DailyValuation_Named(string name, string company, DailyValuation toAddOnto) : this(name, company, toAddOnto.Day, toAddOnto.Value)
        {
        }
    }
}
