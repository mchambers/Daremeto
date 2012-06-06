using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DareyaAPI.Models;
using DareyaAPI.BillingSystem;

namespace DaremetoWorker
{
    class BillingQueueItemProcessor : IQueueItemProcessor
    {
        /*
         * Billing for a completed challenge:
         * 
         *      For each bidder on the Challenge:
         *          Do they have the Actual Bid Amount in their Account Balance?
         *              yes:
         *                  create a transaction record: debit bidder, credit victor: Net Bid Amount
         *                  set transaction to status: Accepted
         *                  set transaction to source: FundedFromBalance
         *                  post the transaction
         *                  
         *              no:
         *                  attempt to add funds from the billing processor: Total Bid Amount
         *                  did the funding add succeed?
         *                      yes:
         *                          create a transaction record: debit bidder, credit victor: Net Bid Amount
         *                          create a transaction record: debit bidder, credit Fees Collected: Fees Amount
         *                          set both transactions' billing processor foreign transaction ID
         *                          set both transactions to status: Accepted
         *                          set both transactions to source: FundedByBillingProcessor
         *                          post transactions in batch
         *                          
         *                      no:
         *                          create a transaction record: debit bidder, credit victor: Net Bid Amount
         *                          create a transaction record: debit bidder, credit Fees Collected: Fees Amount
         *                          
         *                          set both transactions to status: ProcessorDeclined
         *                          post transactions in batch
         *                          
         *      Did all of the transactions go through?
         *          yes:
         *              set the ChallengeStatus State to Paid
         *              set the Challenge State to Paid
         *          no:
         *              set the ChallengeStatus State to PartialPaid
         *              set the Challenge State to PartialPaid
         * */

        private void PerformBillingForCompletedChallenge(long ChallengeID, long CustomerID)
        {
            List<ChallengeBid> bids = RepoFactory.GetChallengeBidRepo().Get(ChallengeID);
            IAccountRepository accountRepo=RepoFactory.GetAccountRepo();
            ITransactionRepository transRepo=RepoFactory.GetTransactionRepo();
            ChallengeStatus chalStatus=RepoFactory.GetChallengeStatusRepo().Get(CustomerID, ChallengeID);

            int bidsPaid = 0;

            if (chalStatus.Status != (int)ChallengeStatus.StatusCodes.Accepted)
            {
                System.Diagnostics.Trace.WriteLine("BILLING: Cust" + CustomerID.ToString() + "_Chal" + ChallengeID.ToString() + " - Caught a non-accepted challenge status attempting to be billed");
                return;
            }

            foreach (ChallengeBid b in bids)
            {
                Customer cust=RepoFactory.GetCustomerRepo().GetWithID(b.CustomerID);

                decimal custBalance = accountRepo.BalanceForCustomerAccount(b.CustomerID);
                if (custBalance >= b.Amount)
                {
                    Transaction t = new Transaction() { Amount=(b.Amount - b.ComputedFees), 
                                                        DebitAccountID=b.CustomerID,
                                                        CreditAccountID=chalStatus.CustomerID, 
                                                        State=TransactionState.PendingInternalTransfer, 
                                                        Source=TransactionSource.FundedFromBalance,
                                                        Reason=TransactionReason.CustomerAwardedBounty
                                                      };

                    if (accountRepo.TransferFundsForTransaction(t))
                    {
                        t.State = TransactionState.Successful;
                        bidsPaid++;
                    }

                    transRepo.RecordTransaction(t);
                }
                else
                {
                    Transaction netBountyTransaction = new Transaction()
                    {
                        Amount=(b.Amount-b.ComputedFees),
                        DebitAccountID=b.CustomerID,
                        CreditAccountID=chalStatus.CustomerID,
                        State=TransactionState.PendingFunds,
                        Source=TransactionSource.FundedFromBillingProcessor,
                        Reason=TransactionReason.CustomerAwardedBounty
                    };

                    Transaction feesTransaction = new Transaction()
                    {
                        Amount=b.ComputedFees,
                        DebitAccountID=b.CustomerID,
                        CreditAccountID=chalStatus.CustomerID,
                        State=TransactionState.PendingFunds,
                        Source=TransactionSource.FundedFromBillingProcessor,
                        Reason=TransactionReason.CustomerAddedFunds
                    };

                    IBillingProcessor billingProcessor = BillingProcessorFactory.GetBillingProcessor((BillingProcessorFactory.SupportedBillingProcessor)cust.BillingType);

                    BillingProcessorResult chargeResult = billingProcessor.Charge(cust.BillingID, b.Amount);

                    netBountyTransaction.ForeignTransactionID = chargeResult.ForeignTransactionID;
                    feesTransaction.ForeignTransactionID = chargeResult.ForeignTransactionID;

                    if (chargeResult.Result == BillingProcessorResult.BillingProcessorResultCode.Paid)
                    {
                        // we got the money!! BOOYAH. stick it in their account.
                        // TODO: We should make sure this succeeds or we could see negative account balances.
                        accountRepo.ModifyCustomerAccountBalance(cust.ID, b.Amount);

                        netBountyTransaction.State = TransactionState.PendingInternalTransfer;
                        feesTransaction.State = TransactionState.PendingInternalTransfer;

                        // attempt the funds transfer.
                        // if we move the money successfully, update the transaction state.
                        if (accountRepo.TransferFundsForTransaction(netBountyTransaction))
                            netBountyTransaction.State = TransactionState.Successful;

                        if (accountRepo.TransferFundsForTransaction(feesTransaction))
                            feesTransaction.State = TransactionState.Successful;

                        if(netBountyTransaction.State==TransactionState.Successful && feesTransaction.State==TransactionState.Successful)
                            bidsPaid++; //only if the entire thing was successful
                    }
                    else
                    {
                        // we no get the money.
                        netBountyTransaction.State = TransactionState.ProcessorDecline;
                        feesTransaction.State = TransactionState.ProcessorDecline;

                        // since they don't have the money, don't perform any transfers.
                    }

                    List<Transaction> transBatch = new List<Transaction>(2);
                    transBatch.Add(netBountyTransaction);
                    transBatch.Add(feesTransaction);

                    // TODO: This transaction ID might not be unique if we have to keep retrying this batch.
                    transRepo.RecordTransactionBatch(transBatch, "Chal"+chalStatus.ChallengeID+"+Cust"+chalStatus.CustomerID+"+"+System.DateTime.Now.Ticks.ToString());
                }
            }

            if (bidsPaid >= bids.Count)
                chalStatus.Status = (int)ChallengeStatus.StatusCodes.Paid;
            else if (bidsPaid < bids.Count)
                chalStatus.Status = (int)ChallengeStatus.StatusCodes.PartialPaid;

            RepoFactory.GetChallengeStatusRepo().Update(chalStatus); // plz work
        }

        public void HandleQueueItem(DareyaAPI.ProcessingQueue.ProcessingQueueItem item)
        {
            long chalID = item.Data["ChalID"];
            long custID = item.Data["CustID"];

            PerformBillingForCompletedChallenge(chalID, custID);
        }
    }
}
