using System;
using Umbraco.Core.Persistence;

namespace UIOMatic
{
    public class QueryEventArgs : EventArgs
    {
        public QueryEventArgs(Type objectType, string tableName, Sql query, string sortColumn,
            string sortOrder, string searchTerm)
        {
            ObjectType = objectType;
            TableName = tableName;
            Query = query;
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            SearhTerm = searchTerm;
        }

        public Type ObjectType { get; set; }

        public string TableName { get; set; }

        public Sql Query { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public string SearhTerm { get; set; }
    }
}