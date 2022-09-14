using System;
using System.Collections.Generic;


namespace UIOMatic
{
    public class QueryEventArgs : EventArgs
    {
        public QueryEventArgs(Type objectType, string tableName, NPoco.Sql query, string sortColumn,
            string sortOrder, string searchTerm, IDictionary<string, string> filters)
        {
            ObjectType = objectType;
            TableName = tableName;
            Query = query;
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            SearhTerm = searchTerm;
            Filters = filters;
        }

        public Type ObjectType { get; set; }

        public string TableName { get; set; }

        public NPoco.Sql Query { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public string SearhTerm { get; set; }
        public IDictionary<string, string> Filters { get; set; }
    }
}
