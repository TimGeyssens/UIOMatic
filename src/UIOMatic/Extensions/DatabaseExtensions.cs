using NPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;

namespace UIOMatic.Extensions
{
    internal static class DatabaseExtensions
    {
        public static IEnumerable<object> Fetch(this Database db, Type type, NPoco.Sql query)
        {
            var method = typeof(Database).GetGenericMethod("Fetch", new [] { type }, new [] { typeof(NPoco.Sql) }); 
            var generic = method.MakeGenericMethod(type);
            return (IEnumerable<object>)generic.Invoke(db, new object[] { query }); 
        }

        public static IEnumerable<object> Query(this Database db, Type type, NPoco.Sql query)
        {
            var method = typeof(Database).GetGenericMethod("Query", new[] { type }, new[] { typeof(NPoco.Sql) });
            var generic = method.MakeGenericMethod(type);
            return (IEnumerable<object>)generic.Invoke(db, new object[] { query });
        }

        public static Page<object> Page(this Database db, Type type, long page, long itemsPerPage, NPoco.Sql query)
        {
            var method = typeof(Database).GetGenericMethod("Page", new[] { type }, new[] { typeof(long), typeof(long), typeof(NPoco.Sql) }); 
            var generic = method.MakeGenericMethod(type);
            var result = generic.Invoke(db, new object[] { page, itemsPerPage, query });
            return new Page<object>
            {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = (long)result.GetPropertyValue("TotalItems"),
                TotalPages = (long)result.GetPropertyValue("TotalPages"),
                Items = ((IList)result.GetPropertyValue("Items")).Cast<object>().ToList()
            };
        }

        public static object SingleOrDefault(this Database db, Type type, object primaryKey)
        {
           
            var method = typeof(Database).GetGenericMethod("SingleOrDefaultById", new[] { type }, new[] { typeof(object) });
            var generic = method.MakeGenericMethod(type);
            return generic.Invoke(db, new object[] { primaryKey });
        }
    }
}
