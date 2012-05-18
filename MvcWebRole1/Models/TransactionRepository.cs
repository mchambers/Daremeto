using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace DareyaAPI.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        private Database.DYDbEntities context;

        public TransactionRepository()
        {
            context = new Database.DYDbEntities();

            Mapper.CreateMap<Database.Transaction, Transaction>();
            Mapper.CreateMap<Transaction, Database.Transaction>();
        }

        public string RecordTransactionBatch(List<Transaction> Transactions, string TransactionGroupID = "")
        {
            TransactionGroup tg = new TransactionGroup();

            if (TransactionGroupID.Equals(""))
                TransactionGroupID = System.Guid.NewGuid().ToString();

            tg.UniqueID = TransactionGroupID;

            foreach (Transaction t in Transactions)
            {
                t.TransactionBatchID = tg.UniqueID;
                context.Transaction.AddObject(Mapper.Map<Transaction, Database.Transaction>(t));
            }
            
            context.SaveChanges();

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