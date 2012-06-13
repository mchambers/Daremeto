using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace DareyaAPI.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        private string connStr;

        public TransactionRepository()
        {
            connStr = ConfigurationManager.ConnectionStrings["DMTPrimary"].ConnectionString;
        }

        private DynamicParameters TransToDynParm(Transaction t)
        {
            DynamicParameters p = new DynamicParameters();

            p.Add("@TransactionBatchID", t.TransactionBatchID, DbType.String, ParameterDirection.Input);
            p.Add("@DebitCustomerID", t.DebitCustomerID, DbType.Int64, ParameterDirection.Input);
            p.Add("@CreditCustomerID", t.CreditCustomerID, DbType.Int64, ParameterDirection.Input);
            p.Add("@Type", (int)t.Type, DbType.Int32, ParameterDirection.Input);
            p.Add("@Amount", t.Amount, DbType.Decimal, ParameterDirection.Input);
            p.Add("@Reason", (int)t.Reason, DbType.Int32, ParameterDirection.Input);
            p.Add("@Source", (int)t.Source, DbType.Int32, ParameterDirection.Input);
            p.Add("@AgentID", t.AgentID, DbType.Int64, ParameterDirection.Input);
            p.Add("@Comment", t.Comment, DbType.String, ParameterDirection.Input);
            p.Add("@State", (int)t.State, DbType.Int32, ParameterDirection.Input);
            p.Add("@ForeignTransactionID", t.ForeignTransactionID, DbType.String, ParameterDirection.Input);
            p.Add("@BillingProcessorType", t.BillingProcessorType, DbType.Int32, ParameterDirection.Input);

            return p;
        }

        public string RecordTransactionBatch(List<Transaction> Transactions, string TransactionGroupID = "")
        {
            TransactionGroup tg = new TransactionGroup();

            if (TransactionGroupID.Equals(""))
                TransactionGroupID = System.Guid.NewGuid().ToString();

            tg.UniqueID = TransactionGroupID;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction dbTrans=conn.BeginTransaction();

                foreach (Transaction t in Transactions)
                {
                    t.TransactionBatchID = tg.UniqueID;
                    //context.Transaction.AddObject(Mapper.Map<Transaction, DMTDataRepositories.Database.Transaction>(t));
                    conn.Execute("spTransactionRecord", TransToDynParm(t), dbTrans, commandType: CommandType.StoredProcedure);
                }

                try
                {
                    dbTrans.Commit();
                }
                catch
                {
                    Trace.WriteLine("TransactionRepo: Failed to commit batch " + tg.UniqueID);
                    tg.UniqueID = null;
                }
                finally
                {
                    dbTrans.Dispose();
                }                
            }

            return tg.UniqueID;
        }

        public bool RecordTransaction(Transaction t)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                if (conn.Execute("spTransactionRecord", TransToDynParm(t), commandType: CommandType.StoredProcedure) > 0)
                    return true;
                else
                    return false;
            }
        }

        public bool RecordTransaction(long CustomerID, float Amount, TransactionType Type, TransactionReason Reason)
        {
            return false;
        }
    }
}