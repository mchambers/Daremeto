using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DareyaAPI.Models;

namespace DaremetoWorker
{
    class BillingQueueItemProcessor : IQueueItemProcessor
    {
        private void PerformBillingForCompletedChallenge(long ChallengeID, string ChallengeStatusUniqueID)
        {
            List<ChallengeBid> bids = RepoFactory.GetChallengeBidRepo().Get(ChallengeID);
            IAccountRepository accountRepo=RepoFactory.GetAccountRepo();
            ITransactionRepository transRepo=RepoFactory.GetTransactionRepo();

            ChallengeStatus chalStatus=RepoFactory.GetChallengeStatusRepo().Get(ChallengeID, ChallengeStatusUniqueID);

            foreach (ChallengeBid b in bids)
            {
                decimal custBalance = RepoFactory.GetAccountRepo().BalanceForCustomerAccount(b.CustomerID);
                if (custBalance >= b.Amount)
                {
                    Transaction t = new Transaction() { Amount=(b.Amount - b.ComputedFees), 
                                                        DebitAccountID=b.CustomerID,
                                                        CreditAccountID=chalStatus.CustomerID, 
                                                        State=TransactionState.Successful, 
                                                        Source=TransactionSource.FundedFromBalance 
                                                      };

                    transRepo.RecordTransaction(t);

                    accountRepo.ModifyCustomerAccountBalance(t.DebitAccountID, -t.Amount);
                    accountRepo.ModifyCustomerAccountBalance(t.CreditAccountID, t.Amount);
                }
                else
                {

                }
            }

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
        }

        public void HandleQueueItem(DareyaAPI.ProcessingQueue.ProcessingQueueItem item)
        {
            throw new NotImplementedException();
        }
    }
}
