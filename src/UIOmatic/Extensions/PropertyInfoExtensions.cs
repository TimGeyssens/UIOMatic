using System.Reflection;
using Umbraco.Core.Persistence;

namespace UIOmatic.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string GetColumnName(this PropertyInfo prop)
        {
            var colName = prop.Name;

            var colAttri = prop.GetCustomAttribute<ColumnAttribute>();
            if (colAttri != null)
                colName = colAttri.Name;

            return colName;
        }
    }
}
