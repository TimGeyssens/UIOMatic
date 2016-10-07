using System.Collections.Generic;

namespace UIOMatic
{
    public static class Constants
    {
        public static readonly IReadOnlyDictionary<string, string> Views = new Dictionary<string, string>
        {
            { "textfield", "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html"},
            { "datetime", "~/App_Plugins/UIOMatic/Backoffice/Views/datetime.html"},
            { "number", "~/App_Plugins/UIOMatic/Backoffice/Views/number.html"}
        }; 
    }
}