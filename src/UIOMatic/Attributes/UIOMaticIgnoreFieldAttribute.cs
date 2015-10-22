using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticIgnoreFieldAttribute : Attribute
    {
    }
}