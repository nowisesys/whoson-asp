// WhosOn ASP.NET Server Side Application
// Copyright (C) 2011-2019 Anders Lövgren, Nowise Systems/BMC-IT, Uppsala University
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WhosOn.ASP
{
    public class QueryResult
    {
        public QueryResult()
        {
            this.reader = null;
        }

        public QueryResult(SqlDataReader reader)
        {
            this.reader = reader;
        }

        public void SetReader(SqlDataReader reader)
        {
            this.reader = reader;
        }

        public List<LogonEvent> Fill()
        {
            return Fill(new List<LogonEvent>());
        }

        public List<LogonEvent> Fill(List<LogonEvent> list)
        {
            while (reader.Read())
            {
                list.Add(Next());
            }
            return list;
        }

        public bool Read()
        {
            return reader.Read();
        }

        public LogonEvent Next()
        {
            LogonEvent record = new LogonEvent();

            record.EventID = reader.GetInt32(reader.GetOrdinal("ID"));
            if (!reader.IsDBNull(reader.GetOrdinal("Username")))
                record.Username = reader.GetString(reader.GetOrdinal("Username")).Trim();
            if (!reader.IsDBNull(reader.GetOrdinal("Domain")))
                record.Domain = reader.GetString(reader.GetOrdinal("Domain")).Trim();
            if (!reader.IsDBNull(reader.GetOrdinal("HwAddress")))
                record.HwAddress = reader.GetString(reader.GetOrdinal("HwAddress")).Trim();
            if (!reader.IsDBNull(reader.GetOrdinal("IpAddress")))
                record.IpAddress = reader.GetString(reader.GetOrdinal("IpAddress")).Trim();
            if (!reader.IsDBNull(reader.GetOrdinal("Hostname")))
                record.Hostname = reader.GetString(reader.GetOrdinal("Hostname")).Trim();
            if (!reader.IsDBNull(reader.GetOrdinal("Workstation")))
                record.Workstation = reader.GetString(reader.GetOrdinal("Workstation")).Trim();
            if (!reader.IsDBNull(reader.GetOrdinal("StartTime")))
                record.StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime"));
            if (!reader.IsDBNull(reader.GetOrdinal("EndTime")))
                record.EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime"));

            return record;
        }

        private SqlDataReader reader;
    }
}
