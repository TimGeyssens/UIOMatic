using System;
using Newtonsoft.Json;

namespace UIOMatic.Extensions
{
    internal static class ObjectExtensions
    {
        public static object GetPropertyValue(this object instance, string propertyName)
        {
            return instance.GetType().GetPropertyValue(propertyName, instance);
        }
    }
}
