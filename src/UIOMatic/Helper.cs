using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using UIOMatic.Atributes;
using Umbraco.Core.Persistence;

namespace UIOMatic
{
    public class Helper
    {

        public static IEnumerable<Type> GetTypesWithUIOMaticAttribute()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(UIOMaticAttribute), true).Length > 0)
                    {
                        yield return type;
                    }
                }
            }
        }

    }
}