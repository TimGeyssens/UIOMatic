using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Atributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticIgnoreFieldAttribute : Attribute
    {
    }
}