using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;

namespace UIOmatic.Extensions
{
    internal static class DatabaseExtensions
    {
        public static IEnumerable<object> Fetch(this Database db, Type type, Sql query)
        {
            var method = typeof(Database).GetGenericMethod("Fetch", new [] { type }, new [] { typeof(Sql) }); 
            var generic = method.MakeGenericMethod(type);
            return (IEnumerable<object>)generic.Invoke(db, new object[] { query }); 
        }
    }
}
