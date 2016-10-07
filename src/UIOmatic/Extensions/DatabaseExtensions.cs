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

        public static Page<object> Page(this Database db, Type type, long page, long itemsPerPage, Sql query)
        {
            var method = typeof(Database).GetGenericMethod("Page", new[] { type }, new[] { typeof(long), typeof(long), typeof(Sql) });
            var generic = method.MakeGenericMethod(type);
            return (Page<object>)generic.Invoke(db, new object[] { page, itemsPerPage, query });
        }

        public static object SingleOrDefault(this Database db, Type type, object primaryKey)
        {
            var method = typeof(Database).GetGenericMethod("SingleOrDefault", new[] { type }, new[] { typeof(object) });
            var generic = method.MakeGenericMethod(type);
            return generic.Invoke(db, new object[] { primaryKey });
        }
    }
}
