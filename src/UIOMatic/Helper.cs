using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using UIOMatic.Attributes;
using Umbraco.Core.Persistence;

namespace UIOMatic
{
    public class Helper
    {

        public static IEnumerable<Type> GetTypesWithUIOMaticAttribute()
        {
            return (IEnumerable<Type>)HttpRuntime.Cache["UIOMaticTypes"] ?? EnsureTypes();
        }

        private static IEnumerable<Type> EnsureTypes()
        {
            var t = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof (UIOMaticAttribute), true).Length > 0)
                    {
                        t.Add(type);
                    }
                }
            }

            HttpRuntime.Cache.Insert("UIOMaticTypes", t);
            

            return t;
        }


        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }


    }
}