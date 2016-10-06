using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Attributes;

namespace UIOMatic
{
    public class Helper
    {
        public static IEnumerable<Type> GetTypesWithUIOMaticAttribute()
        {
            return GetTypesWithUIOMaticAttribute().Where(x => typeof(UIOMaticAttribute).IsAssignableFrom(x));
        }

        public static IEnumerable<Type> GetTypesWithUIOMaticFolderAttribute()
        {
            return (IEnumerable<Type>)HttpRuntime.Cache["UIOMaticFolderTypes"] ?? EnsureTypes();
        }

        private static IEnumerable<Type> EnsureTypes()
        {
            // UIOMaticFolderAttribute is the base type for UIOmatic entities
            var t = Umbraco.Core.TypeFinder.FindClassesWithAttribute<UIOMaticFolderAttribute>();
            HttpRuntime.Cache.Insert("UIOMaticFolderTypes", t);
            return t;
        }


        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            var type = inputObject.GetType();

            //get the property information based on the type
            var propertyInfo = type.GetProperty(propertyName);

            //find the property type
            var propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }


        public static object ChangeType(object value, Type type)
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