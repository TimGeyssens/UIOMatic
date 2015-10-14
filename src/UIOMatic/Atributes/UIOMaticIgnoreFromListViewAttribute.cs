using System;

namespace UIOMatic.Atributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticIgnoreFromListViewAttribute : Attribute
    {
    }
}