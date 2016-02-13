using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Attributes
{
    //case "2":
    //    return "<>";
    //case "3":
    //    return ">";
    //case "4":
    //    return "<";
    //case "5":
    //    return ">=";
    //case "6":
    //    return "<=";
    //case "1":
    //default:
    //    return "=";
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticFilterFieldAttribute : Attribute
    {
        public UIOMaticFilterFieldAttribute(int num = 1)
        {
            this.ShowNumbers = num;
        }
        public int ShowNumbers { get; set; }

        public string DefaultValue { get; set; }

        public string DefaultToValue { get; set; }

        public string OperatorCode { get; set; }
    }
}