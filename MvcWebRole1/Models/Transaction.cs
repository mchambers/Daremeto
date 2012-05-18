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
        CustomerAddedFunds,
        CustomerPaidBounty,
        CustomerAwardedBounty,
        CustomerRequestedRefund,
        CustomerChargeback,
        Other
    }

    public enum TransactionSource
    {
        Unknown,
        FundedByBillingProcessor,
        FundedFromBalance,
        Other
    }

    public enum TransactionState
    {
        Unknown,
        PendingFunds,
        Retry,
        Successful,
        ProcessorDeclined
    }

    public class TransactionGroup
    {
        public string UniqueID { get; set; }
        public long ChallengeID { get; set; }
        public string ChallengeStatusID { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /*
     * When a transaction is processed, if the debit account is at $0,
     * we automatically try to fund it from their funding source and
     * put the transaction into Pending until it clears.
     * 
     * If they had the money in their balance, we clear it automatically
     * and put a null ForeignTransactionID.
     * */
    public class Transaction
    {
        public long ID { get; set; }
        public string TransactionBatchID { get; set; }
        public long DebitAccountID { get; set; }        // who are we getting the money from?
        public long CreditAccountID { get; set; }      // who are we giving the money to?
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public TransactionReason Reason { get; set; }
        public TransactionSource Source { get; set; }
        public long AgentID { get; set; }
        public string Comment { get; set; }
        public TransactionState State { get; set; }
        public string ForeignTransactionID { get; set; } // ie Stripe Transaction ID, merchant auth code
        public int BillingProcessorType { get; set; }    // ie Stripe, PayPal
    }
}