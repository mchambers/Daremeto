using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public interface IAccountRepository
    {
        decimal BalanceForCustomerAccount(long CustomerID);
        void ModifyCustomerAccountBalance(long CustomerID, decimal Amount);
        bool TransferFundsForTransaction(Transaction t);
        void AddToFeesCollectedAccount(decimal Amount);
        long CustomerIDForFeesAccount();
    }
}