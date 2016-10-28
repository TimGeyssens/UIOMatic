using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticActionAttribute : Attribute
    {
        public string Alias { get; set; }

        public string Name { get; set; }

        public string View { get; set; }

        public string Icon { get; set; }

        public UIOMaticActionAttribute(string alias, string name, string view)
        {
            Alias = alias;
            Name = name;
            View = view;
        }
    }
}
