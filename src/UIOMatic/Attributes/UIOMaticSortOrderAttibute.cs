using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticSortOrderAttribute : Attribute
    {
        public int Sequence { get; set; }
        public bool IsDescending { get; set; }

        public UIOMaticSortOrderAttribute(int sequence = 1, bool isDescending = false)
        {
            Sequence = sequence;
            IsDescending = isDescending;
        }
    }
}