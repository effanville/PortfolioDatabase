﻿using GlobalHeldData;
using FinancialStructures.FinanceStructures;
using FinancialStructures.GUIFinanceStructures;
using System;
using System.Collections.Generic;

namespace SecurityHelperFunctions
{
    /// <summary>
    /// Helper class to edit the securities held in the global database.
    /// </summary>
    public static class SecurityEditor
    {
        /// <summary>
        /// Tries to add a security to the underlying global database
        /// </summary>
        public static bool TryAddSecurity(string name, string company, string url)
        {
            return GlobalData.Finances.TryAddSecurityFromName(name, company, url);
        }

        /// <summary>
        /// Returns a security from the database with specified name and company.
        /// </summary>
        public static bool TryGetSecurity(string name, string company, out Security Desired)
        {
            return GlobalData.Finances.TryGetSecurity(name, company, out Desired);
        }

        /// <summary>
        /// Attempts to get the data from the security for display purposes 
        /// </summary>
        public static bool TryGetSecurityData(string name, string company, out List<BasicDayDataView> data)
        {
            return GlobalData.Finances.TryGetSecurityData(name, company, out data);
        }

        /// <summary>
        /// Returns true if security with given name and company exists.
        /// </summary>
        public static bool DoesSecurityExist(string name, string company)
        {
            return GlobalData.Finances.DoesSecurityExistFromName(name, company);
        }

        public static bool TryAddDataToSecurity(string name, string company, DateTime date, double shares, double unitPrice, double Investment = 0)
        {
            return GlobalData.Finances.TryAddDataToSecurity(name, company, date, shares, unitPrice, Investment);
        }
        /// <summary>
        /// Edits data in the security, if possible.
        /// </summary>
        public static bool TryEditSecurity(string name, string company, DateTime date, double shares, double unitPrice, double investment = 0)
        {
            return GlobalData.Finances.TryEditSecurity(name, company,date, shares, unitPrice, investment);
        }

        /// <summary>
        /// Renames the security if this exists.
        /// </summary>
        public static bool TryEditSecurityName(string name, string company, string newName, string newCompany, string url)
        {
            return GlobalData.Finances.TryEditSecurityNameCompany(name, company, newName, newCompany, url);
        }

        /// <summary>
        /// Deletes security if security exists. Does nothing otherwise.
        /// </summary>
        public static bool TryDeleteSecurity(string name, string company)
        {
            return GlobalData.Finances.TryRemoveSecurity(name, company);
        }

        public static bool TryDeleteSecurityData(string name, string company, DateTime date, double shares, double unitPrice, double investment = 0)
        {
            return GlobalData.Finances.TryRemoveSecurityData(name, company, date, shares, unitPrice, investment);
        }
    }
}