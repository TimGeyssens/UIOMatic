using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticListViewFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public string View { get; set; }

        public string Config { get; set; }

        public int Order { get; set; }

        public UIOMaticListViewFieldAttribute()
        {
            View = "label";
        }

        public string GetView()
        {
            return View.StartsWith("~")
                ? View
                : string.Format("~/App_Plugins/UIOMatic/backoffice/views/fieldeditors/{0}.html", View);
        }
    }
}