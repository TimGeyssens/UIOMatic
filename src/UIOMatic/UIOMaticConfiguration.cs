
using System;
using System.Collections.Generic;
using UIOMatic.Services;


namespace UIOMatic
{
    public class UIOMaticConfiguration : IUIOMaticConfiguration
    {
        public string PluginFolder => "~/App_plugins/UIOMatic";

       
        public Dictionary<string, string> Settings { get; set; }


        public string DefaultObjectService { get; set; }

        public Type DefaultObjectServiceType 
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DefaultObjectService) ? Type.GetType(DefaultObjectService) : typeof(NPocoObjectService);
            }
        }
    }

    public interface IUIOMaticConfiguration
    {
        string PluginFolder { get; }
        Dictionary<string, string> Settings { get; set; }
        Type DefaultObjectServiceType { get;  }

        string DefaultObjectService { get; set; }
    }
}
