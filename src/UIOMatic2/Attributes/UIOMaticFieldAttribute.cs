using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public string Tab { get; set; }

        public int TabOrder { get; set; }

        public string Description { get; set; }

        public string View { get; set; }

        public string Config { get; set; }

        public bool IsNameField { get; set; }

        public int Order { get; set; }

        public UIOMaticFieldAttribute()
        {
            View = "textfield";
        }

        public string GetView()
        {
            return View.StartsWith("~")
                ? View
                : string.Format("~/App_Plugins/UIOMatic/backoffice/views/fieldeditors/{0}.html", View);
        }
    }
}