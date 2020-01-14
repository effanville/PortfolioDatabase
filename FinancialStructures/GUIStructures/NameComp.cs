﻿using System;
using System.Collections.Generic;

namespace FinancialStructures.GUIFinanceStructures
{
    public class NameComp : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is NameComp value)
            {
                if (Company == value.Company)
                {
                    if (Name == null)
                    {
                        if (value.Name == null)
                        {
                            return 0;
                        }
                        return 1;
                    }
                    return Name.CompareTo(value.Name);
                }

                return Company.CompareTo(value.Company);
            }

            return 0;
        }

        public bool NewValue { get; set; }

        public NameComp()
        {
            NewValue = true;
        }

        public NameComp(string n, string c, string currency, string u, List<string> s, bool newValue = true)
        {
            Name = n;
            Company = c;
            Currency = currency;
            Url = u;
            Sectors = $"{string.Join(", ", s)}";
            NewValue = newValue;
        }

        public NameComp(string n, string c, string currency, string u, bool newValue = true)
        {
            Name = n;
            Company = c;
            Currency = currency;
            Url = u;
            NewValue = newValue;
        }

        public NameComp(string n, string c, bool newValue = true)
        {
            Name = n;
            Company = c;
            NewValue = newValue;
        }

        private string fUrl;

        public string Url
        {
            get { return fUrl; }
            set { fUrl = value; NewValue = true; }
        }

        private string fName;

        public string Name
        {
            get { return fName; }
            set { fName = value; NewValue = true; }
        }

        private string fCompany;

        public string Company
        {
            get { return fCompany; }
            set { fCompany = value; NewValue = true; }
        }

        private string fCurrency;

        public string Currency
        {
            get { return fCurrency; }
            set { fCurrency = value; NewValue = true; }
        }

        private string fSectors;

        public string Sectors 
        { 
            get { return fSectors; }
            set { fSectors = value; NewValue = true; }
        }
    }
}
