using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DareyaAPI.Models
{
    public interface ITransactionRepository
    {
        string RecordTransactionBatch(List<Transaction> Transactions, string TransactionGroupID = "");
        bool RecordTransaction(Transaction t);
        bool RecordTransaction(long CustomerID, float Amount, TransactionType Type, TransactionReason Reason);
    }
}
