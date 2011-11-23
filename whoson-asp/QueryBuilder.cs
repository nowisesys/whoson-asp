using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            if (filter.StartTime != DateTime.MinValue && filter.EndTime != DateTime.MinValue)
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
            if (filter.StartTime != DateTime.MinValue)
            {
                where.Add(string.Format("StartTime < '{0}'", filter.StartTime));
            }
            if (filter.EndTime != DateTime.MinValue)
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
            if (filter.StartTime != DateTime.MinValue)
            {
                where.Add(string.Format("StartTime > '{0}'", filter.StartTime));
            }
            if (filter.EndTime != DateTime.MinValue)
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
            if (filter.Username != null)
            {
                where.Add(string.Format("Username = '{0}'", filter.Username));
            }
            if (filter.Domain != null)
            {
                where.Add(string.Format("Domain = '{0}'", filter.Domain));
            }
            if (filter.HwAddress != null)
            {
                where.Add(string.Format("HwAddress = '{0}'", filter.HwAddress));
            }
            if (filter.IpAaddres != null)
            {
                where.Add(string.Format("IpAddress = '{0}'", filter.IpAaddres));
            }
            if (filter.Hostname != null)
            {
                where.Add(string.Format("Hostname = '{0}'", filter.Hostname));
            }
            if (filter.Workstation != null)
            {
                where.Add(string.Format("Workstation = '{0}'", filter.Workstation));
            }
            if (filter.StartTime != DateTime.MinValue)
            {
                where.Add(string.Format("StartTime = '{0}'", filter.StartTime));
            }
            if (filter.EndTime != DateTime.MinValue)
            {
                where.Add(string.Format("EndTime = '{0}'", filter.EndTime));
            }
        }

        private LogonEvent filter;
        private LogonEventMatch match;

        private string query;
        private List<string> where;
    }
}
