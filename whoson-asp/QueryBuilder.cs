using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace WhosOn.ASP
{
    public class QueryBuilder
    {
        public QueryBuilder()
        {
            this.filter = new LogonEvent();
        }

        public QueryBuilder(LogonEvent filter)
        {
            this.filter = filter;
        }

        public QueryBuilder(LogonEvent filter, LogonEventMatch match)
        {
            this.filter = filter;
            this.match = match;
        }

        public void SetFilter(LogonEvent filter)
        {
            this.filter = filter;
        }

        public void SetMatch(LogonEventMatch match)
        {
            this.match = match;
        }

        private bool IsSet(string val)
        {
            return val != null && val.Length != 0;
        }

        private bool IsSet(DateTime val)
        {
            return val != null && val.Ticks != 0 && val != UnixEpoch;
        }

        public string Build()
        {
            where = new List<string>();
            query = "SELECT * FROM Logons";

            switch (match)
            {
                case LogonEventMatch.Exact:
                    FilterExact();
                    break;
                case LogonEventMatch.Before:
                    FilterBefore();
                    break;
                case LogonEventMatch.After:
                    FilterAfter();
                    break;
                case LogonEventMatch.Between:
                    FilterBetween();
                    break;
                case LogonEventMatch.Active:
                    FilterActive();
                    break;
                case LogonEventMatch.Closed:
                    FilterClosed();
                    break;
            }

            if (where.Count != 0)
            {
                query += " WHERE ";
                for (int i = 0; i < where.Count; ++i)
                {
                    if (i > 0)
                    {
                        query += " AND ";
                    }
                    query += where[i];
                }
            }
            query += " ORDER BY ID";
            return query;
        }

        public string GetSelect()
        {
            return query;
        }

        private void FilterActive()
        {
            where.Add(string.Format("EndTime IS NULL"));
        }

        private void FilterClosed()
        {
            where.Add(string.Format("EndTime IS NOT NULL"));
        }

        private void FilterBetween()
        {
            if (IsSet(filter.StartTime) && IsSet(filter.EndTime))
            {
                where.Add(string.Format("StartTime BETWEEN '{0}' AND '{1}'", filter.StartTime, filter.EndTime));
            }
        }

        private void FilterBefore()
        {
            if (filter.EventID != 0)
            {
                where.Add(string.Format("ID < {0}", filter.EventID));
            }
            if (IsSet(filter.StartTime))
            {
                where.Add(string.Format("StartTime < '{0}'", filter.StartTime));
            }
            if (IsSet(filter.EndTime))
            {
                where.Add(string.Format("EndTime < '{0}'", filter.EndTime));
            }
        }

        private void FilterAfter()
        {
            if (filter.EventID != 0)
            {
                where.Add(string.Format("ID > {0}", filter.EventID));
            }
            if (IsSet(filter.StartTime))
            {
                where.Add(string.Format("StartTime > '{0}'", filter.StartTime));
            }
            if (IsSet(filter.EndTime))
            {
                where.Add(string.Format("EndTime > '{0}'", filter.EndTime));
            }
        }

        private void FilterExact()
        {
            if (filter.EventID != 0)
            {
                where.Add(string.Format("ID = {0}", filter.EventID));
            }
            if (IsSet(filter.Username))
            {
                where.Add(string.Format("Username = '{0}'", filter.Username));
            }
            if (IsSet(filter.Domain))
            {
                where.Add(string.Format("Domain = '{0}'", filter.Domain));
            }
            if (IsSet(filter.HwAddress))
            {
                where.Add(string.Format("HwAddress = '{0}'", filter.HwAddress));
            }
            if (IsSet(filter.IpAddress))
            {
                where.Add(string.Format("IpAddress = '{0}'", filter.IpAddress));
            }
            if (IsSet(filter.Hostname))
            {
                where.Add(string.Format("Hostname = '{0}'", filter.Hostname));
            }
            if (IsSet(filter.Workstation))
            {
                where.Add(string.Format("Workstation = '{0}'", filter.Workstation));
            }
            if (IsSet(filter.StartTime))
            {
                where.Add(string.Format("StartTime = '{0}'", filter.StartTime));
            }
            if (IsSet(filter.EndTime))
            {
                where.Add(string.Format("EndTime = '{0}'", filter.EndTime));
            }
        }

        private LogonEvent filter;
        private LogonEventMatch match;

        private string query;
        private List<string> where;

        private DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
    }
}
