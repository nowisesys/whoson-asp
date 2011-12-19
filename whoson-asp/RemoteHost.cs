using System;
using System.Web;
using System.Net;

namespace WhosOn.ASP
{
    public class RemoteHost
    {
        private HttpRequest request;
        private IPHostEntry hostent;

        public RemoteHost(HttpRequest request)
        {
            this.request = request;
            this.hostent = Dns.GetHostEntry(request.UserHostAddress);
        }

        public string Address
        {
            get { return hostent.AddressList[0].ToString(); }
        }

        public string HostName
        {
            get { return hostent.HostName; }
        }

        public HttpRequest Request
        {
            get { return request; }
        }
    }
}
