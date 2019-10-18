using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancePortfolioDatabase;
using GlobalHeldData;

namespace BankAccountHelperFunctions
{
    /// <summary>
    /// Helper class to edit the bank accounts held in the global database.
    /// </summary>
    public static class BankAccountEditHelper
    {
        /// <summary>
        /// Tries to add a CashAccount to the underlying global database
        /// </summary>
        public static bool TryAddBankAccount(string name, string company)
        {
            return GlobalData.Finances.TryAddBankAccountFromName(name, company);
        }

        /// <summary>
        /// Returns a CashAccount from the database with specified name and company.
        /// </summary>
        public static bool TryGetBankAccount(string name, string company, out CashAccount Desired)
        {
            return GlobalData.Finances.TryGetBankAccount(name, company, out Desired);
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
        public static bool TryEditBankAccount(string name, string company, DateTime date, double value)
        {
            return GlobalData.Finances.TryEditBankAccount(name, company, date, value);
        }

        /// <summary>
        /// Renames the BankAccount if this exists.
        /// </summary>
        public static bool TryEditBankAccountName(string name, string company, string newName, string newCompany)
        {
            return GlobalData.Finances.TryEditCashAcountNameCompany(name, company, newName, newCompany);
        }

        /// <summary>
        /// Deletes BankAccount if it exists. Does nothing otherwise.
        /// </summary>
        public static bool TryDeleteBankAccount(string name, string company)
        {
            return GlobalData.Finances.TryRemoveBankAccount(name, company);
        }
    }
}
