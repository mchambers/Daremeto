using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DareyaAPI.Models
{
    public enum TransactionType
    {
        Unknown,
        AddFundsToCustomerBalance,
        RemoveFundsFromCustomerBalance
    }

    public enum TransactionReason
    {
        Unknown,
        CustomerPaidBounty,
        CustomerAwardedBounty,
        CustomerRequestedRefund,
        CustomerChargeback,
        Other
    }

    public enum TransactionSource
    {
        None,
        Automated,
        CustomerService,
        Manual,
        Other
    }

    public class TransactionGroup
    {
        public string UniqueID { get; set; }
        public long ChallengeID { get; set; }
        public string ChallengeStatusID { get; set; }
    }

    public class Transaction
    {
        public long ID { get; set; }
        public string TransactionBatchID { get; set; }
        public long CustomerID { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public TransactionReason Reason { get; set; }
        public TransactionSource Source { get; set; }
        public long AgentID { get; set; }
        public string Comment { get; set; }
    }
}