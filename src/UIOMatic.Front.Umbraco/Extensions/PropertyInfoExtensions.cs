using NPoco;
using System.Reflection;

namespace UIOMatic.Front.Umbraco.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string GetColumnName(this PropertyInfo prop)
        {
            var colName = prop.Name;

            var colAttri = prop.GetCustomAttribute<ColumnAttribute>();
            if (colAttri != null && !string.IsNullOrWhiteSpace(colAttri.Name))
                colName = colAttri.Name;

            return colName;
        }
    }
}
