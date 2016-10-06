using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UIOmatic.Extensions
{
    internal static class TypeExtensions
    {
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
    }
}
