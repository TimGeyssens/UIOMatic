﻿using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIOMaticListViewFilterAttribute : Attribute
    {
        public string Name { get; set; }

        public string KeyField { get; set; }

        public string View { get; set; }

        public string Config { get; set; }

        public int Order { get; set; }

        public UIOMaticListViewFilterAttribute()
        {
            View = "dropdown";
        }

        public string GetView()
        {
            return View.StartsWith("~")
                ? View
                : string.Format("~/App_Plugins/UIOMatic/backoffice/views/fieldfilters/{0}.html", View);
        }
    }
}