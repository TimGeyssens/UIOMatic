using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace UIOMatic.Atributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticFieldAttribute: Attribute
    {
        public string Name { get; set; }

        public string Tab { get; set; }

        public string Description { get; set; }

        public string View { get; set; }

        public string Config { get; set; }

        public UIOMaticFieldAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;

            this.View = "textfield";
        }

        public string GetView()
        {
            if (View.StartsWith("~"))
                return View;
            else
                return string.Format("~/App_Plugins/UIOMatic/Backoffice/Views/{0}.html", View);

        }
    }
}