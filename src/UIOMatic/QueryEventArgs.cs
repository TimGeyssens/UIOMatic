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
        public QueryEventArgs(Type currentType, string tableName, Sql query, string sortColumn,
            string sortOrder, string searchTerm)
        {
            this.CurrentType = currentType;
            this.TableName = tableName;
            this.Query = query;
            this.SortColumn = sortColumn;
            this.SortOrder = sortOrder;
            this.SearhTerm = searchTerm;
        }

        public Type CurrentType { get; set; }

        public string TableName { get; set; }

        public Sql Query { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public string SearhTerm { get; set; }
    }
}