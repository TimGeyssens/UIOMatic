using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Util;
using Umbraco.Core.Persistence;

namespace UIOMatic
{
    public class QueryEventArgs : EventArgs
    {
        public QueryEventArgs(Type currentType, string tableName, Sql query)
        {
            this.CurrentType = currentType;
            this.TableName = tableName;
            this.Query = query;
        }

        public Type CurrentType { get; set; }

        public string TableName { get; set; }

        public Sql Query { get; set; }
    }
}