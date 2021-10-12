using NPoco;
using System.Reflection;
using Umbraco.Extensions;

namespace UIOMatic.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string GetColumnName(this PropertyInfo prop)
        {
            var colName = prop.Name;

            var colAttri = prop.GetCustomAttribute<ColumnAttribute>();
            if (colAttri != null && !colAttri.Name.IsNullOrWhiteSpace())
                colName = colAttri.Name;

            return colName;
        }
    }
}
