using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        public string RecordTransactionBatch(List<Transaction> Transactions, string TransactionGroupID = "")
        {
            TransactionGroup tg = new TransactionGroup();

            if (TransactionGroupID.Equals(""))
                TransactionGroupID = System.Guid.NewGuid().ToString();

            tg.UniqueID = TransactionGroupID;

            foreach (Transaction t in Transactions)
            {
                t.TransactionBatchID = tg.UniqueID;
            }

            // record transactions in ... a transaction
            // mind=blown

            return tg.UniqueID;
        }

        public bool RecordTransaction(Transaction t)
        {
            return false;
        }

        public bool RecordTransaction(long CustomerID, float Amount, TransactionType Type, TransactionReason Reason)
        {
            return false;
        }
    }
}