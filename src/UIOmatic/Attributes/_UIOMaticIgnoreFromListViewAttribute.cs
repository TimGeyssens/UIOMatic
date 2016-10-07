using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticIgnoreFromListViewAttribute : Attribute
    {
    }
}