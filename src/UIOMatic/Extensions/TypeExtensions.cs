using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UIOmatic.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetTableName(this Type type)
        {
            var attr = type.GetCustomAttribute<TableNameAttribute>(false);
            var name = attr != null ? attr.Value.Trim('[',']') : type.Name;
            return name;
        }

        public static string GetPrimaryKeyName(this Type type)
        {
            var attr = type.GetCustomAttribute<PrimaryKeyAttribute>(true);
            if (attr != null)
                return attr.Value.Trim('[', ']');

            var attr2 = type.GetPrimaryKeyColumn();
            if (attr2 != null)
                return attr2.Name.Trim('[', ']');
            
            return "Id";
        }

        public static bool AutoIncrementPrimaryKey(this Type type)
        {
            var attr = type.GetCustomAttribute<PrimaryKeyAttribute>(true);
            if (attr != null)
                return attr.autoIncrement;
            
            var attr2 = type.GetPrimaryKeyColumn();
            return attr2 != null && attr2.AutoIncrement;
        }

        public static PrimaryKeyColumnAttribute GetPrimaryKeyColumn(this Type type)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                var attr = propertyInfo.GetCustomAttribute<PrimaryKeyColumnAttribute>();
                if (attr != null)
                {
                    if (string.IsNullOrWhiteSpace(attr.Name))
                    {
                        attr.Name = propertyInfo.Name;
                    }
                    return attr;
                }
            }
            return null;
        }

        public static object GetPropertyValue(this Type type, string propertyName, object instance)
        {
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return propertyInfo != null ? propertyInfo.GetValue(instance, null) : null; 
        }

        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttribute<TAttribute>() != null;
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] genericArgTypes, Type[] paramTypes)
        {
            return type
                .GetMethods()
                .Where(m => m.Name == name)
                .Select(m => new {
                    Method = m,
                    ParamTypes = m.GetParameters().Select(x => x.ParameterType).ToArray(),
                    GenericArgTypes = m.GetGenericArguments()
                })
                .Where(x => x.ParamTypes.Length == paramTypes.Length && x.ParamTypes.SequenceEqual(paramTypes)
                    && x.GenericArgTypes.Length == genericArgTypes.Length)
                .Select(x => x.Method)
                .FirstOrDefault();
        }
    }
}
