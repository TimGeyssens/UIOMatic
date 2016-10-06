using System;

namespace UIOmatic.Extensions
{
    internal static class ObjectExtensions
    {
        public static object GetPropertyValue(this object instance, string propertyName)
        {
            return instance.GetType().GetPropertyValue(propertyName, instance);
        }

        public static void SetPropertyValue(this object instance, string propertyName, object value)
        {
            instance.GetType().SetPropertyValue(propertyName, value, instance);
        }

        public static object ChangeType(this object value, Type type)
        {
            if (value == null && type.IsInterface)
                return null;

            if (value == null && type.IsGenericType)
                return Activator.CreateInstance(type);

            if (value == null)
                return null;

            if (type == value.GetType())
                return value;

            if (type.IsEnum)
            {
                var s1 = value as string;
                return s1 != null
                    ? Enum.Parse(type, s1)
                    : Enum.ToObject(type, value);
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                var innerType = type.GetGenericArguments()[0];
                var innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }

            var s = value as string;
            if (s != null && type == typeof(Guid))
                return new Guid(s);

            if (s != null && type == typeof(Version))
                return new Version(s);

            if (!(value is IConvertible))
                return value;

            return Convert.ChangeType(value, type);
        }
    }
}
