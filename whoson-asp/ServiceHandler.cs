using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

namespace WhosOn.ASP
{

    class RemoteHost
    {
        private HttpRequest request;

        public RemoteHost(HttpRequest request)
        {
            this.request = request;
        }

        public string Address
        {
            get { return request.UserHostAddress; }
        }

        public string HostName
        {
            get { return request.UserHostName; }
        }

        public HttpRequest Request
        {
            get { return request; }
        }
    }

    class ServiceContext
    {
        private WebService service;
        private RemoteHost remote;

        public ServiceContext(ref WebService service)
        {
            this.service = service;
            this.remote = new RemoteHost(service.Context.Request);
        }

        public RemoteHost Remote
        {
            get { return remote; }
        }
    }

    /// <summary>
    /// The request handler for the web service.
    /// </summary>
    public class ServiceHandler
    {
        private WebService service;
        private static ServiceHandler handler;

        protected ServiceHandler(ref WebService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Get handler for web service request.
        /// </summary>
        /// <returns>The request handler object.</returns>
        public static ServiceHandler GetHandler(WebService service)
        {
            ServiceHandler.handler = new ServiceHandler(ref service);
            return ServiceHandler.handler;
        }

        /// <summary>
        /// Insert a new logon event in the database and return its logon event ID.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="domain">The logon domain.</param>
        /// <param name="computer">The computer name (NetBIOS).</param>
        /// <param name="hwaddr">The MAC address.</param>
        /// <returns>The event ID.</returns>
        public int CreateLogonEvent(string user, string domain, string computer, string hwaddr)
        {
            ServiceContext context = new ServiceContext(ref service);
            using (Database db = new Database())
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CreateLogonEvent";

                        SqlParameter param = command.Parameters.Add("@event_id", SqlDbType.Int);
                        param.Direction = ParameterDirection.Output;

                        command.Parameters.AddWithValue("@username", user);
                        command.Parameters.AddWithValue("@domain", domain);
                        command.Parameters.AddWithValue("@hwaddr", hwaddr);
                        command.Parameters.AddWithValue("@ipaddr", context.Remote.Address);
                        command.Parameters.AddWithValue("@hostname", context.Remote.HostName);
                        command.Parameters.AddWithValue("@wksta", computer);

                        command.ExecuteNonQuery();
                        return (int)command.Parameters["@event_id"].Value;
                    }
                }
            }
        }

        /// <summary>
        /// Mark logon event identified by eventID as logged out.
        /// </summary>
        /// <param name="eventID">The logon event ID.</param>
        public void CloseLogonEvent(int eventID)
        {
            ServiceContext context = new ServiceContext(ref service);
            using (Database db = new Database())
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CloseLogonEvent";
                        command.Parameters.AddWithValue("@event_id", eventID);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Delete logon event identified by eventID.
        /// </summary>
        /// <param name="eventID">The logon event ID.</param>
        public void DeleteLogonEvent(int eventID)
        {
            ServiceContext context = new ServiceContext(ref service);
            using (Database db = new Database())
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "DeleteLogonEvent";
                        command.Parameters.AddWithValue("@event_id", eventID);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Find logon event matching given arguments.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="domain">The logon domain.</param>
        /// <param name="computer">The computer name (NetBIOS).</param>
        public LogonEvent FindLogonEvent(string user, string domain, string computer)
        {
            ServiceContext context = new ServiceContext(ref service);
            using (Database db = new Database())
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "FindLogonEvent";
                        command.Parameters.AddWithValue("@username", user);
                        command.Parameters.AddWithValue("@domain", domain);
                        command.Parameters.AddWithValue("@ipaddr", context.Remote.Address);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            QueryResult result = new QueryResult(reader);
                            if (!result.Read())
                            {
                                throw new MissingFieldException();
                            }
                            return result.Next();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns list of logon events matching given filter and match preferences.
        /// </summary>
        /// <param name="filter">The </param>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<LogonEvent> FindLogonEvents(LogonEvent filter, LogonEventMatch match)
        {
            ServiceContext context = new ServiceContext(ref service);
            using (Database db = new Database())
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        QueryBuilder query = new QueryBuilder(filter, match);

                        command.CommandType = CommandType.Text;
                        command.CommandText = query.Build();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            QueryResult result = new QueryResult(reader);
                            List<LogonEvent> list = result.Fill();
                            return list;
                        }
                    }
                }
            }
        }

    }
}
