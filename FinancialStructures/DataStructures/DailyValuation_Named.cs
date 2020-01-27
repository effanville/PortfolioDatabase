using System;

namespace FinancialStructures.DataStructures
{
    public class DailyValuation_Named : DailyValuation
    {
        public override string ToString()
        {
            //both name and company cannot be null so this is all cases.
            if (string.IsNullOrEmpty(Company)&& !string.IsNullOrEmpty(Name))
            {
                return Name + "-" + base.ToString();
            }
            if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Company))
            { 
                return Company + "-" + base.ToString();
            }

            return Company + "-" + Name + "-" + base.ToString();
        }

        /// <summary>
        /// Added company of the Daily valuation
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Added name of the daily valuation.
        /// </summary>
        public string Name { get; set; }

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
