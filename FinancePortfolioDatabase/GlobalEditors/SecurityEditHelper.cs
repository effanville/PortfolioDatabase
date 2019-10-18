using GlobalHeldData;
using FinancePortfolioDatabase;
using System;

namespace SecurityHelperFunctions
{
    /// <summary>
    /// Helper class to edit the securities held in the global database.
    /// </summary>
    public static class SecurityEditHelper
    {
        /// <summary>
        /// Tries to add a security to the underlying global database
        /// </summary>
        public static bool TryAddSecurity(string name, string company)
        {
            return GlobalData.Finances.TryAddSecurityFromName(name, company);
        }

        /// <summary>
        /// Returns a security from the database with specified name and company.
        /// </summary>
        public static bool TryGetSecurity(string name, string company, out Security Desired)
        {
            return GlobalData.Finances.TryGetSecurity(name, company, out Desired);
        }

        /// <summary>
        /// Returns true if security with given name and company exists.
        /// </summary>
        public static bool DoesSecurityExist(string name, string company)
        {
            return GlobalData.Finances.DoesSecurityExistFromName(name, company);
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
        public static bool TryEditSecurityName(string name, string company, string newName, string newCompany)
        {
            return GlobalData.Finances.TryEditSecurityNameCompany(name, company, newName, newCompany);
        }

        /// <summary>
        /// Deletes security if security exists. Does nothing otherwise.
        /// </summary>
        public static bool TryDeleteSecurity(string name, string company)
        {
            return GlobalData.Finances.TryRemoveSecurity(name, company);
        }
    }
}
