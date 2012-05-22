using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class AccountRepository : IAccountRepository
    {
        DMTDataRepositories.Database.DYDbEntities context;

        private const long ACCOUNT_ID_FEES = 1;

        public AccountRepository()
        {
            context = new DMTDataRepositories.Database.DYDbEntities();
        }

        public decimal BalanceForCustomerAccount(long CustomerID)
        {
            DMTDataRepositories.Database.Account a = context.Account.FirstOrDefault<DMTDataRepositories.Database.Account>(acct => acct.CustomerID == CustomerID);
            decimal balance = a.Balance;
            context.Detach(a);
            return a.Balance;
        }

        public void ModifyCustomerAccountBalance(long CustomerID, decimal Amount)
        {
            context.ModifyCustomerAccountBalance(CustomerID, Amount);
        }

        public void AddToFeesCollectedAccount(decimal Amount)
        {
            DMTDataRepositories.Database.Account a = context.Account.FirstOrDefault<DMTDataRepositories.Database.Account>(acct => acct.ID == ACCOUNT_ID_FEES);
            a.Balance += Amount;
            context.SaveChanges();
            context.Detach(a);
        }

        public bool TransferFundsForTransaction(Transaction t)
        {
            return (context.ModifyCustomerAccountsForTransaction(t.DebitAccountID, t.CreditAccountID, t.Amount) != 0);
        }

        public long CustomerIDForFeesAccount()
        {
            return 1;
        }
    }
}