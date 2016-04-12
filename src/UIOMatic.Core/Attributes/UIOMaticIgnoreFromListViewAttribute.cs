using System;

namespace UIOMatic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticIgnoreFromListViewAttribute : Attribute
    {
    }
}