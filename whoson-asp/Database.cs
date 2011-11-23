using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WhosOn.ASP
{
    public class Database : IDisposable
    {
        private string dbstr;         // Connection string
        private SqlConnection dbcon;  // Database connection

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        public void Open()
        {
            SqlException ex = null;
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;

            foreach (ConnectionStringSettings connection in connections)
            {
                try
                {
                    dbstr = connection.ConnectionString;
                    dbcon = new SqlConnection(dbstr);
                    dbcon.Open();
                    break;
                }
                catch (SqlException exception)
                {
                    ex = exception;
                    Console.Error.Write(exception);
                }
            }

            if (dbcon.State != ConnectionState.Open)
            {
                if (ex != null)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Close the SQL server connection.
        /// </summary>
        public void Close()
        {
            if (dbcon != null)
            {
                dbcon.Close();
            }
        }

        /// <summary>
        /// Get SQL server connection.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        public SqlConnection Connection
        {
            get
            {
                if (dbcon == null)
                {
                    Open();
                }
                return dbcon;
            }
        }
    }
}
