using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data.SqlClient;
using System.Data;

namespace WhosOn.ASP
{
    [Serializable]
    public struct LogonEvent
    {
        public int EventID;
        public string Username;
        public string Domain;
        public string HwAddress;
        public string IpAddress;
        public string Hostname;
        public string Workstation;
        public DateTime StartTime;
        public DateTime EndTime;
        public int FirstID;         // Only used in query
        public int LastID;          // Only used in query
        public int Limit;           // Only used in query
    }

    [Serializable]
    public enum LogonEventMatch
    {
        Before, Between, After, Exact, Active, Closed
    }

    [WebService(Namespace = "http://it.bmc.uu.se/whoson")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class LogonAccountingService : System.Web.Services.WebService
    {

        [WebMethod(Description="Creates and return the ID of an logon session. The computer argument is the NetBIOS name.")]
        public int CreateLogonEvent(string user, string domain, string computer, string hwaddr)
        {
            using (Database db = new Database(Database.ConnectionStringName))
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        RemoteHost remote = new RemoteHost(Context.Request);

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "CreateLogonEvent";

                        SqlParameter param = command.Parameters.Add("@event_id", SqlDbType.Int);
                        param.Direction = ParameterDirection.Output;

                        command.Parameters.AddWithValue("@username", user);
                        command.Parameters.AddWithValue("@domain", domain);
                        command.Parameters.AddWithValue("@hwaddr", hwaddr);
                        command.Parameters.AddWithValue("@ipaddr", remote.Address);
                        command.Parameters.AddWithValue("@hostname", remote.HostName);
                        command.Parameters.AddWithValue("@wksta", computer);

                        command.ExecuteNonQuery();
                        return (int)command.Parameters["@event_id"].Value;
                    }
                }
            }
        }

        [WebMethod(Description="Close an existing logon session identified by the event ID")]
        public void CloseLogonEvent(int eventID)
        {
            using (Database db = new Database(Database.ConnectionStringName))
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

        [WebMethod(Description="Deletes the logon event identified by the event ID.")]
        public void DeleteLogonEvent(int eventID)
        {
            using (Database db = new Database(Database.ConnectionStringName))
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

        [WebMethod(Description="Find and return the logon event matching the request parameters.")]
        public LogonEvent FindLogonEvent(string user, string domain, string computer)
        {
            using (Database db = new Database(Database.ConnectionStringName))
            {
                using (SqlConnection connection = db.Connection)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        RemoteHost remote = new RemoteHost(Context.Request);

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "FindLogonEvent";
                        command.Parameters.AddWithValue("@username", user);
                        command.Parameters.AddWithValue("@domain", domain);
                        command.Parameters.AddWithValue("@ipaddr", remote.Address);

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

        [WebMethod(Description="Find and return all logon events matching the filter and match options.")]
        public List<LogonEvent> FindLogonEvents(LogonEvent filter, LogonEventMatch match)
        {
            using (Database db = new Database(Database.ConnectionStringName))
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
