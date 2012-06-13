using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DareyaAPI.Models
{
    public class AccountRepository : IAccountRepository
    {
        private const long ACCOUNT_ID_FEES = 0;
        private string connStr;

        public AccountRepository()
        {
            connStr = ConfigurationManager.ConnectionStrings["DMTPrimary"].ConnectionString;
        }

        public decimal BalanceForCustomerAccount(long CustomerID)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var b = conn.Query("spAccountGetBalance", new { CustomerID=CustomerID }, commandType: CommandType.StoredProcedure);
                var record = b.FirstOrDefault();
                try
                {
                    if (record != null)
                        return record.Balance;
                    else
                        return 0m;
                }
                catch
                {
                    return 0m;
                }
                
            }
        }

        public void ModifyCustomerAccountBalance(long CustomerID, decimal Amount)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                conn.Execute("ModifyCustomerAccountBalance", new { CustomerID = CustomerID, Amount = Amount }, commandType: CommandType.StoredProcedure);
            }
        }

        public void AddToFeesCollectedAccount(decimal Amount)
        {
            throw new NotImplementedException();
        }

        public bool TransferFundsForTransaction(Transaction t)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                if (conn.Execute("ModifyCustomerAccountsForTransaction", new { DebitCustomerID = t.DebitCustomerID, CreditCustomerID = t.CreditCustomerID, Amount = t.Amount }, commandType: CommandType.StoredProcedure) > 0)
                    return true;
                else
                    return false;
            }
        }

        public long CustomerIDForFeesAccount()
        {
            return 0;
        }
    }
}