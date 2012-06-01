using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;

namespace DareyaAPI.Models
{
    class CustomerRepoDapper : ICustomerRepository
    {
        string connStr;
        
        CloudStorageAccount storage;
        CloudTableClient client;
        TableServiceContextV2 context;

        private const string TableName = "CustomerForeignNetworkConnections";

        public CustomerRepoDapper()
        {
            connStr = ConfigurationManager.ConnectionStrings["DMTPrimary"].ConnectionString;

            storage = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            client = storage.CreateCloudTableClient();
            client.CreateTableIfNotExist(TableName);
            context = new TableServiceContextV2(client.BaseUri.ToString(), client.Credentials);
        }

        private DynamicParameters CustToDynParm(Customer c, bool inclID=false)
        {
            var p = new DynamicParameters();
            
            p.Add("@FirstName", c.FirstName, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@LastName", c.LastName, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@EmailAddress", c.EmailAddress, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@Address", c.Address, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@Address2", c.Address2, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@City", c.City, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@State", c.State, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@ZIPCode", c.ZIPCode, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@FacebookAccessToken", c.FacebookAccessToken, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@FacebookExpires", c.FacebookExpires, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@FacebookUserID", c.FacebookUserID, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@Password", c.Password, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@BillingType", c.BillingType, dbType: DbType.Int32, direction: ParameterDirection.Input);
            p.Add("@BillingID", c.BillingID, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@Type", c.Type, dbType: DbType.Int32, direction: ParameterDirection.Input);
            p.Add("@AvatarURL", c.AvatarURL, dbType: DbType.String, direction: ParameterDirection.Input);
            p.Add("@ForeignUserType", c.ForeignUserType, dbType: DbType.Int32, direction: ParameterDirection.Input);
            if (inclID)
                p.Add("@ID", c.ID, dbType: DbType.Int64, direction: ParameterDirection.Input);

            return p;
        }

        public long Add(Customer c)
        {
            var p = CustToDynParm(c);
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                p.Add("@InsertID", dbType: DbType.Int64, direction: ParameterDirection.Output);
                db.Execute("spCustomerAdd", p, commandType: CommandType.StoredProcedure);
                return p.Get<long>("@InsertID");
            }
        }

        public Customer GetWithID(long cID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var c = db.Query<Customer>("spCustomerGetWithID", new { ID = cID }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return c;
            }
        }

        public void Update(Customer c)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var p = CustToDynParm(c, true);
                db.Execute("spCustomerUpdate", p, commandType: CommandType.StoredProcedure);
            }   
        }

        public Customer GetWithEmailAddress(string EmailAddress)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var c = db.Query<Customer>("spCustomerGetWithEmailAddress", new { Email = EmailAddress }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return c;
            }
        }

        public Customer GetWithFacebookID(string FBID)
        {
            /*using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var c = db.Query<Customer>("spCustomerGetWithFacebookID", new { FacebookID = FBID }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return c;
            }*/
            return this.GetWithForeignUserID(FBID, Customer.ForeignUserTypes.Facebook);
        }

        public Customer GetWithForeignUserID(string ID, Customer.ForeignUserTypes type)
        {
            string partitionKey=((int)type).ToString()+"+"+ID;

            var f = (from e in context.CreateQuery<CustomerForeignNetworkConnectionDb>(TableName) where e.PartitionKey == partitionKey select e).FirstOrDefault<CustomerForeignNetworkConnectionDb>();

            if (f != null && f.CustomerID > 0)
            {
                Customer c = this.GetWithID(f.CustomerID);
                context.Detach(f);
                return c;
            }
            else
                return null;
            
            /*using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                var c = db.Query<Customer>("spCustomerGetWithForeignID", new { ForeignUserID = ID, ForeignUserType = type }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return c;
            }*/
        }

        public void AddForeignNetworkForCustomer(long ID, string ForeignID, Customer.ForeignUserTypes type)
        {
            CustomerForeignNetworkConnection f = new CustomerForeignNetworkConnection { CustomerID = ID, UserID = ForeignID, Type = (int)type };
            
            // store it for both kinds of lookup:
            //      * by foreign ID
            //      * by customer ID
            //
            CustomerForeignNetworkConnectionDb fDB1 = new CustomerForeignNetworkConnectionDb(f, true);
            CustomerForeignNetworkConnectionDb fDB2 = new CustomerForeignNetworkConnectionDb(f, false);

            context.AttachTo(TableName, fDB1, null);
            context.AttachTo(TableName, fDB2, null);

            context.UpdateObject(fDB1);
            context.UpdateObject(fDB2);

            context.SaveChangesWithRetries();

            context.Detach(fDB1);
            context.Detach(fDB2);
        }

        public void Remove(long ID)
        {
            using (SqlConnection db = new SqlConnection(connStr))
            {
                db.Open();
                db.Execute("DELETE FROM Customer WHERE ID=@CustomerID", new { CustomerID = ID });
                db.Close();
            }
        }
    }
}