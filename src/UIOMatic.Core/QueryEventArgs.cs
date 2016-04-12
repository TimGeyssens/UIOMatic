using System;
using Umbraco.Core.Persistence;

namespace UIOMatic.Core
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