using System.Reflection;

namespace UIOMatic.Front.Umbraco.Extensions
{
    internal static class ObjectExtensions
    {
        public static object GetPropertyValue(this object instance, string propertyName)
        {
            return instance.GetType().GetPropertyValue(propertyName, instance);
        }

        public static void SetPropertyValue(this object instance, string propertyName, object value)
        {
            var prop = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop.CanWrite)
            {
                prop.SetValue(instance, value);
            }
        }
    }
}
