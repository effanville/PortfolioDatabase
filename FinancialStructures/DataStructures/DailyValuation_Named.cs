using FinancialStructures.Mathematics;
using PortfolioStatsCreatorHelper;
using StringFunctions;
using System;
using System.Collections.Generic;

namespace FinancialStructures.DataStructures
{
    public class DailyValuation_Named : DailyValuation
    {
        public string HTMLHeader(UserOptions options, List<string> names, int maxNumLength, int maxNameLength, int maxCompanyLength)
        {
            string htmlHeader = string.Empty;
            var properties = this.GetType().GetProperties();
            foreach (var props in properties)
            {
                if (names.Contains(props.Name))
                {
                    if (props.PropertyType == typeof(string))
                    {
                        if (props.Name == "Name")
                        {
                            htmlHeader += "<b>" + props.Name.WithMaxLength(maxNameLength - 2).PadRight(maxNameLength) + "</b>";
                        }
                        else
                        {
                            htmlHeader += "<b>" + props.Name.WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength) + "</b>";
                        }

                    }

                    if (props.PropertyType == typeof(double))
                    {
                        htmlHeader += "<b>" + props.Name.WithMaxLength(maxNumLength - 2).PadLeft(maxNumLength) + "</b>";
                    }
                }
            }
            return htmlHeader;
        }

        public string HTMLData(UserOptions options, List<string> names, int maxNumLength, int maxNameLength, int maxCompanyLength)
        {
            var properties = this.GetType().GetProperties();
            string output = string.Empty;
            foreach (var prop in properties)
            {
                if (names.Contains(prop.Name))
                {
                    if (Double.TryParse(prop.GetValue(this).ToString(), out double result))
                    {
                        output += result.ToString().PadLeft(maxNumLength);
                    }
                    else
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            if (prop.Name == "Name")
                            {
                                output += prop.GetValue(this).ToString().WithMaxLength(maxNameLength - 2).PadRight(maxNameLength);
                            }
                            else
                            {
                                output += prop.GetValue(this).ToString().WithMaxLength(maxCompanyLength - 2).PadRight(maxCompanyLength);
                            }
                        }
                    }
                }
            }
            return output;
        }

        public string HTMLTableHeader(UserOptions options, List<string> names)
        {
            var properties = this.GetType().GetProperties();
            string htmlHeader = string.Empty;
            foreach (var property in properties)
            {
                if (names.Contains(property.Name))
                {
                    htmlHeader += "<th scope=\"col\">";
                    htmlHeader += property.Name;
                    htmlHeader += "</th>";
                }
            }

            return htmlHeader;
        }

        public string HTMLTableData(UserOptions options, List<string> names)
        {
            var properties = this.GetType().GetProperties();
            string htmlData = "<th scope=\"row\">";

            for (int i = 0; i < properties.Length; i++)
            {
                if (names.Contains(properties[i].Name))
                {
                    if (i != 0)
                    {
                        htmlData += "<td>";
                    }
                    if (double.TryParse(properties[i].GetValue(this).ToString(), out double value))
                    {
                        htmlData += MathSupport.Trunc(value);
                    }
                    else
                    {
                        htmlData += properties[i].GetValue(this);
                    }

                    htmlData += "</td>";
                }
            }
            htmlData += "</th>";
            return htmlData;
        }

        public override string ToString()
        {
            //both name and company cannot be null so this is all cases.
            if (string.IsNullOrEmpty(Company) && !string.IsNullOrEmpty(Name))
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
