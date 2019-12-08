using System;
using System.Collections.Generic;
using FinancialStructures.FinanceStructures;
using GlobalHeldData;
using FinancialStructures.GUIFinanceStructures;
using FinancialStructures.ReportingStructures;

namespace BankAccountHelperFunctions
{
    /// <summary>
    /// Helper class to edit the bank accounts held in the global database.
    /// </summary>
    public static class BankAccountEditor
    {
        /// <summary>
        /// Tries to add a CashAccount to the underlying global database
        /// </summary>
        public static bool TryAddBankAccount(string name, string company, ErrorReports reports)
        {
            return GlobalData.Finances.TryAddBankAccountFromName(name, company, reports);
        }

        /// <summary>
        /// Returns a CashAccount from the database with specified name and company.
        /// </summary>
        public static bool TryGetBankAccount(string name, string company, out CashAccount Desired)
        {
            return GlobalData.Finances.TryGetBankAccount(name, company, out Desired);
        }

        /// <summary>
        /// Attempts to get the data from the bank account for display purposes 
        /// </summary>
        public static bool TryGetBankAccountData(string name, string company, out List<AccountDayDataView> data)
        {
            return GlobalData.Finances.TryGetAccountData(name, company, out data);
        }

        /// <summary>
        /// Returns true if bankAccount with given name and company exists.
        /// </summary>
        public static bool DoesBankAccountExist(string name, string company)
        {
            return GlobalData.Finances.DoesBankAccountExistFromName(name, company);
        }

        /// <summary>
        /// Edits data in the bankaccount, if possible.
        /// </summary>
        public static bool TryEditBankAccount(string name, string company, DateTime date, double value, ErrorReports reports)
        {
            return GlobalData.Finances.TryEditBankAccount(name, company, date, value, reports);
        }

        /// <summary>
        /// Renames the BankAccount if this exists.
        /// </summary>
        public static bool TryEditBankAccountName(string name, string company, string newName, string newCompany, ErrorReports reports)
        {
            return GlobalData.Finances.TryEditCashAcountNameCompany(name, company, newName, newCompany, reports);
        }

        public static bool TryAddDataToBankAccount(string name, string company, DateTime date, double value)
        {
            return GlobalData.Finances.TryAddDataToBankAccount(name, company, date, value);
        }

        /// <summary>
        /// Deletes BankAccount if it exists. Does nothing otherwise.
        /// </summary>
        public static bool TryDeleteBankAccount(string name, string company, ErrorReports reports)
        {
            return GlobalData.Finances.TryRemoveBankAccount(name, company, reports);
        }

        /// <summary>
        /// Deletes the data from the date specified if it exists.
        /// </summary>
        public static bool TryDeleteBankAccountData(string name, string company, DateTime date, ErrorReports reports)
        {
            return GlobalData.Finances.TryDeleteBankAccountData(name, company, date, reports);
        }

        public static double AllBankAccountValue(DateTime date)
        {
            return GlobalData.Finances.AllBankAccountsValue(date);
        }
    }
}
