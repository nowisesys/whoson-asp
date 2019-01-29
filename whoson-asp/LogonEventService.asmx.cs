using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Data;

namespace WhosOn.ASP
{
    public struct LogonEvent
    {
        public int EventID;
        public string Username;
        public string Domain;
        public string HwAddress;
        public string IpAaddres;
        public string Hostname;
        public string Workstation;
        public DateTime StartTime;
        public DateTime EndTime;
    }

    public enum LogonEventMatch
    {
        Before, Between, After, Exact, Active, Closed
    }

    /// <summary>
    /// The exposed web service class.
    /// </summary>
    [WebService(Namespace = "http://localhost/whoson")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LogonEventService : System.Web.Services.WebService
    {

        [WebMethod]
        public int CreateLogonEvent(string user, string domain, string computer, string hwaddr)
        {
            ServiceHandler handler = ServiceHandler.GetHandler(this);
            return handler.CreateLogonEvent(user, domain, computer, hwaddr);
        }

        [WebMethod]
        public void CloseLogonEvent(int eventID)
        {
            ServiceHandler handler = ServiceHandler.GetHandler(this);
            handler.CloseLogonEvent(eventID);
        }

        [WebMethod]
        public void DeleteLogonEvent(int eventID)
        {
            ServiceHandler handler = ServiceHandler.GetHandler(this);
            handler.DeleteLogonEvent(eventID);
        }

        [WebMethod]
        public LogonEvent FindLogonEvent(string user, string domain, string computer)
        {
            ServiceHandler handler = ServiceHandler.GetHandler(this);
            return handler.FindLogonEvent(user, domain, computer);
        }

        [WebMethod]
        public List<LogonEvent> FindLogonEvents(LogonEvent filter, LogonEventMatch match)
        {
            ServiceHandler handler = ServiceHandler.GetHandler(this);
            return handler.FindLogonEvents(filter, match);
        }

    }
}
