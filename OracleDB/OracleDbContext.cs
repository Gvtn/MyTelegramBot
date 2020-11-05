using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace InfoInkasService.OracleDB
{
    public partial class OracleDbContext : DbContext
    {
        public string _connectionString { get; }
        public OracleDbContext() :
            base()
        {
            OnCreated();
        }
        
        public OracleDbContext(string connectionString) :
            base()
        {
            _connectionString = connectionString;
            OnCreated();
        }

        public OracleDbContext(DbContextOptions<OracleDbContext> options) :
            base(options)
        {
            OnCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Make commments

            optionsBuilder.UseOracle(_connectionString);
            CustomizeConfiguration(ref optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        private static string GetConnectionString(string connectionStringName)
        {
            System.Configuration.ConnectionStringSettings connectionStringSettings = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null)
                throw new InvalidOperationException("Connection string \"" + connectionStringName + "\" could not be found in the configuration file.");
            return connectionStringSettings.ConnectionString;
        }

        partial void CustomizeConfiguration(ref DbContextOptionsBuilder optionsBuilder);

        #region Methods

        public async Task<XmlDocument> GETCASHOUTINFO(long IN_CHAT_ID, int IN_SALEPOINT_ID, DateTime IN_DATE_CASHOUT)
        {

            XmlDocument result;

            OracleConnection connection = (OracleConnection)this.Database.GetDbConnection();
            bool needClose = false;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                needClose = true;
            }

            try
            {
                using (OracleCommand cmd = connection.CreateCommand())
                {
                    if (this.Database.GetCommandTimeout().HasValue)
                        cmd.CommandTimeout = this.Database.GetCommandTimeout().Value;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = @"IBOX_IAS.F_GET_CASHOUT_INFO";
                    cmd.BindByName = true;
                    OracleParameter IN_CHAT_IDParameter = cmd.CreateParameter();
                    IN_CHAT_IDParameter.ParameterName = "in_chat_id";
                    IN_CHAT_IDParameter.Direction = ParameterDirection.Input;
                    IN_CHAT_IDParameter.OracleDbType = OracleDbType.Int64;
                    IN_CHAT_IDParameter.Value = IN_CHAT_ID;

                    cmd.Parameters.Add(IN_CHAT_IDParameter);

                    OracleParameter IN_SALEPOINT_IDParameter = cmd.CreateParameter();
                    IN_SALEPOINT_IDParameter.ParameterName = "in_salepoint_id";
                    IN_SALEPOINT_IDParameter.Direction = ParameterDirection.Input;
                    IN_SALEPOINT_IDParameter.OracleDbType = OracleDbType.Double;
                    IN_SALEPOINT_IDParameter.Value = IN_SALEPOINT_ID;

                    cmd.Parameters.Add(IN_SALEPOINT_IDParameter);

                    OracleParameter IN_DATE_CASHOUTParameter = cmd.CreateParameter();
                    IN_DATE_CASHOUTParameter.ParameterName = "in_date_cashout";
                    IN_DATE_CASHOUTParameter.Direction = ParameterDirection.Input;
                    IN_DATE_CASHOUTParameter.OracleDbType = OracleDbType.Date;
                    IN_DATE_CASHOUTParameter.Value = IN_DATE_CASHOUT;

                    cmd.Parameters.Add(IN_DATE_CASHOUTParameter);

                    OracleParameter returnValueParameter = cmd.CreateParameter();
                    returnValueParameter.Direction = ParameterDirection.ReturnValue;
                    returnValueParameter.OracleDbType = OracleDbType.XmlType;
                    returnValueParameter.ParameterName = "return_value";

                    cmd.Parameters.Add(returnValueParameter);
                    await cmd.ExecuteNonQueryAsync();

                    result = ((OracleXmlType)cmd.Parameters["return_value"].Value).GetXmlDocument();
                }
            }
            finally
            {
                if (needClose)
                    connection.Close();
            }
            return result;
        }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RelationshipsMapping(modelBuilder);
            CustomizeMapping(ref modelBuilder);
        }

        private void RelationshipsMapping(ModelBuilder modelBuilder)
        {
        }

        partial void CustomizeMapping(ref ModelBuilder modelBuilder);

        public bool HasChanges()
        {
            return ChangeTracker.Entries().Any(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added || e.State == Microsoft.EntityFrameworkCore.EntityState.Modified || e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted);
        }

        partial void OnCreated();
    }
}