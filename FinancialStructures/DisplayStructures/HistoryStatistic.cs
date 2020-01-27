﻿using FinancialStructures.Database;
using FinancialStructures.DataStructures;
using FinancialStructures.Mathematics;
using System;
using System.Collections.Generic;

namespace FinancialStructures.DisplayStructures
{
    public class HistoryStatistic
    {
        public string Headers()
        {
            string outputCSVStyle = string.Empty;
            outputCSVStyle += string.Concat("Date, TotalValue", ",");

            outputCSVStyle += string.Concat("BankTotal", ",");

            outputCSVStyle += string.Concat("SecurityTotal", ",");
            foreach (var value in SecurityValues)
            {
                outputCSVStyle += string.Concat(value.Company, ",");
            }
            foreach (var value in BankAccValues)
            {
                outputCSVStyle += string.Concat(value.Company, ",");
            }
            foreach (var value in SectorValues)
            {
                outputCSVStyle += string.Concat(value.Company, ",");
            }

            return outputCSVStyle;
        }
        public override string ToString()
        {
            string outputCSVStyle = string.Empty;
            outputCSVStyle += string.Concat(TotalValue.ToString(), ",");

            outputCSVStyle += string.Concat(BankAccValue.Value, ",");

            outputCSVStyle += string.Concat(SecurityValue.Value, ",");
            foreach (var value in SecurityValues)
            {
                outputCSVStyle += string.Concat( value.Value, ",");
            }
            foreach (var value in BankAccValues)
            { 
                outputCSVStyle += string.Concat(value.Value, ","); 
            }
            foreach (var value in SectorValues)
            { 
                outputCSVStyle += string.Concat(value.Value, ",");
            }

            return outputCSVStyle;
        }
        public DailyValuation TotalValue { get; set; }

        public DailyValuation BankAccValue { get; set; }

        public DailyValuation SecurityValue { get; set; }

        public List<DailyValuation_Named> SecurityValues { get; set; } = new List<DailyValuation_Named>();

        public string SecurityValuesString
        {
            get
            {
                var stringRepresentation = string.Empty;
                foreach (var data in SecurityValues)
                {
                    stringRepresentation = stringRepresentation + "  " + data.ToString();
                }
                return stringRepresentation;
            }
        }
        public List<DailyValuation_Named> Security1YrCar { get; set; } = new List<DailyValuation_Named>();
        public List<DailyValuation_Named> SecurityTotalCar { get; set; } = new List<DailyValuation_Named>();

        public List<DailyValuation_Named> BankAccValues { get; set; } = new List<DailyValuation_Named>();

        public string BankAccValuesString
        {
            get 
            {
                var stringRepresentation = string.Empty;
                foreach (var data in BankAccValues)
                {
                    stringRepresentation = stringRepresentation + "  " + data.ToString();
                }
                return stringRepresentation;
            } 
        }

        public List<DailyValuation_Named> SectorValues { get; set; } = new List<DailyValuation_Named>();

        public string SectorValuesString
        {
            get
            {
                var stringRepresentation = string.Empty;
                foreach (var data in SectorValues)
                {
                    stringRepresentation = stringRepresentation + "  " + data.ToString();
                }
                return stringRepresentation;
            }
        }

        public List<DailyValuation_Named> SectorCar { get; set; } = new List<DailyValuation_Named>();

        public HistoryStatistic()
        { }

        public HistoryStatistic(DailyValuation value, List<DailyValuation_Named> securityValues, List<DailyValuation_Named> bankValues)
        {
            TotalValue = value;
            SecurityValues = securityValues;
            BankAccValues = bankValues;
        }

        public HistoryStatistic(Portfolio portfolio, DateTime date)
        {
            TotalValue = new DailyValuation(date, MathSupport.Truncate(portfolio.Value(date)));
            BankAccValue = new DailyValuation(date, MathSupport.Truncate(portfolio.AllBankAccountsValue(date)));
            SecurityValue = new DailyValuation(date, MathSupport.Truncate(portfolio.AllSecuritiesValue(date)));
            var companyNames = portfolio.GetSecuritiesCompanyNames();
            foreach (var companyName in companyNames)
            {
                var companyValue = new DailyValuation_Named(null, companyName, date, MathSupport.Truncate(portfolio.SecurityCompanyValue(companyName, date)));
                SecurityValues.Add(companyValue);
                var yearCar = new DailyValuation_Named(null, companyName, date, MathSupport.Truncate(portfolio.IRRCompany(companyName, date.AddDays(-365), date)));
                Security1YrCar.Add(yearCar);
                DailyValuation_Named totalCar;
                if (date < portfolio.CompanyFirstDate(companyName))
                {
                    totalCar = new DailyValuation_Named(null, companyName, date, 0.0);
                }
                else
                {
                    totalCar = new DailyValuation_Named(null, companyName, date, MathSupport.Truncate(portfolio.IRRCompany(companyName, portfolio.CompanyFirstDate(companyName), date)));
                }
                SecurityTotalCar.Add(totalCar);
            }

            var companyBankNames = portfolio.GetBankAccountCompanyNames();
            foreach (var companyName in companyBankNames)
            {
                var companyValue = new DailyValuation_Named(null, companyName, date, MathSupport.Truncate(portfolio.BankAccountCompanyValue(companyName,date)));
                BankAccValues.Add(companyValue);
            }

            var sectorNames = portfolio.GetSecuritiesSectors();
            foreach (var sectorName in sectorNames)
            {
                var sectorValue = new DailyValuation_Named(null, sectorName, date, MathSupport.Truncate(portfolio.SectorValue(sectorName, date)));
                SectorValues.Add(sectorValue);
                DailyValuation_Named sectorCar;
                if (date < portfolio.SectorFirstDate(sectorName))
                {
                    sectorCar = new DailyValuation_Named(null, sectorName, date, 0.0);
                }
                else
                {
                    sectorCar = new DailyValuation_Named(null, sectorName, date, MathSupport.Truncate(portfolio.IRRSector(sectorName, portfolio.SectorFirstDate(sectorName), date)));
                }
                SectorCar.Add(sectorCar);
            }
        }
    }
}
