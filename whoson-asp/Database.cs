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

        public const string ConnectionStringName = "WhosOnData";

        /// <summary>
        /// Construct an unconnected database object. Call Open() to establish 
        /// the database connection.
        /// </summary>
        public Database()
        {
        }

        /// <summary>
        /// Construct an connected database object. The database connection is
        /// done using the configured connection string named name.
        /// </summary>
        /// <param name="name">The named connection string.</param>
        /// <exception cref="SqlException"></exception>
        /// <see cref="Open(string)"/>
        public Database(string name)
        {
            Open(name);
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Opens the database connection. This will try all connection strings in 
        /// consequtive until a successful connection has been established. 
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
        /// Opens the database connection using named connection string. The name
        /// parameter is the symbolic name for the connection string, not the 
        /// connection string itself.
        /// 
        /// The default connection string name is Database.ConnectionStringName.
        /// </summary>
        /// <param name="name">The named connection string.</param>
        /// <exception cref="SqlException"></exception>
        public void Open(String name)
        {
            ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings[name];
            dbstr = connection.ConnectionString;
            dbcon = new SqlConnection(dbstr);
            dbcon.Open();
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
