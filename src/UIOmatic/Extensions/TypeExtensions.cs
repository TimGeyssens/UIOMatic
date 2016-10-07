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
            return type
                .GetProperties()
                .Select(x => x.GetCustomAttribute<PrimaryKeyColumnAttribute>())
                .FirstOrDefault(x => x != null);
        }

        public static string GetPrimaryKeyPropertyName(this Type type, string defaultName = "Id")
        {
            var primaryKeyPropertyName = "";
            var primKeyAttrs = type.GetCustomAttributes<PrimaryKeyAttribute>().ToArray();

            if (primKeyAttrs.Any())
                primaryKeyPropertyName = primKeyAttrs.First().Value;

            if (string.IsNullOrWhiteSpace(primaryKeyPropertyName))
            {
                foreach (var property in type.GetProperties())
                {
                    var keyAttri = property.GetCustomAttributes<PrimaryKeyColumnAttribute>();
                    if (!keyAttri.Any()) continue;

                    primaryKeyPropertyName = property.Name;
                }
            }
            else
            {
                primaryKeyPropertyName = defaultName;
            }

            return primaryKeyPropertyName;
        }

        public static object GetPropertyValue(this Type type, string propertyName, object instance)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(instance, null);
        }

        public static void SetPropertyValue(this Type type, string propertyName, object value, object instance)
        {
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            var targetType = propertyInfo.PropertyType.IsNullable() ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            value = value.ChangeType(targetType);

            propertyInfo.SetValue(instance, value, null);

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
